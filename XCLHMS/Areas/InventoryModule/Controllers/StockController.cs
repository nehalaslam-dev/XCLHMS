using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.InventoryModule.Controllers
{
    public class StockController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/Stock/
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpGet]
        public JsonResult IsItemExpired(int? id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<IsItemExpired_Result> list = db.IsItemExpired();
            foreach (var item in list)
            {
                var sid = item.Id;
                var isExpired = item.isexpired;
                result.Add(new
                {
                    sid = sid,
                    isExpired = isExpired
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            ObjectResult<GetStockList_Result> list = db.GetStockList();
            //var data = db.Stocks.Include(p => p.Product).ToList();
            //var final = (from d in data
            //             select new
            //             {
            //                 Id = d.Id,
            //                 productName = d.Product.Name,
            //                 stockDate = d.StockDate,
            //                 batchNo = d.batchno,
            //                 mfgDate = d.MfgDate,
            //                 expiryDate = d.ExpiryDate,
            //                 QtyIn = d.QtyIn,
            //                 QtyOut = d.QtyOut,
            //                 issuedTo = d.IssuedTo,
            //                 transType = d.tranType

            //             });
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetStockSummary()
        //{
        //    //ObjectResult<GetStockSumamry_Result> list = db.GetStockSumamry();
        //    //return View(list.ToList());
        //}

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", stock.ProductId);
            TempData["ProductId"] = stock.ProductId;
            stock.StockDate = null;
            return View(stock);
        }

        // POST: /InventoryModule/Stock/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stock stock)
        {
            try
            {
                SqlParameter p1 = new SqlParameter("@ProductId", Convert.ToInt32(TempData["ProductId"]));
                SqlParameter p2 = new SqlParameter("@StockDate", stock.StockDate);
                SqlParameter p3 = new SqlParameter("@batchno", stock.batchno);
                SqlParameter p4 = new SqlParameter("@MfgDate", stock.MfgDate);
                SqlParameter p5 = new SqlParameter("@ExpiryDate", stock.ExpiryDate);
                SqlParameter p6 = new SqlParameter("@QtyOut", stock.QtyOut);
                SqlParameter p7 = new SqlParameter("@issuedTo", stock.IssuedTo);

                var data = db.Database.ExecuteSqlCommand("InsertStock @ProductId,@StockDate,@batchno,@MfgDate,@ExpiryDate,@QtyOut,@issuedTo", p1, p2, p3, p4, p5, p6,p7);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View();
            }

        }

    }
}
