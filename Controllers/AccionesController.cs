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
            string id = collection["id"];

            Mongo mongo = new Mongo();
            Usuario miUsuario = mongo.GetUsuario(id);

            List<string> contactos = new List<string>(miUsuario.Contactos);
            if (contactos.Contains(correoAgregar))
            {

                //si existe, regresarle un mensaje que ya existe
            }
            else
            {
                Usuario usuarioInv = mongo.GetUsuario(id);
                List<string> contactosInv = new List<string>(usuarioInv.Solicitudes);
                contactosInv.Add(id);
                mongo.ActualizarSolicitudes(correoAgregar, contactosInv.ToArray());
            }

            //si no existe agregarlo a notificaciones
            return View();
        }


    }
}
