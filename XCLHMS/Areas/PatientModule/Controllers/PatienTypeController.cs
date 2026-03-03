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
    public class PatienTypeController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /PatientModule/PatienType/
        public ActionResult Index()
        {
            //return View(db.PatientTypes.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.PatientTypes.OrderBy(a => a.Name).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /PatientModule/PatienType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientType patienttype = db.PatientTypes.Find(id);
            if (patienttype == null)
            {
                return HttpNotFound();
            }
            return View(patienttype);
        }

        // GET: /PatientModule/PatienType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /PatientModule/PatienType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description")] PatientType patienttype)
        {
            if (ModelState.IsValid)
            {
                db.PatientTypes.Add(patienttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patienttype);
        }

        // GET: /PatientModule/PatienType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientType patienttype = db.PatientTypes.Find(id);
            if (patienttype == null)
            {
                return HttpNotFound();
            }
            return View(patienttype);
        }

        // POST: /PatientModule/PatienType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description")] PatientType patienttype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patienttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patienttype);
        }

        //// GET: /PatientModule/PatienType/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int? id)
        //{
           
        //    PatientType patienttype = db.PatientTypes.Find(id);
        //    db.PatientTypes.Remove(patienttype);
        //    db.SaveChanges();
        //    return View(patienttype);
        //}

        [HttpPost]
        public JsonResult DeleteType(int? Id)
        {
            try
            {
                PatientType patienttype = db.PatientTypes.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.PatientTypes.Remove(patienttype);
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
