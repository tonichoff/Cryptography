using System;
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

        static public BigInteger Encrypt(BigInteger cipher, RSAKey openKey)
        {
            return BigInteger.ModPow(cipher, openKey.Key, openKey.Module);
        }

        static public BigInteger Decrypt(BigInteger cipher, RSAKey closeKey)
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
