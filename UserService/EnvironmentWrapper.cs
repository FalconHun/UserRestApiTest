using System;

namespace UserService
{
    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public string GetValue(string key, string fallback)
        {
            return Environment.GetEnvironmentVariable(key) ?? fallback;
        }
    }
}
