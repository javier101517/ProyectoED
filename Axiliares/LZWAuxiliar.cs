using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Axiliares
{
    public class LZWAuxiliar
    {
        public RespuestaLZW Compresion(char[] listado)
        {
            List<string> diccionarioInicial = DiccionarioInicial(listado);
            List<string> diccionarioExtendido = DiccionarioExtendido(listado, diccionarioInicial);
            List<int> respuesta = DiccionarioRespuesta(listado, diccionarioExtendido);

            RespuestaLZW respuestaLZW = new RespuestaLZW();
            respuestaLZW.DiccionarioExtendido = diccionarioExtendido;
            respuestaLZW.Respuesta = respuesta;
            return respuestaLZW;
        }

        public List<string> DiccionarioInicial(char[] listado)
        {
            List<string> diccionario = new List<string>();
            foreach (var item in listado)
            {
                if (!diccionario.Contains(item.ToString()))
                {
                    diccionario.Add(item.ToString());
                }
            }
            return diccionario;
        }

        public List<string> DiccionarioExtendido(char[] listado, List<string> diccionarioInicial)
        {
            List<string> diccionario = diccionarioInicial;
            string palabra = "";
            for (int i = 0; i < listado.Length; i++)
            {
                if (i != 0)
                {
                    palabra += listado[i].ToString();

                }
                else
                {
                    palabra = listado[i].ToString();
                }
                if (!diccionario.Contains(palabra))
                {
                    diccionario.Add(palabra);
                    palabra = listado[i].ToString();
                }
            }

            return diccionario;
        }

        public List<int> DiccionarioRespuesta(char[] listado, List<string> diccionarioExtendido)
        {
            List<int> respuesta = new List<int>();
            string palabra = "";
            for (int i = 0; i < listado.Length; i++)
            {
                int posicion = 0;
                palabra += listado[i].ToString();
                if (diccionarioExtendido.Contains(palabra))
                {
                    posicion = diccionarioExtendido.IndexOf(palabra);

                    if (!respuesta.Contains(posicion))
                    {
                        respuesta.Add(posicion);
                        palabra = "";
                    }
                }
            }

            return respuesta;
        }
    }
}
