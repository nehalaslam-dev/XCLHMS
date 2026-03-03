using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.MLCModule.Controllers
{
    public class MartamController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /MLCModule/Martam/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.PostMartams.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /MLCModule/Martam/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostMartam postmartam = db.PostMartams.Find(id);
            if (postmartam == null)
            {
                return HttpNotFound();
            }
            return View(postmartam);
        }

        // GET: /MLCModule/Martam/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /MLCModule/Martam/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,martamNo,martamDate,Name,FName,Age,Address,letterNo,policeStation,identifiedBy,ArrivalDateTime,martamStart,martamEnd,furnishByPolice,BroughtBy,finalExam1,finalExam2,finalExam3,Remarks,injuryTime,deathTime")] PostMartam postmartam)
        {
            if (ModelState.IsValid)
            {
                db.PostMartams.Add(postmartam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(postmartam);
        }

        // GET: /MLCModule/Martam/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostMartam postmartam = db.PostMartams.Find(id);
            if (postmartam == null)
            {
                return HttpNotFound();
            }
            return View(postmartam);
        }

        // POST: /MLCModule/Martam/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,martamNo,martamDate,Name,FName,Age,Address,letterNo,policeStation,identifiedBy,ArrivalDateTime,martamStart,martamEnd,furnishByPolice,BroughtBy,finalExam1,finalExam2,finalExam3,Remarks,injuryTime,deathTime")] PostMartam postmartam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postmartam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postmartam);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                PostMartam mortem = db.PostMartams.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.PostMartams.Remove(mortem);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/PostMortemReport.aspx?Id=" + Id);
            return View();

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
