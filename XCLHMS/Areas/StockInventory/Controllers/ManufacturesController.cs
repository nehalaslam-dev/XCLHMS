using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.StockInventory.Controllers
{
    public class ManufacturesController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: StockInventory/Manufactures
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {

            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Manufactures.OrderByDescending(a => a.Id).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: StockInventory/Manufactures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufacture manufacture = db.Manufactures.Find(id);
            if (manufacture == null)
            {
                return HttpNotFound();
            }
            return View(manufacture);
        }

        // GET: StockInventory/Manufactures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockInventory/Manufactures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ManufactureName,Description")] Manufacture manufacture)
        {
            if (ModelState.IsValid)
            {
                db.Manufactures.Add(manufacture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(manufacture);
        }

        // GET: StockInventory/Manufactures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufacture manufacture = db.Manufactures.Find(id);
            if (manufacture == null)
            {
                return HttpNotFound();
            }
            return View(manufacture);
        }

        // POST: StockInventory/Manufactures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ManufactureName,Description")] Manufacture manufacture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manufacture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(manufacture);
        }

        // GET: StockInventory/Manufactures/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Manufacture manufacture = db.Manufactures.Find(id);
        //    if (manufacture == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(manufacture);
        //}

        // POST: StockInventory/Manufactures/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Manufacture manufacture = db.Manufactures.Find(id);
                if (id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Manufactures.Remove(manufacture);
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
