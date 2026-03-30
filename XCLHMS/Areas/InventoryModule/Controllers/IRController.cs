using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.InventoryModule.Controllers
{
    public class IRController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/IR/
        public ActionResult Index()
        {
            //var irs = db.IRs.Include(i => i.Customers);
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.IRs.Include(p => p.Customers).ToList();
            var final = (from f in data
                         select new
                         {
                             Id = f.Id,
                             customerName = f.Customers.Name,
                             irCode = f.IRCode,
                             irDate = f.IRDate,
                             Remarks = f.Remarks
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IRList(int? id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetIRList_Result> list = db.GetIRList();
            foreach (var item in list)
            {
                var sid = item.id;
                var isEdit = item.isEdit;
                result.Add(new
                {
                    sid = sid,
                    isEdit = isEdit
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetItemsById(int id)
        {
            List<Products> lstproducts = new List<Products>();
            if (id > 0)
            {
                lstproducts = db.Products.Where(p => p.CategoryId == id).ToList();
            }
            else
            {
                lstproducts.Insert(0, new Products { Id = 0, Name = "--Select--" });
            }
            var result = (from d in lstproducts
                          select new
                          {
                              id = d.Id,
                              name = d.Name
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBatchById(int id)
        {
            var result = (from d in db.GRNDetails
                          where d.ProductId == id
                          select new
                          {
                              id = d.batchno,
                              name = d.batchno
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStockQty(int Id, string BatchNo)
        {
            ObjectResult<GetStockQty_Result> list = db.GetStockQty(Id, BatchNo);
            List<dynamic> result = new List<dynamic>();

            foreach (var item in list)
            {

                var mfgdate = item.MfgDate;
                var expiryDate = item.ExpiryDate;
                var totalQty = item.totalQty;

                result.Add(new
                {
                    mfgdate = mfgdate,
                    expiryDate = expiryDate,
                    totalQty = totalQty

                });

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult save(IR irmaster)
        {
            bool status = false;

            try
            {
                if (Convert.ToInt32(TempData["IRID"]) > 0)
                {
                    DeleteIRDetail(Convert.ToInt32(TempData["IRID"]), irmaster.IRCode);
                }

                var irno = db.IRs.OrderByDescending(x => x.Id).Take(1).Select(x => x.IRCode).ToList().SingleOrDefault();
                if (irno != null)
                {
                    string[] irnoSplit = irno.Split('-');
                    int maxirNo = Convert.ToInt32(irnoSplit[1]) + 1;
                    irmaster.IRCode = "IR-" + maxirNo.ToString();
                }
                else
                {
                    irmaster.IRCode = "IR-1";
                }
                var isValidModel = TryValidateModel(irmaster);
                irmaster.Stocks.ToList().ForEach(c => c.RefCode = irmaster.IRCode);

                if (isValidModel)
                {
                    db.IRs.Add(irmaster);
                    db.SaveChanges();
                    status = true;
                }
                return new JsonResult { Data = new { status = status } };

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public ActionResult GetIRDetail(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetIRDetail_Result> list = db.GetIRDetail(Convert.ToInt32(id));

            foreach (var item in list)
            {
                var customerId = item.customerId;
                var customerName = item.customerName;
                var irCode = item.IRCode;
                var irdate = item.IRDate;
                var irRemarks = item.Remarks;
                var categoryId = item.categoryId;
                var categoryname = item.categoryname;
                var productId = item.ProductId;
                var productName = item.productname;
                var totalQty = item.stockqty;
                var reqQty = item.ReqQty;
                var delQty = item.DelvQty;
                var batchno = item.batchno;
                var mfgDate = item.MfgDate;
                var expiryDate = item.ExpiryDate;
                var suppliment = item.Suppliment;
                var createdDate = item.CreatedDate;

                result.Add(new
                {
                    customerId = customerId,
                    customerName = customerName,
                    irCode = irCode,
                    irdate = irdate,
                    irRemarks = irRemarks,
                    categoryId = categoryId,
                    categoryname = categoryname,
                    productId = productId,
                    productName = productName,
                    totalQty = totalQty,
                    reqQty = reqQty,
                    delQty = delQty,
                    batchno = batchno,
                    mfgDate = mfgDate,
                    expiryDate = expiryDate,
                    suppliment = suppliment,
                    createdDate = createdDate
                });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /InventoryModule/IR/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<IRDetails> ir = db.IRDetails.Where(x => x.IRID == id).ToList();
            if (ir == null)
            {
                return HttpNotFound();
            }
            return View(ir);
        }

        // GET: /InventoryModule/IR/Create
        public ActionResult Create(int? id)
        {
            TempData["IRID"] = id;

            List<ProductCategory> lstcategory = db.ProductCategories.ToList();
            lstcategory.Insert(0, new ProductCategory { Id = 0, Name = "--Select--" });

            List<Products> lstproducts = new List<Products>();

            List<GRNDetail> lstGrndtl = new List<GRNDetail>();

            ViewBag.ItemCategory = new SelectList(lstcategory, "Id", "Name");
            ViewBag.productCategory = new SelectList(lstproducts, "Id", "Name");
            ViewBag.batchno = new SelectList(lstGrndtl, "batchno", "batchno");
            ViewBag.customerId = new SelectList(db.Customers, "Id", "Name");
            return View();
        }

        public bool DeleteIRDetail(int pid, string refCode)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                SqlParameter param1 = new SqlParameter("@refCode", refCode);
                var data = db.Database.ExecuteSqlCommand("DeleteIRDetail @pId,@refCode", param, param1);
                if (data != null)
                {
                    return true;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        // POST: /InventoryModule/IR/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,customerId,IRCode,IRDate,Remarks")] IR ir)
        {
            if (ModelState.IsValid)
            {
                db.IRs.Add(ir);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.customerId = new SelectList(db.Customers, "Id", "Name", ir.customerId);
            return View(ir);
        }

        // GET: /InventoryModule/IR/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IR ir = db.IRs.Find(id);
            if (ir == null)
            {
                return HttpNotFound();
            }
            ViewBag.customerId = new SelectList(db.Customers, "Id", "Name", ir.customerId);
            return View(ir);
        }

        // POST: /InventoryModule/IR/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,customerId,IRCode,IRDate,Remarks")] IR ir)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.customerId = new SelectList(db.Customers, "Id", "Name", ir.customerId);
            return View(ir);
        }

        [HttpPost]
        public JsonResult Delete(int? Id, string RefCode)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", Id);
                SqlParameter param1 = new SqlParameter("@refCode", RefCode);
                var data = db.Database.ExecuteSqlCommand("DeleteIRDetail @pId,@refCode", param, param1);

                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
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

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/IRReport.aspx?Id=" + Id);
            return View();
        }
    }
}




