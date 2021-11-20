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
    public class GruposController : Controller
    {
        public IActionResult Index(string usuarioLogueado)
        {
            Mongo mongo = new Mongo();

            Usuario usuarioLog = mongo.GetUsuario(usuarioLogueado);
            return View("Index", usuarioLog);
        }

        [HttpPost]
        public IActionResult CrearGrupo(string[] integrantes, string usuarioLogueado, string nombreGrupo)
        {
            Mongo mongo = new Mongo();

            if (integrantes.Length <= 1)
            {
                TempData["texto"] = "2 participantes mínimo para crear un grupo";
                TempData["color"] = "error";
                return RedirectToAction("Index", "Grupos", new { usuarioLogueado = usuarioLogueado});
            }

            if (nombreGrupo == null || nombreGrupo == "")
            {
                TempData["texto"] = "Falta nombre del grupo.";
                TempData["color"] = "error";
                return RedirectToAction("Index", "Grupos", new { usuarioLogueado = usuarioLogueado });
            }
            mongo.CrearGrupo(nombreGrupo, usuarioLogueado);

            Grupo grupoCreado = mongo.GetGrupo(usuarioLogueado, nombreGrupo);


            List<Integrante> listadoDeIntegrantes = new List<Integrante>();
            ProcesosAuxilares procesos = new ProcesosAuxilares();

            procesos.AgregarUsuarioAGrupo(usuarioLogueado, grupoCreado);
            foreach (var item in integrantes)
            {
                Integrante integrante = new Integrante();
                integrante.MensajesSinLeer = "0";
                integrante.Usuario = item;
                listadoDeIntegrantes.Add(integrante);

                procesos.AgregarUsuarioAGrupo(item, grupoCreado);
            }

            mongo.ActualizarIntegrantesGrupo(listadoDeIntegrantes.ToArray(), usuarioLogueado, nombreGrupo);


            TempData["texto"] = "Grupo creado correctamente.";
            TempData["color"] = "success";
            return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado} );
        }
        
        public IActionResult Chat(string idGrupo, string usuarioLogueado)
        {
            Mongo mongo = new Mongo();

            Grupo grupo = mongo.GetGrupo(idGrupo);

            RSA rsa = new RSA();
            RespuestaGrupo respuestaGrupo = new RespuestaGrupo();
            respuestaGrupo.GrupoOriginal = grupo;

            TempData["usuarioLogueado"] = usuarioLogueado;
            return View(grupo);
        }


        public IActionResult AgregarMensaje(IFormCollection collection)
        {
            DateTime fecha = DateTime.Now;
            string usuarioLogueado = collection["usuarioLogueado"];
            string Mensaje = collection["mensaje"];
            string ConversacionId = collection["conversacionId"];
            string TipoMensaje = collection["tipoMensaje"];

            char[] listadoMensaje = Mensaje.ToCharArray();

            ProcesosAuxilares procesos = new ProcesosAuxilares();
            Grupo grupo = procesos.ActualizarMensajeGrupo(usuarioLogueado, Mensaje, ConversacionId, TipoMensaje);
            RespuestaChat respuestaChat = new RespuestaChat();
            if (grupo != null)
            {
                //respuestaChat.chatOriginal = grupo;
                //respuestaChat.conversacionesDescifradas = procesos.DescifrarChatParaVista(grupo);
                TempData["usuarioLogueado"] = usuarioLogueado;
                return View("Chat", grupo);
            }
            else
            {
                //respuestaChat.chatOriginal = grupo;
                //respuestaChat.conversacionesDescifradas = procesos.DescifrarChatParaVista(grupo);
                TempData["texto"] = "Error al enviar mensaje.";
                TempData["color"] = "error";
                TempData["usuario"] = usuarioLogueado;
                return View("Chat", grupo);
            }
        }

        public IActionResult AgregarArchivos(IFormFile adjunto, string usuarioLogueado, string conversacionId, string tipoMensaje)
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

            

            

           
            //procesos.ActualizarMenajse(usuarioLogueado, respuesta, conversacionId, tipoMensaje);

            return Ok();
        }
    }
}
