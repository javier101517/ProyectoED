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
        public ActionResult Index(Usuario Usuario)
        {
            return View(Usuario);
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
    }
}
