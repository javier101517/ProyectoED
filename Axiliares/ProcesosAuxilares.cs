using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Axiliares
{
    public class ProcesosAuxilares
    {
        public Chats ActualizarMenajse(string UsarioEnvia, string Mensaje, string ConversacionId)
        {
            try
            {
                DateTime fecha = DateTime.Now;
                Conversacion nuevoMensaje = new Conversacion();
                nuevoMensaje.Fecha = fecha.ToString();
                nuevoMensaje.Mensaje = Mensaje;
                nuevoMensaje.Usuario = UsarioEnvia;

                Mongo mongo = new Mongo();
                Chats chat = mongo.GetChat(ConversacionId);
                List<Conversacion> historial = new List<Conversacion>(chat.Historial);
                historial.Add(nuevoMensaje);

                int mensajesNuevosRecibe = int.Parse(chat.MensajesNuevosRecibe);
                mensajesNuevosRecibe++;
                chat.MensajesNuevosRecibe = mensajesNuevosRecibe.ToString();
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
