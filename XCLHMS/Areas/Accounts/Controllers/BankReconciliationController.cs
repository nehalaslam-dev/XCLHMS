using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class BankReconciliationController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadReconciliation(int month, int year)
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
                WHERE MONTH(c.ChequeDate) = {0} AND YEAR(c.ChequeDate) = {1}
            ", month, year).ToList();
            
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
        public JsonResult MarkCleared(int id, DateTime clearedDate)
        {
            try {
                db.Database.ExecuteSqlCommand(
                    "UPDATE ChequeRegister SET IsCleared = 1, ClearedDate = {0} WHERE Id = {1}",
                    clearedDate, id
                );
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
