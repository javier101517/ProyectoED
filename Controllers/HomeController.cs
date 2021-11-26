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


        public IActionResult LogOut(string usuarioLogueado)
        {
            Mongo mongo = new Mongo();
            mongo.ActualizarEstadoDeLogout(usuarioLogueado);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(IFormCollection collection)
        {
            string usuario = collection["usuarioLogueado"];
            string password = collection["password"];

            if (usuario == "" || password == "")
            {
                TempData["texto"] = "Usuario o contraseña incorrectos";
                TempData["color"] = "error";
                return RedirectToAction("Index");
            }

            Mongo mongo = new Mongo();
            Usuario UsuarioLogueado = mongo.Login(usuario, password);

           

            if (UsuarioLogueado != null )
            {
                if (UsuarioLogueado.Estado == "1")
                {
                    TempData["texto"] = "Usuario ya se encuentra logueado";
                    TempData["color"] = "error";
                    return RedirectToAction("Index");
                }
                else
                {
                    mongo.ActualizarEstadoDeLogin(UsuarioLogueado.Correo);
                    return RedirectToAction("Index", "Principal", new { usuarioLogueado = UsuarioLogueado.Correo });

                }

            }
            else
            {



                TempData["texto"] = "Usuario o contraseña incorrectos";
                TempData["color"] = "error";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        public IActionResult Registro(IFormCollection collection)
        {
            Mongo mongo = new();
            string email = collection["email"];
            string usuario = collection["usuario"];
            string password = collection["password"];
            string password2 = collection["password2"];

            if (usuario == "")
            {
                TempData["texto"] = "Debe ingresar un usuario";
                TempData["color"] = "error";
                return RedirectToAction("Registrarse");
            }
            if (password != password2)
            {
                TempData["texto"] = "Contraseñas diferentes";
                TempData["color"] = "error";
                return RedirectToAction("Registrarse");
            }
            if (email == "")
            {
                TempData["texto"] = "Debe ingresar un correo";
                TempData["color"] = "error";
                return RedirectToAction("Registrarse");
            }

            Usuario existeUsuario = mongo.GetUsuario(email);
            if (existeUsuario != null)
            {
                TempData["texto"] = "Correo ya se encuentra registrado";
                TempData["color"] = "error";
                return RedirectToAction("Registrarse");
            }


            Base64 base64 = new();
            string respuesta = base64.Encriptar(password);

            mongo.InsertarUsuario("Usuarios", email, respuesta, usuario);

            TempData["texto"] = "Usuario registrado correctamente";
            TempData["color"] = "success";
            return RedirectToAction("Index");
        }
    }

   
}
