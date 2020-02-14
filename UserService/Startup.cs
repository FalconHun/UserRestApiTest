using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.DataAccess;
using UserService.Options;

namespace UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetupDbContext(services);

            services.AddControllers();

            services.AddSingleton<IEnvironmentWrapper, EnvironmentWrapper>();
            services.AddSingleton<IConnectionParamProvider, ConnectionParamProvider>();
            services.AddSingleton<IRabbitMqMessagePublisher, RabbitMqMessagePublisher>();

            services.AddSwaggerGen(x => x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "UserManager API", Version = "v1" }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });

            app.UseSwaggerUI(option => { option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description); });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PrepareDb(app);
        }

        private void SetupDbContext(IServiceCollection services)
        {
            var databaseType = Configuration["DatabaseType"] ?? "InMemory";

            if (databaseType == "InMemory")
            {
                services.AddDbContext<SqlDbContext>(options =>
                {
                    options.UseInMemoryDatabase("UserDatabase");
                });

                services.AddScoped<IUserManager, UserManagerSql>();
            }
            else if (databaseType == "Sql")
            {
                var dbConnectionString = Configuration["DBConnectionString"] ?? "UnknownConnectionString";

                services.AddDbContext<SqlDbContext>(options =>
                {
                    options.UseSqlServer(dbConnectionString);
                });

                services.AddScoped<IUserManager, UserManagerSql>();
            }
            else if (databaseType == "MongoDb")
            {
                services.Configure<MongoDbContextOptions>(options =>
                {
                    options.ConnectionString = Configuration["DBConnectionString"] ?? "TestConnection";
                    options.Database = Configuration["MongoDbDatabase"] ?? "TestDatabase";
                });

                services.AddScoped<MongoDbContext, MongoDbContext>();
                services.AddScoped<IUserManager, UserManagerMongoDb>();
            }
        }

        private void PrepareDb(IApplicationBuilder app)
        {
            PrepDb.PrepPopulation(app);
        }
    }
}
