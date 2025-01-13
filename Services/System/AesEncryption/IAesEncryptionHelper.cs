namespace MailCrafter.Services;

public interface IAesEncryptionHelper
{
    string Decrypt(string cipherText);
    string Encrypt(string plainText);
}