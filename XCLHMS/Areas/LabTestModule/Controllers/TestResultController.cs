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
using XCLHMS.Models;

namespace XCLHMS.Areas.LabTestModule.Controllers
{
    public class TestResultController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /LabTestModule/TestResult/
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult LoadGrid()
        //{
        //    ObjectResult<GetTestResult_Result> list = db.GetTestResult();
        //    var jsonresult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
        //    jsonresult.MaxJsonLength = int.MaxValue;
        //    return jsonresult;
        //}

        [HttpPost]
        public JsonResult save(TestResult testresult)
        {
            bool status = false;
            try
            {
                if (Convert.ToInt32(TempData["LabTestId"]) > 0)
                {
                    DeleteTestResultDetail(Convert.ToInt32(TempData["LabTestId"]));
                }
                var isValidModel = TryValidateModel(testresult);
                if (isValidModel)
                {
                    db.TestResults.Add(testresult);
                    db.SaveChanges();
                    status = true;
                }
                return new JsonResult { Data = new { status = status } };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        public JsonResult GetAllTestDetail(string myValue)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetAllLabDetail_Result> list = db.GetAllLabDetail(myValue);
            foreach (var item in list)
            {

                var testcatId = item.testcatId;
                var testCategory = item.testCategory;
                var patientId = item.pateintId;
                var patientName = item.patientname;
                var regNo = item.RegNo;
                var testdate = item.testdate;
                var deliveryDate = item.deliveryDate;
                var testId = item.testid;
                var testName = item.testname;
                var normalrange = item.normalrange;
                var unit = item.unit;

                result.Add(new
                {
                    testcatId = testcatId,
                    testCategory = testCategory,
                    patientId = patientId,
                    patientName = patientName,
                    regNo = regNo,
                    testdate = testdate,
                    deliveryDate = deliveryDate,
                    testId = testId,
                    testName = testName,
                    normalrange = normalrange,
                    unit = unit
                });

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", Id);
                var data = db.Database.ExecuteSqlCommand("DeleteTestResultDetail @pId", param);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetRangeById(int Id)
        {
            //List<dynamic> result = new List<dynamic>();
            var data = (from d in db.Tests
                        where d.Id == Id
                        select new
                        {
                            normalRange = d.NormalRange,
                            unit = d.Unit
                        }).ToList().SingleOrDefault();


            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetTestResultDetailById(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetTestResultDetailById_Result> list = db.GetTestResultDetailById(Convert.ToInt32(id));
            foreach (var item in list)
            {
                var Id = item.Id;
                var labtestId = item.labtestId;
                var LabId = item.LabId;
                var labName = item.labName;
                var labtestcatid = item.labtestcatid;
                var testCateory = item.testCateory;
                var patientId = item.pateintId;
                var patientName = item.pateintName;
                var regNo = item.regNo;
                var testId = item.testId;
                var testName = item.testName;
                var testdate = item.testdate;
                var deliverydate = item.deliverydate;
                var normalRange = item.normalRange;
                var actualRange = item.actualRange;
                var unitName = item.unitName;

                result.Add(new
                {
                    Id = Id,
                    labtestId = labtestId,
                    LabId = LabId,
                    labName = labName,
                    labtestcatid = labtestcatid,
                    testCateory = testCateory,
                    patientId = patientId,
                    patientName = patientName,
                    regNo = regNo,
                    testId = testId,
                    testName = testName,
                    testdate = testdate,
                    deliverydate = deliverydate,
                    normalRange = normalRange,
                    actualRange = actualRange,
                    unitName = unitName
                });

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /LabTestModule/TestResult/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<TestResultDetail> testresultdetail = db.TestResultDetails.Where(x => x.TestResultId == id).ToList();
            if (testresultdetail == null)
            {
                return HttpNotFound();
            }
            return View(testresultdetail);
        }

        // GET: /LabTestModule/TestResult/Create
        public ActionResult Create(int? id)
        {
            TempData["LabTestId"] = id;
            List<Pateint> lstPatient = new List<Pateint>();
            lstPatient.Insert(0, new Pateint { Id = 0, Name = "--Select--" });

            ViewBag.PatientId = new SelectList(lstPatient, "Id", "Name");
            ViewBag.LabTestCatId = new SelectList(db.TestCategories, "Id", "Name");
            ViewBag.LabTestId = new SelectList(db.Labs, "Id", "Name");
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Name");

            return View();
        }



        public bool DeleteTestResultDetail(int pid)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                var data = db.Database.ExecuteSqlCommand("DeleteTestResultDetail @pId", param);
                if (data != null)
                {
                    return true;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/TestResult.aspx?Id=" + Id);
            return View();

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
