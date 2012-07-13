using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using proyecto_seminario.Models;
namespace proyecto_seminario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Comentarios(string id) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            if (id == null || id=="" || id=="0")
            {
                ViewBag.lista = (from i in db.vista_comentarios select i).ToList();
                ViewBag.palabra = "*";
            }
            else
            {
                ViewBag.lista = (from i in db.vista_comentarios where i.Texto.Contains(id) select i).ToList();
                ViewBag.palabra = id;
            }
            ViewBag.cantidad = (from i in db.Comentarios select i).ToList();
            
            ViewBag.ofensivo = (from i in db.diccionarios select i).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Comentarios(FormCollection f) {
            string palabra = f["ofensivo"];
            ViewBag.palabra = palabra;
            BibliotecaDataContext db = new BibliotecaDataContext();
            ViewBag.lista = (from i in db.vista_comentarios where i.Texto.Contains(palabra) select i).ToList();
            ViewBag.ofensivo = (from i in db.diccionarios select i).ToList();
            ViewBag.cantidad = (from i in db.vista_comentarios where i.Texto.Contains(palabra) select i).ToList();
            return View();
        }
        public ActionResult Eliminar_comentario(int id,string clave) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            var v = (from i in db.Comentarios where i.Id_Com == id select i);
            if(clave=="*")
                 v = (from i in db.Comentarios where i.Id_Com == id select i);
            else
                 v = (from i in db.Comentarios where  i.Texto.Contains(clave) select i);
            foreach (var item in v) {
                db.Comentarios.DeleteOnSubmit(item);
                db.SubmitChanges();
            }
            return RedirectToAction("Comentarios");        
        }

        [HttpPost]
        public ActionResult Insertar_palabra(FormCollection f) {
            string pal="";
            BibliotecaDataContext db= new BibliotecaDataContext();
            if(f["palabra"]!=null && f["palabra"]!=""){
                pal = f["palabra"];
                if ((from i in db.diccionarios where i.palabra == pal select i).Count() == 0) {
                    diccionario d = new diccionario() { palabra = pal };
                    db.diccionarios.InsertOnSubmit(d);
                    db.SubmitChanges();
                }
            }
            return RedirectToAction("Comentarios");
        }
        public ActionResult Eliminar_varios(string id) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            
            var v = (from i in db.Comentarios where i.Texto.Contains(id) select i);
            foreach (var i in v) {
                db.Comentarios.DeleteOnSubmit(i);
            }
            db.SubmitChanges();
            return RedirectToAction("Comentarios");
        }
        public ActionResult Categorias() {
            BibliotecaDataContext db= new BibliotecaDataContext();
            ViewBag.categos = (from i in db.Categorias select i).ToList();
            return View();
        }

        public ActionResult Eliminar_categorias(int id) {
            BibliotecaDataContext db = new BibliotecaDataContext();
            var x=(from i in db.Categorias where i.Id_cat==id select i).ToList();
            foreach (var n in x) {
                var rel = (from j in db.Rel_Categoria_Contenidos where j.Id_cat == n.Id_cat select j).ToList();
                foreach (var m in rel)
                    db.Rel_Categoria_Contenidos.DeleteOnSubmit(m);
                db.SubmitChanges();
                db.Categorias.DeleteOnSubmit(n);
                db.SubmitChanges();
            }
            return RedirectToAction("Categorias");
        }

        public ActionResult Actualizar_categoria(FormCollection f) {
            int id = Convert.ToInt32( f["id"]);
            BibliotecaDataContext db= new BibliotecaDataContext();
            int con = (from i in db.Categorias where i.Nombre == f["nuevo"].ToLower().Trim() select i).Count();
            if (con == 0)
            {
                Categoria c = db.Categorias.Single(u => u.Id_cat == id);
                c.Nombre = f["nuevo"].ToLower().Trim();
                db.SubmitChanges();
            }
            else {
                var relaciones = (from i in db.Rel_Categoria_Contenidos where i.Id_cat==id select i).ToList();
                foreach (var v in relaciones) {
                    db.Rel_Categoria_Contenidos.DeleteOnSubmit(v);
                    db.SubmitChanges();
                }
                var catego=(from i in db.Categorias where i.Id_cat==id select i).ToList();
                foreach (var v in catego) {
                    db.Categorias.DeleteOnSubmit(v);
                    db.SubmitChanges();
                }
                int ID = (from i in db.Categorias where i.Nombre == f["nuevo"].ToLower().Trim() select i).ToArray()[0].Id_cat;
                foreach (var v in relaciones) {
                    
                    if((from i in db.Rel_Categoria_Contenidos where i.Id_cat==ID && i.IdContenido==v.IdContenido select i).Count()==0){
                        Rel_Categoria_Contenido r = new Rel_Categoria_Contenido() { Id_cat = ID, IdContenido = v.IdContenido };
                        db.Rel_Categoria_Contenidos.InsertOnSubmit(r);
                        db.SubmitChanges();
                    }
                }
            }
            return RedirectToAction("Categorias");
        }
        public ActionResult Contenidos() {
            BibliotecaDataContext db = new BibliotecaDataContext();
            ViewBag.lista = (from i in db.Contenidos where i.estado==0 select i).ToList();
            return View();
        }
        public ActionResult Usuarios() {
            BibliotecaDataContext db = new BibliotecaDataContext();
            ViewBag.userlist = (from i in db.Perfil_Usuarios
                                    join j in db.aspnet_Memberships
                                        on i.Id_User equals j.UserId
                                    select new detalle_perfil(){
                                        Id_user=(Guid)i.Id_User,
                                        avatar=i.Avatar,
                                        nickname=i.Nickname,
                                        username=i.aspnet_User.UserName,
                                        mail=j.Email ,
                                        karma=Convert.ToInt32(i.Karma),
                                        estado=Convert.ToInt32( i.estado)
                                    });
            return View();
        }
        public ActionResult Bannear(Guid id) {
            BibliotecaDataContext db= new BibliotecaDataContext();
            Perfil_Usuario pu = db.Perfil_Usuarios.Single(u => u.Id_User == id);
            pu.estado = 0;
            db.SubmitChanges();
            string x = (from i in db.aspnet_Users where i.UserId == id select i).ToArray()[0].UserName;
            Roles.RemoveUserFromRole(x,"Miembro");
            return RedirectToAction("Usuarios");
        }
        public ActionResult Activar(Guid id)
        {
            BibliotecaDataContext db = new BibliotecaDataContext();
            Perfil_Usuario pu = db.Perfil_Usuarios.Single(u => u.Id_User == id);
            pu.estado = 1;
            db.SubmitChanges();
            string x = (from i in db.aspnet_Users where i.UserId == id select i).ToArray()[0].UserName;
            Roles.AddUserToRole(x, "Miembro");
            return RedirectToAction("Usuarios");
        }
        public ActionResult Mas_rol(string id) {
            Roles.AddUserToRole(id, "Administrador");
            return RedirectToAction("Usuarios");
        }
        public ActionResult Menos_rol(string id)
        {
            Roles.RemoveUserFromRole(id,"Administrador");
            return RedirectToAction("Usuarios");
        }
        public ActionResult Disminuir_karma(FormCollection f) {
            Guid id =new Guid(  f["userid"]);
            
            int i=0;
            if (f["cantidad"] != null && f["cantidad"] != "")
                i =Convert.ToInt32( f["cantidad"]);
            BibliotecaDataContext db = new BibliotecaDataContext();
            Perfil_Usuario pu = db.Perfil_Usuarios.Single(u => u.Id_User == id);
            pu.Karma = pu.Karma - i;
            db.SubmitChanges();
            return RedirectToAction("Usuarios");
        }
    }
    
}
