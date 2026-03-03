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
    public class DosagesController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: StockInventory/Dosages
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = (from d in db.Dosages
                        join a in db.AUs on d.AUId equals a.Id into auGroup
                        from a in auGroup.DefaultIfEmpty()
                        orderby d.Id descending
                        select new
                        {
                            d.Id,
                            d.DosageName,
                            d.Description,
                            d.Active,
                            AUName = a != null ? a.Name : ""
                        }).ToList();

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: StockInventory/Dosages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dosage dosage = db.Dosages.Find(id);
            if (dosage == null)
            {
                return HttpNotFound();
            }
            return View(dosage);
        }

        // GET: StockInventory/Dosages/Create
        // GET: StockInventory/Dosages/Create
        public ActionResult Create()
        {
            ViewBag.AUId = new SelectList(db.AUs.OrderBy(a => a.Name).ToList(), "Id", "Name");
            return View();
        }

        // POST: StockInventory/Dosages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DosageName,Description,Active,AUId")] Dosage dosage)
        {
            if (ModelState.IsValid)
            {
                if (dosage.Active == false)
                    dosage.Active = true;
                db.Dosages.Add(dosage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AUId = new SelectList(db.AUs.OrderBy(a => a.Name).ToList(), "Id", "Name", dosage.AUId);
            return View(dosage);
        }

        // GET: StockInventory/Dosages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dosage dosage = db.Dosages.Find(id);
            if (dosage == null)
            {
                return HttpNotFound();
            }
            ViewBag.AUId = new SelectList(db.AUs.OrderBy(a => a.Name).ToList(), "Id", "Name", dosage.AUId);
            return View(dosage);
        }

        // POST: StockInventory/Dosages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DosageName,Description,Active,AUId")] Dosage dosage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dosage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AUId = new SelectList(db.AUs.OrderBy(a => a.Name).ToList(), "Id", "Name", dosage.AUId);
            return View(dosage);
        }

        // GET: StockInventory/Dosages/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Dosage dosage = db.Dosages.Find(id);
        //    if (dosage == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dosage);
        //}

        // POST: StockInventory/Dosages/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                Dosage dosage = db.Dosages.Find(id);
                if (id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Dosages.Remove(dosage);
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
