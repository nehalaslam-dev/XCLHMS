using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class BudgetController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /Accounts/Budget/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            // Bypass EDMX limitation by using raw SQL query
            var data = db.Database.SqlQuery<BudgetDTO>(@"
                SELECT b.Id, b.HeadId, b.BudgetEstimates, b.RevisedBudget, b.AdditionalBudget, b.SupplementaryBudget, 
                       b.TotalRevisedBudget, b.ReAppropriationPositive, b.ReAppropriationNegative, b.TotalRevisedBudgetAfterReApp, 
                       b.FiscalYear, b.budgetDate, b.CreatedDate, b.ModifiedDate, c.Code as COACode, c.Name as COAName 
                FROM Budget b LEFT JOIN COA c ON b.HeadId = c.ID").ToList();
            
            var mapped = data.Select(x => new {
                Id = x.Id,
                HeadId = x.HeadId,
                BudgetEstimates = x.BudgetEstimates,
                RevisedBudget = x.RevisedBudget,
                AdditionalBudget = x.AdditionalBudget,
                SupplementaryBudget = x.SupplementaryBudget,
                TotalRevisedBudget = x.TotalRevisedBudget,
                ReAppropriationPositive = x.ReAppropriationPositive,
                ReAppropriationNegative = x.ReAppropriationNegative,
                TotalRevisedBudgetAfterReApp = x.TotalRevisedBudgetAfterReApp,
                COA = new { Name = x.COAName, Code = x.COACode }
            });

            return Json(new { data = mapped }, JsonRequestBehavior.AllowGet);
        }

        public class BudgetDTO {
            public int Id { get; set; }
            public int HeadId { get; set; }
            public decimal? BudgetEstimates { get; set; }
            public decimal? RevisedBudget { get; set; }
            public decimal? AdditionalBudget { get; set; }
            public decimal? SupplementaryBudget { get; set; }
            public decimal? TotalRevisedBudget { get; set; }
            public decimal? ReAppropriationPositive { get; set; }
            public decimal? ReAppropriationNegative { get; set; }
            public decimal? TotalRevisedBudgetAfterReApp { get; set; }
            public string FiscalYear { get; set; }
            public DateTime? budgetDate { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public string COACode { get; set; }
            public string COAName { get; set; }
        }

        [HttpPost]
        public JsonResult Save(Budget model)
        {
            try {
                // Auto Calculations
                model.TotalRevisedBudget = (model.BudgetEstimates ?? 0) + (model.RevisedBudget ?? 0) + (model.AdditionalBudget ?? 0) + (model.SupplementaryBudget ?? 0);
                model.TotalRevisedBudgetAfterReApp = (model.TotalRevisedBudget ?? 0) + (model.ReAppropriationPositive ?? 0) - (model.ReAppropriationNegative ?? 0);

                if (model.Id == 0)
                {
                    model.CreatedDate = DateTime.Now;
                    db.Budgets.Add(model);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                    model.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                }
                
                // Execute raw SQL to update the extended fields mapped in partial class (bypassing EDMX limitation)
                db.Database.ExecuteSqlCommand(
                    "UPDATE Budget SET BudgetEstimates={0}, RevisedBudget={1}, AdditionalBudget={2}, SupplementaryBudget={3}, TotalRevisedBudget={4}, ReAppropriationPositive={5}, ReAppropriationNegative={6}, TotalRevisedBudgetAfterReApp={7}, FiscalYear={8} WHERE Id={9}",
                    model.BudgetEstimates ?? 0, model.RevisedBudget ?? 0, model.AdditionalBudget ?? 0, model.SupplementaryBudget ?? 0,
                    model.TotalRevisedBudget ?? 0, model.ReAppropriationPositive ?? 0, model.ReAppropriationNegative ?? 0,
                    model.TotalRevisedBudgetAfterReApp ?? 0, model.FiscalYear ?? "", model.Id
                );

                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetCodeById(int Id)
        {
            var data = (from d in db.COA
                        where d.ID == Id
                        select d.Code).ToList().SingleOrDefault();
            if (data == null)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /Accounts/Budget/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: /Accounts/Budget/Create
        public ActionResult Create()
        {
            var data = db.COAs.Where(p => p.ParentID != 0).ToList();
            ViewBag.HeadId = new SelectList(data, "ID", "Name");
            return View();
        }

        // POST: /Accounts/Budget/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,HeadId,Budget1,budgetDate,CreatedDate,ModifiedDate")] Budget budget)
        {
            //if (ModelState.IsValid)
            //{
            //    budget.budgetDate = DateTime.Now;
            //    budget.CreatedDate = DateTime.Now;
            //    db.Budgets.Add(budget);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            db.InsertBudget(budget.HeadId, budget.Budget1);
            return RedirectToAction("Index");

            var data = db.COAs.Where(p => p.ParentID != 0).ToList();
            ViewBag.HeadId = new SelectList(data, "ID", "Name", budget.HeadId);
            return View(budget);
        }

        // GET: /Accounts/Budget/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            TempData["budgetDate"] = budget.budgetDate;
            TempData["crtDate"] = budget.CreatedDate;
            if (budget == null)
            {
                return HttpNotFound();
            }
            var data = db.COAs.Where(p => p.ParentID != 0).ToList();
            ViewBag.HeadId = new SelectList(data, "ID", "Name", budget.HeadId);
            return View(budget);
        }

        // POST: /Accounts/Budget/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,HeadId,Budget1,budgetDate,CreatedDate,ModifiedDate")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.budgetDate = Convert.ToDateTime(TempData["budgetDate"]);
                budget.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                budget.ModifiedDate = DateTime.Now;
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var data = db.COAs.Where(p => p.ParentID != 0).ToList();
            ViewBag.HeadId = new SelectList(data, "ID", "Name", budget.HeadId);
            return View(budget);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Budget budget = db.Budgets.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Budgets.Remove(budget);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
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
