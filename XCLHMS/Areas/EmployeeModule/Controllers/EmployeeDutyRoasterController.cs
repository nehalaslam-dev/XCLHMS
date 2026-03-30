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
    public class EmployeeDutyRoasterController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/EmployeeDutyRoaster/
        public ActionResult Index()
        {
            //var employeedutyroasters = db.EmployeeDutyRoasters.Include(e => e.DutyShifts).Include(e => e.Employee);
            //return View(employeedutyroasters.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.EmployeeDutyRoasters.Include(p => p.ShiftPeriods).Include(p => p.DutyShifts).Include(p => p.Employee).ToList();
            var final = (from d in data
                         select new
                         {
                             Id = d.Id,
                             ShiftPeriods = d.ShiftPeriods.Name,
                             DutyShifts = d.DutyShifts.Name,
                             employee = d.Employee.Name,
                             timeIn = d.InTime,
                             timeout = d.OutTime

                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmployeeModule/EmployeeDutyRoaster/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeDutyRoaster employeedutyroaster = db.EmployeeDutyRoasters.Find(id);
            if (employeedutyroaster == null)
            {
                return HttpNotFound();
            }
            return View(employeedutyroaster);
        }

        // GET: /EmployeeModule/EmployeeDutyRoaster/Create
        public ActionResult Create()
        {
            List<ShiftPeriods> lstperiod = db.ShiftPeriods.ToList();
            lstperiod.Insert(0, new ShiftPeriods { Id = 0, Name = "--Select Shift--" });

            List<DutyShifts> lstdutyshift = new List<DutyShifts>();
            ViewBag.PeriodId = new SelectList(lstperiod, "Id", "Name");
            ViewBag.ShiftId = new SelectList(lstdutyshift, "Id", "Name");
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name");
            return View();
        }

        public JsonResult GetShiftByPeriodId(int id)
        {
            List<DutyShifts> DutyShifts = new List<DutyShifts>();
            if (id > 0)
            {
                DutyShifts = db.DutyShifts.Where(p => p.PeriodID == id).ToList();
            }
            else
            {
                DutyShifts.Insert(0, new DutyShifts { Id = 0, Name = "--Select--" });
            }
            var result = (from d in DutyShifts
                          select new
                          {
                              id = d.Id,
                              name = d.Name
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /EmployeeModule/EmployeeDutyRoaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmployeeId,PeriodId,ShiftId,InTime,OutTime,CreatedDate,ModifyDate")] EmployeeDutyRoaster employeedutyroaster)
        {
            if (ModelState.IsValid)
            {
                employeedutyroaster.CreatedDate = DateTime.Now;
                db.EmployeeDutyRoasters.Add(employeedutyroaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<ShiftPeriods> lstperiod = db.ShiftPeriods.ToList();
            lstperiod.Insert(0, new ShiftPeriods { Id = 0, Name = "--Select Shift--" });
            List<DutyShifts> lstdutyshift = new List<DutyShifts>();

            ViewBag.PeriodId = new SelectList(lstperiod, "Id", "Name");
            ViewBag.ShiftId = new SelectList(lstdutyshift, "Id", "Name");
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", employeedutyroaster.EmployeeId);
            return View(employeedutyroaster);
        }

        // GET: /EmployeeModule/EmployeeDutyRoaster/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeDutyRoaster employeedutyroaster = db.EmployeeDutyRoasters.Find(id);
            TempData["crtDate"] = employeedutyroaster.CreatedDate;
            if (employeedutyroaster == null)
            {
                return HttpNotFound();
            }

            ViewBag.PeriodId = new SelectList(db.ShiftPeriods, "Id", "Name", employeedutyroaster.PeriodId);
            ViewBag.ShiftId = new SelectList(db.DutyShifts, "Id", "Name", employeedutyroaster.ShiftId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", employeedutyroaster.EmployeeId);
            return View(employeedutyroaster);
        }

        // POST: /EmployeeModule/EmployeeDutyRoaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmployeeId,PeriodId,ShiftId,InTime,OutTime,CreatedDate,ModifyDate")] EmployeeDutyRoaster employeedutyroaster)
        {
            if (ModelState.IsValid)
            {
                employeedutyroaster.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                employeedutyroaster.ModifyDate = DateTime.Now;
                db.Entry(employeedutyroaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PeriodId = new SelectList(db.ShiftPeriods, "Id", "Name", employeedutyroaster.PeriodId);
            ViewBag.ShiftId = new SelectList(db.DutyShifts, "Id", "Name", employeedutyroaster.ShiftId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", employeedutyroaster.EmployeeId);
            return View(employeedutyroaster);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                EmployeeDutyRoaster roaster = db.EmployeeDutyRoasters.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.EmployeeDutyRoasters.Remove(roaster);
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

