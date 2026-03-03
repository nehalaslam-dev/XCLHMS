using System.Web.Mvc;

namespace XCLHMS.Areas.InventoryModule
{
    public class InventoryModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InventoryModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InventoryModule_default",
                "InventoryModule/{controller}/{action}/{id}",
                new { controller = "Vendor", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}