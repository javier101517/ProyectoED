using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace WebApplication1.Clases.Cifrado
{
    public class RSA : ICifrado
    {
        public int[] llavePublica { get; set; }

        public int[] llavePrivada { get; set; }

        private FileInfo originalFile { get; set; }

        private FileInfo cipherFile { get; set; }

        public RSA()
        {

        }

        public RSA(FileInfo _originalFile, FileInfo _cipherFile)
        {
            this.originalFile = _originalFile;
            this.cipherFile = _cipherFile;
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GENERACIÓN DE LLAVE

        public void DefinirLlave(int _n, int _e, int _d)
        {
            this.llavePrivada = new int[] { _n, _d };
            this.llavePublica = new int[] { _n, _e };
        }

        public void GenerarLlave(int _p, int _q)
        {
            int n = _p * _q;
            Console.WriteLine("n = " + n);
            int Φn = (_p - 1) * (_q - 1);
            Console.WriteLine("Φn = " + Φn);
            //1 < e < Φ(n)
            int e = GenerarCoPrimo(n, Φn);
            Console.WriteLine("e = " + e);
            int d = GenerarMCD(Φn, e);
            Console.WriteLine("d = " + d);
            llavePrivada = new int[] { n, d };
            llavePublica = new int[] { n, e };
            Console.WriteLine("\nLlave Privada (n, d) = (" + n + ", " + d + ")");
            Console.WriteLine("\nLlave Publica (n, e) = (" + n + ", " + e + ")");
        }

        private int GenerarCoPrimo(int _numero1, int _numero2)
        {
            List<int> ListaCoPrimos = new List<int>();

            for (int x = 2; x < _numero1; x++)
            {
                bool esCoprimo1 = false;
                bool esCoprimo2 = false;

                esCoprimo1 = esCoPrimo(_numero1, x);

                if (x < _numero2)
                {
                    esCoprimo2 = esCoPrimo(_numero2, x);
                }

                if (esCoprimo1 && esCoprimo2)
                {
                    ListaCoPrimos.Add(x);
                }
            }

            Random random = new Random();

            return ListaCoPrimos.ElementAt(random.Next(0, ListaCoPrimos.Count - 1));
        }

        private bool esCoPrimo(int _a, int _b)
        {
            if ((_a == 1 && _b == 0) || (_b == 0))
            {
                if (_a == 1 && _b == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                int resto = -1;
                resto = _a % _b;

                return esCoPrimo(_b, resto);
            }
        }

        private int GenerarMCD(int _Φn, int _e)
        {
            int[,] matriz = { { _Φn, _Φn}, { _e, 1} };

            while(matriz[1,0] != 1)
            {
                int pos1 = matriz[0, 0] - (matriz[0, 0] / matriz[1, 0]) * matriz[1, 0]; 
                int pos2 = matriz[0, 1] - (matriz[0, 0] / matriz[1, 0]) * matriz[1, 1];

                if (pos1 < 0)
                {
                    while(pos1 < 0)
                    {
                        pos1 = pos1 + _Φn;
                    }
                }
                if (pos2 < 0)
                {
                    while (pos2 < 0)
                    {
                        pos2 = pos2 + _Φn;
                    }
                }

                matriz[0, 0] = matriz[1, 0];
                matriz[0, 1] = matriz[1, 1];
                matriz[1, 0] = pos1;
                matriz[1, 1] = pos2;
            }
            
            if(matriz[1, 1] == _e)
            {
                return (matriz[1, 1] + _Φn);
            }

            return matriz[1, 1];
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // CIFRADO

        public List<byte> Cifrar()
        {
            List<byte> listaByteCifrado = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoOrigin();
            foreach (var _Byte in listaCharArchivo)
            {
                var byteCifrado = BigInteger.ModPow(_Byte, llavePrivada[1], llavePrivada[0]);
                foreach (var bytes in byteCifrado.ToByteArray())
                {
                    listaByteCifrado.Add(bytes);
                }
            }

            return listaByteCifrado;
        }

        public int CifrarLlave(int llave)
        {
            int claveCifrada = 0;
            //Llave Privada = (713, 247)
            var byteCifrado = BigInteger.ModPow(llave, 247, 713);
            foreach (var bytes in byteCifrado.ToByteArray())
            {
                if (bytes != 0)
                {
                    if(bytes >= 1 && bytes <= 3)
                    {
                        claveCifrada = claveCifrada + (256 * bytes);
                    }
                    else
                    {
                        claveCifrada = bytes;
                    }
                }
            }

            return claveCifrada;
        }

        private List<int> LecturaArchivoOrigin()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            FileStream stream = originalFile.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            while (letter != -1)
            {
                try
                {
                    letter = reader.ReadByte();
                }
                catch
                {
                    letter = reader.Read();
                }

                if (letter != -1)
                {
                    ListaASCII.Add(letter);
                    Console.Write((char)letter);
                }
            }
            reader.Close();
            stream.Close();

            return ListaASCII;
        }

        public void CrearArchivoCifrado()
        {
            if (File.Exists(cipherFile.FullName))
            {
                File.Delete(cipherFile.FullName);
            }

            //File.Create(PathFileHUFF);
            List<byte> _FileCompress = Cifrar();
            FileStream stream = new FileStream(cipherFile.FullName, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            for (int x = 0; x < _FileCompress.Count; x++)
            {
                writer.Write(_FileCompress.ElementAt(x));
            }

            writer.Close();
            stream.Close();

            Console.WriteLine("\n\nARCHIVO CIFRADO EXITOSAMENTE");
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // DESCIFRADO

        public List<byte> Descifrar()
        {
            List<byte> listaByteDescifrado = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoCipher();
            foreach (var _Byte in listaCharArchivo)
            {
                var byteDescifrado = BigInteger.ModPow(_Byte, llavePublica[1], llavePublica[0]);
                foreach (var bytes in byteDescifrado.ToByteArray())
                {
                    listaByteDescifrado.Add(bytes);
                }
            }

            return listaByteDescifrado;
        }

        public int DescifrarLlave(int llaveCifrada)
        {
            int claveDescifrada = 0;
            //Llave Publica = (713, 163)
            var integer = BigInteger.ModPow(llaveCifrada, 163, 713);
            foreach (var bytes in integer.ToByteArray())
            {
                if (bytes != 0)
                {
                    if (bytes >= 1 && bytes <= 3)
                    {
                        claveDescifrada = claveDescifrada + (256 * bytes);
                    }
                    else
                    {
                        claveDescifrada = bytes;
                    }
                }
            }

            return claveDescifrada;
        }

        private List<int> LecturaArchivoCipher()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            FileStream stream = cipherFile.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            while (letter != -1)
            {
                try
                {
                    letter = reader.ReadByte();
                }
                catch
                {
                    letter = reader.Read();
                }

                if (letter != -1 && letter != 0)
                {
                    if (letter >= 1 && letter <= 8)
                    {
                        int temp = ListaASCII.Last();
                        ListaASCII.RemoveAt(ListaASCII.Count - 1);
                        temp = temp + (256 * letter);
                        ListaASCII.Add(temp);
                    }
                    else
                    {
                        ListaASCII.Add(letter);
                        Console.Write((char)letter);
                    }
                }
            }
            reader.Close();
            stream.Close();

            return ListaASCII;
        }

        public void CrearArchivoDescifrado()
        {

            if (File.Exists(originalFile.FullName))
            {
                File.Delete(originalFile.FullName);
            }

            List<byte> _FileDecipher = Descifrar();
            FileStream stream = new FileStream(originalFile.FullName, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            for (int x = 0; x < _FileDecipher.Count; x++)
            {
                writer.Write(_FileDecipher.ElementAt(x));
            }

            writer.Close();
            stream.Close();

            Console.WriteLine("\n\nARCHIVO DESCIFRADO EXITOSAMENTE");
        }
    }
}
