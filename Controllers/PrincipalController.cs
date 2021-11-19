using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: PrincipalController
        public IActionResult Index()
        {
            Mongo mongo = new Mongo();
            RespuestaPantallaPrincipal respuesta = new RespuestaPantallaPrincipal();
            respuesta.Usuario = mongo.GetUsuario("javier@javier.com");
            respuesta.Historial = mongo.GetChats("javier@javier.com");
            return View(respuesta);
        }

        [HttpGet]
        public ActionResult AceptarSolicitud(string usuario, string invitado)
        {
            Mongo mongo = new Mongo();
            Usuario respuesta = mongo.GetUsuario(usuario);
            List<string> arreglo = new List<string>();

            if (respuesta == null)
            {
                arreglo.Add(invitado);
            }
            else
            {
                arreglo = new List<string>(respuesta.Contactos);
                arreglo.Add(invitado);
            }

            bool respuestaActualizar = mongo.ActualizarContactos(usuario, arreglo.ToArray());

            if (respuestaActualizar)
            {
                mongo.EliminarSolicitud(usuario, invitado);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public ActionResult RechazarSolicitud(string usuario, string invitado)
        {
            Mongo mongo = new Mongo();

            mongo.EliminarSolicitud(usuario, invitado);
            return Ok();
        }
    
        public IActionResult Chat(string id)
        {
            Mongo mongo = new Mongo();
            Chats chat = mongo.GetChat(id);

            return View(chat);
        }

        public IActionResult AgregarMensaje(IFormCollection collection)
        {
            DateTime fecha = DateTime.Now;
            string UsarioEnvia = collection["envia"];
            string Mensaje = collection["mensaje"];
            string ConversacionId = collection["conversacionId"];

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

            return View("Chat", chat);
        }
    
        public IActionResult IniciarChat(string id)
        {
            string[] listado = id.Split(",");

            Mongo mongo = new Mongo();
            mongo.CrearChat(listado[0], listado[1]);
            Chats chat = mongo.GetChat(listado[0], listado[1]);

            return View("Chat", chat);
        }

    }
}
