using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.EmployeeModule.Controllers
{
    public class ShiftPeriodController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/ShiftPeriod/
        public ActionResult Index()
        {
           // return View(db.ShiftPeriods.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.ShiftPeriods.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmployeeModule/ShiftPeriod/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftPeriod shiftperiod = db.ShiftPeriods.Find(id);
            if (shiftperiod == null)
            {
                return HttpNotFound();
            }
            return View(shiftperiod);
        }

        // GET: /EmployeeModule/ShiftPeriod/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmployeeModule/ShiftPeriod/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description,Active,CreatedDate,ModifiedDate")] ShiftPeriod shiftperiod)
        {
            if (ModelState.IsValid)
            {
                shiftperiod.CreatedDate = DateTime.Now;
                db.ShiftPeriods.Add(shiftperiod);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shiftperiod);
        }

        // GET: /EmployeeModule/ShiftPeriod/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftPeriod shiftperiod = db.ShiftPeriods.Find(id);
            TempData["crtDate"] = shiftperiod.CreatedDate;
            if (shiftperiod == null)
            {
                return HttpNotFound();
            }
            return View(shiftperiod);
        }

        // POST: /EmployeeModule/ShiftPeriod/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description,Active,CreatedDate,ModifiedDate")] ShiftPeriod shiftperiod)
        {
            if (ModelState.IsValid)
            {
                shiftperiod.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                shiftperiod.ModifiedDate = DateTime.Now;
                db.Entry(shiftperiod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shiftperiod);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                ShiftPeriod shiftperiod = db.ShiftPeriods.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.ShiftPeriods.Remove(shiftperiod);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
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
