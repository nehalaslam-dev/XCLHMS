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
    public class SemanAnalysisController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /LabTestModule/SemanAnalysis/
        public ActionResult Index()
        {
           // var semananalysis = db.SemanAnalysis.Include(s => s.Labs).Include(s => s.Pateint);
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.SemanAnalysis.Include(s => s.Labs).Include(s => s.Pateint).Include(s=>s.Employee).ToList();
            var final = (from d in data
                         select new { 
                           Id=d.Id,
                           labName = d.Labs.Name,
                           patientName =d.Pateint.Name,
                           doctorName = d.Employee.Name,
                           nic = d.Pateint.NIC,
                           mrno = d.Pateint.MRNo,
                           testdate = d.TestDate,
                           sampleDate = d.sampleDate
                          
                         
                         });
         
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPateintName(string myvalue)
        {
            var data = (from p in db.Pateints
                        where p.MRNo == myvalue || p.NIC == myvalue
                        select new
                        {
                            id = p.Id,
                            name = p.Name
                        }).SingleOrDefault();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LabTestModule/SemanAnalysis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SemanAnalysis SemanAnalysis = db.SemanAnalysis.Find(id);
            if (SemanAnalysis == null)
            {
                return HttpNotFound();
            }
            return View(SemanAnalysis);
        }

        // GET: /LabTestModule/SemanAnalysis/Create
        public ActionResult Create()
        {
            ViewBag.employeeId = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name");
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name");
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name");
            return View();
        }

        // POST: /LabTestModule/SemanAnalysis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientId,LabId,employeeId,TestDate,sampleDate,Quantity,colour,consistency,reaction,count,countUnit,motility,motilityUnit,normal,normalUnit,puscells,puscellsUnit,Remarks")] SemanAnalysis SemanAnalysis)
        {
            if (ModelState.IsValid)
            {
                db.SemanAnalysis.Add(SemanAnalysis);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employeeId = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name", SemanAnalysis.employeeId);
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name", SemanAnalysis.LabId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", SemanAnalysis.PatientId);
            return View(SemanAnalysis);
        }

        // GET: /LabTestModule/SemanAnalysis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SemanAnalysis SemanAnalysis = db.SemanAnalysis.Find(id);
            if (SemanAnalysis == null)
            {
                return HttpNotFound();
            }
            ViewBag.employeeId = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name", SemanAnalysis.employeeId);
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name", SemanAnalysis.LabId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", SemanAnalysis.PatientId);
            return View(SemanAnalysis);
        }

        // POST: /LabTestModule/SemanAnalysis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PatientId,LabId,employeeId,TestDate,sampleDate,Quantity,colour,consistency,reaction,count,countUnit,motility,motilityUnit,normal,normalUnit,puscells,puscellsUnit,Remarks")] SemanAnalysis SemanAnalysis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(SemanAnalysis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employeeId = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name", SemanAnalysis.employeeId);
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name", SemanAnalysis.LabId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", SemanAnalysis.PatientId);
            return View(SemanAnalysis);
        }


        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                SemanAnalysis sa = db.SemanAnalysis.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.SemanAnalysis.Remove(sa);
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

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/SemanAnalysis.aspx?Id=" + Id);
            return View();

        }
    }
}

