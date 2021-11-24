using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccionesController : Controller
    {
        // POST: AccionesController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarUsuario(IFormCollection collection)
        {
            string correoAgregar = collection["correo"];
            string usuarioLogueado = collection["id"];

            if (correoAgregar != usuarioLogueado)
            {
                RespuestaPantallaPrincipal respuestaPantallaPrincipal = new RespuestaPantallaPrincipal();
                Mongo mongo = new Mongo();
                Usuario miUsuario = mongo.GetUsuario(usuarioLogueado);
                List<string> contactos = new List<string>(miUsuario.Contactos);
                List<Chats> listadoChats = mongo.GetChats(usuarioLogueado);
                respuestaPantallaPrincipal.Historial = listadoChats;
                respuestaPantallaPrincipal.UsuarioLogueado = miUsuario;

                if (contactos.Contains(correoAgregar))
                {
                    TempData["texto"] = "Usuario ya se encuentra agregado en los contactos.";
                    TempData["Color"] = "error";
                    return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado });
                }

                Usuario usuario2 = mongo.GetUsuario(correoAgregar);
                List<string> contactosUsuario2 = new List<string>(usuario2.Solicitudes);
                if (contactosUsuario2.Contains(usuarioLogueado))
                {
                    TempData["texto"] = "La invitacion ya fue enviada, se espera la confirmacion.";
                    TempData["Color"] = "error";
                    return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado });
                }

                Usuario usuarioAgregar = mongo.GetUsuario(correoAgregar);
                if (usuarioAgregar == null)
                {
                    TempData["texto"] = "Usuario no existe, por favor verifique.";
                    TempData["Color"] = "error";
                    return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado });
                }

                List<string> usuarioAgregarSolicitudes = new List<string>(usuarioAgregar.Solicitudes);
                if (usuarioAgregarSolicitudes.Contains(usuarioLogueado))
                {
                    TempData["texto"] = "Invitación ya fue enviada.";
                    TempData["Color"] = "success";
                    return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado });
                }

                usuarioAgregarSolicitudes.Add(usuarioLogueado);
                mongo.ActualizarSolicitudes(correoAgregar, usuarioAgregarSolicitudes.ToArray());

                TempData["texto"] = "Invitación enviada.";
                TempData["Color"] = "success";
                return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado });
            }
            else
            {
                TempData["texto"] = "No puedes enviar invitación a tu usuario, por favor coloca otra";
                TempData["Color"] = "error";
                return RedirectToAction("Index", "Principal", new { usuarioLogueado = usuarioLogueado });
            }
        }

        public bool AceptarSolicitud(string usuario, string invitado)
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

            return mongo.ActualizarContactos(usuario, arreglo.ToArray());
        }
    }
}
