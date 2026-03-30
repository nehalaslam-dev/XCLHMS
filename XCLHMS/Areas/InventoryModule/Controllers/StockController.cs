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
            //var data = db.Stocks.Include(p => p.Stock).ToList();
            //var final = (from d in data
            //             select new
            //             {
            //                 Id = d.Id,
            //                 productName = d.Stock.Name,
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
            Stock Stock = db.Stocks.Find(id);
            if (Stock == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ProductId = new SelectList(db.Stock, "Id", "Name", Stock.ProductId);
            TempData["ProductId"] = Stock.ProductId;
            Stock.StockDate = null;
            return View(Stock);
        }

        // POST: /InventoryModule/Stock/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stock Stock)
        {
            try
            {
                SqlParameter p1 = new SqlParameter("@ProductId", Convert.ToInt32(TempData["ProductId"]));
                SqlParameter p2 = new SqlParameter("@StockDate", Stock.StockDate);
                SqlParameter p3 = new SqlParameter("@batchno", Stock.batchno);
                SqlParameter p4 = new SqlParameter("@MfgDate", Stock.MfgDate);
                SqlParameter p5 = new SqlParameter("@ExpiryDate", Stock.ExpiryDate);
                SqlParameter p6 = new SqlParameter("@QtyOut", Stock.QtyOut);
                SqlParameter p7 = new SqlParameter("@issuedTo", Stock.IssuedTo);

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




