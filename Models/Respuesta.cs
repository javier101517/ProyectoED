﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RespuestaPantallaPrincipal
    {
        public Usuario Usuario { get; set; }

        public List<Chats> Historial { get; set; }
    }

    public class RespuestaPantallaContactos
    {
        public Usuario Usuario { get; set; }
        public List<String> Contactos { get; set; }
    }
}
