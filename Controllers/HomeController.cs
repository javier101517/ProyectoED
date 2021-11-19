using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Axiliares;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(IFormCollection collection)
        {
            string usuario = collection["usuario"];
            string password = collection["password"];

            if (usuario == "" || password == "")
            {
                TempData["Notificar"] = "Usuario o contraseña incorrectos";
                return RedirectToAction("Index");
            }

            Mongo mongo = new Mongo();
            Usuario Usuario = mongo.Login(usuario, password);
            if (Usuario != null)
            {

                return RedirectToAction("Index", "Principal", new { correo = Usuario.Correo });
            }
            else
            {
                ViewData["texto"] = "Usuario o contraseña incorrectos";
                ViewData["Color"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        public IActionResult Registro(IFormCollection collection)
        {
            string email = collection["email"];
            string usuario = collection["usuario"];
            string password = collection["password"];
            string password2 = collection["password2"];

            if (usuario == "")
            {
                TempData["Notificar"] = "Debe ingresar un usuario";
                return RedirectToAction("Registrarse");
            }
            if (password != password2)
            {
                TempData["Notificar"] = "Contraseñas diferentes";
                return RedirectToAction("Registrarse");
            }
            if (email == "")
            {
                TempData["Notificar"] = "Debe ingresar un correo";
                return RedirectToAction("Registrarse");
            }

            Base64 base64 = new();
            string respuesta = base64.Encriptar(password);

            Mongo mongo = new();
            mongo.InsertarUsuario("Usuarios", email, respuesta, usuario);

            return RedirectToAction("Index");
        }
    }

   
}
