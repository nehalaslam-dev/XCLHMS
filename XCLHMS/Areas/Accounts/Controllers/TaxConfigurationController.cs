using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class TaxConfigurationController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadTaxes()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.TaxConfigurations.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveTax(TaxConfiguration model)
        {
            try {
                if (model.ID == 0)
                {
                    model.CreatedDate = DateTime.Now;
                    db.TaxConfigurations.Add(model);
                }
                else
                    db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
