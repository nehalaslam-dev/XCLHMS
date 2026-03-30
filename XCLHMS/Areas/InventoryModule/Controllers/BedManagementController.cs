using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.InventoryModule.Controllers
{
    public class BedManagementController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/BedManagement/
        public ActionResult Index()
        {
            //var bedmanagements = db.BedManagements.Include(b => b.Beds);
            //return View(bedmanagements.ToList());
            return View();
        }

        [HttpGet]
        public JsonResult GetBedById(int id)
        {
            List<Beds> lstBeds = new List<Beds>();
            if (id > 0)
            {
                lstBeds = db.Beds.Where(p => p.WardId == id).ToList();

            }
            else
            {
                lstBeds.Insert(0, new Beds { ID = 0, BedNumber = "--Select--" });
            }
            var result = (from d in lstBeds
                          select new
                          {
                              id = d.ID,
                              name = d.BedNumber
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPateintName(string myvalue)
        {
            var data = (from p in db.Pateints
                        where p.MRNo == myvalue && p.PatientTypeID == 1
                        select new
                        {
                            id = p.Id,
                            name = p.Name,
                            fname = p.FName,
                            address = p.Address,
                            contact = p.ContactNo,
                            age=p.Age
                        }).SingleOrDefault();
            if (data == null)
            {
                return Json(new { data = "-1" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadGrid()
        {
            ObjectResult<GetBedAllocationDetail_Result> list = db.GetBedAllocationDetail();
            var jsonresult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue;
            return jsonresult;
        }

        // GET: /InventoryModule/BedManagement/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BedManagement bedmanagement = db.BedManagements.Find(id);
            if (bedmanagement == null)
            {
                return HttpNotFound();
            }
            return View(bedmanagement);
        }

        // GET: /InventoryModule/BedManagement/Create
        public ActionResult Create()
        {
            List<Wards> lstWard = db.Wards.ToList();
            lstWard.Insert(0, new Wards { Id = 0, WardName = "--Select--" });
            List<Beds> lstBeds = new List<Beds>();
            List<Pateint> lstPatient = new List<Pateint>();
            lstPatient.Insert(0, new Pateint { Id = 0, Name = "--Select--" });

            ViewBag.WardId = new SelectList(lstWard, "Id", "WardName");
            ViewBag.BedID = new SelectList(lstBeds, "ID", "BedNumber");
            ViewBag.PatientID = new SelectList(lstPatient, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult save(int bedId, int patientId)
        {
            string result = "Error! Record Is Not Complete!";
            try
            {
                BedManagement bedmgt = new BedManagement();
                bedmgt.BedID = bedId;
                bedmgt.PatientID = patientId;             
                bedmgt.AllotmentDate = DateTime.Now;
                bedmgt.CreatedDate = DateTime.Now;
                db.BedManagements.Add(bedmgt);

                db.SaveChanges();
                result = "Success! Record saved!";

            }
            catch (Exception ex)
            {

                result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: /InventoryModule/BedManagement/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BedManagement bedmanagement = db.BedManagements.Find(id);
            TempData["AllotmentDate"] = bedmanagement.AllotmentDate;
            TempData["crtDate"] = bedmanagement.CreatedDate;

            var lstPatient = db.Pateints.Where(p => p.Id == bedmanagement.PatientID);
            if (bedmanagement == null)
            {
                return HttpNotFound();
            }
            ViewBag.BedID = new SelectList(db.Beds, "ID", "BedNumber", bedmanagement.BedID);
            ViewBag.PatientID = new SelectList(lstPatient, "Id", "Name");
            return View(bedmanagement);
        }

        // POST: /InventoryModule/BedManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BedID,PatientID,AllotmentDate,dischargeType,DischargeDate,CreatedDate,ModifiedDate")] BedManagement bedmanagement)
        {
            if (ModelState.IsValid)
            {
                bedmanagement.AllotmentDate = Convert.ToDateTime(TempData["AllotmentDate"]);
                bedmanagement.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                bedmanagement.ModifiedDate = DateTime.Now;
                db.Entry(bedmanagement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BedID = new SelectList(db.Beds, "ID", "BedNumber", bedmanagement.BedID);
            ViewBag.PatientID = new SelectList(db.Pateints, "Id", "Name", bedmanagement.PatientID);
            return View(bedmanagement);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                BedManagement bedmgt = db.BedManagements.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.BedManagements.Remove(bedmgt);
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

