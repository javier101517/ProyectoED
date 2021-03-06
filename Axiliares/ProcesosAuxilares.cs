using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Clases.Cifrado;
using WebApplication1.Models;

namespace WebApplication1.Axiliares
{
    public class ProcesosAuxilares
    {
        public Chats ActualizarMenajse(string UsuarioEnvia, string Mensaje, string ConversacionId, string tipoMensaje, string claveCifrada)
        {
            try
            {
                DateTime fecha = DateTime.Now;
                Conversacion nuevoMensaje = new Conversacion();
                nuevoMensaje.Fecha = fecha.ToString();
                nuevoMensaje.Mensaje = Mensaje;
                nuevoMensaje.Usuario = UsuarioEnvia;
                nuevoMensaje.tipo = tipoMensaje;
                nuevoMensaje.Estado = "0";

                Mongo mongo = new Mongo();
                Chats chat = mongo.GetChat(ConversacionId);
                chat.Clave = claveCifrada;
                List<Conversacion> historial = new List<Conversacion>(chat.Historial);
                historial.Add(nuevoMensaje);

                if (chat.Usuario1 == UsuarioEnvia)
                {
                    //guardar en usuario 2 
                    int mensajesNuevosRecibe = int.Parse(chat.MensajesNuevosUsuario2);
                    mensajesNuevosRecibe++;
                    chat.MensajesNuevosUsuario2 = mensajesNuevosRecibe.ToString();
                }
                else
                {
                    //guardar en usuario 1
                    int mensajesNuevosRecibe = int.Parse(chat.MensajesNuevosUsuario1);
                    mensajesNuevosRecibe++;
                    chat.MensajesNuevosUsuario1 = mensajesNuevosRecibe.ToString();
                }
                chat.Historial = historial.ToArray();
                mongo.ActualizarConversacion(chat);
                return chat;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public Grupo ActualizarMensajeGrupo(string UsuarioEnvia, string Mensaje, string ConversacionId, string tipoMensaje, List<int> lstLlave)
        {
            try
            {
                RSA rsa = new RSA();
                string respuesta = string.Empty;
                foreach (var caracter in Mensaje.ToCharArray())
                {
                    respuesta += Convert.ToChar(rsa.CifrarLlave((char)caracter));
                }

                DateTime fecha = DateTime.Now;
                Conversacion nuevoMensaje = new Conversacion();
                nuevoMensaje.Fecha = fecha.ToString();
                nuevoMensaje.Mensaje = respuesta;
                nuevoMensaje.Usuario = UsuarioEnvia;
                nuevoMensaje.tipo = tipoMensaje;
                nuevoMensaje.Estado = "0";

                Mongo mongo = new Mongo();
                Grupo grupo = mongo.GetGrupo(ConversacionId);
                List<Conversacion> historial = new List<Conversacion>(grupo.Historial);
                nuevoMensaje.Id = Convert.ToString(historial.Count + 1);
                historial.Add(nuevoMensaje);

                //if (grupo.Historial == UsuarioEnvia)
                //{
                //    guardar en usuario 2
                //    int mensajesNuevosRecibe = int.Parse(grupo.MensajesNuevosUsuario2);
                //    mensajesNuevosRecibe++;
                //    grupo.MensajesNuevosUsuario2 = mensajesNuevosRecibe.ToString();
                //}
                //else
                //{
                //    guardar en usuario 1
                //    int mensajesNuevosRecibe = int.Parse(grupo.MensajesNuevosUsuario1);
                //    mensajesNuevosRecibe++;
                //    grupo.MensajesNuevosUsuario1 = mensajesNuevosRecibe.ToString();
                //}
                grupo.Historial = historial.ToArray();
                mongo.ActualizarHistorialDeGrupo(grupo);
                nuevoMensaje.Mensaje = Mensaje;
                grupo.Historial[grupo.Historial.Length - 1] = nuevoMensaje;
                return grupo;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }


        public List<Conversacion> DescifrarChatParaVista(Chats chatOriginal)
        {
            List<Conversacion> nuevoHistorial = new List<Conversacion>();
            RespuestaChat respuestaChat = new RespuestaChat();
            SDES sdes = new SDES();

            foreach (var item in chatOriginal.Historial)
            {
                string[] listadoCaracteres = item.Mensaje.Split('~');

                char[] listadoFinal = new char[listadoCaracteres.Length-1];

                for (int i = 0; i < listadoCaracteres.Length-1; i++)
                {
                    if (listadoCaracteres[i] != "")
                    {
                        listadoFinal[i] = Convert.ToChar(listadoCaracteres[i]);
                    }
                }

                char[] arregloDes = sdes.DescifrarArreglo(625, chatOriginal.Clave.ToCharArray());
                string clavedescifrada = string.Empty;
                foreach (var caracter in arregloDes)
                {
                    clavedescifrada += caracter.ToString();
                }
                int i_clave = Convert.ToInt32(clavedescifrada);
                char[] listadoDescifrado = sdes.DescifrarArreglo(i_clave, listadoFinal);
                string mensajeDescifrado = "";

                foreach (var item2 in listadoDescifrado)
                {
                    mensajeDescifrado += item2;
                }

                Conversacion ConversacionDescifrada = new Conversacion();
                ConversacionDescifrada.Fecha = item.Fecha;
                ConversacionDescifrada.Mensaje = mensajeDescifrado;
                ConversacionDescifrada.Usuario = item.Usuario;
                ConversacionDescifrada.tipo = item.tipo;
                ConversacionDescifrada.Estado = item.Estado;

                nuevoHistorial.Add(ConversacionDescifrada);
            }

            return nuevoHistorial;


        }

        public void AgregarUsuarioAGrupo(string correo, Grupo grupoCreado )
        {
            Mongo mongo = new Mongo();
            Usuario usuario = mongo.GetUsuario(correo);
            List<DescripcionGrupo> listadoGrupos = new List<DescripcionGrupo>(usuario.Grupos);

            DescripcionGrupo descripcionGrupo = new DescripcionGrupo();
            descripcionGrupo.IdGrupo = grupoCreado.Id;
            descripcionGrupo.NombreGrupo = grupoCreado.NombreGrupo;

            listadoGrupos.Add(descripcionGrupo);
            usuario.Grupos = listadoGrupos.ToArray();
            mongo.ActualizarUsuarioConGrupo(usuario);
        }


    }
}
