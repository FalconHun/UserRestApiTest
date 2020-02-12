using NUnit.Framework;
using NSubstitute;
using UserService;

namespace UserServiceTests
{
    public class ConnectionParamProviderTests
    {
        [Test]
        public void Constructor_ReadsValuesFromEnvironment()
        {
            var env = Substitute.For<IEnvironmentWrapper>();

            env.GetValue("hostname", Arg.Any<string>()).Returns("testhostname");
            env.GetValue("port", Arg.Any<string>()).Returns("10");
            env.GetValue("virtualhost", Arg.Any<string>()).Returns("testvirtualhost");
            env.GetValue("exchangename", Arg.Any<string>()).Returns("testexchangename");
            env.GetValue("queuename", Arg.Any<string>()).Returns("testqueuename");
            env.GetValue("RoutingKey", Arg.Any<string>()).Returns("testroutingkey");

            var provider = new ConnectionParamProvider(env);

            Assert.AreEqual(provider.HostName, "testhostname");
            Assert.AreEqual(provider.Port, 10);
            Assert.AreEqual(provider.VirtualHost, "testvirtualhost");
            Assert.AreEqual(provider.ExchangeName, "testexchangename");
            Assert.AreEqual(provider.QueueName, "testqueuename");
            Assert.AreEqual(provider.RoutingKey, "testroutingkey");
        }
    }
}
