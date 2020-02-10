namespace LoggerService
{
    interface IEnvironmentWrapper
    {
        string GetValue(string key, string fallback);
    }
}
