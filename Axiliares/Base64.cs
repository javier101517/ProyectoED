using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Axiliares
{
    public class Base64
    {
        public string Encriptar(string texto)
        {
            string respuesta = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(texto);
            respuesta = Convert.ToBase64String(encryted);

            return respuesta;
        }

        //public static string DesEncriptar(this string _cadenaAdesencriptar)
        //{
        //    string result = string.Empty;
        //    byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
        //    //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
        //    result = System.Text.Encoding.Unicode.GetString(decryted);
        //    return result;
        //}
    }
}
