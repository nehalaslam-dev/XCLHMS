using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class ExpenditureStatementController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMonthlyExpenditure(string fiscalYear)
        {
            db.Configuration.ProxyCreationEnabled = false;
            string[] years = fiscalYear.Split('-');
            int startYear = int.Parse(years[0]);
            int endYear = years.Length > 1 ? int.Parse(years[1]) : startYear + 1;
            if (endYear < 100) endYear += 2000; // Handle 2-digit years if any

            // Generate statement dynamically from Budget and Account Register for 100% interconnected accurate data
            string sql = $@"
                SELECT 
                    c.Code,
                    c.Name as HeadName,
                    ISNULL(b.TotalRevisedBudgetAfterReApp, 0) as BudgetAmount,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 7 AND YEAR(r.billdate) = {startYear}), 0) as JulExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 8 AND YEAR(r.billdate) = {startYear}), 0) as AugExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 9 AND YEAR(r.billdate) = {startYear}), 0) as SepExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 10 AND YEAR(r.billdate) = {startYear}), 0) as OctExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 11 AND YEAR(r.billdate) = {startYear}), 0) as NovExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 12 AND YEAR(r.billdate) = {startYear}), 0) as DecExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 1 AND YEAR(r.billdate) = {endYear}), 0) as JanExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 2 AND YEAR(r.billdate) = {endYear}), 0) as FebExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 3 AND YEAR(r.billdate) = {endYear}), 0) as MarExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 4 AND YEAR(r.billdate) = {endYear}), 0) as AprExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 5 AND YEAR(r.billdate) = {endYear}), 0) as MayExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND MONTH(r.billdate) = 6 AND YEAR(r.billdate) = {endYear}), 0) as JunExpense,
                    ISNULL((SELECT SUM(GrossAmount) FROM AccountRegister r WHERE r.HeadId = b.HeadId AND r.EntryType != 'Receipt' AND ( (YEAR(r.billdate) = {startYear} AND MONTH(r.billdate) >= 7) OR (YEAR(r.billdate) = {endYear} AND MONTH(r.billdate) <= 6) )), 0) as ExpenditureAmount
                FROM Budget b
                LEFT JOIN COA c ON b.HeadId = c.ID
                WHERE b.FiscalYear = '{fiscalYear}'
            ";

            var reportData = db.Database.SqlQuery<StatementDTO>(sql).ToList();

            foreach(var r in reportData)
            {
                r.Balance = r.BudgetAmount - r.ExpenditureAmount;
            }

            return Json(new { data = reportData }, JsonRequestBehavior.AllowGet);
        }

        public class StatementDTO {
            public string Code { get; set; }
            public string HeadName { get; set; }
            public decimal BudgetAmount { get; set; }
            public decimal JulExpense { get; set; }
            public decimal AugExpense { get; set; }
            public decimal SepExpense { get; set; }
            public decimal OctExpense { get; set; }
            public decimal NovExpense { get; set; }
            public decimal DecExpense { get; set; }
            public decimal JanExpense { get; set; }
            public decimal FebExpense { get; set; }
            public decimal MarExpense { get; set; }
            public decimal AprExpense { get; set; }
            public decimal MayExpense { get; set; }
            public decimal JunExpense { get; set; }
            public decimal ExpenditureAmount { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
