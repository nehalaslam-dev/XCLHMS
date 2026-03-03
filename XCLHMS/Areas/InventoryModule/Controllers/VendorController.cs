using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.InventoryModule.Controllers
{
    public class VendorController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/Vendor/
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Vendors.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }



        // GET: /InventoryModule/Vendor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // GET: /InventoryModule/Vendor/Create
        public ActionResult Create()
        {            
            return View();
        }

        // POST: /InventoryModule/Vendor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Code,Address,ContactNo,Email,ContactPerson,Active,CreatedDate,ModifiedDate")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                vendor.CreatedDate = DateTime.Now;
                db.Vendors.Add(vendor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(vendor);
        }

        // GET: /InventoryModule/Vendor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            TempData["crtDate"] = vendor.CreatedDate;
            if (vendor == null)
            {
                return HttpNotFound();
            }            
           return View(vendor);           
        }

        // POST: /InventoryModule/Vendor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Code,Address,ContactNo,Email,ContactPerson,Active,CreatedDate,ModifiedDate")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                vendor.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                vendor.ModifiedDate = DateTime.Now;
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(vendor);
        }     

       

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Vendor vendor = db.Vendors.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Vendors.Remove(vendor);
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
