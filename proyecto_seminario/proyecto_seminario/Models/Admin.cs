using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto_seminario.Models
{
    public class DetalleComentario
    {
        public int idcomentario { get; set;}
        public string Texto { get; set; }
        public Guid idusuario { get; set; }
        public string avatar { get; set; }
        public int karma { get; set; }
    }
}