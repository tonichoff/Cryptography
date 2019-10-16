using System;
using System.IO;
using System.Numerics;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            string textPath = "text.txt";
            string openKeyFilePath = "open_key.txt";
            string closeKeyFilePath = "close_key.txt";
            string encryptFilePath = "encrypt_text.txt";
            string decryptFilePath = "decrypt_text.txt";

            RSA.GenerateKeys(out RSAKey openKey, out RSAKey closeKey);
            openKey.WriteToFile(openKeyFilePath);
            closeKey.WriteToFile(closeKeyFilePath);
            RSA.EncryptFile(textPath, encryptFilePath, openKeyFilePath);
            RSA.DecryptFile(encryptFilePath, decryptFilePath, closeKeyFilePath);
        }
    }
}
