using System;
using Crypto;

namespace Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            var elGamalSystem = new ElGamalSystem(128, "text.txt");
            var (openKey, closeKey) = elGamalSystem.GenerateKeys();
            var signature = elGamalSystem.CreateSignature(closeKey);
            if (!elGamalSystem.IsValidSignature(signature, openKey))
            {
                throw new InvalidProgramException("Signature doesn't valid!!!");
            }
            else
            {
                Console.WriteLine("Signature is valid!");
            }
        }
    }
}
