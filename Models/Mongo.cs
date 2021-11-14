using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Axiliares;

namespace WebApplication1.Models
{
    public class Mongo
    {
        private MongoClient Conexion()
        {
            var cliente = new MongoClient("mongodb://url:4vT5CLqbsb*$-SF@cluster0-shard-00-00.ezs44.mongodb.net:27017,cluster0-shard-00-01.ezs44.mongodb.net:27017,cluster0-shard-00-02.ezs44.mongodb.net:27017/myFirstDatabase?ssl=true&replicaSet=atlas-fm2ttf-shard-0&authSource=admin&retryWrites=true&w=majority");
            return cliente;
        }

        public void InsertarUsuario(string Tabla, string Email, string Password, string Usuario)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var collection = database.GetCollection<BsonDocument>(Tabla);
            
            var document = new BsonDocument
            {
                { "Contactos", new BsonDocument{ } },
                { "Correo", Email },
                { "Grupos", new BsonDocument{ } },
                { "Password", Password },
                { "Solicitudes", 0 },
                { "Nombre", Usuario}
            };

            collection.InsertOne(document);
        }

        public Usuario GetUsuario(string Usuario, string Password)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Usuarios");
            var filtro = Builders<Usuario>.Filter.Eq("Correo", Usuario);
            var respuesta = coleccion.Find(filtro).FirstOrDefault();

            Base64 base64 = new();
            string pass = base64.Encriptar(Password);

            if (respuesta == null)
            {
                return null;
            }
            if (respuesta.Correo == Usuario)
            {
                if (respuesta.Password == pass)
                {
                    return respuesta;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Usuario GetUsuario(string correo)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Usuarios");
            var filtro = Builders<Usuario>.Filter.Eq("Correo", correo);
            var respuesta = coleccion.Find(filtro).FirstOrDefault();

            if (respuesta == null)
            {
                return null;
            }
            else
            {
                return respuesta;
            }
        }

        public bool ActualizarContactos(string usuario, string[] arreglo) {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Usuarios");

            var filtro = Builders<Usuario>.Filter.Eq("Correo", usuario);
            var update = Builders<Usuario>.Update.Set("Contactos", arreglo);
            var respuesta = coleccion.UpdateOne(filtro, update);

            return respuesta.IsModifiedCountAvailable;
        }

        public bool ActualizarSolicitudes(string usuario, string[] arreglo)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Usuarios");

            var filtro = Builders<Usuario>.Filter.Eq("Correo", usuario);
            var update = Builders<Usuario>.Update.Set("Solicitudes", arreglo);
            var respuesta = coleccion.UpdateOne(filtro, update);

            return respuesta.IsModifiedCountAvailable;
        }

        public bool EliminarSolicitud(string usuario, string solicitud)
        {
            Usuario respuesta = GetUsuario(usuario);

            List<string> listado = new List<string>(respuesta.Solicitudes);
            for (int i = 0; i < listado.Count; i++)
            {
                if (listado[i] == solicitud)
                {
                    listado.Remove(solicitud);
                }
            }
            return ActualizarSolicitudes(usuario, listado.ToArray());
        }
    
    }
}
