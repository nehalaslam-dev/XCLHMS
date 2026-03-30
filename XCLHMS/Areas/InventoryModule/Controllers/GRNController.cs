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
    public class GRNController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /InventoryModule/GRN/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var final = (from a in db.GRNs
                         join c in db.POes on a.POId equals c.Id
                         select new
                         {
                             Id = a.Id,
                             PONO = c.PONO,
                             GRNNO = a.GRNNO,
                             GRNDate = a.GRNDate,
                             TotalAmount = a.TotalAmount,
                             IsComplete = a.IsComplete,
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
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

        [HttpGet]
        public JsonResult GetDescriptionByproductId(int POId, int ProductId)
        {
            var data = (from pd in db.PODetails
                        join p in db.Products on pd.ProductId equals p.Id
                        where pd.POID == POId && pd.ProductId == ProductId
                        select new
                        {
                            description = p.Description,
                            orderQty = pd.Qty
                        }).ToList().SingleOrDefault();

            if (data == null)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        // GET: /InventoryModule/GRN/Create
        public ActionResult Create(int? id)
        {
            List<ProductCategory> lstcategory = db.ProductCategories.ToList();
            lstcategory.Insert(0, new ProductCategory { Id = 0, Name = "--Select--" });

            List<Stock> lstproducts = new List<Stock>();

            TempData["GRNId"] = id;
            ViewBag.POId = new SelectList(db.POes, "Id", "PONO");
            ViewBag.ItemCategory = new SelectList(lstcategory, "Id", "Name");
            ViewBag.productCategory = new SelectList(lstproducts, "Id", "Name");
            return View();
        }

        [HttpPost]
        public JsonResult save(GRN grnmaster)
        {
            try
            {
                bool status = false;
                if (Convert.ToInt32(TempData["GRNId"]) > 0)
                {
                    DeleteGRNDetail(Convert.ToInt32(TempData["GRNId"]),grnmaster.GRNNO);
                }
                var grnNo = db.GRNs.OrderByDescending(x => x.Id).Take(1).Select(x => x.GRNNO).ToList().SingleOrDefault();
                if (grnNo != null)
                {
                    string[] grnNoSplit = grnNo.Split('-');
                    int maxGrnNo = Convert.ToInt32(grnNoSplit[1]) + 1;
                    grnmaster.GRNNO = "GRN-" + maxGrnNo.ToString();
                }
                else
                {
                    grnmaster.GRNNO = "GRN-1";
                }

                var isValidModel = TryValidateModel(grnmaster);
                grnmaster.Stocks.ToList().ForEach(c => c.RefCode = grnmaster.GRNNO);
                if (isValidModel)
                {
                    db.GRNs.Add(grnmaster);
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

        public bool DeleteGRNDetail(int pid, string refCode)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                SqlParameter param1 = new SqlParameter("@refCode", refCode);
                var data = db.Database.ExecuteSqlCommand("DeleteGRNDetail @pId,@refCode", param,param1);
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

        public ActionResult GetGRNDetail(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetGRNDetail_Result> list = db.GetGRNDetail(Convert.ToInt32(id));

            foreach (var item in list)
            {
                var poId = item.poid;
                var grnNo = item.grnno;
                var grnDate = item.grndate;
                var totalAmt = item.totalamount;
                var isComplete = item.iscomplete;
                var productId = item.productid;
                var productName = item.productname;
                var description = item.description;
                var orderQty = item.orderQty;
                var qty = item.qty;
                var rejectQty = item.rejectqty;
                var costPrice = item.costprice;
                var batchNo = item.batchno;
                var mfgDate = item.mgfdate;
                var expiryDate = item.expirydate;
                var catId = item.categoryId;
                var categoryName = item.categoryName;

                result.Add(new
                {
                    poId = poId,
                    grnNo = grnNo,
                    grnDate = grnDate,
                    totalAmt = totalAmt,
                    isComplete = isComplete,
                    productId = productId,
                    productName = productName,
                    description = description,
                    orderQty = orderQty,
                    qty = qty,
                    rejectQty = rejectQty,
                    costPrice = costPrice,
                    batchNo = batchNo,
                    mfgDate = mfgDate,
                    expiryDate = expiryDate,
                    catId = catId,
                    categoryName = categoryName

                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult Delete(int? Id, string RefCode)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", Id);
                SqlParameter param1 = new SqlParameter("@refCode", RefCode);
                var data = db.Database.ExecuteSqlCommand("DeleteGRNDetail @pId,@refCode", param,param1);
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

        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<GRNDetail> grndetail = db.GRNDetails.Where(x => x.GRNID == id).ToList();

            if (grndetail == null)
            {
                return HttpNotFound();
            }
            return View(grndetail);
        }

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/GRNReport.aspx?Id=" + Id);
            return View();

        }

    }
}



