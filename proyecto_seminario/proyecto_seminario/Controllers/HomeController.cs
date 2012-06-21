using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using proyecto_seminario.Models;
namespace proyecto_seminario.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int id)
        {

            
            BibliotecaDataContext db = new BibliotecaDataContext();
            var lista = (from f in db.Categorias
                        select f).ToList();
            ViewBag.catego = lista;
            ViewBag.miscat = (from t in db.ListaCategos select t).ToList();
            ViewBag.Gusta = (from t in db.Cant_Gustas select t).ToList();
            if (id == 0)
            {
                ViewBag.con = (from f in db.VistaGeneralLibros orderby f.IdContenido descending select f).ToList();
                ViewBag.art = (from f in db.VistaGeneralPublicacions where f.Tipo=="Articulo"  orderby f.IdContenido descending select f).ToList();
                ViewBag.tuto = (from f in db.VistaGeneralPublicacions where f.Tipo == "Tutorial" orderby f.IdContenido descending select f).ToList();
                ViewBag.curso = (from f in db.VistaGeneralPublicacions where f.Tipo == "Curso" orderby f.IdContenido descending select f).ToList(); 
            }
            else
            {
                ViewBag.con = (from f in db.VistaGeneralLibros join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where g.Id_cat == id orderby f.IdContenido descending select f).ToList();
                ViewBag.art = (from f in db.VistaGeneralPublicacions join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where g.Id_cat==id && f.Tipo=="Articulo" orderby f.IdContenido descending select f).ToList();
                ViewBag.tuto = (from f in db.VistaGeneralPublicacions join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where g.Id_cat == id && f.Tipo == "Tutorial" orderby f.IdContenido descending select f).ToList();
                ViewBag.curso = (from f in db.VistaGeneralPublicacions join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where g.Id_cat == id && f.Tipo == "Curso" orderby f.IdContenido descending select f).ToList(); 
            }
            return View();
        }
      

        public ActionResult About()
        {
            return View();
        }
    }
}
