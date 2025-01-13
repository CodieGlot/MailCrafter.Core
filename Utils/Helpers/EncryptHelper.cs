using BC = BCrypt.Net.BCrypt;

namespace MailCrafter.Utils.Helpers;
public static class EncryptHelper
{
    public static string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }
    public static bool Verify(string text, string hash)
    {
        return BC.Verify(text, hash);
    }
}
