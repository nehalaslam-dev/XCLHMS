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
    public class AUController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: StockInventory/AU
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {

            db.Configuration.ProxyCreationEnabled = false;
            var data = db.AUs.OrderByDescending(a => a.Id).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: StockInventory/AU/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AU aU = db.AUs.Find(id);
            if (aU == null)
            {
                return HttpNotFound();
            }
            return View(aU);
        }

        // GET: StockInventory/AU/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockInventory/AU/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,CreatedDate")] AU aU)
        {
            if (ModelState.IsValid)
            {
                if (aU.CreatedDate == default(DateTime))
                    aU.CreatedDate = DateTime.Now;
                db.AUs.Add(aU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aU);
        }

        // GET: StockInventory/AU/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AU aU = db.AUs.Find(id);
            if (aU == null)
            {
                return HttpNotFound();
            }
            return View(aU);
        }

        // POST: StockInventory/AU/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,CreatedDate")] AU aU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aU);
        }

        // POST: StockInventory/AU/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                AU aU = db.AUs.Find(id);
                if (id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.AUs.Remove(aU);
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
