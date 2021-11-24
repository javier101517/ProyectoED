using System.Collections.Generic;

namespace WebApplication1.Clases.Compresion
{
    public interface ICompresion
    {
        List<byte> Comprimir();

        char[] ComprimirArrego(char[] arrayOrigin);

        List<byte> Descomprimir();

        char[] DescomprimirArreglo(char[] arrayCompress);
    }
}
