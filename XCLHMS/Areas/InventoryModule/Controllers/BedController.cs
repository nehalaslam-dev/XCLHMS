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
    public class BedController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/Beds/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Beds.Include(w => w.Wards).ToList();
            var final = (from d in data
                         select new
                         {
                             ID=d.ID,
                             WardName = d.Wards.WardName,
                             BedNumber = d.BedNumber,
                             IsOccupied = d.IsOccupied,
                             description = d.Description
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /InventoryModule/Beds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beds Beds = db.Beds.Find(id);
            if (Beds == null)
            {
                return HttpNotFound();
            }
            return View(Beds);
        }

        // GET: /InventoryModule/Beds/Create
        public ActionResult Create()
        {
            ViewBag.WardID = new SelectList(db.Wards, "Id", "WardName");
            return View();
        }

        // POST: /InventoryModule/Beds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,WardId,BedNumber,Description,IsOccupied,CreatedDate,ModifiedDate")] Beds Beds)
        {
            if (ModelState.IsValid)
            {
                Beds.CreatedDate = DateTime.Now;
                db.Beds.Add(Beds);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.WardID = new SelectList(db.Wards, "Id", "WardName",Beds.WardId);

            return View(Beds);
        }

        // GET: /InventoryModule/Beds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beds Beds = db.Beds.Find(id);
            TempData["crtDate"] = Beds.CreatedDate;
            if (Beds == null)
            {
                return HttpNotFound();
            }

            ViewBag.WardID = new SelectList(db.Wards, "Id", "WardName", Beds.WardId);
       
            return View(Beds);
        }

        // POST: /InventoryModule/Beds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,WardId,BedNumber,Description,IsOccupied,CreatedDate,ModifiedDate")] Beds Beds)
        {
            if (ModelState.IsValid)
            {
                Beds.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                Beds.ModifiedDate = DateTime.Now;
                db.Entry(Beds).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.WardID = new SelectList(db.Wards, "Id", "WardName", Beds.WardId);
          
            return View(Beds);
        }

     

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Beds beds = db.Beds.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Beds.Remove(beds);
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

