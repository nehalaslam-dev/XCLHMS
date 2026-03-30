using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class ChequeRegisterController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            ViewBag.Vendors = db.Vendors.OrderBy(v => v.Name).Select(v => new SelectListItem { Text = v.Name, Value = v.Id.ToString() }).ToList();
            ViewBag.Heads = db.COAs.Where(c => c.ParentID != 0).OrderBy(c => c.Code).Select(c => new SelectListItem { Text = c.Name + " (" + c.Code + ")", Value = c.ID.ToString() }).ToList();
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Database.SqlQuery<ChequeRegisterDTO>(@"
                SELECT c.Id, c.ChequeNo, c.ChequeDate, c.Amount, c.BillNo, c.IsCleared, 
                       c.ClearedDate, c.Notes, c.CreatedDate, c.VendorId, 
                       v.Name as VendorName, a.HeadId, coa.Code as ObjectHead 
                FROM ChequeRegister c
                LEFT JOIN Vendors v ON c.VendorId = v.Id
                LEFT JOIN AccountRegister a ON c.BillNo = a.BillNo
                LEFT JOIN COA coa ON a.HeadId = coa.ID
            ").ToList();
            
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public class ChequeRegisterDTO
        {
            public int Id { get; set; }
            public string ChequeNo { get; set; }
            public DateTime? ChequeDate { get; set; }
            public decimal? Amount { get; set; }
            public string BillNo { get; set; }
            public bool? IsCleared { get; set; }
            public DateTime? ClearedDate { get; set; }
            public string Notes { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int? VendorId { get; set; }
            public string VendorName { get; set; }
            public int? HeadId { get; set; }
            public string ObjectHead { get; set; }
        }

        [HttpPost]
        public JsonResult Save(ChequeRegisterDTO model)
        {
            try {
                if (model.Id == 0)
                {
                    db.Database.ExecuteSqlCommand(
                        "INSERT INTO ChequeRegister (ChequeNo, ChequeDate, Amount, BillNo, VendorId, IsCleared, ClearedDate, Notes, CreatedDate) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        model.ChequeNo, model.ChequeDate ?? DateTime.Now, model.Amount ?? 0, model.BillNo, model.VendorId, model.IsCleared ?? false, model.ClearedDate, model.Notes, DateTime.Now
                    );
                }
                else
                {
                    db.Database.ExecuteSqlCommand(
                        "UPDATE ChequeRegister SET ChequeNo={0}, ChequeDate={1}, Amount={2}, BillNo={3}, VendorId={4}, IsCleared={5}, ClearedDate={6}, Notes={7} WHERE Id={8}",
                        model.ChequeNo, model.ChequeDate ?? DateTime.Now, model.Amount ?? 0, model.BillNo, model.VendorId, model.IsCleared ?? false, model.ClearedDate, model.Notes, model.Id
                    );
                }
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
