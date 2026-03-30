using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class TaxController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /Accounts/taxes/
        public ActionResult Index()
        {
            //var taxdetails = db.TaxDetails.Include(t => t.taxes);
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.TaxDetails.Include(t => t.taxes);
            var final = (from d in data
                         select new { 
                            Id=d.Id,
                            taxCategory = d.taxes.TaxName,
                            taxName = d.TaxName,
                            taxVal = d.TaxValues
                         
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Accounts/taxes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxDetail taxdetail = db.TaxDetails.Find(id);
            if (taxdetail == null)
            {
                return HttpNotFound();
            }
            return View(taxdetail);
        }

        // GET: /Accounts/taxes/Create
        public ActionResult Create()
        {
            ViewBag.TaxId = new SelectList(db.taxes, "Id", "TaxName");
            return View();
        }

        // POST: /Accounts/taxes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,TaxId,TaxName,TaxValues,CreatedDate,ModifiedDate")] TaxDetail taxdetail)
        {
            if (ModelState.IsValid)
            {
                taxdetail.CreatedDate = DateTime.Now;
                db.TaxDetails.Add(taxdetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TaxId = new SelectList(db.taxes, "Id", "TaxName", taxdetail.TaxId);
            return View(taxdetail);
        }

        // GET: /Accounts/taxes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxDetail taxdetail = db.TaxDetails.Find(id);
            TempData["crtDate"] = taxdetail.CreatedDate;
            if (taxdetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaxId = new SelectList(db.taxes, "Id", "TaxName", taxdetail.TaxId);
            return View(taxdetail);
        }

        // POST: /Accounts/taxes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,TaxId,TaxName,TaxValues,CreatedDate,ModifiedDate")] TaxDetail taxdetail)
        {
            if (ModelState.IsValid)
            {
                taxdetail.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                taxdetail.ModifiedDate = DateTime.Now;
                db.Entry(taxdetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaxId = new SelectList(db.taxes, "Id", "TaxName", taxdetail.TaxId);
            return View(taxdetail);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                TaxDetail taxes = db.TaxDetails.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.TaxDetails.Remove(taxes);
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

