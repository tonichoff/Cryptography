using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace Crypto
{
    public class ElGamalSignature
    {
        public ElGamalSignature(BigInteger r, BigInteger s)
        {
            this.r = r;
            this.s = s;
        }

        public void WriteToFile(string path)
        {
            using var file = File.CreateText(path);
            file.Write(r);
            file.Write(Separator);
            file.Write(s);
        }

        public static ElGamalSignature ReadFromFile(string path)
        {
            var content = File.ReadAllText(path).Split(Separator);
            return new ElGamalSignature(BigInteger.Parse(content[0]), BigInteger.Parse(content[1]));
        }

        public BigInteger r { get; private set; }
        public BigInteger s { get; private set; }
        private const char Separator = ' ';
    }

    public class ElGamalKey
    {
        public ElGamalKey(BigInteger key, BigInteger generator, BigInteger prime)
        {
            Key = key;
            Generator = generator;
            Prime = prime;
        }

        public void WriteToFile(string path)
        {
            using var file = File.CreateText(path);
            file.Write(Key);
            file.Write(Separator);
            file.Write(Generator);
            file.Write(Separator);
            file.Write(Prime);
        }

        public static ElGamalKey ReadFromFile(string path)
        {
            var content = File.ReadAllText(path).Split(Separator);
            return new ElGamalKey(BigInteger.Parse(content[0]), BigInteger.Parse(content[1]), BigInteger.Parse(content[2]));
        }

        public BigInteger Key { get; private set; }
        public BigInteger Prime { get; private set; }
        public BigInteger Generator { get; private set; }
        private const char Separator = ' ';
    }

    class ElGamalSystem
    {
        public ElGamalSystem(int dimension, string fileName)
        {
            this.dimension = dimension;
            this.fileName = fileName;
        }

        public (ElGamalKey, ElGamalKey) GenerateKeys()
        {
            BigInteger prime, safePrime, generator;
            while (true)
            {
                prime = Algos.GenetateRandomPrime(
                    BigInteger.Pow(2, dimension / 4), 
                    BigInteger.Pow(2, dimension - 1),
                    Algos.MillerRabin
                );
                safePrime = 2 * prime + 1;
                if (Algos.MillerRabin(safePrime))
                {
                    break;
                }
            }
            while (true)
            {
                generator = Algos.GenetateRandomPrime(3, safePrime - 2, Algos.MillerRabin);
                if (BigInteger.ModPow(generator, 2, safePrime ) != 1 && BigInteger.ModPow(generator, prime, safePrime) != 1)
                {
                    break;
                }
            }
            var closeKey = new ElGamalKey(Algos.GenerateRandom(2, safePrime - 2), generator, safePrime);
            var openKey = new ElGamalKey(BigInteger.ModPow(generator, closeKey.Key, safePrime), generator, safePrime);
            return (openKey, closeKey);
        }

        public ElGamalSignature CreateSignature(ElGamalKey closeKey)
        {
            using var sHA256 = SHA256.Create(); ;
            using var file = File.OpenRead(fileName);
            var hash = new BigInteger(sHA256.ComputeHash(file), true);
            var k = Algos.GenerateRandomCoprime(closeKey.Prime - 1);
            var r = BigInteger.ModPow(closeKey.Generator, k, closeKey.Prime);
            var s = ((hash - closeKey.Key * r) * Algos.ReverseByMod(k, closeKey.Prime - 1)) % (closeKey.Prime - 1);
            return new ElGamalSignature(r, s);
        }

        public bool IsValidSignature(ElGamalSignature signature, ElGamalKey openKey)
        {
            if (!(0 < signature.r && signature.r < openKey.Prime) || !(0 < signature.s && signature.s < openKey.Prime - 1))
            {
                return false;
            }
            using var sHA256 = SHA256.Create(); ;
            using var file = File.OpenRead(fileName);
            var hash = new BigInteger(sHA256.ComputeHash(file), true);
            return (BigInteger.ModPow(openKey.Key, signature.r, openKey.Prime)
                    * BigInteger.ModPow(signature.r, signature.s, openKey.Prime)) % openKey.Prime
                    == BigInteger.ModPow(openKey.Generator, hash, openKey.Prime);
        }

        private int dimension;
        private string fileName;
    }
}