using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;

namespace XCLHMS.Models
{
    public class CreateApplicationModel
    {

        public int? menuID { set; get; }
        public int? menuParentID { set; get; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "MenuName")]
        public String menuName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "ControllerName")]
        public String applicationPath { get; set; }

    }
}