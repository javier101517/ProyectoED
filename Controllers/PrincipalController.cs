using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Axiliares;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: PrincipalController
        public IActionResult Index(string correo)
        {
            Mongo mongo = new Mongo();
            RespuestaPantallaPrincipal respuesta = new RespuestaPantallaPrincipal();
            respuesta.Usuario = mongo.GetUsuario(correo);
            respuesta.Historial = mongo.GetChats(correo);
            TempData["usuario"] = correo;
            return View(respuesta);
        }

        [HttpGet]
        public ActionResult AceptarSolicitud(string usuario, string invitado)
        {
            Mongo mongo = new Mongo();
            //Usuario respuesta = mongo.GetUsuario(usuario);
            //List<string> arreglo = new List<string>();

            //if (respuesta == null)
            //{
            //    arreglo.Add(invitado);
            //}
            //else
            //{
            //    arreglo = new List<string>(respuesta.Contactos);
            //    arreglo.Add(invitado);
            //}

            //bool respuestaActualizar = mongo.ActualizarContactos(usuario, arreglo.ToArray());
            AccionesController acciones = new AccionesController();
            if (acciones.AceptarSolicitud(usuario, invitado))
            {
                mongo.EliminarSolicitud(usuario, invitado);
                acciones.AceptarSolicitud(invitado, usuario);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public ActionResult RechazarSolicitud(string usuario, string invitado)
        {
            Mongo mongo = new Mongo();

            mongo.EliminarSolicitud(usuario, invitado);
            return Json("false");
        }
    
        public IActionResult Chat(string id, string usuario)
        {
            Mongo mongo = new Mongo();
            Chats chat = mongo.GetChat(id);
            TempData["usuario"] = usuario;
            return View(chat);
        }

        public IActionResult AgregarMensaje(IFormCollection collection)
        {
            DateTime fecha = DateTime.Now;
            string UsuarioEnvia = collection["envia"];
            string Mensaje = collection["mensaje"];
            string ConversacionId = collection["conversacionId"];

            ProcesosAuxilares procesos = new ProcesosAuxilares();
            Chats chat = procesos.ActualizarMenajse(UsuarioEnvia, Mensaje, ConversacionId);
            
            if (chat != null)
            {
                return View("Chat", chat);
            }
            else
            {
                TempData["texto"] = "Error al enviar mensaje.";
                TempData["color"] = "error";
                return View("Chat", chat);
            }
            //Conversacion nuevoMensaje = new Conversacion();
            //nuevoMensaje.Fecha = fecha.ToString();
            //nuevoMensaje.Mensaje = Mensaje;
            //nuevoMensaje.Usuario = UsarioEnvia;
            
            //Mongo mongo = new Mongo();
            //Chats chat = mongo.GetChat(ConversacionId);
            //List<Conversacion> historial = new List<Conversacion>(chat.Historial);
            //historial.Add(nuevoMensaje);

            //int mensajesNuevosRecibe = int.Parse(chat.MensajesNuevosRecibe);
            //mensajesNuevosRecibe++;
            //chat.MensajesNuevosRecibe = mensajesNuevosRecibe.ToString();
            //chat.Historial = historial.ToArray();
            //mongo.ActualizarConversacion(chat);

            
        }
    
        public IActionResult IniciarChat(string id)
        {
            string[] listado = id.Split(",");
            Mongo mongo = new Mongo();
            Chats chat = mongo.GetChat(listado[0], listado[1]);

            if ( chat != null)
            {
                return View("Chat", chat);
            }

            chat = mongo.GetChat(listado[1], listado[0]);
            if (chat != null)
            {
                return View("Chat", chat);
            }

            mongo.CrearChat(listado[0], listado[1]);
            chat = mongo.GetChat(listado[0], listado[1]);
            return View("Chat", chat);
        }

        public IActionResult AgregarArchivos(IFormFile adjunto, string envia, string conversacionId)
        {
            var result = new StringBuilder();
            using (var stream = new MemoryStream())
            {
                using (var reader = new StreamReader(adjunto.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                }
            }

            char[] letras = result.ToString().ToCharArray();

            LZWAuxiliar lzw = new LZWAuxiliar();
            RespuestaLZW respuesta = lzw.Compresion(letras);
            ProcesosAuxilares procesos = new ProcesosAuxilares();

            string diccionarioExtendido = respuesta.DiccionarioExtendido.ToArray().ToString();

            //procesos.ActualizarMenajse(envia, respuesta, conversacionId);

            return Ok();
        }

    }
}
