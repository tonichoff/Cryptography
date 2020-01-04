using System;
using System.Numerics;

namespace ElGamal_signature_scheme
{
    static class Algos
    {
        public static BigInteger EuclidExtended(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
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

        public static BigInteger GenetateRandomPrime(BigInteger Min, BigInteger Max)
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
                if (MillerRabin(candidate))
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

        public static bool MillerRabin(BigInteger n)
        {
            Console.WriteLine("Start TestPrime");
            if (n % 2 == 0)
            {
                return false;
            }
            var t = n - 1;
            var s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                ++s;
            }
            var rounds = 40;
            for (var i = 0; i < rounds; ++i)
            {
                Console.WriteLine($"Round {i} from {rounds}");
                var a = GenerateRandom(2, n - 2);
                var x = BigInteger.ModPow(a, t, n);
                if (x == 1 || x == n - 1)
                {
                    continue;
                }
                var flag = false;
                for (var j = 0; j < s; ++j)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                    {
                        return false;
                    }
                    if (x == n - 1)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        public static bool Fermat(BigInteger n)
        {
            Console.WriteLine("Start TestPrime");
            if (n == 1)
            {
                return false;
            }
            else if (n == 2)
            {
                return true;
            }
            var rounds = 20;
            for (int i = 0; i < rounds; ++i)
            {
                Console.WriteLine($"Round {i} from {rounds}");
                var a = GenerateRandom(2, n - 2);
                var r = BigInteger.ModPow(a, n - 1, n);
                if (r != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public static BigInteger Sqrt(BigInteger number)
        {
            // https://social.msdn.microsoft.com/Forums/ru-RU/f9aca8c2-af21-40e4-b5bb-c9613b9db4ca/-biginteger?forum=fordesktopru.
            var root = number;
            int bitLength = 1;
            while (root / 2 != 0)
            {
                root /= 2;
                bitLength++;
            }
            bitLength = (bitLength + 1) / 2;
            root = number >> bitLength;

            var lastRoot = BigInteger.Zero;
            do
            {
                lastRoot = root;
                root = (BigInteger.Divide(number, root) + root) >> 1;
            }
            while (!((root ^ lastRoot).ToString() == "0"));
            return root;
        }

        public static BigInteger GenerateRandom(BigInteger leftBound, BigInteger rightBound)
        {
            var diff = rightBound - leftBound;
            // https://stackoverflow.com/questions/17357760/how-can-i-generate-a-random-biginteger-within-a-certain-range.
            byte[] bytes = diff.ToByteArray();
            BigInteger result;
            var random = new Random();
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
                result = new BigInteger(bytes);
            } while (result >= diff);
            result += leftBound;
            if (result < leftBound || result > rightBound || result < 0)
            {
                throw new Exception("Generated number outside of ranges");
            }
            return result;
        }
    }
}
