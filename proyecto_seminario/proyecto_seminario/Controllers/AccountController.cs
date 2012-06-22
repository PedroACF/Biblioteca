using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using proyecto_seminario.Models;
namespace proyecto_seminario.Controllers
{
    public class AccountController : Controller
    {
        
        //
        // GET: /Account/LogOn
        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn
        public ActionResult Perfil() {
            BibliotecaDataContext db = new BibliotecaDataContext();
            Guid id =  (Guid)Session["userid"];

            var fila= from f1 in db.aspnet_Memberships join f2 in db.Perfil_Usuarios on f1.UserId equals f2.Id_User
                      where f1.UserId==id
                      select new{
                        avatar=f2.Avatar,
                        nick=f2.Nickname,
                        apellido=f2.Apellidos,
                        nombre=f2.Nombre,
                        mail=f1.Email,
                        interes=f2.Intereses,
                        ubicacion=f2.Ubicacion,
                        karma=f2.Karma
                      };

            ViewBag.Avatar=fila.ToArray()[0].avatar;
            ViewBag.nick = fila.ToArray()[0].nick;
            ViewBag.apellido = fila.ToArray()[0].apellido;
            ViewBag.nombre = fila.ToArray()[0].nombre;
            ViewBag.mail = fila.ToArray()[0].mail;
            ViewBag.interes = fila.ToArray()[0].interes;
            ViewBag.ubicacion = fila.ToArray()[0].ubicacion;
            ViewBag.karma = fila.ToArray()[0].karma;
            ViewBag.coment = (from p in db.Comentarios where p.Id_Us == id select p).Count();
            ViewBag.gusta = (from p in db.Cant_Gustas select p).ToList();
            ViewBag.publicacion=(from p in db.Contenidos where p.Id_User==id select p).ToList();
            return View();
        }
        public ActionResult Perfil2(Guid id)
        {
            BibliotecaDataContext db = new BibliotecaDataContext();
         

            var fila = from f1 in db.aspnet_Memberships
                       join f2 in db.Perfil_Usuarios on f1.UserId equals f2.Id_User
                       where f1.UserId == id
                       select new
                       {
                           avatar = f2.Avatar,
                           nick = f2.Nickname,
                           apellido = f2.Apellidos,
                           nombre = f2.Nombre,
                           mail = f1.Email,
                           interes = f2.Intereses,
                           ubicacion = f2.Ubicacion,
                           karma = f2.Karma
                       };

            ViewBag.Avatar = fila.ToArray()[0].avatar;
            ViewBag.nick = fila.ToArray()[0].nick;
            ViewBag.apellido = fila.ToArray()[0].apellido;
            ViewBag.nombre = fila.ToArray()[0].nombre;
            ViewBag.mail = fila.ToArray()[0].mail;
            ViewBag.interes = fila.ToArray()[0].interes;
            ViewBag.ubicacion = fila.ToArray()[0].ubicacion;
            ViewBag.karma = fila.ToArray()[0].karma;
            ViewBag.coment = (from p in db.Comentarios where p.Id_Us == id select p).Count();
            ViewBag.gusta = (from p in db.Cant_Gustas select p).ToList();
            ViewBag.publicacion = (from p in db.Contenidos where p.Id_User == id select p).ToList();
            return View();
        }
        public ActionResult EditarPerfil(){
            //            Username:	
            //Apellido:	
            //Nombre:	
            //E-Mail:	admin@adm.com
            //Intereses:	
            //Ubicacion:
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditarPerfil(Perfil modelo,HttpPostedFileBase avatar) {
            if (ModelState.IsValid)
            {
                Guid id = (Guid)Session["userid"];
                BibliotecaDataContext db = new BibliotecaDataContext();
                aspnet_User us = db.aspnet_Users.Single(u => u.UserId == id);
                aspnet_Membership mem = db.aspnet_Memberships.Single(u => u.UserId == id);
                Perfil_Usuario pu = db.Perfil_Usuarios.Single(u => u.Id_User == id);
                if (avatar != null)
                {
                    var data = new byte[avatar.ContentLength];
                    avatar.InputStream.Read(data, 0, avatar.ContentLength);
                    var path = ControllerContext.HttpContext.Server.MapPath("../Avatar");
                    var filename = Path.Combine(path, Path.GetFileName(avatar.FileName));
                    System.IO.File.WriteAllBytes(Path.Combine(path, filename), data);
                    pu.Avatar = "../Avatar/" + avatar.FileName;

                }
                if (modelo.nickname != null)
                {
                    us.UserName = modelo.nickname;
                    us.LoweredUserName = modelo.nickname;
                    pu.Nickname = modelo.nickname;
                }
                if (modelo.mail != null)
                {
                    mem.Email = modelo.mail;
                    mem.LoweredEmail = modelo.mail;
                }
             
                pu.Nombre = modelo.nombre;
                pu.Apellidos = modelo.apellido;
                pu.Intereses = modelo.interes;
                pu.Ubicacion = modelo.ubicacion;
                db.SubmitChanges();
       //../Avatar/default.jpg
                    return Redirect("Perfil");

            }
            return View();
        }

        
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            
            if (ModelState.IsValid)
            {
                string user = Membership.GetUserNameByEmail(model.UserName);
                if (user != null)
                    model.UserName = user;
                if (Membership.ValidateUser(model.UserName, model.Password)&&(Session["Captcha"]==null||Session["Captcha"].ToString()==model.Captcha))
                {
                    
                    string pass = model.Password;
                    string nom = model.UserName;

                    Session.Remove("conteo");
                    Session.Remove("Captcha");
                    BibliotecaDataContext con = new BibliotecaDataContext();
                    
                    var fila= from f1 in con.aspnet_Memberships join f2 in con.aspnet_Users on f1.UserId equals f2.UserId
                              where f2.UserName==nom
                              select new{
                                ID=f1.UserId
                              };

                    Session["userid"] = fila.ToArray()[0].ID;
                    ViewBag.id = (Guid)Session["userid"];
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                     
                            
                        return RedirectToAction("Index", "Home");
                    }      
                    
                    

                }
                else
                {

                    if (Session["conteo"] == null)
                        Session["conteo"] = 2;
                    else {
                        if (Session["conteo"].ToString() != "0")
                        {
                            String temp = Session["conteo"].ToString();
                            int t = Convert.ToInt16(temp) - 1;
                            Session["conteo"] = t;
                        }
                    }
                    ModelState.AddModelError("", "El usuario o password es incorrecto.");

                    if (Convert.ToInt16(Session["conteo"]) == 0)
                    {
                        ModelState.AddModelError("", "No puedes seguir intentando");
                        Session["conteo"] = 0;
                    }
                    else {
                        ModelState.AddModelError("", "Puedes intentar " + Session["conteo"] + " veces mas");
                    }
                    ViewBag.x = Session["conteo"];
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff
       
        public ActionResult LogOff()
        {
            Session.Remove("userid") ;
            
            FormsAuthentication.SignOut();
            
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                model.UserName = "User" + (Membership.GetAllUsers().Count + 1);
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
               

                if (createStatus == MembershipCreateStatus.Success)
                {
                   
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
         public ActionResult CaptchaImage( bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);

            
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            
            Session["Captcha"] = a + b;
            ViewBag.y=Session["Captcha"];
            
            FileContentResult img = null;
             
            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

            
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x-r, y - r, r, r);
                    }
                }

            
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

            
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }
        
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
