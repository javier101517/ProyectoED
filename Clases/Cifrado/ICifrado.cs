using System.Collections.Generic;

namespace WebApplication1.Clases.Cifrado
{
    public interface ICifrado
    {
        List<byte> Cifrar();

        List<byte> Descifrar();
    }
}
