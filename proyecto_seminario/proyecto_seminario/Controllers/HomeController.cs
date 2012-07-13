﻿using System;
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
            ViewBag.contenido = (from t in db.Contenidos select t).ToList();
            if (id == 0)
            {
                ViewBag.con = (from f in db.VistaGeneralLibros where f.estado==1  orderby f.IdContenido descending select f).ToList();
                ViewBag.art = (from f in db.VistaGeneralPublicacions where f.estado == 1 && f.Tipo == "Articulo" orderby f.IdContenido descending select f).ToList();
                ViewBag.tuto = (from f in db.VistaGeneralPublicacions where f.estado == 1 && f.Tipo == "Tutorial" orderby f.IdContenido descending select f).ToList();
                ViewBag.curso = (from f in db.VistaGeneralPublicacions where f.estado == 1 && f.Tipo == "Curso" orderby f.IdContenido descending select f).ToList(); 
            }
            else
            {
                ViewBag.con = (from f in db.VistaGeneralLibros join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where f.estado==1 && g.Id_cat == id orderby f.IdContenido descending select f).ToList();
                ViewBag.art = (from f in db.VistaGeneralPublicacions join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where f.estado==1 && g.Id_cat==id && f.Tipo=="Articulo" orderby f.IdContenido descending select f).ToList();
                ViewBag.tuto = (from f in db.VistaGeneralPublicacions join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where f.estado==1 && g.Id_cat == id && f.Tipo == "Tutorial" orderby f.IdContenido descending select f).ToList();
                ViewBag.curso = (from f in db.VistaGeneralPublicacions join g in db.Rel_Categoria_Contenidos on f.IdContenido equals g.IdContenido where f.estado==1 && g.Id_cat == id && f.Tipo == "Curso" orderby f.IdContenido descending select f).ToList(); 
            }
            return View();
        }
      

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Busqueda(FormCollection f) {
            string criterio = f["criterio"];
            string filtro = f["porque"];
            ViewBag.dato = criterio;
            BibliotecaDataContext db = new BibliotecaDataContext();
            if (filtro == "*")
            {

                ViewBag.lista = (from p in db.VistaGeneralLibros where p.Descripcion.Contains(criterio) select p).ToList();
                ViewBag.lista2 = (from p in db.VistaGeneralPublicacions join q in db.Publicacions on p.IdContenido equals q.IdPublicacion where p.Descripcion.Contains(criterio) || q.Tema.Contains(criterio) select p).ToList();
                ViewBag.con = (from p in db.Contenidos where (p.Descripcion.Contains(criterio) || p.Publicacion.Tema.Contains(criterio)) select p).Count();
            }
            else
            {
                if(filtro=="Libro")
                    ViewBag.lista = (from p in db.VistaGeneralLibros where p.Descripcion.Contains(criterio) select p).ToList();
                ViewBag.lista2 = (from p in db.VistaGeneralPublicacions join q in db.Publicacions on p.IdContenido equals q.IdPublicacion where (p.Descripcion.Contains(criterio)||q.Tema.Contains(criterio)) && p.Tipo==filtro select p).ToList();
                ViewBag.con = (from p in db.Contenidos where (p.Descripcion.Contains(criterio)||p.Publicacion.Tema.Contains(criterio)) && p.Tipo == filtro select p).Count();
            }
            
            return View();
        }
    }
}
