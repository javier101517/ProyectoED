using Cifrado.Clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Axiliares;
using WebApplication1.Clases.Cifrado;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: PrincipalController
        public IActionResult Index(string usuarioLogueado)
        {
            Mongo mongo = new Mongo();
            RespuestaPantallaPrincipal respuesta = new RespuestaPantallaPrincipal();
            respuesta.UsuarioLogueado = mongo.GetUsuario(usuarioLogueado);
            respuesta.Historial = mongo.GetChats(usuarioLogueado);
            TempData["usuario"] = usuarioLogueado;
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
    
        public IActionResult Chat(string id, string usuarioLogueado)
        {
            Mongo mongo = new Mongo();
            Chats chat = mongo.GetChat(id);

            ProcesosAuxilares procesos = new ProcesosAuxilares();
            List<Conversacion> nuevoHistorial = procesos.DescifrarChatParaVista(chat);

            //List<Conversacion> nuevoHistorial = new List<Conversacion>();
            RespuestaChat respuestaChat = new RespuestaChat();
            //SDES sdes = new SDES();
            respuestaChat.chatOriginal = chat;
            respuestaChat.conversacionesDescifradas = nuevoHistorial;

            //foreach (var item in chat.Historial)
            //{
            //    string[] listadoCaracteres = item.Mensaje.Split('~');
            //    char[] listadoFinal = new char[listadoCaracteres.Length] ;

            //    for (int i = 0; i < listadoCaracteres.Length; i++)
            //    {
            //        if (listadoCaracteres[i] != "")
            //        {
            //            listadoFinal[i] =  Convert.ToChar(listadoCaracteres[i]);
            //        }
            //    } 

            //    char[] listadoDescifrado = sdes.DescifrarArreglo(listadoFinal);
            //    string mensajeDescifrado = "";

            //    foreach (var item2 in listadoDescifrado)
            //    {
            //        mensajeDescifrado += item2;
            //    }

            //    Conversacion ConversacionDescifrada = new Conversacion();
            //    ConversacionDescifrada.Fecha = item.Fecha;
            //    ConversacionDescifrada.Mensaje = mensajeDescifrado;
            //    ConversacionDescifrada.Usuario = item.Usuario;

            //    nuevoHistorial.Add(ConversacionDescifrada);
            //}

            //respuestaChat.conversacionesDescifradas = nuevoHistorial;

            if (chat.Usuario1 == usuarioLogueado)
            {
                chat.MensajesNuevosUsuario1 = "0";
            }
            else
            {
                chat.MensajesNuevosUsuario2 = "0";
            }

            mongo.ActualizarConversacion(chat);

            TempData["usuario"] = usuarioLogueado;
            return View(respuestaChat);
        }

        public IActionResult AgregarMensaje(IFormCollection collection)
        {
            DateTime fecha = DateTime.Now;
            string UsuarioEnvia = collection["envia"];
            string Mensaje = collection["mensaje"];
            string ConversacionId = collection["conversacionId"];

            char[] listadoMensaje = Mensaje.ToCharArray();

            ProcesosAuxilares procesos = new ProcesosAuxilares();

            SDES sdes = new SDES();
            char[] respuestaDelCifrado = sdes.CifrarArreglo(listadoMensaje);
            string mensajeCifrado = "";
            foreach (var item in respuestaDelCifrado)
            {
                mensajeCifrado += item + "~";
            }

            Chats chat = procesos.ActualizarMenajse(UsuarioEnvia, mensajeCifrado, ConversacionId);
            RespuestaChat respuestaChat = new RespuestaChat();
            if (chat != null)
            {
                respuestaChat.chatOriginal = chat;
                respuestaChat.conversacionesDescifradas = procesos.DescifrarChatParaVista(chat);
                TempData["usuario"] = UsuarioEnvia;
                return View("Chat", respuestaChat);
            }
            else
            {
                respuestaChat.chatOriginal = chat;
                respuestaChat.conversacionesDescifradas = procesos.DescifrarChatParaVista(chat);
                TempData["texto"] = "Error al enviar mensaje.";
                TempData["color"] = "error";
                return View("Chat", respuestaChat);
            }
        }
    
        public IActionResult IniciarChat(string id)
        {
            string[] listado = id.Split(",");
            Mongo mongo = new Mongo();
            Chats chat = mongo.GetChat(listado[0], listado[1]);
            RespuestaChat respuestaChat = new RespuestaChat();
            ProcesosAuxilares procesos = new ProcesosAuxilares();

            if ( chat != null)
            {
                respuestaChat.chatOriginal = chat;
                respuestaChat.conversacionesDescifradas = procesos.DescifrarChatParaVista(chat);
                return View("Chat", respuestaChat);
            }

            chat = mongo.GetChat(listado[1], listado[0]);
            if (chat != null)
            {
                respuestaChat.chatOriginal = chat;
                respuestaChat.conversacionesDescifradas = procesos.DescifrarChatParaVista(chat);
                return View("Chat", respuestaChat);
            }

            mongo.CrearChat(listado[0], listado[1]);
            chat = mongo.GetChat(listado[0], listado[1]);

            respuestaChat.chatOriginal = chat;
            respuestaChat.conversacionesDescifradas = null;

            return View("Chat", respuestaChat);
        }

        public IActionResult AgregarArchivos(IFormFile adjunto, string envia, string conversacionId)
        {
            //var result = new StringBuilder();
            //using (var stream = new MemoryStream())
            //{
            //    using (var reader = new StreamReader(adjunto.OpenReadStream()))
            //    {
            //        while (reader.Peek() >= 0)
            //            result.AppendLine(reader.ReadLine());
            //    }
            //}

            //char[] listado = result.ToString().ToCharArray();

            //LZWAuxiliar lzw = new LZWAuxiliar();
            //RespuestaLZW respuesta = lzw.Compresion(letras);
            //ProcesosAuxilares procesos = new ProcesosAuxilares();

            //string diccionarioExtendido = respuesta.DiccionarioExtendido.ToArray().ToString();


            //SDES sdes = new SDES();
            //List<byte> listado = sdes.Cifrar(listado);

            //procesos.ActualizarMenajse(envia, respuesta, conversacionId);

            return Ok();
        }

    }
}
