using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ContactosController : Controller
    {
        // GET: ContactosController
        public ActionResult Index(string id)
        {
            Mongo mongo = new Mongo();
            Usuario usuario =  mongo.GetUsuario(id);
            List<string> listado = new List<string>(usuario.Contactos);

            RespuestaPantallaContactos respuesta = new RespuestaPantallaContactos();
            respuesta.Usuario = usuario;
            respuesta.Contactos = listado;
            return View(respuesta);
        }

        public IActionResult Delete(string id)
        {
            try
            {
                string[] usuarios = id.Split(",");
                Mongo mongo = new Mongo();
                if (mongo.EliminarContacto(usuarios[0], usuarios[1]))
                {
                    ViewData["texto"] = "Usuario eliminado correctamente";
                    ViewData["Color"] = "alert alert-success";
                }
                else
                {
                    ViewData["texto"] = "Error al eliminar el usuario, intente mas tarde.";
                    ViewData["Color"] = "alert alert-danger";
                }
                return RedirectToAction("Index", new { id = usuarios[0] });
            }
            catch
            {
                ViewData["texto"] = "Error al eliminar el usuario, intente mas tarde.";
                ViewData["Color"] = "alert alert-danger";
                return View();
            }
        }
    }
}
