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
    public class TestCategoryController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /LabTestModule/TestCategory/
        public ActionResult Index()
        {
            //return View(db.TestCategories.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.TestCategories.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LabTestModule/TestCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCategory testcategory = db.TestCategories.Find(id);
            if (testcategory == null)
            {
                return HttpNotFound();
            }
            return View(testcategory);
        }

        // GET: /LabTestModule/TestCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /LabTestModule/TestCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description,IsActive,CreatedDate,ModifiedDate")] TestCategory testcategory)
        {
            if (ModelState.IsValid)
            {
                db.TestCategories.Add(testcategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testcategory);
        }

        // GET: /LabTestModule/TestCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCategory testcategory = db.TestCategories.Find(id);
            if (testcategory == null)
            {
                return HttpNotFound();
            }
            return View(testcategory);
        }

        // POST: /LabTestModule/TestCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description,IsActive,CreatedDate,ModifiedDate")] TestCategory testcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testcategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testcategory);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                TestCategory testCat = db.TestCategories.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.TestCategories.Remove(testCat);
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
