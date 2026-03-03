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
    public class EmployeeTypeController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/EmployeeType/
        public ActionResult Index()
        {
            //return View(db.EmployeeTypes.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.EmployeeTypes.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmployeeModule/EmployeeType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeType employeetype = db.EmployeeTypes.Find(id);
            if (employeetype == null)
            {
                return HttpNotFound();
            }
            return View(employeetype);
        }

        // GET: /EmployeeModule/EmployeeType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmployeeModule/EmployeeType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmpType,Comments")] EmployeeType employeetype)
        {
            if (ModelState.IsValid)
            {
                db.EmployeeTypes.Add(employeetype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employeetype);
        }

        // GET: /EmployeeModule/EmployeeType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeType employeetype = db.EmployeeTypes.Find(id);
            if (employeetype == null)
            {
                return HttpNotFound();
            }
            return View(employeetype);
        }

        // POST: /EmployeeModule/EmployeeType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmpType,Comments")] EmployeeType employeetype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeetype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employeetype);
        }

    

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                EmployeeType emptype = db.EmployeeTypes.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.EmployeeTypes.Remove(emptype);
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
