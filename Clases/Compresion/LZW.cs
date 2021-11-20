using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace WebApplication1.Clases.Compresion
{
    public class LZW
    {
        Dictionary<string, int> Diccionario { get; set; }

        private double CompressRatio { get; set; }

        private double CompressFactor { get; set; }

        private int ReductionPer { get; set; }

        private string FileName { get; set; }

        private string PathFileHUFF { get; set; }

        private FileInfo originalFile { get; set; }

        private FileInfo compressFile { get; set; }

        public LZW(FileInfo _originalFile, FileInfo _compressFile)
        {
            this.originalFile = _originalFile;
            this.compressFile = _compressFile;

            if (_originalFile != null && _compressFile == null)
            {
                this.FileName = _originalFile.FullName;
                this.PathFileHUFF = this.FileName.Replace(_originalFile.Extension, ".lzw");
            }
            if (_originalFile == null && _compressFile != null)
            {
                this.PathFileHUFF = _compressFile.FullName;
                this.FileName = this.PathFileHUFF.Replace(_compressFile.Extension, ".txt");
            }
            if (_originalFile == null && _compressFile == null)
            {
                this.FileName = _originalFile.FullName;
                this.PathFileHUFF = _compressFile.FullName;
            }
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // COMPRESIÓN

        public List<byte> Comprimir()
        {
            List<byte> listaByteCompresion = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoOrigin();
            Dictionary<string, int> Diccionario = CrearDiccionario(listaCharArchivo);
            List<int> listaTextoCompresion = CrearListaCompresion(Diccionario, listaCharArchivo);

            listaByteCompresion = CrearListaByteCompresion(Diccionario, listaTextoCompresion);
            ObtenerCompressRatio(listaByteCompresion.Count, listaCharArchivo.Count);
            ObtenerCompressFactor(listaCharArchivo.Count, listaByteCompresion.Count);

            return listaByteCompresion;
        }

        public char[] ComprimirArrego(char[] arrayOrigin)
        {
            List<byte> listaByteCompresion = new List<byte>();

            List<int> listaCharArchivo = new List<int>();
            for (int x = 0; x < arrayOrigin.Length; x++)
            {
                listaCharArchivo.Add(arrayOrigin[x]);
            }

            Dictionary<string, int> Diccionario = CrearDiccionario(listaCharArchivo);
            List<int> listaTextoCompresion = CrearListaCompresion(Diccionario, listaCharArchivo);

            listaByteCompresion = CrearListaByteCompresion(Diccionario, listaTextoCompresion);
            ObtenerCompressRatio(listaByteCompresion.Count, listaCharArchivo.Count);
            ObtenerCompressFactor(listaCharArchivo.Count, listaByteCompresion.Count);

            char[] lstCompress = new char[listaByteCompresion.Count];
            for (int y = 0; y < listaByteCompresion.Count; y++)
            {
                lstCompress[y] = (char)listaByteCompresion.ElementAt(y);
            }

            return lstCompress;
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

        private Dictionary<string, int> CrearDiccionario(List<int> _listaCharArchivo)
        {
            Dictionary<string, int> Diccionario = new Dictionary<string, int>();

            int NoASCII = -1, Cont = 1;
            string caracter = string.Empty;
            for (int x = 0; x < _listaCharArchivo.Count; x++)
            {
                NoASCII = _listaCharArchivo.ElementAt(x);
                caracter = Convert.ToString((char)NoASCII);
                if (!Diccionario.ContainsKey(caracter))
                {
                    Diccionario.Add(caracter, Cont);
                    Cont++;
                }
            }
            //var sortedDict = from entry in Diccionario orderby entry.Key ascending select entry;

            return Diccionario;
        }

        private List<int> CrearListaCompresion(Dictionary<string, int> _Diccionario, List<int> _listaCharArchivo)
        {
            Dictionary<string, int> Diccionario = new Dictionary<string, int>();
            List<int> listaCompresion = new List<int>();

            string caracter = string.Empty, nuevo = string.Empty;
            int NoASCII = -1, previo = 0;

            for (int x = 0; x < _Diccionario.Count; x++)
            {
                Diccionario.Add(_Diccionario.ElementAt(x).Key, _Diccionario.ElementAt(x).Value);
            }

            int Cont = Diccionario.Count + 1;
            for (int y = 0; y < _listaCharArchivo.Count; y++)
            {
                NoASCII = _listaCharArchivo.ElementAt(y);
                caracter = Convert.ToString((char)NoASCII);
                nuevo = nuevo + caracter;

                if (y != _listaCharArchivo.Count - 1)
                {
                    if (!Diccionario.ContainsKey(nuevo))
                    {
                        Diccionario.Add(nuevo, Cont);
                        string anterior = nuevo.Remove(nuevo.Length - 1);
                        nuevo = Convert.ToString(nuevo.Last());
                        Diccionario.TryGetValue(anterior, out previo);
                        listaCompresion.Add(previo);
                        Cont++;
                    }
                }
                else
                {
                    if (!Diccionario.ContainsKey(nuevo))
                    {
                        Diccionario.Add(nuevo, Cont);
                        string anterior = nuevo.Remove(nuevo.Length - 1);
                        nuevo = Convert.ToString(nuevo.Last());
                        Diccionario.TryGetValue(anterior, out previo);
                        listaCompresion.Add(previo);
                        Diccionario.TryGetValue(nuevo, out previo);
                        listaCompresion.Add(previo);
                        Cont++;
                    }
                    else
                    {
                        Diccionario.TryGetValue(nuevo, out previo);
                        listaCompresion.Add(previo);
                    }
                }
            }
            listaCompresion.Add(Diccionario.Count);

            return listaCompresion;
        }

        private List<byte> CrearListaByteCompresion(Dictionary<string, int> _Diccionario, List<int> _listaCompresion)
        {
            List<byte> ListaByteCompresion = new List<byte>();
            string textoComprimido = string.Empty;
            string strbyte = string.Empty;
            int maxDiccionario = _listaCompresion.Last();
            double d_bitLong = Math.Log(maxDiccionario, 2);
            int bitLong = Convert.ToInt16(d_bitLong);
            if (d_bitLong != Math.Floor(d_bitLong))
            {
                bitLong++;
            }
            _listaCompresion.RemoveAt(_listaCompresion.Count - 1);

            ListaByteCompresion.Add(Convert.ToByte(bitLong));
            ListaByteCompresion.Add(Convert.ToByte(_Diccionario.Count));

            for (int x = 0; x < _Diccionario.Count; x++)
            {
                ListaByteCompresion.Add(Convert.ToByte(_Diccionario.ElementAt(x).Key.ElementAt(0)));
            }



            for (int y = 0; y < _listaCompresion.Count; y++)
            {
                strbyte = Convert.ToString(_listaCompresion.ElementAt(y), 2);
                strbyte = strbyte.PadLeft(bitLong, '0');
                textoComprimido = textoComprimido + strbyte;
            }

            if ((textoComprimido.Length % 8) != 0)
            {
                int completar = ((textoComprimido.Length / 8) + 1) * 8 - textoComprimido.Length;
                for (int i = 0; i < completar; i++)
                {
                    textoComprimido = textoComprimido + '0';
                }
            }

            strbyte = string.Empty;
            while (textoComprimido.Length != 0)
            {
                strbyte = textoComprimido.Substring(0, 8);
                textoComprimido = textoComprimido.Substring(8);
                //strbyte = strbyte + textoComprimido.ElementAt(z);
                if (strbyte.Length == 8)
                {
                    ListaByteCompresion.Add(Convert.ToByte(strbyte, 2));
                    strbyte = string.Empty;
                }
            }

            return ListaByteCompresion;
        }

        private void ObtenerCompressRatio(int _noBytesCompress, int _noBytesOrigin)
        {
            this.CompressRatio = Convert.ToDouble(Decimal.Divide(_noBytesCompress, _noBytesOrigin));
            this.CompressRatio = Math.Round(this.CompressRatio, 2);

            this.ReductionPer = 100 - Convert.ToInt32(this.CompressRatio * 100);
        }

        private void ObtenerCompressFactor(int _noBytesOrigin, int _noBytesCompress)
        {
            this.CompressFactor = Convert.ToDouble(Decimal.Divide(_noBytesOrigin, _noBytesCompress));
            this.CompressFactor = Math.Round(this.CompressFactor, 2);
        }

        public void CrearArchivoCompresion()
        {
            if (File.Exists(compressFile.FullName))
            {
                File.Delete(compressFile.FullName);
            }

            //File.Create(PathFileHUFF);
            List<byte> _FileCompress = Comprimir();
            FileStream stream = new FileStream(compressFile.FullName, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            for (int x = 0; x < _FileCompress.Count; x++)
            {
                writer.Write(_FileCompress.ElementAt(x));
            }

            writer.Close();
            stream.Close();

            Console.WriteLine("\n\nARCHIVO COMPRIMIDO EXITOSAMENTE");
            Console.WriteLine("\nCompress Ratio: " + CompressRatio);
            Console.WriteLine("\nCompress Factor: " + CompressFactor);
            Console.WriteLine("\nReductionPer: " + ReductionPer);
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // DESCOMPRESIÓN

        public List<byte> Descomprimir()
        {
            List<byte> listaByteDescompresion = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoCompress();
            int bitLong = listaCharArchivo.ElementAt(0);
            listaCharArchivo.RemoveAt(0);
            Dictionary<int, string> Diccionario = CrearDiccionarioDescompresion(listaCharArchivo);
            listaByteDescompresion = CrearListaByteDescompresion(Diccionario, listaCharArchivo, bitLong);

            return listaByteDescompresion;
        }

        public char[] DescomprimirArreglo(char[] arrayCompress)
        {
            List<byte> listaByteDescompresion = new List<byte>();

            List<int> listaCharArchivo = new List<int>();
            for (int x = 0; x < arrayCompress.Length; x++)
            {
                listaCharArchivo.Add(arrayCompress[x]);
            }

            int bitLong = listaCharArchivo.ElementAt(0);
            listaCharArchivo.RemoveAt(0);
            Dictionary<int, string> Diccionario = CrearDiccionarioDescompresion(listaCharArchivo);
            listaByteDescompresion = CrearListaByteDescompresion(Diccionario, listaCharArchivo, bitLong);

            char[] lstDescompress = new char[listaByteDescompresion.Count];
            for (int y = 0; y < listaByteDescompresion.Count; y++)
            {
                lstDescompress[y] = (char)listaByteDescompresion.ElementAt(y);
            }

            return lstDescompress;
        }

        private List<int> LecturaArchivoCompress()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            FileStream stream = compressFile.Open(FileMode.Open, FileAccess.Read);
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

        private Dictionary<int, string> CrearDiccionarioDescompresion(List<int> _listaCharArchivo)
        {
            Dictionary<int, string> Diccionario = new Dictionary<int, string>();

            int NoASCII = -1, Cont = 1;
            string caracter = string.Empty;
            int numDiccionario = _listaCharArchivo.ElementAt(0);
            _listaCharArchivo.RemoveAt(0);

            for (int x = 0; x < numDiccionario; x++)
            {
                NoASCII = _listaCharArchivo.ElementAt(0);
                caracter = Convert.ToString((char)NoASCII);
                if (!Diccionario.ContainsValue(caracter))
                {
                    Diccionario.Add(Cont, caracter);
                    Cont++;
                }
                _listaCharArchivo.RemoveAt(0);
            }
            //var sortedDict = from entry in Diccionario orderby entry.Key ascending select entry;

            return Diccionario;
        }

        private List<byte> CrearListaByteDescompresion(Dictionary<int, string> _Diccionario, List<int> _listaCharArchivo, int _bitLong)
        {
            List<byte> listaByteDescompresion = new List<byte>();

            string textoBits = string.Empty;
            string strbyte = string.Empty;
            for (int y = 0; y < _listaCharArchivo.Count; y++)
            {
                strbyte = Convert.ToString(_listaCharArchivo.ElementAt(y), 2);
                strbyte = strbyte.PadLeft(8, '0');
                textoBits = textoBits + strbyte;
            }

            int Cont = _Diccionario.Count + 1;
            strbyte = string.Empty;
            string previo = string.Empty;
            string textoOriginal = string.Empty;

            while (textoBits.Length > _bitLong)
            {
                strbyte = textoBits.Substring(0, _bitLong);
                //strbyte = strbyte + textoBits.ElementAt(0);
                textoBits = textoBits.Substring(_bitLong);
                if (strbyte.Length == _bitLong)
                {
                    int key = Convert.ToInt32(strbyte, 2);

                    if (key != 0)
                    {
                        if (previo == string.Empty)
                        {
                            _Diccionario.TryGetValue(key, out previo);
                            strbyte = string.Empty;

                            textoOriginal = textoOriginal + previo;
                        }
                        else
                        {
                            strbyte = string.Empty;

                            string actual = string.Empty;
                            _Diccionario.TryGetValue(key, out actual);

                            if (actual != null)
                            {
                                string nuevo = previo + actual.ElementAt(0);
                                _Diccionario.Add(Cont, nuevo);
                                Cont++;
                                textoOriginal = textoOriginal + actual;

                                previo = actual;
                            }
                            else
                            {
                                string nuevo = previo + previo.ElementAt(0);
                                _Diccionario.Add(Cont, nuevo);
                                Cont++;
                                textoOriginal = textoOriginal + nuevo;
                            }
                        }
                    }
                }

                if (textoOriginal.Length >= 1000)
                {
                    for (int x = 0; x < textoOriginal.Length; x++)
                    {
                        char caracter = textoOriginal.ElementAt(x);
                        byte byteASCII;
                        if (caracter != '\\')
                        {
                            byteASCII = Convert.ToByte(caracter);
                            listaByteDescompresion.Add(byteASCII);
                            //Console.Write((char)byteASCII);
                        }
                        else
                        {
                            x++;
                            if (textoOriginal.ElementAt(x) == 'r')
                            {
                                byteASCII = 13;
                                listaByteDescompresion.Add(byteASCII);
                                x++;
                            }
                            if (textoOriginal.ElementAt(x) == 'n')
                            {
                                byteASCII = 10;
                                listaByteDescompresion.Add(byteASCII);
                                x++;
                            }
                        }
                    }
                    textoOriginal = string.Empty;
                }
            }

            if (textoOriginal.Length > 0)
            {
                for (int x = 0; x < textoOriginal.Length; x++)
                {
                    char caracter = textoOriginal.ElementAt(x);
                    byte byteASCII;
                    if (caracter != '\\')
                    {
                        byteASCII = Convert.ToByte(caracter);
                        listaByteDescompresion.Add(byteASCII);
                        //Console.Write((char)byteASCII);
                    }
                    else
                    {
                        x++;
                        if (textoOriginal.ElementAt(x) == 'r')
                        {
                            byteASCII = 13;
                            listaByteDescompresion.Add(byteASCII);
                            x++;
                        }
                        if (textoOriginal.ElementAt(x) == 'n')
                        {
                            byteASCII = 10;
                            listaByteDescompresion.Add(byteASCII);
                            x++;
                        }
                    }
                }
                textoOriginal = string.Empty;
            }

            return listaByteDescompresion;
        }

        public void CrearArchivoDesompresion()
        {

            if (File.Exists(originalFile.FullName))
            {
                File.Delete(originalFile.FullName);
            }

            //File.Create(PathFileHUFF);
            List<byte> _FileDecompress = Descomprimir();
            FileStream stream = new FileStream(originalFile.FullName, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            for (int x = 0; x < _FileDecompress.Count; x++)
            {
                writer.Write(_FileDecompress.ElementAt(x));
            }

            writer.Close();
            stream.Close();

            Console.WriteLine("\n\nARCHIVO DESCOMPRIMIDO EXITOSAMENTE");
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////
        // JSON
        //public Compresion ObtenerDatosCompresion()
        //{
        //    Compresion infoCompress = new Compresion();
        //    infoCompress.FileName = this.FileName;
        //    infoCompress.PathFileHUFF = this.PathFileHUFF;
        //    infoCompress.CompressRatio = this.CompressRatio;
        //    infoCompress.CompressFactor = this.CompressFactor;
        //    infoCompress.ReductionPer = this.ReductionPer;

        //    return infoCompress;
        //}
    }
}
