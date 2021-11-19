using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Axiliares
{
    public class ProcesosAuxilares
    {
        public Chats ActualizarMenajse(string UsuarioEnvia, string Mensaje, string ConversacionId)
        {
            try
            {
                DateTime fecha = DateTime.Now;
                Conversacion nuevoMensaje = new Conversacion();
                nuevoMensaje.Fecha = fecha.ToString();
                nuevoMensaje.Mensaje = Mensaje;
                nuevoMensaje.Usuario = UsuarioEnvia;

                Mongo mongo = new Mongo();
                Chats chat = mongo.GetChat(ConversacionId);
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
    }
}
