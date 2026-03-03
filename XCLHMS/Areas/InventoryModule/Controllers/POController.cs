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
    public class POController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            //db.Configuration.ProxyCreationEnabled = false;
            var data = db.POes.Include(p => p.Vendor).OrderByDescending(p => p.Id).ToList();
            var final = (from f in data
                         select new
                         {
                             Id = f.Id,
                             vendorName = f.Vendor.Name,
                             PONO = f.PONO,
                             PODate = f.PQDate,
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemsById(int id)
        {
            List<Product> lstproducts = new List<Product>();
            if (id > 0)
            {
                lstproducts = db.Products.Where(p => p.AUId == id).ToList();
            }
            else
            {
                lstproducts.Insert(0, new Product { Id = 0, Name = "--Select--" });
            }
            var result = (from d in lstproducts
                          select new
                          {
                              id = d.Id,
                              name = d.Name
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemDetailsById(int id)
        {
            var data = (from p in db.Products
                        join d in db.Dosages on p.DosageId equals d.Id into dd
                        from d in dd.DefaultIfEmpty()
                        join b in db.Brands on p.BrandId equals b.Id into bb
                        from b in bb.DefaultIfEmpty()
                        join m in db.Manufactures on p.ManufactureId equals m.Id into mm
                        from m in mm.DefaultIfEmpty()
                        where p.Id == id
                        select new
                        {
                            DosageId = p.DosageId,
                            DosageName = d.DosageName,
                            BrandId = p.BrandId,
                            BrandName = b.BrandName,
                            ManufactureId = p.ManufactureId,
                            ManufactureName = m.ManufactureName
                        }).FirstOrDefault();

            if (data == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDescriptionByproductId(int Id)
        {
            var data = (from d in db.Products
                        where d.Id == Id
                        select d.Description).ToList().SingleOrDefault();
            if (data == null)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPODetailsByPOId(int id)
        {
            var details = (from d in db.PODetails
                           where d.POID == id
                           select new
                           {
                               vId = d.PO.VendorId,
                               poNo = d.PO.PONO,
                               poDate = d.PO.PQDate,
                               Tender_ = d.Tender_,
                               AUId = d.AUId,
                               AUName = d.AU.Name,
                               productId = d.ProductId,
                               productName = d.Product.Name,
                               DosageId = d.DosageId,
                               DosageName = d.Dosage.DosageName,
                               BrandId = d.BrandId,
                               BrandName = d.Brand.BrandName,
                               ManufactureId = d.ManufactureId,
                               ManufactureName = d.Manufacture.ManufactureName,
                               qty = d.Qty,
                               Remarks = d.Remarks
                           }).ToList();

            return Json(details, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Create(int? id)
        {
            // Agar edit mode hai, to record load karo
            if (id != null)
            {
                var po = db.POes
                    .Include(p => p.Vendor)
                    .Include(p => p.PODetails)
                    .FirstOrDefault(p => p.Id == id);

                if (po == null)
                    return HttpNotFound();

                // Populate dropdowns
                ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", po.VendorId);
                ViewBag.Dosage = new SelectList(db.Dosages, "Id", "DosageName");
                ViewBag.BrandName = new SelectList(db.Brands, "Id", "BrandName");
                ViewBag.Manufacture = new SelectList(db.Manufactures, "Id", "ManufactureName");
                ViewBag.AU = new SelectList(db.AUs, "Id", "Name");
                ViewBag.productCategory = new SelectList(db.Products, "Id", "Name");

                // Pass details to view
                ViewBag.EditMode = true;
                TempData["POId"] = id;
                return View(po);
            }

            // Normal create (new record)
            List<AU> lstcategory = db.AUs.ToList();
            lstcategory.Insert(0, new AU { Id = 0, Name = "--Select--" });

            List<Product> lstproducts = new List<Product>();

            ViewBag.Dosage = new SelectList(db.Dosages.ToList(), "Id", "DosageName");
            ViewBag.BrandName = new SelectList(db.Brands.ToList(), "Id", "BrandName");
            ViewBag.Manufacture = new SelectList(db.Manufactures.ToList(), "Id", "ManufactureName");
            ViewBag.AU = new SelectList(lstcategory, "Id", "Name");
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            ViewBag.productCategory = new SelectList(lstproducts, "Id", "Name");

            ViewBag.EditMode = false;
            return View();
        }

        [HttpPost]
        public JsonResult save(PO pomaster)
        {
            bool status = false;

            try
            {
                int poId = pomaster.Id; // 👈 frontend hidden field se Id mil rahi hai

                if (string.IsNullOrEmpty(pomaster.PONO))
                {
                    return new JsonResult { Data = new { status = false, message = "Please enter PO Number." } };
                }

                if (poId > 0)
                {
                    var existingPO = db.POes.Include(p => p.PODetails).FirstOrDefault(p => p.Id == poId);

                    if (existingPO != null)
                    {
                        existingPO.VendorId = pomaster.VendorId;
                        existingPO.PONO = pomaster.PONO;
                        existingPO.PQDate = pomaster.PQDate;

                        db.PODetails.RemoveRange(existingPO.PODetails);

                        foreach (var item in pomaster.PODetails)
                        {
                            existingPO.PODetails.Add(new PODetail
                            {
                                Tender_ = item.Tender_,
                                DosageId = item.DosageId,
                                AUId = item.AUId,
                                ProductId = item.ProductId,
                                BrandId = item.BrandId,
                                ManufactureId = item.ManufactureId,
                                Qty = item.Qty,
                                Remarks = item.Remarks
                            });
                        }

                        db.SaveChanges();
                        status = true;
                    }
                }
                else
                {
                    db.POes.Add(pomaster);
                    db.SaveChanges();
                    status = true;
                }

                return new JsonResult { Data = new { status = status } };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { status = false, message = ex.Message } };
            }
        }



        public bool DeletePODetail(int pid)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                var data = db.Database.ExecuteSqlCommand("DeletePODetail @pId", param);
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

        public ActionResult GetPODetail(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetPODetail_Result> list = db.GetPODetail5(Convert.ToInt32(id));

            foreach (var item in list)
            {
                var vId = item.vendorid;
                var poNo = item.pono;
                var poDate = item.pqdate;
                var Tender_ = item.Tender_;
                var DosageId = item.DosageId;
                var DosageName = item.DosageName;
                var AUId = item.AUId;
                var AUName = item.AU;
                var productId = item.productid;
                var productName = item.productname;
                var BrandId = item.BrandId;
                var BrandName = item.BrandName;
                var ManufactureId = item.ManufactureId;
                var ManufactureName = item.ManufactureName;
                var qty = item.qty;
                var Remarks = item.Remarks;

                result.Add(new
                {
                    vId = vId,
                    poNo = poNo,
                    poDate = poDate,
                    Tender_ = item.Tender_,
                    DosageId = DosageId,
                    DosageName = DosageName,
                    AUId = AUId,
                    AUName = AUName,
                    productId = productId,
                    productName = productName,
                    BrandId = BrandId,
                    BrandName = BrandName,
                    ManufactureId = ManufactureId,
                    ManufactureName = ManufactureName,
                    qty = qty,
                    Remarks = item.Remarks


                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", Id);
                var data = db.Database.ExecuteSqlCommand("DeletePODetail @pId", param);
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
            List<PODetail> podetail = db.PODetails.Where(x => x.POID == id).ToList();

            if (podetail == null)
            {
                return HttpNotFound();
            }
            return View(podetail);
        }

        public ActionResult LoadReport(string Id)
        {
            Response.Redirect("~/Reports/POReport.aspx?Id=" + Id);
            return View();

        }

        public JsonResult GetLookupNames(int dosageId, int brandId, int manufactureId)
        {
            var dosage = db.Dosages.FirstOrDefault(d => d.Id == dosageId);
            var brand = db.Brands.FirstOrDefault(b => b.Id == brandId);
            var manufacture = db.Manufactures.FirstOrDefault(m => m.Id == manufactureId);

            return Json(new
            {
                DosageName = dosage?.DosageName ?? "",
                BrandName = brand?.BrandName ?? "",
                ManufactureName = manufacture?.ManufactureName ?? ""
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
