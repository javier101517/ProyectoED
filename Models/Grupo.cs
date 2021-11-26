using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Grupo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string LlavePrivada { get; set; }
        public string LlavePublica { get; set; }
        public string UsuarioCreador { get; set; }
        public string NombreGrupo { get; set; }
        public Integrante[] Integrantes { get; set; }
        public Conversacion[] Historial { get; set; }
    }

    public class Integrante
    {
        public string Usuario { get; set; }
        public string MensajesSinLeer { get; set; }
    }

    public class DescripcionGrupo
    {
        public string IdGrupo { get; set; }
        public string NombreGrupo { get; set; }
    }
}
