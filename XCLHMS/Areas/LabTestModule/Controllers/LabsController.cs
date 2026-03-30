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
    public class LabsController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /PatientModule/Labs/
        public ActionResult Index()
        {            
            //return View(db.Labs.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Labs.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /PatientModule/Labs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labs Labs = db.Labs.Find(id);
            if (Labs == null)
            {
                return HttpNotFound();
            }
            return View(Labs);
        }

        // GET: /PatientModule/Labs/Create
        public ActionResult Create()
        {            
            return View();
        }

        // POST: /PatientModule/Labs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description,IsActive,CreatedDate,ModifiedDate")] Labs Labs)
        {
            if (ModelState.IsValid)
            {
                db.Labs.Add(Labs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

         
            return View(Labs);
        }

        // GET: /PatientModule/Labs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labs Labs = db.Labs.Find(id);
            if (Labs == null)
            {
                return HttpNotFound();
            }
           
            return View(Labs);
        }

        // POST: /PatientModule/Labs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description,IsActive,CreatedDate,ModifiedDate")] Labs Labs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Labs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
         
            return View(Labs);
        }

        //// GET: /PatientModule/Labs/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int? id)
        //{
        //    Labs labs = db.Labs.Find(id);
        //    db.Labs.Remove(labs);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Labs labs = db.Labs.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Labs.Remove(labs);
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

