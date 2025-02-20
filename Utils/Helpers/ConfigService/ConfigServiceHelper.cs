using Microsoft.Extensions.Configuration;

namespace MailCrafter.Utils.Helpers;
public class ConfigServiceHelper : IConfigServiceHelper
{
    private readonly IConfiguration _config;

    public ConfigServiceHelper(IConfiguration config)
    {
        _config = config;
    }
    public string GetString(string key)
    {
        var value = _config[key];

        return value is not null
            ? value
            : throw new ArgumentException($"Value of {key} has not been set.");
    }
    public int GetInt(string key)
    {
        var value = GetString(key);

        try
        {
            return Convert.ToInt32(value);
        }
        catch (Exception)
        {
            throw new ArgumentException($"Value of {key} is not a valid integer.");
        }
    }
}