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
    public class TestController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /LabTestModule/Test/
        public ActionResult Index()
        {
            //var tests = db.Tests.Include(t => t.TestCategory);
            //return View(tests.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Tests.Include(p => p.TestCategory).ToList();
            var final = (from d in data
                         select new
                         {
                             Id = d.Id,
                             category = d.TestCategory.Name,
                             Name = d.Name,
                             Normalrange = d.NormalRange,
                             Unit = d.Unit,
                             Description = d.Description
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /LabTestModule/Test/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // GET: /LabTestModule/Test/Create
        public ActionResult Create()
        {
            ViewBag.TestCatId = new SelectList(db.TestCategories, "Id", "Name");

            return View();
        }

        // POST: /LabTestModule/Test/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TestCatId,Name,NormalRange,Unit,Description")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Tests.Add(test);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TestCatId = new SelectList(db.TestCategories, "Id", "Name", test.TestCatId);
            return View(test);
        }

        // GET: /LabTestModule/Test/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.TestCatId = new SelectList(db.TestCategories, "Id", "Name", test.TestCatId);
            return View(test);
        }

        // POST: /LabTestModule/Test/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TestCatId,Name,NormalRange,Unit,Description")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TestCatId = new SelectList(db.TestCategories, "Id", "Name", test.TestCatId);
            return View(test);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Test tests = db.Tests.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Tests.Remove(tests);
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
