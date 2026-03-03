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
    public class EmployeeAllowanceController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/EmployeeAllowance/
        public ActionResult Index()
        {
            //var employeeallowances = db.EmployeeAllowances.Include(e => e.EmployeeType);
            //return View(employeeallowances.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.EmployeeAllowances.Include(e => e.EmployeeType).ToList();
            var final = (from d in data
                         select new
                         {
                             Id = d.Id,
                             EmpType = d.EmployeeType.EmpType,
                             HouseRent = d.HouseRent,
                             Conveyance = d.Conveyance,
                             Casuality = d.Casuality,
                             Entertaiment = d.Entertaiment,
                             NonPracticing = d.NonPracticing,
                             RCA = d.RCA,
                             Medical = d.Medical,
                             Uniform = d.Uniform,
                             Mess = d.Mess,
                             Nursing = d.Nursing,
                             Health = d.Health,
                             Institute = d.Institute,
                             Hardworking = d.Hardworking,
                             Orderly = d.Orderly,
                             SeniorPost = d.SeniorPost,
                             Washing = d.Washing,
                             Computer = d.Computer,
                             HPA = d.HPA
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmployeeModule/EmployeeAllowance/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeAllowance employeeallowance = db.EmployeeAllowances.Find(id);
            if (employeeallowance == null)
            {
                return HttpNotFound();
            }
            return View(employeeallowance);
        }

        // GET: /EmployeeModule/EmployeeAllowance/Create
        public ActionResult Create()
        {
            ViewBag.EmpTypeId = new SelectList(db.EmployeeTypes, "Id", "EmpType");
            return View();
        }

        // POST: /EmployeeModule/EmployeeAllowance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmpTypeId,HouseRent,Conveyance,Casuality,Entertaiment,NonPracticing,RCA,Medical,Uniform,Mess,Nursing,Health,Institute,Hardworking,Orderly,SeniorPost,Washing,Computer,HPA")] EmployeeAllowance employeeallowance)
        {
            if (ModelState.IsValid)
            {
                db.EmployeeAllowances.Add(employeeallowance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpTypeId = new SelectList(db.EmployeeTypes, "Id", "EmpType", employeeallowance.EmpTypeId);
            return View(employeeallowance);
        }

        // GET: /EmployeeModule/EmployeeAllowance/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeAllowance employeeallowance = db.EmployeeAllowances.Find(id);
            if (employeeallowance == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpTypeId = new SelectList(db.EmployeeTypes, "Id", "EmpType", employeeallowance.EmpTypeId);
            return View(employeeallowance);
        }

        // POST: /EmployeeModule/EmployeeAllowance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmpTypeId,HouseRent,Conveyance,Casuality,Entertaiment,NonPracticing,RCA,Medical,Uniform,Mess,Nursing,Health,Institute,Hardworking,Orderly,SeniorPost,Washing,Computer,HPA")] EmployeeAllowance employeeallowance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeallowance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpTypeId = new SelectList(db.EmployeeTypes, "Id", "EmpType", employeeallowance.EmpTypeId);
            return View(employeeallowance);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                EmployeeAllowance empAllowance = db.EmployeeAllowances.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.EmployeeAllowances.Remove(empAllowance);
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
