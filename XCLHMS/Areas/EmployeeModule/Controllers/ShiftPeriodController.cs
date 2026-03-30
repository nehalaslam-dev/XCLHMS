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

        // GET: /EmployeeModule/ShiftPeriods/
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

        // GET: /EmployeeModule/ShiftPeriods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftPeriods ShiftPeriods = db.ShiftPeriods.Find(id);
            if (ShiftPeriods == null)
            {
                return HttpNotFound();
            }
            return View(ShiftPeriods);
        }

        // GET: /EmployeeModule/ShiftPeriods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmployeeModule/ShiftPeriods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description,Active,CreatedDate,ModifiedDate")] ShiftPeriods ShiftPeriods)
        {
            if (ModelState.IsValid)
            {
                ShiftPeriods.CreatedDate = DateTime.Now;
                db.ShiftPeriods.Add(ShiftPeriods);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ShiftPeriods);
        }

        // GET: /EmployeeModule/ShiftPeriods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftPeriods ShiftPeriods = db.ShiftPeriods.Find(id);
            TempData["crtDate"] = ShiftPeriods.CreatedDate;
            if (ShiftPeriods == null)
            {
                return HttpNotFound();
            }
            return View(ShiftPeriods);
        }

        // POST: /EmployeeModule/ShiftPeriods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description,Active,CreatedDate,ModifiedDate")] ShiftPeriods ShiftPeriods)
        {
            if (ModelState.IsValid)
            {
                ShiftPeriods.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                ShiftPeriods.ModifiedDate = DateTime.Now;
                db.Entry(ShiftPeriods).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ShiftPeriods);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                ShiftPeriods ShiftPeriods = db.ShiftPeriods.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.ShiftPeriods.Remove(ShiftPeriods);
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

