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
           // var budgets = db.Budgets.Include(b => b.COA);
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Budgets.Include(b => b.COA).ToList();
            var final = (from f in data
                         select new
                         {
                             Id=f.Id,
                             headName = f.COA.Name,
                             headCode=f.COA.Code,
                             budgetdate = f.budgetDate,
                             budget = f.Budget1
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCodeById(int Id)
        {
            var data = (from d in db.COAs
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
