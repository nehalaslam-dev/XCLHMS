using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.PatientModule.Controllers
{
    public class PateintAppointmentsController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /PatientModule/PateintAppointments/
        public ActionResult Index()
        {                  
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var final = db.PateintAppointments.Include(p => p.Employee).Include(p => p.Pateint).ToList();
            var data = (from f in final
                        select new
                        {
                            Id = f.Id,
                            pateintName = f.Pateint.Name,
                            doctorName = f.Employee.Name,
                            appDate = f.AppDate,
                            appTime = f.AppTime,
                            appStatus = f.AppStatus,
                            Reason = f.Description
                        });
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /PatientModule/PateintAppointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PateintAppointment pateintappointment = db.PateintAppointments.Find(id);
            if (pateintappointment == null)
            {
                return HttpNotFound();
            }
            return View(pateintappointment);
        }

        // GET: /PatientModule/PateintAppointments/Create
        public ActionResult Create()
        {
            List<Pateint> lstPatient = new List<Pateint>();
            lstPatient.Insert(0, new Pateint { Id = 0, Name = "--Select--" });
            ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name");
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name");
            return View();
        }

        // POST: /PatientModule/PateintAppointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientId,EmployeeId,AppDate,AppTime,AppStatus,Description")] PateintAppointment pateintappointment)
        {
           
            if (ModelState.IsValid)
            {        
                pateintappointment.AppStatus = "Waiting";
                db.PateintAppointments.Add(pateintappointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", pateintappointment.EmployeeId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", pateintappointment.PatientId);
        
            return View(pateintappointment);
        }

        // GET: /PatientModule/PateintAppointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PateintAppointment pateintappointment = db.PateintAppointments.Find(id);
            TempData["AppStatus"] = pateintappointment.AppStatus;
            if (pateintappointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", pateintappointment.EmployeeId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", pateintappointment.PatientId);
            return View(pateintappointment);
        }

        // POST: /PatientModule/PateintAppointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PatientId,EmployeeId,AppDate,AppTime,AppStatus,Description")] PateintAppointment pateintappointment)
        {
            if (ModelState.IsValid)
            {
                pateintappointment.AppStatus = TempData["AppStatus"].ToString();
                db.Entry(pateintappointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "Id", "Name", pateintappointment.EmployeeId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", pateintappointment.PatientId);
            return View(pateintappointment);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                PateintAppointment pa = db.PateintAppointments.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.PateintAppointments.Remove(pa);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetPateintByToken(string tokenno)
        {
            var data = (from d in db.Tokens
                        join p in db.Pateints on d.PatientId equals p.Id
                        where d.TokenNumber == tokenno
                        select new
                        {
                            id = p.Id,
                            name = p.Name
                        }).SingleOrDefault();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
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
