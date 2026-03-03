using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;

namespace XCLHMS.Areas.InventoryModule.Controllers
{
    public class PharmacyController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/Pharmacy/
        public ActionResult Index()
        {
            //return View(db.Pharmacies.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = (from d in db.Pharmacies
                        from c in db.Products.Where(o => d.ProductId == o.Id).DefaultIfEmpty()
                        from b in db.Pateints.Where(o => d.PatientId == o.Id).DefaultIfEmpty()

                        select new
                        {
                            Id = d.Id,
                            productName = c.Name,
                            patientname = b.Name,
                            recDate = d.recievedDate,
                            issueDate = d.IssuedDate,
                            qtyRec = d.QtyRec,
                            qtyIssue = d.QtyIssue,
                            comments = d.comments
                        }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMedicineByToken(string token)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetMedicineBytoken_Result> list = db.GetMedicineBytoken(token);
            foreach (var item in list)
            {
                var medicine = item.Medicines;
                var qty = item.Qty;
                var comments = item.comments;

                result.Add(new
                {
                    medicine = medicine,
                    qty = qty,
                    comments = comments

                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemNameById(int Id)
        {
            var data = (from d in db.Products
                        where d.Id == Id
                        select d.Name).ToList().SingleOrDefault();
            if (data == null)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPateintByToken(string tokenno)
        {
            var data = (from d in db.Tokens
                        join p in db.Pateints on d.PatientId equals p.Id
                        where d.TokenNumber == tokenno
                        select new
                        {
                            id = p.Id,
                            name = p.Name
                        }).SingleOrDefault();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }



        // GET: /InventoryModule/Pharmacy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pharmacy pharmacy = db.Pharmacies.Find(id);
            if (pharmacy == null)
            {
                return HttpNotFound();
            }
            return View(pharmacy);
        }

        // GET: /InventoryModule/Pharmacy/Create
        public ActionResult Create(int? Id)
        {

            return View();
        }

        // POST: /InventoryModule/Pharmacy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,PatientId,recievedDate,IssuedDate,QtyRec,QtyIssue,comments,CreatedDate,ModifiedDate")] Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                db.Pharmacies.Add(pharmacy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pharmacy);
        }

        // GET: /InventoryModule/Pharmacy/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pharmacy pharmacy = db.Pharmacies.Find(Id);
            if (pharmacy == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientId = new SelectList(db.Pateints, "Id", "Name");

            return View(pharmacy);
        }

        // POST: /InventoryModule/Pharmacy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pharmacy pharmacy)
        {
            try
            {

                SqlParameter p1 = new SqlParameter("@patientId", pharmacy.PatientId);
                SqlParameter p2 = new SqlParameter("@issuedDate", pharmacy.IssuedDate);
                SqlParameter p3 = new SqlParameter("@qtyIssue", pharmacy.QtyIssue);
                SqlParameter p4 = new SqlParameter("@Id", pharmacy.Id);

                db.Database.ExecuteSqlCommand("UpdatePharmacy @patientId,@issuedDate,@qtyIssue,@Id", p1, p2, p3, p4);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                return View();
            }
        }

        // GET: /InventoryModule/Pharmacy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pharmacy pharmacy = db.Pharmacies.Find(id);
            if (pharmacy == null)
            {
                return HttpNotFound();
            }
            return View(pharmacy);
        }

        // POST: /InventoryModule/Pharmacy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pharmacy pharmacy = db.Pharmacies.Find(id);
            db.Pharmacies.Remove(pharmacy);
            db.SaveChanges();
            return RedirectToAction("Index");
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
