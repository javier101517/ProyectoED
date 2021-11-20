using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            //string[] integrantes = collection["integrantes"];
            //string usuarioLogueado = collection["usuarioLogueado"];
            //string nombreGrupo = collection["nombreGrupo"];

            if (integrantes.Length <= 1)
            {
                TempData["texto"] = "2 participantes minimo para crear un grupo";
                TempData["color"] = "error";
            }

            if (nombreGrupo == null || nombreGrupo == "")
            {
                TempData["texto"] = "Falta nombre del grupo.";
                TempData["color"] = "error";
            }
            Mongo mongo = new Mongo();
            mongo.CrearGrupo(nombreGrupo, usuarioLogueado);

            List<Integrante> listadoDeIntegrantes = new List<Integrante>();
            foreach (var item in integrantes)
            {
                Integrante integrante = new Integrante();
                integrante.MensajesSinLeer = "0";
                integrante.Usuario = item;
                listadoDeIntegrantes.Add(integrante);
            }

            mongo.ActualizarIntegrantesGrupo(listadoDeIntegrantes.ToArray(), usuarioLogueado, nombreGrupo);

            return Ok();
        }
    }
}
