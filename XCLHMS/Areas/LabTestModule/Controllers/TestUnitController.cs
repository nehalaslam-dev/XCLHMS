using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.LabTestModule.Controllers
{
    public class TestUnitController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /LabTestModule/TestUnit/
        public ActionResult Index()
        {
            //return View(db.TestUnits.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.TestUnits.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LabTestModule/TestUnit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestUnit testunit = db.TestUnits.Find(id);
            if (testunit == null)
            {
                return HttpNotFound();
            }
            return View(testunit);
        }

        // GET: /LabTestModule/TestUnit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /LabTestModule/TestUnit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,UnitName,Description")] TestUnit testunit)
        {
            if (ModelState.IsValid)
            {
                db.TestUnits.Add(testunit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testunit);
        }

        // GET: /LabTestModule/TestUnit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestUnit testunit = db.TestUnits.Find(id);
            if (testunit == null)
            {
                return HttpNotFound();
            }
            return View(testunit);
        }

        // POST: /LabTestModule/TestUnit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,UnitName,Description")] TestUnit testunit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testunit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testunit);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                TestUnit units = db.TestUnits.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.TestUnits.Remove(units);
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
