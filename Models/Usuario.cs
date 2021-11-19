using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Usuario
    {
        //[BsonElement("id")]
        //[BsonRepresentation(BsonType.String)]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string[] Contactos { get; set; }
        public string[] Solicitudes { get; set; }
        public Object Grupos { get; set; }
    }
        
}
