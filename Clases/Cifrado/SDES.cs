using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cifrado.Clases
{
    public class SDES
    {
        public string FileName { get; set; }

        public string PathFileCipher { get; set; }
        private string llaveSecreta { get; set; }

        private string K1 { get; set; }

        private string K2 { get; set; }

        private FileInfo originalFile { get; set; }

        private FileInfo cipherFile { get; set; }

        public SDES()
        {
            this.originalFile = null;
            this.cipherFile = null;
        }

        public SDES(FileInfo _originalFile, FileInfo _cipherFile)
        {
            this.originalFile = _originalFile;
            this.cipherFile = _cipherFile;
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GENERACIÓN DE LLAVE
        private string P10(string _llave)
        {
            char[] arregloLlave = _llave.ToCharArray();
            char[] arregloPermutado = new char[10];

            arregloPermutado[0] = arregloLlave[2];
            arregloPermutado[1] = arregloLlave[4];
            arregloPermutado[2] = arregloLlave[1];
            arregloPermutado[3] = arregloLlave[6];
            arregloPermutado[4] = arregloLlave[3];
            arregloPermutado[5] = arregloLlave[9];
            arregloPermutado[6] = arregloLlave[0];
            arregloPermutado[7] = arregloLlave[8];
            arregloPermutado[8] = arregloLlave[7];
            arregloPermutado[9] = arregloLlave[5];

            return new string(arregloPermutado);
        }

        private string P8(string _llave)
        {
            char[] arregloLlave = _llave.ToCharArray();
            char[] arregloPermutado = new char[8];

            arregloPermutado[0] = arregloLlave[5];
            arregloPermutado[1] = arregloLlave[2];
            arregloPermutado[2] = arregloLlave[6];
            arregloPermutado[3] = arregloLlave[3];
            arregloPermutado[4] = arregloLlave[7];
            arregloPermutado[5] = arregloLlave[4];
            arregloPermutado[6] = arregloLlave[9];
            arregloPermutado[7] = arregloLlave[8];

            return new string(arregloPermutado);
        }

        private string LS_1(string _arreglo)
        {
            char[] arregloBits = _arreglo.ToCharArray();
            char[] arregloLS = new char[5];

            arregloLS[0] = arregloBits[1];
            arregloLS[1] = arregloBits[2];
            arregloLS[2] = arregloBits[3];
            arregloLS[3] = arregloBits[4];
            arregloLS[4] = arregloBits[0];

            return new string(arregloLS);
        }

        private string LS_2(string _arreglo)
        {
            char[] arregloBits = _arreglo.ToCharArray();
            char[] arregloLS = new char[5];

            arregloLS[0] = arregloBits[2];
            arregloLS[1] = arregloBits[3];
            arregloLS[2] = arregloBits[4];
            arregloLS[3] = arregloBits[0];
            arregloLS[4] = arregloBits[1];

            return new string(arregloLS);
        }

        public void ObtenerLlave(string _llave)
        {
            this.llaveSecreta = _llave;

            GenerarLlaves();
        }

        public void GenerarLlaves()
        {
            string llave = P10(llaveSecreta);
            llave = LS_1(llave.Substring(0, 5)) + LS_1(llave.Substring(5, 5));
            K1 = P8(llave);
            llave = LS_2(llave.Substring(0, 5)) + LS_2(llave.Substring(5, 5));
            K2 = P8(llave);
        }
        
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // FUNCIONES

        private string IP(string _arreglo)
        {
            char[] arreglo = _arreglo.ToCharArray();
            char[] arregloPermutado = new char[8];

            arregloPermutado[0] = arreglo[1];
            arregloPermutado[1] = arreglo[5];
            arregloPermutado[2] = arreglo[2];
            arregloPermutado[3] = arreglo[0];
            arregloPermutado[4] = arreglo[3];
            arregloPermutado[5] = arreglo[7];
            arregloPermutado[6] = arreglo[4];
            arregloPermutado[7] = arreglo[6];

            return new string(arregloPermutado);
        }

        private string IP_1(string _arreglo)
        {
            char[] arreglo = _arreglo.ToCharArray();
            char[] arregloIpermutado = new char[8];

            arregloIpermutado[0] = arreglo[3];
            arregloIpermutado[1] = arreglo[0];
            arregloIpermutado[2] = arreglo[2];
            arregloIpermutado[3] = arreglo[4];
            arregloIpermutado[4] = arreglo[6];
            arregloIpermutado[5] = arreglo[1];
            arregloIpermutado[6] = arreglo[7];
            arregloIpermutado[7] = arreglo[5];

            return new string(arregloIpermutado);
        }

        private string EP(string _arreglo)
        {
            char[] arreglo = _arreglo.ToCharArray();
            char[] arregloExpandido = new char[8];

            arregloExpandido[0] = arreglo[3];
            arregloExpandido[1] = arreglo[0];
            arregloExpandido[2] = arreglo[1];
            arregloExpandido[3] = arreglo[2];
            arregloExpandido[4] = arreglo[1];
            arregloExpandido[5] = arreglo[2];
            arregloExpandido[6] = arreglo[3];
            arregloExpandido[7] = arreglo[0];

            return new string(arregloExpandido);
        }

        private string P4(string _arreglo)
        {
            char[] arreglo = _arreglo.ToCharArray();
            char[] arregloPermutado = new char[4];

            arregloPermutado[0] = arreglo[1];
            arregloPermutado[1] = arreglo[3];
            arregloPermutado[2] = arreglo[2];
            arregloPermutado[3] = arreglo[0];

            return new string(arregloPermutado);
        }

        private string NOR(string _arreglo, string _llave)
        {
            char[] arreglo = _arreglo.ToCharArray();
            char[] arregloLlave = _llave.ToCharArray();
            char[] arregloNOR = new char[_arreglo.Length];

            for (int x = 0; x < _arreglo.Length; x++)
            {
                if (arreglo[x] == arregloLlave[x])
                    arregloNOR[x] = '0';
                else
                    arregloNOR[x] = '1';
            }

            return new string(arregloNOR);
        }

        private string S0(string _arreglo)
        {
            string[,] matrizS0 = { 
                { "01", "00", "11", "10" }, 
                { "11", "10", "01", "00" },
                { "00", "10", "01", "11" },
                { "11", "01", "11", "10" },
            };

            int fila = Convert.ToInt16(_arreglo.ElementAt(0).ToString() + _arreglo.ElementAt(3).ToString(), 2);
            int col = Convert.ToInt16(_arreglo.ElementAt(1).ToString() + _arreglo.ElementAt(2).ToString(), 2);

            return matrizS0[fila, col];
        }

        private string S1(string _arreglo)
        {
            string[,] matrizS1 = {
                { "00", "01", "10", "11" },
                { "10", "00", "01", "11" },
                { "11", "00", "01", "00" },
                { "10", "01", "00", "11" },
            };

            int fila = Convert.ToInt16(_arreglo.ElementAt(0).ToString() + _arreglo.ElementAt(3).ToString(), 2);
            int col = Convert.ToInt16(_arreglo.ElementAt(1).ToString() + _arreglo.ElementAt(2).ToString(), 2);

            return matrizS1[fila, col];
        }

        private string Ronda(string _arreglo, string _llave)
        {
            string arreglo = _arreglo.ToString();
            string left = arreglo.Substring(0, 4);
            string right = arreglo.Substring(4, 4);

            arreglo = EP(right);
            arreglo = NOR(arreglo, _llave);
            arreglo = S0(arreglo.Substring(0, 4)) + S1(arreglo.Substring(4, 4));
            arreglo = P4(arreglo);
            arreglo = NOR(left, arreglo);
            arreglo = arreglo + right;

            return arreglo;
        }
        
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // CIFRADO

        public byte CifrarASCII(int _ascii)
        {
            string arreglo = Convert.ToString(_ascii, 2);
            arreglo = arreglo.PadLeft(8, '0');

            arreglo = IP(arreglo);
            arreglo = Ronda(arreglo, K1);
            arreglo = arreglo.Substring(4, 4) + arreglo.Substring(0, 4);
            arreglo = Ronda(arreglo, K2);
            arreglo = IP_1(arreglo);

            return Convert.ToByte(arreglo, 2);
        }

        public char[] CifrarArreglo(char[] lstContenido)
        {
            char[] lstCifrado = new char[lstContenido.Length];
            int cont = 0;

            foreach (var caracter in lstContenido)
            {
                byte ascii = CifrarASCII(caracter);
                lstContenido[cont] = Convert.ToChar(ascii);
                cont++;
            }

            return lstCifrado;
        }

        public List<byte> Cifrar()
        {
            List<byte> listaByteCifrar = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoOrigin();
            for (int x = 0; x < listaCharArchivo.Count; x++)
            {
                listaByteCifrar.Add(CifrarASCII(listaCharArchivo[x]));
            }

            return listaByteCifrar;
        }

        private List<int> LecturaArchivoOrigin()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            FileStream stream = originalFile.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream, new UTF8Encoding());
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

        public byte DescifrarASCII(int _ascii)
        {
            string arreglo = Convert.ToString(_ascii, 2);
            arreglo = arreglo.PadLeft(8, '0');

            arreglo = IP(arreglo);
            arreglo = Ronda(arreglo, K2);
            arreglo = arreglo.Substring(4, 4) + arreglo.Substring(0, 4);
            arreglo = Ronda(arreglo, K1);
            arreglo = IP_1(arreglo);

            return Convert.ToByte(arreglo, 2);
        }

        public char[] DescifrarArreglo(char[] lstContCifrado)
        {
            char[] lstOriginal = new char[lstContCifrado.Length];
            int cont = 0;

            foreach (var caracter in lstContCifrado)
            {
                byte ascii = DescifrarASCII(caracter);
                lstOriginal[cont] = Convert.ToChar(ascii);
                cont++;
            }

            return lstOriginal;
        }

        public List<byte> Descifrar()
        {
            List<byte> listaByteDescifrar = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoCipher();
            for (int x = 0; x < listaCharArchivo.Count; x++)
            {
                listaByteDescifrar.Add(DescifrarASCII(listaCharArchivo[x]));
            }

            return listaByteDescifrar;
        }

        private List<int> LecturaArchivoCipher()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            FileStream stream = cipherFile.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream, new UTF8Encoding());
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


        public void PruebaArchivo()
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

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // JSON

        public Cifrados ObtenerDatosArchivo()
        {
            Cifrados infoCipher = new Cifrados();
            infoCipher.FileName = string.Empty;
            infoCipher.PathFileCipher = string.Empty;
            return infoCipher;
        }
    }
}
