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
    public class DutyShiftController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/DutyShift/
        public ActionResult Index()
        {
            //var dutyshifts = db.DutyShifts.Include(d => d.ShiftPeriod);
            //return View(dutyshifts.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.DutyShifts.Include(e => e.ShiftPeriod).ToList();
            var final = (from f in data
                         select new
                         {
                             Id = f.Id,
                             shiftName = f.ShiftPeriod.Name,
                             dutyshiftName = f.Name,
                             Description = f.Description,
                             StartTime = f.StartTime,
                             EndTime = f.EndTime

                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmployeeModule/DutyShift/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DutyShift dutyshift = db.DutyShifts.Find(id);
            if (dutyshift == null)
            {
                return HttpNotFound();
            }
            return View(dutyshift);
        }

        // GET: /EmployeeModule/DutyShift/Create
        public ActionResult Create()
        {
            ViewBag.PeriodID = new SelectList(db.ShiftPeriods, "Id", "Name");
            return View();
        }

        // POST: /EmployeeModule/DutyShift/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PeriodID,Name,Description,StartTime,EndTime,CreateDate,ModifiedDate")] DutyShift dutyshift)
        {
            if (ModelState.IsValid)
            {
                dutyshift.CreateDate = DateTime.Now;
                db.DutyShifts.Add(dutyshift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PeriodID = new SelectList(db.ShiftPeriods, "Id", "Name", dutyshift.PeriodID);
            return View(dutyshift);
        }

        // GET: /EmployeeModule/DutyShift/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DutyShift dutyshift = db.DutyShifts.Find(id);
            TempData["crtDate"] = dutyshift.CreateDate;
            if (dutyshift == null)
            {
                return HttpNotFound();
            }
            ViewBag.PeriodID = new SelectList(db.ShiftPeriods, "Id", "Name", dutyshift.PeriodID);
            return View(dutyshift);
        }

        // POST: /EmployeeModule/DutyShift/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PeriodID,Name,Description,StartTime,EndTime,CreateDate,ModifiedDate")] DutyShift dutyshift)
        {
            if (ModelState.IsValid)
            {
                dutyshift.CreateDate = Convert.ToDateTime(TempData["crtDate"]);
                dutyshift.ModifiedDate = DateTime.Now;
                db.Entry(dutyshift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PeriodID = new SelectList(db.ShiftPeriods, "Id", "Name", dutyshift.PeriodID);
            return View(dutyshift);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                DutyShift dutyshift = db.DutyShifts.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.DutyShifts.Remove(dutyshift);
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
