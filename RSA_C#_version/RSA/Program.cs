using System;
using System.IO;
using System.Numerics;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            var textPath = "text.txt";
            var openKeyFilePath = "open_key.txt";
            var closeKeyFilePath = "close_key.txt";
            var encryptFilePath = "encrypt_text.txt";
            var decryptFilePath = "decrypt_text.txt";

            RSA.GenerateKeys(out RSAKey openKey, out RSAKey closeKey);
            openKey.WriteToFile(openKeyFilePath);
            closeKey.WriteToFile(closeKeyFilePath);
            RSA.EncryptFile(textPath, encryptFilePath, openKeyFilePath);
            RSA.DecryptFile(encryptFilePath, decryptFilePath, closeKeyFilePath);
        }
    }
}
