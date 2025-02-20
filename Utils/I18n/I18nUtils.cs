using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace MailCrafter.Core.Utils;
public static class I18nUtils
{
    private static readonly ResourceManager ResourceManager = new ResourceManager("MailCrafter.Utils.I18n.Resources.Resource", typeof(I18nUtils).Assembly);
    public static string GetResource(string key, string defaultValue = "", string language = "", params object[] args)
    {
        try
        {
            var value = ResourceManager.GetString(key, CultureInfo.CreateSpecificCulture(language));

            if (!string.IsNullOrEmpty(value))
            {
                return string.Format(value, args);
            }

            return defaultValue;
        }
        catch (MissingManifestResourceException)
        {
            return defaultValue;
        }
    }
    public static string GetDescription(Enum enumValue, string defaultValue = "", string language = "", params object[] args)
    {
        var enumType = enumValue.GetType();

        var memberInfo = enumType.GetMember(enumValue.ToString());

        if (memberInfo.Length == 0)
        {
            return defaultValue;
        }

        var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Length > 0)
        {
            var description = ((DescriptionAttribute)attributes[0]).Description;

            try
            {
                var value = ResourceManager.GetString(description, CultureInfo.CreateSpecificCulture(language));

                if (!string.IsNullOrEmpty(value))
                {
                    return string.Format(value, args);
                }
            }
            catch (MissingManifestResourceException)
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }
}