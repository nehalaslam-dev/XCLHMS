using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCLHMS.Models
{
    public class MenuModel
    {
        public int menuId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentId { get; set; }
        public bool HasChild { get { return ChildMenu != null && ChildMenu.Count() > 0; } }
        public List<MenuModel> ChildMenu { get; set; }


    }
}