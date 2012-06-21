using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto_seminario.Models
{
    public class Libros
    {
        
        public string titulo { set; get; }
        public string autor { set; get; }
        public string descripcion { set; get; }
        public int aniopublicacion { set; get; }
        public string categoria { set; get; }
    }
}