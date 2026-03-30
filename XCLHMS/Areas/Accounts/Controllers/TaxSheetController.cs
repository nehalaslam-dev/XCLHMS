using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class TaxSheetController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadSheet(DateTime? startDate, DateTime? endDate)
        {
            db.Configuration.ProxyCreationEnabled = false;

            if (!startDate.HasValue) startDate = new DateTime(ConfigurationHelper.GetCurrentYear(), 7, 1);
            if (!endDate.HasValue) endDate = DateTime.Now;

            string sql = @"
                SELECT 
                    a.Id,
                    v.PaymentSection,
                    v.NTN,
                    v.CNIC,
                    v.Name as VendorName,
                    v.Code as City,
                    v.Address,
                    v.Status as TaxStatus,
                    a.GrossAmount as TaxableAmount,
                    a.IncomeTax,
                    a.SST as SRB,
                    a.StampDuty,
                    a.NetAmount,
                    c.Code as ObjectHead,
                    a.BillNo,
                    ch.ChequeNo,
                    ch.ChequeDate
                FROM AccountRegister a
                JOIN Vendors v ON a.VendorId = v.Id
                JOIN COA c ON a.HeadId = c.ID
                LEFT JOIN ChequeRegister ch ON a.BillNo = ch.BillNo
                WHERE (a.IncomeTax > 0 OR a.SST > 0 OR a.StampDuty > 0)
                  AND a.billdate >= {0} AND a.billdate <= {1}
                ORDER BY a.billdate DESC
            ";

            var reportData = db.Database.SqlQuery<TaxSheetDTO>(sql, startDate, endDate).ToList();
            return Json(new { data = reportData }, JsonRequestBehavior.AllowGet);
        }

        public class TaxSheetDTO {
            public int Id { get; set; }
            public string PaymentSection { get; set; }
            public string NTN { get; set; }
            public string CNIC { get; set; }
            public string VendorName { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public string TaxStatus { get; set; }
            public decimal? TaxableAmount { get; set; }
            public decimal? IncomeTax { get; set; }
            public decimal? SRB { get; set; }
            public decimal? StampDuty { get; set; }
            public decimal? NetAmount { get; set; }
            public string ObjectHead { get; set; }
            public string BillNo { get; set; }
            public string ChequeNo { get; set; }
            public DateTime? ChequeDate { get; set; }
        }
    }
    
    // Simple helper class since it's used inside
    public static class ConfigurationHelper {
        public static int GetCurrentYear() {
            var n = DateTime.Now;
            if (n.Month < 7) return n.Year - 1;
            return n.Year;
        }
    }
}
