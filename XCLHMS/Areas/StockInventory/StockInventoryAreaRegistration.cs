using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XCLHMS.Areas.StockInventory
{
    public class StockInventoryAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StockInventory";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StockInventory_default",
                "StockInventory/{controller}/{action}/{id}",
                new { controller = "Vendors", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
