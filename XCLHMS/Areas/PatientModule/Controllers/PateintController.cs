
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Helpers;
using XCLHMS.Models;

namespace XCLHMS.Areas.PatientModule.Controllers
{
    public class PateintController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /PatientModule/Pateint/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult IsNicExists(string nic)
        {
            bool status;
            var nicno = db.Pateints.Where(p => p.NIC == nic).Select(p => p.NIC).FirstOrDefault();

            if (nicno != null)
            {
                status = false;
            }
            else
            {
                status = true;
            }

            return Json(status);
        }

        //[HttpPost]
        //public ActionResult LoadGrid(int iDisplayLenght,int iDisplayStart,int iSortCol_0,string sSortDir_0,string sSearch)
        //{
        //    int displayLength = iDisplayLenght;
        //    int displayStart = iDisplayStart;
        //    int sortCol = iSortCol_0;
        //    string sortDir = sSortDir_0;
        //    string search = sSearch;

        //    ObjectResult<spGetPatient_Result> patientList = db.spGetPatient(displayLength, displayStart, sortCol, sortDir, search);

        //    int filterCount = patientList.Count();

        //    var result = new
        //    {
        //        iTotalRecords = GetPatientTotalCount(),
        //        iTotalDisplayRecords = filterCount,
        //        aaData = patientList
        //    };

        //    //db.Configuration.ProxyCreationEnabled = false;
        //    //var data = db.Pateints.Include(a => a.PatientType).ToList();
        //    //var final = (from d in data
        //    //             select new
        //    //             {
        //    //                 Id = d.Id,
        //    //                 NIC = d.NIC,
        //    //                 MRNO = d.MRNo,
        //    //                 Name = d.Name,
        //    //                 pateintType = d.PatientType.Name,
        //    //                 Gender = d.Gender,
        //    //                 Age = d.Age,
        //    //                 bloodGroup = d.BloodGroup,
        //    //                 contactNo = d.ContactNo,
        //    //                 AdmitDate = d.AdmitDate

        //    //             });
        //    var jsonresult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
        //    jsonresult.MaxJsonLength = int.MaxValue;
        //    return jsonresult;
        //}

        //private int GetPatientTotalCount()
        //{
        //    int totalPatientCount = 0;
        //    totalPatientCount = db.Pateints.Count();
        //    return totalPatientCount;
        //}

        public ActionResult LoadReport(string mrno)
        {
            if (TempData["mrno"] != null)
            {
                Response.Redirect("~/Reports/PateintReport.aspx?MRNO=" + TempData["mrno"].ToString());
            }
            // return View();
            return RedirectToAction("Edit");

        }

        // GET: /PatientModule/Pateint/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pateint pateint = db.Pateints.Find(id);
            if (pateint == null)
            {
                return HttpNotFound();
            }
            return View(pateint);
        }

        // GET: /PatientModule/Pateint/Create
        public ActionResult Create()
        {
          
          
            ViewBag.PatientTypeID = new SelectList(db.PatientTypes, "Id", "Name");
         
            return View();
        }

        

        // POST: /PatientModule/Pateint/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NIC,NICType,PatientTypeID,MRNo,Name,FName,Gender,Age,BloodGroup,Address,ContactNo,AdmitDate,Description,CreatedDate,ModifiedDate,EmployeeId")] Pateint pateint)
        {

            if (ModelState.IsValid)
            {
                var mrNo = db.Pateints.OrderByDescending(x => x.Id).Take(1).Select(x => x.MRNo).ToList().FirstOrDefault();
                // var mrNo = db.Pateints.Max(x => x.MRNo);
                if (mrNo != null)
                {
                    string[] mrNoSplit = mrNo.Split('-');
                    int maxMrNo = Convert.ToInt32(mrNoSplit[1]) + 1;

                    pateint.MRNo = "MR-" + maxMrNo.ToString();
                }
                else
                {
                    pateint.MRNo = "MR-1";
                }

                pateint.AdmitDate = DateTime.Now;
                pateint.CreatedDate = DateTime.Now;

                db.InsertPatient(pateint.NIC, pateint.NICType, pateint.PatientTypeID, pateint.MRNo, pateint.Name, pateint.FName,
                                 pateint.Gender, pateint.Age, pateint.BloodGroup, pateint.Address, pateint.ContactNo, pateint.AdmitDate,
                                 pateint.Description, pateint.CreatedDate, SessionHelper.EmployeeID);

                int myId = GetPatientByMR(pateint.MRNo);

                TempData["mrno"] = pateint.MRNo;

                return RedirectToAction("Edit", new { @Id = myId });

                //return RedirectToAction("Index");
            }


            ViewBag.PatientTypeID = new SelectList(db.PatientTypes, "Id", "Name", pateint.PatientTypeID);
            
            return View(pateint);
        }

        public int GetPatientByMR(string mrno)
        {
            int Id = db.Pateints.Where(s => s.MRNo == mrno).Select(s => s.Id).FirstOrDefault();
            return Id;
        }

        // GET: /PatientModule/Pateint/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pateint pateint = db.Pateints.Find(id);
            TempData["crtDate"] = pateint.CreatedDate;
            TempData["admitDate"] = pateint.AdmitDate;

            TempData["mrno"] = pateint.MRNo;

            if (pateint == null)
            {
                return HttpNotFound();
            }


            ViewBag.PatientTypeID = new SelectList(db.PatientTypes, "Id", "Name", pateint.PatientTypeID);
                     
            return View(pateint);
        }

        // POST: /PatientModule/Pateint/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NIC,NICType,PatientTypeID,MRNo,Name,FName,Gender,Age,BloodGroup,Address,ContactNo,AdmitDate,Description,CreatedDate,ModifiedDate,EmployeeId")] Pateint pateint)
        {
            if (ModelState.IsValid)
            {
                pateint.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                pateint.AdmitDate = Convert.ToDateTime(TempData["admitDate"]);
                pateint.employeeId = SessionHelper.EmployeeID;
                pateint.ModifiedDate = DateTime.Now;
                db.Entry(pateint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.PatientTypeID = new SelectList(db.PatientTypes, "Id", "Name", pateint.PatientTypeID);
          
            return View(pateint);
        }

        [HttpPost]
        public JsonResult DeletePatient(int? Id)
        {
            try
            {
                Pateint patient = db.Pateints.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Pateints.Remove(patient);
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
