using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class ContingencyRegisterController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            // Populate Object Heads for dropdown
            var heads = db.COAs.Where(x => x.ParentID != 0).OrderBy(x => x.Code).ToList();
            ViewBag.Heads = new SelectList(heads, "ID", "Code");
            
            // Populate Vendors for dropdown
            var vendors = db.Vendors.OrderBy(x => x.Name).ToList();
            ViewBag.VendorId = new SelectList(vendors, "Id", "Name");
            
            return View();
        }

        [HttpGet]
        public JsonResult GetLoadData(int? headId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                
                // Bypass EDMX to get all columns including unmapped taxes from Schema
                string sql = @"
                    SELECT a.Id, a.HeadId, a.BillNo, a.billdate, a.VendorId, a.ChequeNo, 
                           a.EntryType, a.AnnualBudget, a.BudgetRelease, a.GrossAmount, 
                           a.IncomeTax, a.SST, a.NetAmount, a.Balance, a.ManualTax,
                           a.StampDuty, a.GST,
                           c.Code as ObjectHead, v.Name as VendorName
                    FROM AccountRegister a 
                    LEFT JOIN COA c ON a.HeadId = c.ID 
                    LEFT JOIN Vendors v ON a.VendorId = v.Id ";
                    
                if (headId.HasValue && headId > 0)
                {
                    sql += " WHERE a.HeadId = " + headId.Value;
                }
                sql += " ORDER BY a.billdate DESC";

                var entries = db.Database.SqlQuery<AccountRegisterDTO>(sql).ToList();

                // 2. Get Annual Budget Balance for this head properly from raw SQL
                decimal balance = 0;
                if (headId.HasValue && headId > 0)
                {
                    balance = db.Database.SqlQuery<decimal?>(
                        "SELECT TotalRevisedBudgetAfterReApp FROM Budget WHERE HeadId = {0}", headId
                    ).FirstOrDefault() ?? 0;

                    var consumed = db.Database.SqlQuery<decimal?>(
                        "SELECT SUM(GrossAmount) FROM AccountRegister WHERE HeadId = {0}", headId
                    ).FirstOrDefault() ?? 0;
                    
                    balance = balance - consumed;
                }

                return Json(new { success = true, entries = entries, budgetBalance = balance }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public class AccountRegisterDTO {
            public int Id { get; set; }
            public int? HeadId { get; set; }
            public string BillNo { get; set; }
            public DateTime? billdate { get; set; }
            public int? VendorId { get; set; }
            public int? ChequeNo { get; set; }
            public string EntryType { get; set; }
            public decimal? AnnualBudget { get; set; }
            public decimal? BudgetRelease { get; set; }
            public decimal? GrossAmount { get; set; }
            public decimal? IncomeTax { get; set; }
            public decimal? SST { get; set; }
            public decimal? StampDuty { get; set; }
            public decimal? GST { get; set; }
            public decimal? NetAmount { get; set; }
            public decimal? Balance { get; set; }
            public decimal? ManualTax { get; set; }
            public string ObjectHead { get; set; }
            public string VendorName { get; set; }
        }

        [HttpPost]
        public JsonResult Save(AccountRegisterDTO model)
        {
            try
            {
                // Calculations
                decimal gross = model.GrossAmount ?? 0;
                decimal it = model.IncomeTax ?? 0;
                decimal sst = model.SST ?? 0;
                decimal gst = model.GST ?? 0;
                decimal stamp = model.StampDuty ?? 0; 
                
                model.NetAmount = gross - (it + sst + gst + stamp);
                
                if (model.Id == 0)
                {
                    db.Database.ExecuteSqlCommand(
                        "INSERT INTO AccountRegister (HeadId, BillNo, billdate, VendorId, ChequeNo, EntryType, AnnualBudget, BudgetRelease, GrossAmount, IncomeTax, SST, GST, StampDuty, NetAmount, Balance, ManualTax) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15})",
                         model.HeadId, model.BillNo, model.billdate ?? DateTime.Now, model.VendorId, model.ChequeNo, model.EntryType, model.AnnualBudget, model.BudgetRelease, model.GrossAmount, it, sst, gst, stamp, model.NetAmount, model.Balance, model.ManualTax
                    );
                }
                else
                {
                    db.Database.ExecuteSqlCommand(
                        "UPDATE AccountRegister SET HeadId={0}, BillNo={1}, billdate={2}, VendorId={3}, ChequeNo={4}, EntryType={5}, AnnualBudget={6}, BudgetRelease={7}, GrossAmount={8}, IncomeTax={9}, SST={10}, GST={11}, StampDuty={12}, NetAmount={13}, Balance={14}, ManualTax={15} WHERE Id={16}",
                        model.HeadId, model.BillNo, model.billdate ?? DateTime.Now, model.VendorId, model.ChequeNo, model.EntryType, model.AnnualBudget, model.BudgetRelease, model.GrossAmount, it, sst, gst, stamp, model.NetAmount, model.Balance, model.ManualTax, model.Id
                    );
                }

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
                var data = db.AccountRegister.Find(id);
                if (data != null)
                {
                    db.AccountRegister.Remove(data);
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
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
