using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Helpers;
using XCLHMS.Models;

namespace XCLHMS.Areas.LabTestModule.Controllers
{
    public class LabRegistrationController : Controller
    {
        private HMSEntities db = new HMSEntities();


        // GET: /LabTestModule/LabRegistration/
        public ActionResult Index()
        {
            //var labregistrations = db.LabRegistrations.Include(l => l.Pateint).Include(l => l.Test);
            return View();
        }

        public ActionResult LoadGrid()
        {
            ObjectResult<GetLabRegistartion_Result> list = db.GetLabRegistartion();
            var jsonresult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue;
            return jsonresult;

        }

        public ActionResult GetAllTest(int Id)
        {
            var data = (from d in db.Tests
                        where d.TestCatId == Id
                        select new
                        {
                            testId = d.Id,
                            testName = d.Name
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTestById(int id)
        {
            List<Test> lstTests = new List<Test>();
            if (id > 0)
            {
                lstTests = db.Tests.Where(p => p.TestCatId == id).ToList();
                lstTests.Insert(0, new Test { Id = -1, Name = "ALL" });
            }
            else
            {
                lstTests.Insert(0, new Test { Id = 0, Name = "--Select--" });
            }
            var result = (from d in lstTests
                          select new
                          {
                              id = d.Id,
                              name = d.Name
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPateintName(string myvalue)
        {
            var data = (from p in db.Pateints
                        where p.MRNo == myvalue
                        select new
                        {
                            id = p.Id,
                            name = p.Name
                        }).SingleOrDefault();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult save(LabRegistration[] reg)
        {

            string result = "Error! Record Is Not Complete!";
            string finalregNo = string.Empty;

            try
            {
                if (reg != null)
                {
                    DateTime crDate = DateTime.Now;

                    var regNo = db.LabRegistrations.OrderByDescending(x => x.Id).Take(1).Select(x => x.RegNo).ToList().FirstOrDefault();
                    if (regNo != null)
                    {

                        string[] regNoSplit = regNo.Split('-');
                        int maxregNo = Convert.ToInt32(regNoSplit[1]) + 1;

                        finalregNo = "L-00" + maxregNo.ToString();
                    }
                    else
                    {
                        finalregNo = "L-001";
                    }
                    if (TempData["regValue"] != null)
                    {
                        DeleteLabRegbyRegNo(TempData["regValue"].ToString());

                    }


                    foreach (var item in reg)
                    {
                        LabRegistration acReg = new LabRegistration();
                        acReg.PatientId = item.PatientId;
                        acReg.RegDate = item.RegDate;
                        acReg.deliveryDate = item.deliveryDate;
                        //if (TempData["regValue"] != null)
                        //{
                        //    acReg.RegNo = item.RegNo;
                        //}
                        //else
                        //{
                            acReg.RegNo = finalregNo;
                        //}
                        acReg.TestId = item.TestId;
                        acReg.PrescribedById = item.PrescribedById;
                        acReg.TestConductById = item.TestConductById;
                        acReg.CreatedDate = crDate;
                        acReg.employeeId = SessionHelper.EmployeeID;
                        db.LabRegistrations.Add(acReg);
                    }
                    db.SaveChanges();

                    result = "Success! Record saved!";
                }
            }
            catch (Exception ex)
            {

                result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteLabRegbyRegNo(string regno)
        {
            if (regno != null)
            {
                try
                {
                    SqlParameter param = new SqlParameter("@regno", regno);
                    var data = db.Database.ExecuteSqlCommand("DeleteLabRegbyRegNo @regno", param);
                    if (data != null)
                    {
                        return true;
                    }

                }
                catch (Exception)
                {
                    throw;
                }

            }

            return false;
        }

        [HttpPost]
        public JsonResult Delete(string regno)
        {
            try
            {
                SqlParameter param = new SqlParameter("@regno", regno);
                var data = db.Database.ExecuteSqlCommand("DeleteLabRegbyRegNo @regno", param);
                if (data != null)
                {
                    return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }               
            }
            catch (Exception)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLabRecordByRegNo(string RegNo)
        {
            TempData["regValue"] = RegNo;

            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetLabRecordByRegNo_Result> list = db.GetLabRecordByRegNo(RegNo);

            foreach (var item in list)
            {
                var patientId = item.Id;
                var patientName = item.patientName;
                var regno = item.regno;
                var regdate = item.regdate;
                var deliveryDate = item.deliveryDate;
                var categoryId = item.categoryId;
                var categoryName = item.categoryName;
                var testId = item.testId;
                var testName = item.testname;
                var prescribId = item.prescribeId;
                var prescribBy = item.prescribedBy;
                var conductbyId = item.conductId;
                var conductedBy = item.conductedBy;

                result.Add(new
                {
                    patientId = patientId,
                    patientName = patientName,
                    regno = regno,
                    regdate = regdate,
                    deliveryDate = deliveryDate,
                    categoryId = categoryId,
                    categoryName = categoryName,
                    testId = testId,
                    testName = testName,
                    prescribId = prescribId,
                    prescribBy = prescribBy,
                    conductbyId = conductbyId,
                    conductedBy = conductedBy
                });
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        // GET: /LabTestModule/LabRegistration/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabRegistration labregistration = db.LabRegistrations.Find(id);
            if (labregistration == null)
            {
                return HttpNotFound();
            }
            return View(labregistration);
        }

        // GET: /LabTestModule/LabRegistration/Create
        public ActionResult Create(string regNum)
        {
            TempData["regValue"] = regNum;

            List<TestCategory> lstCat = db.TestCategories.ToList();
            lstCat.Insert(0, new TestCategory { Id = 0, Name = "--Select--" });
            List<Test> lstTest = new List<Test>();
            List<Pateint> lstPatient = new List<Pateint>();
            lstPatient.Insert(0, new Pateint { Id = 0, Name = "--Select--" });

            ViewBag.CategoryId = new SelectList(lstCat, "Id", "Name");
            ViewBag.TestId = new SelectList(lstTest, "Id", "Name");
            ViewBag.PatientId = new SelectList(lstPatient, "Id", "Name");
            ViewBag.PrescribedById = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name");
            ViewBag.TestConductById = new SelectList(db.Employees.Where(x => x.DesignationID == 10), "Id", "Name");

            return View();
        }

        // POST: /LabTestModule/LabRegistration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientId,RegNo,RegDate,TestId,CreatedDate")] LabRegistration labregistration)
        {
            if (ModelState.IsValid)
            {
                db.LabRegistrations.Add(labregistration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "NIC", labregistration.PatientId);
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Name", labregistration.TestId);
            return View(labregistration);
        }

        // GET: /LabTestModule/LabRegistration/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabRegistration labregistration = db.LabRegistrations.Find(id);
            if (labregistration == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "NIC", labregistration.PatientId);
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Name", labregistration.TestId);
            return View(labregistration);
        }

        // POST: /LabTestModule/LabRegistration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PatientId,RegNo,RegDate,TestId,CreatedDate")] LabRegistration labregistration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(labregistration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "NIC", labregistration.PatientId);
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Name", labregistration.TestId);
            return View(labregistration);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult LoadReport(string regno)
        {
            Response.Redirect("~/Reports/LabRegistration.aspx?regno=" + regno);
            return View();

        }
    }
}
