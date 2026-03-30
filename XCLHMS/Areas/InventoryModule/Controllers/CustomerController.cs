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
    public class CustomerController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/Customers/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Customers.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /InventoryModule/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers Customers = db.Customers.Find(id);
            if (Customers == null)
            {
                return HttpNotFound();
            }
            return View(Customers);
        }

        // GET: /InventoryModule/Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /InventoryModule/Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description")] Customers Customers)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(Customers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Customers);
        }

        // GET: /InventoryModule/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers Customers = db.Customers.Find(id);
            if (Customers == null)
            {
                return HttpNotFound();
            }
            return View(Customers);
        }

        // POST: /InventoryModule/Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description")] Customers Customers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Customers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Customers);
        }

        // GET: /InventoryModule/Customers/Delete/5

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Customers customers = db.Customers.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Customers.Remove(customers);
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

