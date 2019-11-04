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
            Console.WriteLine("Start generating keys");
            var content = File.ReadAllText("primes.txt").Split('\n');
            //var p = BigInteger.Parse(content[0]);
            //var q = BigInteger.Parse(content[1]);
            var p = GenetateRandomPrime();
            Console.WriteLine($"p is generated == {p}");
            var q = GenetateRandomPrime();
            Console.WriteLine($"q is generated == {p}");
            var n = p * q;
            var eulerResult = (p - 1) * (q - 1);
            // For open key used fifth number Fermat.
            var e = 65537;
            Console.WriteLine($"Start generating e");
            EuclidExtended(e, eulerResult, out BigInteger d, out BigInteger temp);
            d = (d % eulerResult + eulerResult) % eulerResult; 

            openKey = new RSAKey(e, n);
            closeKey = new RSAKey(d, n);
        }

        static public void EncryptFile(string sourceFilePath, string encryptFilePath, string fileKeyPath)
        {
            Console.WriteLine($"Start encrypting");
            var openKey = RSAKey.ReadFromFile(fileKeyPath);
            using (var encodeFile = File.CreateText(encryptFilePath))
            {
                var bytesFromSourceFile = File.ReadAllBytes(sourceFilePath);
                var isFirstByte = true;
                var count = 0;
                foreach (byte @byte in bytesFromSourceFile)
                {
                    Console.WriteLine($"Byte number {count++} from {bytesFromSourceFile.Length}");
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
            Console.WriteLine($"Start decrypting");
            var closeKey = RSAKey.ReadFromFile(fileKeyPath);
            using (var decodedFile = File.Create(decryptFilePath))
            {
                var encodedInformation = File.ReadAllText(encryptFilePath).Split(Separator);
                var count = 0;
                foreach (var block in encodedInformation)
                {
                    Console.WriteLine($"Byte number {count++} from {encodedInformation.Length}");
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

        private static BigInteger GenetateRandomPrime()
        {
            Console.WriteLine("Start generating prime");
            while (true)
            {
                var candidate = GenerateRandom(Min, Max);
                Console.WriteLine($"Candidate {candidate}");
                while (candidate <= 2)
                {
                    candidate = GenerateRandom(Min, Max);
                }
                candidate |= 1;
                if (TrialDivision(candidate))
                {
                    Console.WriteLine($"End generating prime {candidate}");
                    return candidate;
                }
            }
        }

        public static bool TrialDivision(BigInteger candidate)
        {
            Console.WriteLine("Start TestPrime");
            var closestSqrt = Sqrt(candidate);
            for (BigInteger i = 2; i <= closestSqrt; ++i)
            {
                Console.WriteLine($"Test {i} from {closestSqrt}");
                if (BigInteger.Remainder(candidate, i) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static BigInteger Sqrt(BigInteger number)
        {
            // https://social.msdn.microsoft.com/Forums/ru-RU/f9aca8c2-af21-40e4-b5bb-c9613b9db4ca/-biginteger?forum=fordesktopru.
            BigInteger root = number;
            int bitLength = 1;
            while (root / 2 != 0)
            {
                root /= 2;
                bitLength++;
            }
            bitLength = (bitLength + 1) / 2;
            root = number >> bitLength;

            BigInteger lastRoot = BigInteger.Zero;
            do
            {
                lastRoot = root;
                root = (BigInteger.Divide(number, root) + root) >> 1;
            }
            while (!((root ^ lastRoot).ToString() == "0"));
            return root;
        }

        private static BigInteger GenerateRandom(BigInteger leftBound, BigInteger rightBound)
        {
            Console.WriteLine("Start generating random");
            var maxBytes = rightBound.ToByteArray();
            var minBytes = leftBound.ToByteArray();
            var resultBytes = new byte[maxBytes.Length];
            if (minBytes.Length < maxBytes.Length)
            {
                var buffer = new byte[maxBytes.Length];
                Array.Copy(minBytes, 0, buffer, maxBytes.Length - minBytes.Length, minBytes.Length);
                minBytes = buffer;
            }
            var isLessMax = false;
            var isMoreMin = false;
            var random = new Random();
            for (var i = 0; i < maxBytes.Length; i++)
            {
                Console.WriteLine($"Generate byte {i} from {maxBytes.Length}");
                var upperBound = Byte.MaxValue + 1;
                if (!isLessMax)
                {
                    upperBound = maxBytes[i] + 1;
                }
                var lowerBound = Byte.MinValue;
                if (!isMoreMin)
                {
                    lowerBound = minBytes[i];
                }
                var randomByte = (byte)random.Next(lowerBound, upperBound);
                isLessMax = randomByte < maxBytes[i];
                isMoreMin = randomByte > minBytes[i];
                resultBytes[i] = randomByte;
            }
            return new BigInteger(resultBytes);
        }

        private const char Separator = ' ';
        private const int Base = 8;
        private static BigInteger Max = BigInteger.Pow(2, Base) - 1;
        private static BigInteger Min = BigInteger.Pow(2, Base - 1);
    }
}
