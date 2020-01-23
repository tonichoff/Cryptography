using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Crypto
{
    class CurvePoint
    {
        public CurvePoint(bool isZero)
        {
            IsZero = isZero;
        }

        public CurvePoint(BigInteger x, BigInteger y)
        {
            this.x = x;
            this.y = y;
            IsZero = false;
        }

        public CurvePoint MulOnScalar(BigInteger scalar)
        {
            if (scalar % ECDSA.Curve.n == 0 || IsZero)
            {
                return new CurvePoint(true);
            }
            if (scalar < 0)
            {
                return Neg().MulOnScalar(-scalar);
            }
            var point = new CurvePoint(x, y);
            var result = new CurvePoint(true);
            foreach (var bit in GetNextBit(scalar))
            {
                if (bit == 1)
                {
                    result += point;
                }
                point += point;
            }
            return result;
        }

        public IEnumerable<int> GetNextBit(BigInteger number)
        {
            var n = new BigInteger(number.ToByteArray());
            while (n > 0)
            {
                var result = n & 1; 
                yield return (int) result;
                n >>= 1;
            }
        }

        public CurvePoint Neg()
        {
            if (IsZero)
            {
                return new CurvePoint(true);
            }
            return new CurvePoint(x, -y % ECDSA.Curve.p);
        }

        static public CurvePoint operator +(CurvePoint a, CurvePoint b)
        {
            if (a.IsZero)
            {
                return b;
            }
            if (b.IsZero)
            {
                return a;
            }
            if (a.x == b.x && a.y != b.y)
            {
                return new CurvePoint(true);
            }
            var m = (a == b) ? (3 * a.x * a.x + ECDSA.Curve.a) * ECDSA.Inverse(2 * a.y, ECDSA.Curve.p) :
                               (a.y - b.y) * ECDSA.Inverse(a.x - b.x, ECDSA.Curve.p);
            var x = (m * m - a.x - b.x) % ECDSA.Curve.p;
            var y = (a.y + m * (x - a.x)) % ECDSA.Curve.p;
            return new CurvePoint(x, -y);
        }

        static public bool operator ==(CurvePoint a, CurvePoint b)
        {
            return (a.IsZero && b.IsZero) || (a.x == b.x && a.y == b.y);
        }

        static public bool operator !=(CurvePoint a, CurvePoint b)
        {
            if ((a.IsZero && !b.IsZero) || (!a.IsZero && b.IsZero))
            {
                return true;
            }
            if (a.IsZero && b.IsZero)
            {
                return false;
            }
            return (a.x != b.x || a.y != b.y);
        }

        public BigInteger x { get; private set; }
        public BigInteger y { get; private set; }
        public bool IsZero { get; set; }
    }
}
