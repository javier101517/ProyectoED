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
            string miCorreo = collection["id"];

            Mongo mongo = new Mongo();
            Usuario miUsuario = mongo.GetUsuario(miCorreo);

            List<string> contactos = new List<string>(miUsuario.Contactos);
            if (contactos.Contains(correoAgregar))
            {

                TempData["Notificar"] = "Usuario ya se encuentra agregado en los contactos.";
                return RedirectToAction("Index", "Principal", miUsuario);
            }
            else
            {
                Usuario usuarioAgregar = mongo.GetUsuario(correoAgregar);
                if (usuarioAgregar == null)
                {
                    TempData["Notificar"] = "Usuario no existe, por favor verifique.";
                    return RedirectToAction("Index", "Principal", miUsuario);
                }
                
                List<string> usuarioAgregarSolicitudes = new List<string>(usuarioAgregar.Solicitudes);
                if (usuarioAgregarSolicitudes.Contains(miCorreo))
                {
                    TempData["Notificar"] = "Invitación ya fue enviada.";
                    return RedirectToAction("Index", "Principal", miUsuario);
                }

                usuarioAgregarSolicitudes.Add(miCorreo);
                mongo.ActualizarSolicitudes(correoAgregar, usuarioAgregarSolicitudes.ToArray());
                
                TempData["Notificar"] = "Invitación enviada.";
                return RedirectToAction("Index", "Principal", miUsuario);
            }

            
        }


    }
}
