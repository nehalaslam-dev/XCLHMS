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
    public class ProductsController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/Products/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDosageByAU(int auId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var dosages = db.Dosages
                .Where(d => d.AUId == auId)   // Assuming Dosage table has AUId foreign key
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.DosageName
                })
                .ToList();

            return Json(dosages, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Products.Include(p => p.Brand).Include(p => p.Dosage).Include(p => p.AU).Include(p => p.Manufacture).OrderByDescending(p => p.Id).ToList();
            var final = (from f in data
                         select new
                         {
                             Id = f.Id,
                             //Category = f.ProductCategory.Name,
                             //vendor = f.Vendor.Name,
                             Brand = f.Brand.BrandName,
                             Manufacture = f.Manufacture.ManufactureName,
                             Dosage = f.Dosage.DosageName,
                             AU = f.AU.Name,
                             Name = f.Name,
                             //Code = f.Code,
                             //Qty = f.Qty,
                             //strength = f.strength,
                             //genericName = f.genericName,
                             //Active = f.Active,
                             //Description = f.Description
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }


        // GET: /InventoryModule/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: /InventoryModule/Products/Create
        public ActionResult Create()
        {

            //ViewBag.CategoryId = new SelectList(db.ProductCategories, "Id", "Name");
            //ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "BrandName");
            ViewBag.ManufactureId = new SelectList(db.Manufactures, "Id", "ManufactureName");
            ViewBag.DosageId = new SelectList(Enumerable.Empty<SelectListItem>(), "Id", "DosageName");
            ViewBag.AUId = new SelectList(db.AUs, "Id", "Name");

            return View();
        }

        // POST: /InventoryModule/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BrandId,ManufactureId,DosageId,AUId,Name,CreatedDate,ModifiedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            //ViewBag.CategoryId = new SelectList(db.ProductCategories, "Id", "Name", product.CategoryId);
            //ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", product.VendorId);
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "BrandName", product.BrandId);
            ViewBag.ManufactureId = new SelectList(db.Manufactures, "Id", "ManufactureName", product.ManufactureId);
            ViewBag.DosageId = new SelectList(db.Dosages, "Id", "DosageName", product.DosageId);
            ViewBag.AUId = new SelectList(db.AUs, "Id", "Name", product.AUId);
            return View(product);
        }

        // GET: /InventoryModule/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            TempData["crtDate"] = product.CreatedDate;
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(db.ProductCategories, "Id", "Name", product.CategoryId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", product.VendorId);
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "BrandName", product.BrandId);
            ViewBag.ManufactureId = new SelectList(db.Manufactures, "Id", "ManufactureName", product.ManufactureId);

            var selectedAUId = product.AUId;
            var filteredDosages = db.Dosages.Where(d => d.AUId == selectedAUId).ToList();
            ViewBag.DosageId = new SelectList(filteredDosages, "Id", "DosageName", product.DosageId);

            ViewBag.AUId = new SelectList(db.AUs, "Id", "Name", product.AUId);
            return View(product);
        }

        // POST: /InventoryModule/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,VendorId,BrandId,ManufactureId,DosageId,AUId,Name,Code,Qty,strength,genericName,Description,Active,CreatedDate,ModifiedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                product.ModifiedDate = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.ProductCategories, "Id", "Name", product.CategoryId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", product.VendorId);
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "BrandName", product.BrandId);
            ViewBag.ManufactureId = new SelectList(db.Manufactures, "Id", "ManufactureName", product.ManufactureId);
            ViewBag.DosageId = new SelectList(db.Dosages, "Id", "DosageName", product.DosageId);
            ViewBag.AUId = new SelectList(db.AUs, "Id", "Name", product.AUId);
            return View(product);
        }



        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Product product = db.Products.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Products.Remove(product);
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
