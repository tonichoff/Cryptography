using System;
using System.Globalization;
using System.Numerics;

namespace Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            var curve = new Curve(
                0, 
                7,
                new CurvePoint(
                    BigInteger.Parse(
                        "079be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798",
                        NumberStyles.AllowHexSpecifier
                    ),
                    BigInteger.Parse(
                        "0483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8",
                        NumberStyles.AllowHexSpecifier
                    )
                ),
                BigInteger.Parse(
                    "0fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141",
                    NumberStyles.AllowHexSpecifier
                ),
                1,
                BigInteger.Parse(
                    "0fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f",
                    NumberStyles.AllowHexSpecifier
                )
            );
            var ecdsa = new ECDSA(256, "text.txt", curve);
            var (openKey, closeKey) = ecdsa.CreateKeys();
            var signature = ecdsa.CreateSignature(closeKey);
            if (ecdsa.CheckSignature(signature, openKey))
            {
                Console.WriteLine("Signature is valid");
            }
            else
            {
                Console.WriteLine(":C");
            }
        }
    }
}
