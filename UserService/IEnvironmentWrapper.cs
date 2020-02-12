namespace UserService
{
    public interface IEnvironmentWrapper
    {
        string GetValue(string key, string fallback);
    }
}
