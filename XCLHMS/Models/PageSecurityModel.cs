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


namespace XCLHMS.Models
{
    public class PageSecurityModel
    {

        //public int appCompanyMapID { set; get; }
        //public int companyID { set; get; }

        public int menuID { set; get; }
        //public IEnumerable<SelectListItem> ActionsList { get; set; }

        //public string hiddevalueforRepopulate { get; set; }
        public string SelectType { get; set; }

  
}
}


public enum AppSelectedTypesForPageSecurity
{
    Designation,
    Employee
}

//public class TreeViewNode
//{
//    public string id { get; set; }
//    public string parent { get; set; }
//    public string text { get; set; }
//}



   