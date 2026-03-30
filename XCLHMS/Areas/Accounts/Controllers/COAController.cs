using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class COAController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new XCLHMS.Models.COA());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = db.COAs.Find(id);
            if (model == null) return HttpNotFound();
            return View("Create", model);
        }

        // --- FETCH DATA FOR GRID ---
        [HttpGet]
        public ActionResult LoadGrid()
        {
            try
            {
                // Disables proxy to prevent circular reference errors during JSON serialization
                db.Configuration.ProxyCreationEnabled = false;

                // Use the bridge alias COAs from partial class to ensure it's accessible
                var data = db.COAs.OrderBy(x => x.Code).ToList();

                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(COA model)
        {
            try
            {
                if (model.ID == 0)
                {
                    model.CreatedDate = DateTime.Now;
                    if (model.ParentID == 0) model.ParentID = 0; // Ensure it's set
                    db.COAs.Add(model);
                }
                else
                {
                    var existing = db.COAs.Find(model.ID);
                    if (existing != null)
                    {
                        existing.Code = model.Code;
                        existing.Name = model.Name;
                        existing.Description = model.Name; // Map Name to Description as well
                        existing.ModifiedDate = DateTime.Now;
                        db.Entry(existing).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var data = db.COAs.Find(id);
                if (data != null)
                {
                    db.COAs.Remove(data);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Record not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}