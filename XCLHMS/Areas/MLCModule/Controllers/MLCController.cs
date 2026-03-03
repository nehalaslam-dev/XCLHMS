using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.MLCModule.Controllers
{
    public class MLCController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /MLCModule/MLC/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var final = (from a in db.MLCs
                         select new
                         {
                             Id = a.Id,
                             mlcNo = a.MlcNo,
                             patientName = a.PatientName,
                             Dated = a.Dated,
                             mlcType = a.MLCType,
                             Age = a.Age,
                             gender = a.Gender,
                             letter = a.LetterNo
                         }).ToList();
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult save(MLC mlcmaster)
        {
            try
            {
                bool status = false;
                if (Convert.ToInt32(TempData["MLCId"]) > 0)
                {
                    DeleteMLCDetail(Convert.ToInt32(TempData["MLCId"]));
                }

                var isValidModel = TryValidateModel(mlcmaster);
                if (isValidModel)
                {
                    db.MLCs.Add(mlcmaster);
                    db.SaveChanges();
                    status = true;
                }
                return new JsonResult { Data = new { status = status } };
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public ActionResult GetMLCDetail(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetMLCDetail_Result> list = db.GetMLCDetail(Convert.ToInt32(id));

            foreach (var item in list)
            {
                var mlcNo = item.MlcNo;
                var patientName = item.PatientName;
                var title = item.Title;
                var guardianName = item.GuardianName;
                var mlctype = item.MLCType;
                var age = item.Age;
                var gender = item.Gender;
                var ps = item.PS;
                var address = item.Address;
                var identifyMarks = item.IdentifyMarks;
                var letterNo = item.LetterNo;
                var dated = item.Dated;
                var incidentDt = item.IncidentDateTime;
                var arrivalDt = item.ArrivalDateTime;
                var place = item.Place;
                var blonging = item.Blonging;
                var history = item.History;
                var injury = item.Injuries;
                var xray = item.Xray;
                var mlcId = item.MLCId;
                var injuryNo = item.InjuryNo;
                var injuryNature = item.InjuryNature;
                var weapon = item.Weapon;
                var duration = item.Duration;

                result.Add(new
                {
                    mlcNo = mlcNo,
                    patientName = patientName,
                    title = title,
                    guardianName = guardianName,
                    mlctype = mlctype,
                    age = age,
                    gender = gender,
                    ps = ps,
                    address = address,
                    identifyMarks = identifyMarks,
                    letterNo = letterNo,
                    dated = dated,
                    incidentDt = incidentDt,
                    arrivalDt = arrivalDt,
                    place = place,
                    blonging = blonging,
                    history = history,
                    injury = injury,
                    xray = xray,
                    mlcId = mlcId,
                    injuryNo = injuryNo,
                    injuryNature = injuryNature,
                    weapon = weapon,
                    duration = duration
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteMLCDetail(int pid)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                var data = db.Database.ExecuteSqlCommand("DeleteMLCDetail @pId", param);
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

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", Id);
                var data = db.Database.ExecuteSqlCommand("DeleteMLCDetail @pId", param);
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


        // GET: /MLCModule/MLC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MLC mlc = db.MLCs.Find(id);
            if (mlc == null)
            {
                return HttpNotFound();
            }
            return View(mlc);
        }

        // GET: /MLCModule/MLC/Create
        public ActionResult Create(int? id)
        {
            TempData["MLCId"] = id;
            return View();
        }

        // POST: /MLCModule/MLC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MlcNo,PatientName,Title,GuardianName,MLCType,Age,Gender,PS,Address,IdentifyMarks,LetterNo,Dated,IncidentDateTime,ArrivalDateTime,Place,Blonging,History,Injuries,Xray")] MLC mlc)
        {
            if (ModelState.IsValid)
            {
                db.MLCs.Add(mlc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mlc);
        }

        // GET: /MLCModule/MLC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MLC mlc = db.MLCs.Find(id);
            if (mlc == null)
            {
                return HttpNotFound();
            }
            return View(mlc);
        }

        // POST: /MLCModule/MLC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MlcNo,PatientName,Title,GuardianName,MLCType,Age,Gender,PS,Address,IdentifyMarks,LetterNo,Dated,IncidentDateTime,ArrivalDateTime,Place,Blonging,History,Injuries,Xray")] MLC mlc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mlc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mlc);
        }

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/MLC.aspx?Id=" + Id);
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
