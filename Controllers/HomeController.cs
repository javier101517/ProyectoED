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
            //var cliente = new MongoClient("mongodb://url:4vT5CLqbsb*$-SF@cluster0-shard-00-00.ezs44.mongodb.net:27017,cluster0-shard-00-01.ezs44.mongodb.net:27017,cluster0-shard-00-02.ezs44.mongodb.net:27017/myFirstDatabase?ssl=true&replicaSet=atlas-fm2ttf-shard-0&authSource=admin&retryWrites=true&w=majority");
            //var database = cliente.GetDatabase("ProyectoED");
            //var coleccion = database.GetCollection<Usuario>("Usuarios");

            //var id = new ObjectId("617f3d168a28e73cbc2eb5b2");
            //List<Usuario> listado = coleccion.Find(_ => true).ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(IFormCollection collection)
        {
            //string usuario = collection["usuario"];
            //string password = collection["password"];

            //if (usuario == "" || password == "")
            //{
            //    TempData["Notificar"] = "Usuario o contraseña incorrectos";
            //    return RedirectToAction("Index");
            //}

            Mongo mongo = new Mongo();
            Usuario respuesta = mongo.GetUsuario("javier@javier.com", "1234");


            if (respuesta != null)
            {
                return RedirectToAction("Index", "Principal", respuesta);
            }
            else
            {
                TempData["Notificar"] = "Usuario o contraseña incorrectos";
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

            //string result = string.Empty;
            //byte[] encryted = System.Text.Encoding.Unicode.GetBytes(password);
            //result = Convert.ToBase64String(encryted);

            Mongo mongo = new();
            mongo.InsertarUsuario("Usuarios", email, respuesta, usuario);

            return RedirectToAction("Index", "Principal");
        }
    }

   
}
