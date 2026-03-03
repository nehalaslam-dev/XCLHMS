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
    public class EmployeeRoleController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/EmployeeRole/
        public ActionResult Index()
        {
            //return View(db.EmployeeRoles.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = (from er in db.EmployeeRoles
                        join e in db.Employees on er.employeeid equals e.Id
                        join d in db.Designations on er.designationId equals d.Id
                        select new
                        {
                            Id=er.employeeroleid,
                            empName = e.Name,
                            desgName = d.Name,
                            isDefault = er.isdefault

                        });
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);

        }

        // GET: /EmployeeModule/EmployeeRole/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeRole employeerole = db.EmployeeRoles.Find(id);
            if (employeerole == null)
            {
                return HttpNotFound();
            }
            return View(employeerole);
        }

        // GET: /EmployeeModule/EmployeeRole/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name");
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name");
            return View();
        }

        // POST: /EmployeeModule/EmployeeRole/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "employeeroleid,employeeid,designationId,isdefault")] EmployeeRole employeerole)
        {
            if (ModelState.IsValid)
            {
                db.EmployeeRoles.Add(employeerole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name");
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name");
            return View(employeerole);
        }

        // GET: /EmployeeModule/EmployeeRole/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeRole employeerole = db.EmployeeRoles.Find(id);
            if (employeerole == null)
            {
                return HttpNotFound();
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", employeerole.employeeid);
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name", employeerole.designationId);

            return View(employeerole);
        }

        // POST: /EmployeeModule/EmployeeRole/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "employeeroleid,employeeid,designationId,isdefault")] EmployeeRole employeerole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeerole).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", employeerole.employeeid);
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name", employeerole.designationId);

            return View(employeerole);
        }


        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                EmployeeRole empRole = db.EmployeeRoles.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.EmployeeRoles.Remove(empRole);
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
