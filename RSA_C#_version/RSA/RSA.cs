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
                file.Write(Separator);
                file.Write(Module);
            }
        }

        public static RSAKey ReadFromFile(string path)
        {
            var content = File.ReadAllText(path).Split(Separator);
            return new RSAKey(BigInteger.Parse(content[0]), BigInteger.Parse(content[1]));
        }

        public BigInteger Key { get; private set; }
        public BigInteger Module { get; private set; }

        private const char Separator = ' ';
    }

    class RSA
    {
        static public void GenerateKeys(out RSAKey openKey, out RSAKey closeKey)
        {
            var content = File.ReadAllText("primes.txt").Split('\n');
            var p = BigInteger.Parse(content[0]);
            var q = BigInteger.Parse(content[1]);
            var n = p * q;
            var eulerResult = (p - 1) * (q - 1);
            // For open key used fifth number Fermat.
            var e = 65537;
            EuclidExtended(e, eulerResult, out BigInteger d, out BigInteger temp);
            d = (d % eulerResult + eulerResult) % eulerResult; 

            openKey = new RSAKey(e, n);
            closeKey = new RSAKey(d, n);
        }

        static public void EncryptFile(string sourceFilePath, string encryptFilePath, string fileKeyPath)
        {
            var openKey = RSAKey.ReadFromFile(fileKeyPath);
            using (var encodeFile = File.CreateText(encryptFilePath))
            {
                var bytesFromSourceFile = File.ReadAllBytes(sourceFilePath);
                var isFirstByte = true;
                foreach (byte @byte in bytesFromSourceFile)
                {
                    var code = Encrypt(new BigInteger(@byte), openKey);
                    if (!isFirstByte)
                    {
                        encodeFile.Write(Separator);
                    }
                    else
                    {
                        isFirstByte = false;
                    }
                    encodeFile.Write(code);
                }
            }
        }

        static public void DecryptFile(string encryptFilePath, string decryptFilePath, string fileKeyPath)
        {
            var closeKey = RSAKey.ReadFromFile(fileKeyPath);
            using (var decodedFile = File.Create(decryptFilePath))
            {
                var encodedInformation = File.ReadAllText(encryptFilePath).Split(Separator);
                foreach (var block in encodedInformation)
                {
                    var decodedBlock = Decrypt(BigInteger.Parse(block), closeKey).ToByteArray()[0];
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

        static private BigInteger EuclidExtended(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            var gcd = EuclidExtended(b % a, a, out BigInteger x1, out BigInteger y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }

        private const char Separator = ' ';
    }
}
