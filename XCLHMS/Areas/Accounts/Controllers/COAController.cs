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
    public class COAController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /Accounts/COA/
        public ActionResult Index()
        {
            //var coas = db.COAs;
            //return View(coas.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.COAs.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Accounts/COA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COA coa = db.COAs.Find(id);
            if (coa == null)
            {
                return HttpNotFound();
            }
            return View(coa);
        }

        // GET: /Accounts/COA/Create
        public ActionResult Create()
        {
            ViewBag.ParentID = new SelectList(db.COAs, "ID", "Name");
            return View();
        }

        // POST: /Accounts/COA/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ParentID,Code,Name,Description,CreatedDate,ModifiedDate")] COA coa)
        {
            if (ModelState.IsValid)
            {
                coa.CreatedDate = DateTime.Now;
                db.COAs.Add(coa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentID = new SelectList(db.COAs, "ID", "Name", coa.ID);
            return View(coa);
        }

        // GET: /Accounts/COA/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COA coa = db.COAs.Find(id);
            TempData["crtDate"] = coa.CreatedDate;
            if (coa == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentID = new SelectList(db.COAs, "ID", "Name", coa.ID);

            return View(coa);
        }

        // POST: /Accounts/COA/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,ParentID,Name,Description,CreatedDate,ModifiedDate")] COA coa)
        {
            if (ModelState.IsValid)
            {
                coa.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                coa.ModifiedDate = DateTime.Now;
                db.Entry(coa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentID = new SelectList(db.COAs, "ID", "Name", coa.ID);

            return View(coa);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                COA coa = db.COAs.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.COAs.Remove(coa);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult myTreeView()
        {
            List<COA> all = new List<COA>();
            all = db.COAs.OrderBy(x => x.ParentID).ToList();
            return View(all);
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
