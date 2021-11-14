using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Chats
    {
        public Object Codigo { get; set; }
    }


    public class Contenido
    {
        public string Cantidad { get; set; }
        public List<Conversacion> Historial { get; set; }
        
    }
    
    public class Conversacion
    {
        public string usuario { get; set; }
        public string mensaje { get; set; }
    }
}
