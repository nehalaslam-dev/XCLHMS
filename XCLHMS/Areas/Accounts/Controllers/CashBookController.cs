using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class CashBookController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCashBookData()
        {
            db.Configuration.ProxyCreationEnabled = false;
            // The cashbook is a view, EF may throw exceptions if not perfectly mapped or if key inference fails.
            // Best perfectly safe approach avoids EDMX limitation by using raw SQL query:
            var data = db.Database.SqlQuery<CashBookDTO>(@"
                SELECT row, Id, HeadName, Code, BillNo, billdate, PartyName, ChequeNo, 
                       BudgetRelease, Revenue, Expense, Balance, RunningBalance 
                FROM cashbook ORDER BY billdate ASC, row ASC
            ").ToList();
            
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public class CashBookDTO
        {
            public long? row { get; set; }
            public int Id { get; set; }
            public string HeadName { get; set; }
            public string Code { get; set; }
            public string BillNo { get; set; }
            public DateTime? billdate { get; set; }
            public string PartyName { get; set; }
            public int? ChequeNo { get; set; }
            public decimal? BudgetRelease { get; set; }
            public decimal? Revenue { get; set; }
            public decimal? Expense { get; set; }
            public decimal? Balance { get; set; }
            public int? RunningBalance { get; set; }
        }

        [HttpPost]
        public JsonResult SaveEntry(cashbook model)
        {
            try {
                // Calculation: Balance = Revenue - Expense
                model.Balance = (model.Revenue ?? 0) - (model.Expense ?? 0);
                
                // For auto opening/closing: total calculations are usually done in a service or stored proc.
                // We will perform local calculation for the current entry.
                
                if (model.Id == 0)
                    db.cashbook.Add(model);
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
