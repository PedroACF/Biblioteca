﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using proyecto_seminario.Models;
namespace proyecto_seminario.Controllers
{
    public class ContenidoController : Controller
    {
        //
        // GET: /Contenido/
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Miembro")]
        public ActionResult SubirLibro() {
            BibliotecaDataContext db = new BibliotecaDataContext();
           
            ViewBag.lista = db.Categorias;
            return View();
        }
        
        [Authorize(Roles = "Miembro")]
        [HttpPost]
        public ActionResult SubirLibro(Libros lib,HttpPostedFileBase portada,HttpPostedFileBase contenido) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            Guid id = (Guid)Session["userid"];
            ViewBag.lista = db.Categorias;
            int ID;
            
            if (ModelState.IsValid) {
                          
                
                Contenido con = new Contenido()
                {
                    Id_User=id,
                    Titulo=lib.titulo,
                    Descripcion=lib.descripcion,
                    Tipo="Libro",
                    observaciones="",
                    estado=0,
                    Fecha_publicacion=DateTime.Now
                };
                db.Contenidos.InsertOnSubmit(con);
                db.SubmitChanges();
                ID = db.Contenidos.OrderByDescending(ax => con.IdContenido).Select(a => con.IdContenido).ToArray()[0];
                DateTime dt = new DateTime();
                dt.AddYears(lib.aniopublicacion);
                var data = new byte[portada.ContentLength];
                var data2 = new byte[contenido.ContentLength];
                portada.InputStream.Read(data, 0, portada.ContentLength);
                contenido.InputStream.Read(data2, 0, contenido.ContentLength);
                var path = ControllerContext.HttpContext.Server.MapPath("../contenidos");
                var path2 = ControllerContext.HttpContext.Server.MapPath("../contenidos");
                var filename = Path.Combine(path, Path.GetFileName(portada.FileName));
                var filename2 = Path.Combine(path2, Path.GetFileName(contenido.FileName));
                System.IO.File.WriteAllBytes(Path.Combine(path, filename), data);
                System.IO.File.WriteAllBytes(Path.Combine(path2, filename2), data2);
                Libro l = new Libro()
                {
                    IdPublicacion=ID,
                    Autor=lib.autor,
                    AnoPublicacion=dt,
                    Portada="/contenidos/"+portada.FileName,
                    Tema="/contenidos/"+contenido.FileName
                };
                db.Libros.InsertOnSubmit(l);
                db.SubmitChanges();
                string[] miarray = lib.categoria.ToLower().Split(',');
                List<Categoria> lcat = new List<Categoria>();
                foreach (var x in miarray)
                {
                    string aux = x.Trim();
                    if (db.Categorias.Where(a => a.Nombre == aux).Count() == 0)
                    {
                        Categoria c = new Categoria() { Nombre = aux };
                        db.Categorias.InsertOnSubmit(c);
                        db.SubmitChanges();
                    }
                    int idcat = db.Categorias.Where(a => a.Nombre == aux).Select(a => a.Id_cat).ToArray()[0];
                    Rel_Categoria_Contenido r = new Rel_Categoria_Contenido() { Id_cat=idcat,IdContenido=ID};
                    db.Rel_Categoria_Contenidos.InsertOnSubmit(r);
                    db.SubmitChanges();
                }
                
                return Redirect("VistaLibro/"+ID);
            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Miembro")]
        public ActionResult Comentar(FormCollection f) {
            Guid ID;
            if (Session["userid"] == null)
                ID = Guid.Empty;
            else
                ID = (Guid)Session["userid"];
            int id = Convert.ToInt32(f["id"]);
            BibliotecaDataContext db = new BibliotecaDataContext();
            if (f["coment"] != null && f["coment"] != "")
            {
                Comentario c = new Comentario() { Id_Cont = id, Id_Us = ID, Fecha = DateTime.Now, Texto = f["coment"] };

                db.Comentarios.InsertOnSubmit(c);
                db.SubmitChanges();
            }
            return Redirect("/Contenido/VistaLibro/" + id);
        }
        public ActionResult VistaLibro(int id) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            var x = from i in db.Contenidos
                    join j in db.Libros on i.IdContenido equals j.IdPublicacion
                    where i.IdContenido == id
                    select new
                    {
                        titulo=i.Titulo,
                        descripcion=i.Descripcion,
                        tema=j.Tema,
                        autor=j.Autor,
                        fecha=j.AnoPublicacion,
                        id=i.Id_User
                    };
            ViewBag.id = id;
            ViewBag.titulo = x.ToArray()[0].titulo;
            ViewBag.desc = x.ToArray()[0].descripcion;
            ViewBag.tema = x.ToArray()[0].tema;
            ViewBag.autor = x.ToArray()[0].autor;
            ViewBag.fecha = x.ToArray()[0].fecha;
            Guid ID = (Guid)x.ToArray()[0].id;
            var y = (from i in db.Perfil_Usuarios where i.Id_User == ID select i).ToArray()[0];
            ViewBag.avatar = y.Avatar;
            ViewBag.karma = y.Karma;
            ViewBag.listas = (from i in db.Contenidos where i.Id_User == ID select i).ToList();
            ViewBag.comcont = (from p in db.Comentarios where p.Id_Cont == id select p).Count();
            ViewBag.comentario = (from p in db.Comentarios where p.Id_Cont == id select p).ToList();
            ViewBag.cosa = (from p in db.Gustas where p.Id_Us == ID && p.IdPub == id select p).Count().ToString();
            ViewBag.userid = ID;
            ViewBag.listauser = (from p in db.Perfil_Usuarios select p).ToList();
            ViewBag.gustas = (from i in db.Gustas where i.IdPub == id select i).Count();
            return View();
        }
       [Authorize(Roles = "Miembro")]
        public ActionResult Publicar(string id) {
            ViewBag.titulo = id;
            return View();
        }
         [Authorize(Roles = "Miembro")]
        [HttpPost,ValidateInput(false)]
        public ActionResult Publicar(ContenidoVista c,FormCollection f) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            int ID = 0;
            Guid id = (Guid)Session["userid"];
            if (ModelState.IsValid)
            {
                Contenido con = new Contenido()
                {
                    Id_User = id,
                    Titulo = c.titulo,
                    Descripcion = c.descripcion,
                    Tipo = f["tipo"],
                    observaciones="",
                    estado=0,
                    Fecha_publicacion=DateTime.Now
                };
                db.Contenidos.InsertOnSubmit(con);
                db.SubmitChanges();
                ID = db.Contenidos.OrderByDescending(ax => con.IdContenido).Select(a => con.IdContenido).ToArray()[0];
                Publicacion p = new Publicacion()
                {
                    IdPublicacion = ID,
                    Tema = f["ckeditor"]
                };
                db.Publicacions.InsertOnSubmit(p);
                db.SubmitChanges();
                string[] miarray = c.categoria.ToLower().Split(',');
                List<Categoria> lcat = new List<Categoria>();
                foreach (var x in miarray)
                {
                    string aux = x.Trim();
                    if (db.Categorias.Where(a => a.Nombre == aux).Count() == 0)
                    {
                        Categoria cc = new Categoria() { Nombre = aux };
                        db.Categorias.InsertOnSubmit(cc);
                        db.SubmitChanges();
                    }
                    int idcat = db.Categorias.Where(a => a.Nombre == aux).Select(a => a.Id_cat).ToArray()[0];
                    Rel_Categoria_Contenido r = new Rel_Categoria_Contenido() { Id_cat = idcat, IdContenido = ID };
                    db.Rel_Categoria_Contenidos.InsertOnSubmit(r);
                    db.SubmitChanges();
                }
                return Redirect("../DetallePublicacion/" + ID);
            }
            return View();
        
        }
        public ActionResult DetallePublicacion(int id)
        {
            ViewBag.con = id;
            Guid ID;
            if (Session["userid"] == null)
                ID = Guid.Empty;
            else
                ID = (Guid)Session["userid"];
            
            BibliotecaDataContext db = new BibliotecaDataContext();
            ViewBag.tipo = (from p in db.Contenidos where p.IdContenido == id select p).ToArray()[0].Tipo;
            ViewBag.titulo = (from p in db.Contenidos where p.IdContenido == id select p).ToArray()[0].Titulo;
            
            ViewBag.contenido = (from p in db.Publicacions where p.IdPublicacion == id select new { tema = p.Tema }).ToArray()[0].tema;
            
            ViewBag.comentario = (from p in db.Comentarios where p.Id_Cont == id select p).ToList();
            ViewBag.cosa = (from p in db.Gustas where p.Id_Us == ID && p.IdPub == id select p).Count().ToString();
            Guid IDD = (Guid)(from p in db.Contenidos where p.IdContenido==id select p).ToArray()[0].Id_User;
            var y = (from i in db.Perfil_Usuarios where i.Id_User == IDD select i).ToArray()[0];
            ViewBag.avatar = y.Avatar;
            ViewBag.karma = y.Karma;
            ViewBag.userid = IDD;
            ViewBag.comcont = (from p in db.Comentarios where p.Id_Cont == id select p).Count();
            ViewBag.listas = (from i in db.Contenidos where i.Id_User == IDD select i).ToList();
            ViewBag.listauser = (from p in db.Perfil_Usuarios select p).ToList();
            ViewBag.gustas=(from i in db.Gustas where i.IdPub==id select i).Count();
            return View();
        }
        
        [HttpPost,Authorize(Roles = "Miembro")]
        public ActionResult DetallePublicacion(FormCollection f) {
            Guid ID;
            if (Session["userid"] == null)
                ID = Guid.Empty;
            else
                ID = (Guid)Session["userid"];
            int id = Convert.ToInt32(f["id"]);
            BibliotecaDataContext db = new BibliotecaDataContext();

            ViewBag.tipo = (from p in db.Contenidos where p.IdContenido == id select p).ToArray()[0].Tipo;
            ViewBag.con=id;
            
            ViewBag.contenido = (from p in db.Publicacions where p.IdPublicacion == id select new { tema = p.Tema }).ToArray()[0].tema;
            if (f["coment"] != null && f["coment"] != "")
            {
                Comentario c = new Comentario() { Id_Cont = id, Id_Us = ID, Fecha = DateTime.Now, Texto = f["coment"] };

                db.Comentarios.InsertOnSubmit(c);
                db.SubmitChanges();
            }
            ViewBag.comentario=(from p in db.Comentarios where p.Id_Cont==id select p).ToList();
            ViewBag.cosa = (from p in db.Gustas where p.Id_Us==ID && p.IdPub==id select p).Count().ToString();
            Guid IDD = (Guid)(from p in db.Contenidos where p.IdContenido == id select p).ToArray()[0].Id_User;
            var y = (from i in db.Perfil_Usuarios where i.Id_User == IDD select i).ToArray()[0];
            ViewBag.avatar = y.Avatar;
            ViewBag.karma = y.Karma;
            ViewBag.userid = IDD;
            ViewBag.comcont = (from p in db.Comentarios where p.Id_Cont == id select p).Count();
            ViewBag.listas = (from i in db.Contenidos where i.Id_User == IDD select i).ToList();
            ViewBag.listauser = (from p in db.Perfil_Usuarios select p).ToList();
            ViewBag.gustas = (from i in db.Gustas where i.IdPub == id select i).Count();
            return View();
        }
        public ActionResult Gusta(int id) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            Gusta h = new Gusta() { Id_Us=(Guid)(Session["userid"]),IdPub=id};
            db.Gustas.InsertOnSubmit(h);
            db.SubmitChanges();
            string tipo = (from t in db.Contenidos where t.IdContenido == id select t).ToArray()[0].Tipo;
            if(tipo=="Libro")
                return Redirect("/Contenido/VistaLibro/" + id);
            return Redirect("/Contenido/DetallePublicacion/"+id);
        }
         [Authorize(Roles = "Miembro")]
        public ActionResult NoGusta(int id)
        {
            BibliotecaDataContext db = new BibliotecaDataContext();
            var det = from p in db.Gustas where p.Id_Us == (Guid)(Session["userid"]) && p.IdPub == id select p;
            foreach (var x in det) {
                db.Gustas.DeleteOnSubmit(x);
            }
            db.SubmitChanges();
            string tipo = (from t in db.Contenidos where t.IdContenido == id select t).ToArray()[0].Tipo;
            if (tipo == "Libro")
                return Redirect("/Contenido/VistaLibro/" + id);
            return Redirect("/Contenido/DetallePublicacion/" + id);
        }
         [Authorize(Roles = "Miembro")]
        public ActionResult EdicionLibro(int id) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            var x = (from i in db.Contenidos
                     join j in db.Libros on i.IdContenido equals j.IdPublicacion
                     where i.IdContenido == id
                     select new Libros() { 
                        titulo=i.Titulo,
                        descripcion=i.Descripcion,
                        autor=j.Autor                
                     }
                  );
            ViewBag.observaciones = (from i in db.Contenidos where i.IdContenido == id select i).ToArray()[0].observaciones;
            ViewBag.id = id;
            ViewBag.titulo = x.ToArray()[0].titulo;
            ViewBag.descripcion = x.ToArray()[0].descripcion;
            ViewBag.autor = x.ToArray()[0].autor;
            return View();
        }
        [HttpPost]
        public ActionResult EdicionLibro(FormCollection f, HttpPostedFileBase portada, HttpPostedFileBase contenido)
        {
            BibliotecaDataContext db = new BibliotecaDataContext();
            Guid id = (Guid)Session["userid"];
            ViewBag.lista = db.Categorias;
            int ID= Convert.ToInt32( f["ID"]);
            Contenido c = db.Contenidos.Single(u => u.IdContenido == ID);
            Libro l = db.Libros.Single(u => u.IdPublicacion == ID);
            if (f["autor"] != null && f["autor"] != "")
                l.Autor = f["Autor"];
            if (f["titulo"] != null && f["titulo"] != "")
                c.Titulo = f["titulo"];
            if (f["descripcion"] != null && f["descripcion"] != "")
                c.Descripcion = f["descripcion"];
            if (f["anio"] != null && f["anio"] != "")
                l.AnoPublicacion = DateTime.Now;
            
            if (portada!=null)
            {
                var data = new byte[portada.ContentLength];
                portada.InputStream.Read(data, 0, portada.ContentLength);
                var path = ControllerContext.HttpContext.Server.MapPath("../contenidos");
                var filename = Path.Combine(path, Path.GetFileName(portada.FileName));
                System.IO.File.WriteAllBytes(Path.Combine(path, filename), data);
                l.Portada = "/contenidos/"+portada.FileName;
            }
            if (contenido != null)
            {
                var data2 = new byte[contenido.ContentLength];
                contenido.InputStream.Read(data2, 0, contenido.ContentLength);
                var path2 = ControllerContext.HttpContext.Server.MapPath("../contenidos");
                var filename2 = Path.Combine(path2, Path.GetFileName(contenido.FileName));
                System.IO.File.WriteAllBytes(Path.Combine(path2, filename2), data2);
                l.Tema = "/contenidos/"+contenido.FileName;
            }
            
                c.estado = 0;
            db.SubmitChanges();
            if (f["categoria"] != null && f["categoria"] != "")
            {
                string[] miarray = f["categoria"].ToLower().Split(',');
                List<Categoria> lcat = new List<Categoria>();
                foreach (var x in miarray)
                {
                    string aux = x.Trim();
                    if (db.Categorias.Where(a => a.Nombre == aux).Count() == 0)
                    {
                        Categoria cat = new Categoria() { Nombre = aux };
                        db.Categorias.InsertOnSubmit(cat);
                        db.SubmitChanges();
                    }
                    int idcat = db.Categorias.Where(a => a.Nombre == aux).Select(a => a.Id_cat).ToArray()[0];
                    if ((from i in db.Rel_Categoria_Contenidos where i.Id_cat == idcat && i.IdContenido == ID select i).Count() == 0)
                    {
                        Rel_Categoria_Contenido r = new Rel_Categoria_Contenido() { Id_cat = idcat, IdContenido = ID };
                        db.Rel_Categoria_Contenidos.InsertOnSubmit(r);
                        db.SubmitChanges();
                    }
                }
            }
           return Redirect("/Contenido/VistaLibro/" + ID);
        }
         [Authorize(Roles = "Miembro")]
        public ActionResult EdicionPublicacion(int id)
        {
            BibliotecaDataContext db = new BibliotecaDataContext();
            ViewBag.id = id;
            ViewBag.lista=(from i in db.Contenidos
                               join j in db.Publicacions on i.IdContenido equals j.IdPublicacion
                               where i.IdContenido==id select new ContenidoVista(){
                                    titulo=i.Titulo,
                                    descripcion=i.Descripcion,
                                    Tipo=i.Tipo,
                                    text=j.Tema
                               } ).ToArray()[0];
            ViewBag.observaciones = (from i in db.Contenidos where i.IdContenido == id select i).ToArray()[0].observaciones;
            return View();
        }
        
        [HttpPost,ValidateInput(false)]        
        public ActionResult EdicionPublicacion(FormCollection f) {
            int ID = Convert.ToInt32(f["id"]);
            BibliotecaDataContext db = new BibliotecaDataContext();
            Contenido c = db.Contenidos.Single(u => u.IdContenido == ID);
            Publicacion p = db.Publicacions.Single(u => u.IdPublicacion == ID);
            if (f["titulo"] != null && f["titulo"] != "")
                c.Titulo = f["titulo"];
            if (f["descripcion"] != null && f["descripcion"] != "")
                c.Descripcion = f["descripcion"];
            if (f["ckeditor"] != null && f["ckeditor"] != "")
                p.Tema = f["ckeditor"];
            
                c.estado = 0;
            db.SubmitChanges();
            if (f["categoria"] != null && f["categoria"] != "")
            {
                string[] miarray = f["categoria"].ToLower().Split(',');
                List<Categoria> lcat = new List<Categoria>();
                foreach (var x in miarray)
                {
                    string aux = x.Trim();
                    if (db.Categorias.Where(a => a.Nombre == aux).Count() == 0)
                    {
                        Categoria cat = new Categoria() { Nombre = aux };
                        db.Categorias.InsertOnSubmit(cat);
                        db.SubmitChanges();
                    }
                    int idcat = db.Categorias.Where(a => a.Nombre == aux).Select(a => a.Id_cat).ToArray()[0];
                    if ((from i in db.Rel_Categoria_Contenidos where i.Id_cat == idcat && i.IdContenido == ID select i).Count() == 0)
                    {
                        Rel_Categoria_Contenido r = new Rel_Categoria_Contenido() { Id_cat = idcat, IdContenido = ID };
                        db.Rel_Categoria_Contenidos.InsertOnSubmit(r);
                        db.SubmitChanges();
                    }
                }
            }
            
            return Redirect("/Contenido/DetallePublicacion/" + ID);
        }
        [Authorize(Roles = "Administrador")]
        public ActionResult RevisionLibro(int id)
        {
            BibliotecaDataContext db = new BibliotecaDataContext();
            var x = from i in db.Contenidos
                    join j in db.Libros on i.IdContenido equals j.IdPublicacion
                    where i.IdContenido == id
                    select new
                    {
                        titulo = i.Titulo,
                        descripcion = i.Descripcion,
                        tema = j.Tema,
                        autor = j.Autor,
                        fecha = j.AnoPublicacion,
                        id = i.Id_User
                    };
            ViewBag.id = id;
            ViewBag.titulo = x.ToArray()[0].titulo;
            ViewBag.desc = x.ToArray()[0].descripcion;
            ViewBag.tema = x.ToArray()[0].tema;
            ViewBag.autor = x.ToArray()[0].autor;
            ViewBag.fecha = x.ToArray()[0].fecha;
            Guid ID = (Guid)x.ToArray()[0].id;
            var y = (from i in db.Perfil_Usuarios where i.Id_User == ID select i).ToArray()[0];
            ViewBag.avatar = y.Avatar;
            ViewBag.karma = y.Karma;
            ViewBag.cosa = (from p in db.Gustas where p.Id_Us == ID && p.IdPub == id select p).Count().ToString();
            ViewBag.userid = ID;
            return View();
  
        }
        [Authorize(Roles="Administrador")]
        public ActionResult Bueno(int id) {
            BibliotecaDataContext db= new BibliotecaDataContext();
            Contenido c = db.Contenidos.Single(u => u.IdContenido == id);
            c.estado = 1;
            db.SubmitChanges();
            return RedirectToAction("Contenidos", "Admin");
        }
        [Authorize(Roles = "Administrador")]
        public ActionResult RevisionPublicacion(int id)
        {
            ViewBag.con = id;
            BibliotecaDataContext db = new BibliotecaDataContext();
            ViewBag.tipo = (from p in db.Contenidos where p.IdContenido == id select p).ToArray()[0].Tipo;
            ViewBag.titulo = (from p in db.Contenidos where p.IdContenido == id select p).ToArray()[0].Titulo;

            ViewBag.contenido = (from p in db.Publicacions where p.IdPublicacion == id select new { tema = p.Tema }).ToArray()[0].tema;


            return View();
        }
        [Authorize(Roles="Administrador")]
        [HttpPost]        
        public ActionResult Observar(FormCollection f) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            int id =Convert.ToInt32( f["idcont"]);
            Contenido c = db.Contenidos.Single(u => u.IdContenido == id);
            c.observaciones = f["observacion"];
            c.estado = 2;
            db.SubmitChanges();
            return RedirectToAction("Contenidos", "Admin");
        }
    }
}
