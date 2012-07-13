using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace proyecto_seminario.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password actual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuevo Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma nuevo password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name or mail")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        
        [Display(Name = "Cuanto es:")]
        public string Captcha { get; set; }
    }

    public class RegisterModel
    {
    
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class Perfil {

         
            
            //E-Mail:	admin@adm.com
            //Intereses:	
            //Ubicacion:
        public string avatar { set; get; }
        public string nickname { set; get; }
        public string apellido { set; get; }
        public string nombre { set; get; }
        public string mail { set; get; }
        public string ubicacion { set; get; } 
        public string interes { set; get; }
   

    }

    public class detalle_perfil {
        public Guid Id_user { set; get; }
        public string username { set; get; }
        public string avatar { set; get; }
        public string nickname { set; get; }
        public string mail { set; get; }
        public int karma { set; get; }
        public int estado { set; get; }
    }
 
}
