using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

        public Usuario Login(string Usuario, string Password)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Usuarios");
            var filtro = Builders<Usuario>.Filter.Eq("Correo", Usuario);
            Usuario usuarioRespuesta = coleccion.Find(filtro).FirstOrDefault();

            Base64 base64 = new();
            string pass = base64.Encriptar(Password);

            if (usuarioRespuesta == null)
            {
                return null;
            }
            if (usuarioRespuesta.Correo == Usuario)
            {
                if (usuarioRespuesta.Password == pass)
                {
                    return usuarioRespuesta;
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

        public void InsertarUsuario(string Tabla, string Email, string Password, string Usuario)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var collection = database.GetCollection<BsonDocument>(Tabla);

            var document = new BsonDocument
            {
                { "Contactos", new BsonArray{ } },
                { "Correo", Email },
                { "Grupos", new BsonArray{ } },
                { "Password", Password },
                { "Solicitudes", new BsonArray{ } },
                { "Nombre", Usuario}
            };

            collection.InsertOne(document);
        }

        
        public void CrearChat(string UsuarioEnvia, string UsuarioRecibe) 
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var collection = database.GetCollection<BsonDocument>("Chats");

            var document = new BsonDocument
            {
                { "Usuario1", UsuarioEnvia },
                { "Historial", new BsonArray() },
                { "Usuario2", UsuarioRecibe },
                { "MensajesNuevosUsuario1", "0" },
                { "MensajesNuevosUsuario2", "0"}
            };
            
            collection.InsertOne(document);
        }

        public void CrearGrupo(string nombreDelGrupo, string usuarioCreador)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var collection = database.GetCollection<BsonDocument>("Grupos");

            var document = new BsonDocument
            {
                { "NombreGrupo", nombreDelGrupo },
                { "UsuarioCreador", usuarioCreador },
                { "Integrantes", new BsonArray() },
                { "Historial", new BsonArray() },
            };

            collection.InsertOne(document);
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

        public List<Chats> GetChats(string usuario)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Chats>("Chats");
            //var filtro = Builders<Chats>.Filter.Eq("Envia", usuario) & Builders<Chats>.Filter.Eq("Recibe", usuario);
            var filtro = Builders<Chats>.Filter.Eq("Usuario1", usuario) | Builders<Chats>.Filter.Eq("Usuario2", usuario);
            var respuesta = coleccion.Find(filtro).ToList();

            return respuesta;
        }

        public Chats GetChat(string id)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Chats>("Chats");
            var filtro = Builders<Chats>.Filter.Eq("Id", id);
            var respuesta = coleccion.Find(filtro).FirstOrDefault();

            return respuesta;
        }

        public Chats GetChat(string usuarioEnvia, string usuarioRecibe)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Chats>("Chats");
            var filtro = Builders<Chats>.Filter.Eq("Usuario1", usuarioEnvia) & Builders<Chats>.Filter.Eq("Usuario2", usuarioRecibe);
            var respuesta = coleccion.Find(filtro).FirstOrDefault();

            return respuesta;
        }

        public bool ActualizarContactos(string usuario, string[] arreglo) {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Usuarios");

            var filtro = Builders<Usuario>.Filter.Eq("Correo", usuario);
            var update = Builders<Usuario>.Update.Set("Contactos", arreglo);
            var respuesta = coleccion.UpdateOne(filtro, update);

            return respuesta.IsModifiedCountAvailable;
        }

        public bool ActualizarIntegrantesGrupo(Integrante[] integrantes, string usuarioCreador, string nombreGrupo)
        {
            var database = Conexion().GetDatabase("ProyectoED");
            var coleccion = database.GetCollection<Usuario>("Grupos");

            var filtro = Builders<Usuario>.Filter.Eq("UsuarioCreador", usuarioCreador) & Builders<Usuario>.Filter.Eq("NombreGrupo", nombreGrupo);
            var update = Builders<Usuario>.Update.Set("Integrantes", integrantes);
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

        public bool ActualizarConversacion(Chats chat)
        {
            try
            {
                var database = Conexion().GetDatabase("ProyectoED");
                var coleccion = database.GetCollection<Chats>("Chats");

                var filtro = Builders<Chats>.Filter.Eq("Id", chat.Id);
                var update = Builders<Chats>.Update.Set("MensajesNuevosUsuario1", chat.MensajesNuevosUsuario1)
                    .Set("MensajesNuevosUsuario2", chat.MensajesNuevosUsuario2)
                    .Set("Historial", chat.Historial);
                var respuesta = coleccion.UpdateOne(filtro, update);

                return true;
            }
            catch (Exception e)
            {
                var test = e.Message;
                throw;
            }

            //cuando actualice se puede agregar un icono de enviado

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
       
        public bool EliminarContacto(string usuario, string contacto)
        {
            Usuario respuesta = GetUsuario(usuario);

            List<string> listado = new List<string>(respuesta.Contactos);
            for (int i = 0; i < listado.Count; i++)
            {
                if (listado[i] == contacto)
                {
                    listado.Remove(contacto);
                }
            }
            return ActualizarContactos(usuario, listado.ToArray());


        }
    }
}
