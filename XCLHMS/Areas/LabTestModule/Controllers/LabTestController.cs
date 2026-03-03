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
    public class LabTestController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /LabTestModule/LabTest/
        public ActionResult Index()
        {
            //var labtests = db.LabTests.Include(l => l.Employee).Include(l => l.Employee1).Include(l => l.Lab).Include(l => l.Pateint).Include(l => l.Test);
            //return View(labtests.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.LabTests.Include(l => l.Employee).Include(l => l.Employee1).Include(l => l.Lab).Include(l => l.Pateint).Include(l => l.TestCategory).ToList();
            var final = (from d in data
                         select new
                         {
                             Id = d.Id,
                             LabName = d.Lab.Name,
                             pateintName = d.Pateint.Name,
                             mrno=d.Pateint.MRNo,
                             nic=d.Pateint.NIC,
                             testName = d.TestCategory.Name,
                             prescribdBy = d.Employee.Name,
                             conductBy = d.Employee1.Name,
                             description = d.Description

                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LabTestModule/LabTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabTest labtest = db.LabTests.Find(id);
            if (labtest == null)
            {
                return HttpNotFound();
            }
            return View(labtest);
        }

        // GET: /LabTestModule/LabTest/Create
        public ActionResult Create()
        {
            ViewBag.PrescribedById = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name");
            ViewBag.TestConductById = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name");
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name");
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name");
            ViewBag.TestId = new SelectList(db.TestCategories, "Id", "Name");
            return View();
        }

        // POST: /LabTestModule/LabTest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LabId,PatientId,TestId,PrescribedById,TestConductById,Description,CreatedDate,ModifiedDate")] LabTest labtest)
        {
            if (ModelState.IsValid)
            {
                db.LabTests.Add(labtest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PrescribedById = new SelectList(db.Employees, "Id", "Name", labtest.PrescribedById);
            ViewBag.TestConductById = new SelectList(db.Employees, "Id", "Name", labtest.TestConductById);
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name", labtest.LabId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", labtest.PatientId);
            ViewBag.TestId = new SelectList(db.TestCategories, "Id", "Name", labtest.TestId);
            return View(labtest);
        }

        // GET: /LabTestModule/LabTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabTest labtest = db.LabTests.Find(id);
            if (labtest == null)
            {
                return HttpNotFound();
            }
            ViewBag.PrescribedById = new SelectList(db.Employees, "Id", "Name", labtest.PrescribedById);
            ViewBag.TestConductById = new SelectList(db.Employees, "Id", "Name", labtest.TestConductById);
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name", labtest.LabId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", labtest.PatientId);
            ViewBag.TestId = new SelectList(db.TestCategories, "Id", "Name", labtest.TestId);
            return View(labtest);
        }

        // POST: /LabTestModule/LabTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LabId,PatientId,TestId,PrescribedById,TestConductById,Description,CreatedDate,ModifiedDate")] LabTest labtest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(labtest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PrescribedById = new SelectList(db.Employees, "Id", "Name", labtest.PrescribedById);
            ViewBag.TestConductById = new SelectList(db.Employees, "Id", "Name", labtest.TestConductById);
            ViewBag.LabId = new SelectList(db.Labs, "Id", "Name", labtest.LabId);
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name", labtest.PatientId);
            ViewBag.TestId = new SelectList(db.TestCategories, "Id", "Name", labtest.TestId);
            return View(labtest);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                LabTest labs = db.LabTests.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.LabTests.Remove(labs);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetPateintByTokenAndMR(string myValue)
        {
            var data = (from  p in db.Pateints
                        where p.MRNo == myValue || p.NIC == myValue
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
