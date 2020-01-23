using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace Crypto
{
    class Curve
    {
        public Curve(BigInteger a, BigInteger b, CurvePoint G, BigInteger n, BigInteger h, BigInteger p)
        {
            this.a = a;
            this.b = b;
            this.G = G;
            this.n = n;
            this.h = h;
            this.p = p;
        }

        public BigInteger p { get; private set; }
        public BigInteger a { get; private set; }
        public BigInteger b { get; private set; }
        public CurvePoint G { get; private set; }
        public BigInteger n { get; private set; }
        public BigInteger h { get; private set; }
    }

    class Signature
    {
        public Signature(BigInteger r, BigInteger s)
        {
            this.r = r;
            this.s = s;
        }

        public BigInteger r { get; private set; }
        public BigInteger s { get; private set; }
    }

    class ECDSA
    {
        public ECDSA(int dimension, string fileName, Curve curve)
        {
            this.dimension = dimension;
            this.fileName = fileName;
            Curve = curve;
        }

        public (CurvePoint, BigInteger) CreateKeys()
        {
            var closeKey = Algos.GenerateRandom(1, Curve.n - 1);
            var openKey = Curve.G.MulOnScalar(closeKey);
            return (openKey, closeKey);
        }

        public Signature CreateSignature(BigInteger closeKey)
        {
            var z = GetHash();
            while (true)
            {
                var k = Algos.GenerateRandom(1, Curve.n - 1);
                var P = Curve.G.MulOnScalar(k);
                var r = P.x % Curve.n;
                if (r == 0)
                {
                    continue;
                }
                var s = (Inverse(k, Curve.n) * (z + r * closeKey)) % Curve.n;
                if (s == 0)
                {
                    continue;
                }
                return new Signature(r, s);
            }
        }

        public bool CheckSignature(Signature signature, CurvePoint openKey)
        {
            var z = GetHash();
            var w = Inverse(signature.s, Curve.n);
            var u = (w * z) % Curve.n;
            var v = (w * signature.r) % Curve.n;
            var P = Curve.G.MulOnScalar(u) + openKey.MulOnScalar(v);
            return signature.r % Curve.n == P.x % Curve.n;
        }

        static public BigInteger Inverse(BigInteger n, BigInteger m)
        {
            if (n < 0)
            {
                return m - Inverse(-n, m);
            }
            var gcd = Algos.EuclidExtended(n, m, out BigInteger x, out BigInteger y);
            if ((n * x + m * y) % m != gcd || gcd != 1)
            {
                throw new InvalidProgramException("Inverse don't exist");
            }
            return x % m;
        }

        private int CountBits(BigInteger n)
        {
            //shit as fuck
            var copy = new BigInteger(n.ToByteArray());
            var count = 0;
            while (copy > 0)
            {
                ++count;
                copy >>= 1;
            }
            return count;
        }

        private BigInteger GetHash()
        {
            using var sha256 = SHA256.Create();
            using var file = File.OpenRead(fileName);
            var hash = new BigInteger(sha256.ComputeHash(file), true);
            var bitesZ = CountBits(hash);
            var bitesN = CountBits(Curve.n);
            hash <<= (bitesZ > bitesN) ? bitesZ - bitesN : 0;
            return hash;
        }

        private int dimension;
        private string fileName;
        public static Curve Curve { get; private set; }
    }
}
