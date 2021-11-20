using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Chats
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Usuario1 { get; set; }
        public string Usuario2 { get; set; }
        public string MensajesNuevosUsuario1 { get; set; }
        public string MensajesNuevosUsuario2 { get; set; }
        public Conversacion[] Historial { get; set; }
    }

    public class Conversacion
    {
        public string Usuario { get; set; }
        public string Mensaje { get; set; }
        public string Fecha { get; set; }
        public string tipo { get; set; }
    }
}
