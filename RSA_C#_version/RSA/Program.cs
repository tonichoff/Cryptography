using System;
using System.Numerics;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            RSA.GenerateKeys(out RSAKey openKey, out RSAKey closeKey);

            Console.WriteLine($"Open key:  {{{openKey.Key}, {openKey.Module} }}");
            Console.WriteLine($"Close key: {{{closeKey.Key}, {closeKey.Module} }}");

            // Example from https://ru.wikipedia.org/wiki/RSA.
            BigInteger message = 111111;
            BigInteger code = RSA.Encrypt(message, openKey);
            BigInteger decryptMessage = RSA.Decrypt(code, closeKey);

            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"Code:    {code}");
            Console.WriteLine($"Decrypt: {decryptMessage}");
            Console.ReadKey();
        }
    }
}
