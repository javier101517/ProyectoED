using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RespuestaPantallaPrincipal
    {
        public Usuario UsuarioLogueado { get; set; }

        public List<Chats> Historial { get; set; }
    }

    public class RespuestaPantallaContactos
    {
        public Usuario Usuario { get; set; }
        public List<String> Contactos { get; set; }
    }

    public class RespuestaChat
    {
        public Chats chatOriginal { get; set; }
        public List<Conversacion> conversacionesDescifradas { get; set; }
    }

    public class RespuestaGrupo
    {
        public Grupo GrupoOriginal { get; set; }
        public List<Conversacion> conversacionesDescifradas { get; set; }
    }

}
