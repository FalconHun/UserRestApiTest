namespace UserService
{
    interface IEnvironmentWrapper
    {
        string GetValue(string key, string fallback);
    }
}
