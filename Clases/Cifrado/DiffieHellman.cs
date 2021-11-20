using System.Numerics;

namespace WebApplication1.Clases.Cifrado
{
    public class DiffieHellman
    {
        private int secretNumber { get; set; }

        public int primeNumber { get; set; }

        public int generator { get; set; }

        public DiffieHellman()
        {
            this.primeNumber = 23;
            this.generator = 11;
        }

        public int GenerarLlaveComun(int secretNumber)
        {
            int claveComun = 0;
            var integer = BigInteger.ModPow(generator, secretNumber, primeNumber);
            foreach (var bytes in integer.ToByteArray())
            {
                if (bytes != 0)
                {
                    if (bytes >= 1 && bytes <= 3)
                    {
                        claveComun = claveComun + (256 * bytes);
                    }
                    else
                    {
                        claveComun = bytes;
                    }
                }
            }

            return claveComun;
        }

        public int GenerarLlaveSecreta(int claveComun, int secretNumber)
        {
            int llaveSecreta = 0;
            var integer = BigInteger.ModPow(claveComun, secretNumber, primeNumber);
            foreach (var bytes in integer.ToByteArray())
            {
                if (bytes != 0)
                {
                    if (bytes >= 1 && bytes <= 3)
                    {
                        llaveSecreta = llaveSecreta + (256 * bytes);
                    }
                    else
                    {
                        llaveSecreta = bytes;
                    }
                }
            }

            return llaveSecreta;
        }
    }
}
