using System;
using System.IO;
using System.Numerics;

namespace RSA
{
    public class RSAKey
    {
        public RSAKey(BigInteger key, BigInteger module)
        {
            Key = key;
            Module = module;
        }

        public void WriteToFile(string path)
        {
            using (var file = File.CreateText(path))
            {
                file.Write(Key);
                file.Write(' ');
                file.Write(Module);
            }
        }

        public static RSAKey ReadFromFile(string path)
        {
            var content = File.ReadAllText(path).Split(' ');
            return new RSAKey(BigInteger.Parse(content[0]), BigInteger.Parse(content[1]));
        }

        public BigInteger Key { get; private set; }
        public BigInteger Module { get; private set; }
    }

    class RSA
    {
        static public void GenerateKeys(out RSAKey openKey, out RSAKey closeKey)
        {
            // Example from https://ru.wikipedia.org/wiki/RSA.
            BigInteger p = 3557;
            BigInteger q = 2579;
            BigInteger n = p * q;
            BigInteger eulerResult = (p - 1) * (q - 1);
            // For open key used fifth number Fermat.
            BigInteger e = 65537;
            BigInteger d = EuclidExtendedModified(eulerResult, e);

            openKey = new RSAKey(e, n);
            closeKey = new RSAKey(d, n);
        }

        static public void EncryptFile(string sourceFilePath, string encodeFilePath, string fileKeyPath)
        {
            RSAKey openKey = RSAKey.ReadFromFile(fileKeyPath);
            using (var encodeFile = File.CreateText(encodeFilePath))
            {
                byte[] bytesFromSourceFile = File.ReadAllBytes(sourceFilePath);
                bool isFirstByte = true;
                foreach (byte @byte in bytesFromSourceFile)
                {
                    BigInteger code = Encrypt(new BigInteger(@byte), openKey);
                    if (!isFirstByte)
                    {
                        encodeFile.Write(" ");
                    }
                    else
                    {
                        isFirstByte = false;
                    }
                    encodeFile.Write(code);
                }
            }
        }

        static public void DecryptFile(string encodeFilePath, string decodeFilePath, string fileKeyPath)
        {
            RSAKey closeKey = RSAKey.ReadFromFile(fileKeyPath);
            using (var decodedFile = File.Create(decodeFilePath))
            {
                var encodedInformation = File.ReadAllText(encodeFilePath).Split(' ');
                foreach (var block in encodedInformation)
                {
                    byte decodedBlock = Decrypt(BigInteger.Parse(block), closeKey).ToByteArray()[0];
                    decodedFile.WriteByte(decodedBlock);
                }
            }
        }

        static private BigInteger Encrypt(BigInteger cipher, RSAKey openKey)
        {
            return BigInteger.ModPow(cipher, openKey.Key, openKey.Module);
        }

        static private BigInteger Decrypt(BigInteger cipher, RSAKey closeKey)
        {
            return BigInteger.ModPow(cipher, closeKey.Key, closeKey.Module);
        }

        static private BigInteger EuclidExtendedModified(BigInteger eulerResult, BigInteger openKey)
        {
            BigInteger a = eulerResult;
            BigInteger b = openKey;
            BigInteger c = eulerResult;
            BigInteger d = 1;
            while (b != 1)
            {
                BigInteger g = a / b;
                BigInteger h = g * d;
                BigInteger i = a - b * g;
                if (i < 0)
                {
                    i = (i / eulerResult + 1) * eulerResult + i;
                }
                BigInteger j = c - h;
                if (j < 0)
                {
                    j = (j / eulerResult + 1) * eulerResult + j;
                }
                a = b;
                b = i;
                c = d;
                d = j;
            }
            return d;
        }
    }
}
