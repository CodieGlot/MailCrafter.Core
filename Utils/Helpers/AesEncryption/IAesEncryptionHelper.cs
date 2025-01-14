namespace MailCrafter.Utils.Helpers;

public interface IAesEncryptionHelper
{
    string Decrypt(string cipherText);
    string Encrypt(string plainText);
}