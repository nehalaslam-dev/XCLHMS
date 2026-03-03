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

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class AccountLedgerController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /Accounts/AccountLedger/
        public ActionResult Index()
        {
            //return View(db.AccountLedgers.ToList());
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.AccountLedgers.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Accounts/AccountLedger/Create
        public ActionResult Create(int? id)
        {
            TempData["crtDate"] = db.AccountLedgers.Where(x => x.Id == id).Select(x => x.CreatedDate).SingleOrDefault();
            TempData["LedgerId"] = id;
            ViewBag.HeadId = new SelectList(db.COAs, "ID", "Name");
            return View();
        }

        [HttpPost]
        public JsonResult save(AccountLedger accMaster)
        {
            bool status = false;
            bool flag = false;
            
            flag = DeleteLedgerDetail(Convert.ToInt32(TempData["LedgerId"]));
            if (flag == true)
            {
                var vrno = db.AccountLedgers.OrderByDescending(x => x.voucherNumber).Take(1).Select(x => x.voucherNumber).ToList().SingleOrDefault();

                if (vrno != null)
                {
                    string[] vrNoSplit = vrno.Split('-');
                    int maxvrNo = Convert.ToInt32(vrNoSplit[1]) + 1;
                    accMaster.voucherNumber = "VR-" + maxvrNo + "-" + DateTime.Now.Day + "" + DateTime.Now.Month + "" + DateTime.Now.Year;
                }
                else
                {
                    accMaster.voucherNumber = "VR-1-" + DateTime.Now.Day + "" + DateTime.Now.Month + "" + DateTime.Now.Year;
                }
                if (TempData["crtDate"] == null)
                {
                    accMaster.CreatedDate = DateTime.Now;

                }
                else
                {
                    accMaster.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                    accMaster.ModifiedDate = DateTime.Now;
                }

                var isValidModel = TryValidateModel(accMaster);
                if (isValidModel)
                {
                    db.AccountLedgers.Add(accMaster);
                    db.SaveChanges();
                    status = true;
                }
            }

            return new JsonResult { Data = new { status = status } };
        }

        public bool DeleteLedgerDetail(int pid)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                var data = db.Database.ExecuteSqlCommand("DeleteLedgerDetail @pId", param);
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

        public ActionResult GetLedgerDetail(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetLedgerDetail_Result> list = db.GetLedgerDetail(Convert.ToInt32(id));
            foreach (var item in list)
            {
                var Id = item.Id;
                var voucherNumber = item.voucherNumber;
                var voucherDate = item.voucherDate;
                var description = item.Description;
                var isPosted = item.Isposted;
                var HeadId = item.HeadId;
                var headName = item.HeadName;
                var debit = item.Debit;
                var credit = item.credit;
                var narration = item.narration;

                result.Add(new
                {
                    Id = Id,
                    voucherNumber = voucherNumber,
                    voucherDate = voucherDate,
                    description = description,
                    isPosted = isPosted,
                    HeadId = HeadId,
                    headName = headName,
                    debit = debit,
                    credit = credit,
                    narration = narration
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
                var data = db.Database.ExecuteSqlCommand("DeleteLedgerDetail @pId", param);
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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<AccountLedgerDetail> ledgerdetail = db.AccountLedgerDetails.Where(x => x.LedgerId == id).ToList();

            if (ledgerdetail == null)
            {
                return HttpNotFound();
            }
            return View(ledgerdetail);
        }       
    }
}
