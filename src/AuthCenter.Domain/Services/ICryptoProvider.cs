namespace AuthCenter.Domain.Services;

public interface ICryptoProvider
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string ComputeHash(string input);
    string ComputeHmac(string input);
}
