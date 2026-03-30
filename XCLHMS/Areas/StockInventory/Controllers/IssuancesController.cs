using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models; // Ensure this matches your Models namespace

namespace XCLHMS.Areas.StockInventory.Controllers
{
    public class IssuancesController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: StockInventory/Issuances
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;

            // Check: If 'Issuances' shows a red line, remove the 's'
            var data = (from d in db.Issuances
                        join a in db.Products on d.ProductId equals a.Id into productGroup
                        from a in productGroup.DefaultIfEmpty()
                        orderby d.SNO descending
                        select new
                        {
                            d.SNO,
                            d.Head,
                            d.Qty,
                            d.Date,
                            ProductName = a != null ? a.Name : ""
                        }).ToList();

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Check: If 'Issuance' shows a red line, add an 's'
            Issuance issuance = db.Issuances.Find(id);
            if (issuance == null) return HttpNotFound();

            return View(issuance);
        }

        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products.OrderBy(a => a.Name).ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SNO,Head,ProductId,Qty,Date")] Issuance issuance)
        {
            if (ModelState.IsValid)
            {
                db.Issuances.Add(issuance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products.OrderBy(a => a.Name).ToList(), "Id", "Name", issuance.ProductId);
            return View(issuance);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Issuance issuance = db.Issuances.Find(id);
            if (issuance == null) return HttpNotFound();

            ViewBag.ProductId = new SelectList(db.Products.OrderBy(a => a.Name).ToList(), "Id", "Name", issuance.ProductId);
            return View(issuance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SNO,Head,ProductId,Qty,Date")] Issuance issuance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(issuance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products.OrderBy(a => a.Name).ToList(), "Id", "Name", issuance.ProductId);
            return View(issuance);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Issuance issuance = db.Issuances.Find(id);
                if (issuance == null) return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);

                db.Issuances.Remove(issuance);
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
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}