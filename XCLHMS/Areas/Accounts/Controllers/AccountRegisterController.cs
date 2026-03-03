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
    public class AccountRegisterController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /Accounts/AccountRegister/
        public ActionResult Index()
        {
            //var accountregisters = db.AccountRegisters.Include(a => a.COA).Include(a => a.Vendor);
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.AccountRegisters.Include(a => a.COA).ToList();
            var final = (from d in data
                         select new
                         {
                             HeadId = d.HeadId,
                             HeadName = d.COA.Name,
                             HeadCode = d.COA.Code,

                         }).Distinct();
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTaxById(int id)
        {
            List<TaxDetail> lstTaxes = new List<TaxDetail>();
            if (id > 0)
            {
                lstTaxes = db.TaxDetails.Where(p => p.TaxId == id).ToList();
            }
            else
            {
                lstTaxes.Insert(0, new TaxDetail { Id = 0, TaxName = "--Select--" });
            }
            var result = (from d in lstTaxes
                          select new
                          {
                              id = d.TaxValues,
                              name = d.TaxName
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBudgetInfoById(int Id)
        {

            List<dynamic> result = new List<dynamic>();
            var accCode = (from d in db.COAs where d.ID == Id select new { Code = d.Code }).SingleOrDefault();
            var sumBudget = (from b in db.Budgets where b.HeadId == Id select b.Budget1).Sum();
            //var data = (from d in db.COAs
            //            join c in db.Budgets on d.ID equals c.HeadId
            //            where c.HeadId == Id
            //            select new
            //            {
            //                Code = d.Code,
            //                budget = c.Budget1
            //            }).SingleOrDefault();
            if (accCode != null && sumBudget != null)
            {
                //var code = accCode;
                //var budget = sumBudget;

                result.Add(new
                {
                    code = accCode.Code,
                    budget = sumBudget
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult save(string HeadId, AccountRegister[] reg)
        {
            string result = "Error! Record Is Not Complete!";

            try
            {
                if (HeadId != null && reg != null)
                {
                    DeleteAccountDetail(Convert.ToInt32(HeadId));
                    foreach (var item in reg)
                    {
                        AccountRegister acReg = new AccountRegister();
                        acReg.HeadId = item.HeadId;
                        acReg.BillNo = item.BillNo;
                        acReg.billdate = item.billdate;
                        acReg.VendorId = item.VendorId;
                        acReg.ChequeNo = item.ChequeNo;
                        acReg.AnnualBudget = item.AnnualBudget;
                        acReg.BudgetRelease = item.BudgetRelease;
                        acReg.GrossAmount = item.GrossAmount;
                        acReg.EntryType = item.EntryType;
                        acReg.IncomeTax = item.IncomeTax;
                        acReg.SST = item.SST;
                        acReg.NetAmount = item.NetAmount;
                        acReg.Balance = item.Balance;
                        db.AccountRegisters.Add(acReg);
                    }
                    db.SaveChanges();
                    result = "Success! Record saved!";
                }
            }
            catch (Exception ex)
            {

                result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccountRegsiter(string id)
        {
            List<dynamic> result = new List<dynamic>();
            ObjectResult<GetAccountRegister_Result> list = db.GetAccountRegister(Convert.ToInt32(id));

            foreach (var item in list)
            {
                var headId = item.HeadId;
                var accountName = item.accountName;
                var accountCode = item.Code;
                var vendorid = item.vendorId;
                var vendorname = item.vendorName;
                var chequeNo = item.ChequeNo;
                var billno = item.BillNo;
                var billdate = item.billdate;
                var annualBudget = item.AnnualBudget;
                var relBudget = item.BudgetRelease;
                var grossAmount = item.GrossAmount;
                var entryType = item.EntryType;
                var incomeTax = item.IncomeTax;
                var sst = item.SST;
                var netAmount = item.NetAmount;
                var balance = item.Balance;

                result.Add(new
                {
                    headId = headId,
                    accountName = accountName,
                    accountCode = accountCode,
                    vendorid = vendorid,
                    vendorname = vendorname,
                    chequeNo = chequeNo,
                    billno = billno,
                    billdate = billdate,
                    annualBudget = annualBudget,
                    relBudget = relBudget,
                    grossAmount = grossAmount,
                    entryType=entryType,
                    incomeTax = incomeTax,
                    sst = sst,
                    netAmount = netAmount,
                    balance = balance
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CashbookReport(string Id)
        {
            Response.Redirect("~/Reports/Cashbook.aspx");
            return View();

        }

        // GET: /Accounts/AccountRegister/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountRegister accountregister = db.AccountRegisters.Find(id);
            if (accountregister == null)
            {
                return HttpNotFound();
            }
            return View(accountregister);
        }

        // GET: /Accounts/AccountRegister/Create
        public ActionResult Create(int? id)
        {
            List<tax> lstTax = db.taxes.ToList();
            lstTax.Insert(0, new tax { Id = 0, TaxName = "--Select--" });
            List<TaxDetail> lsttaxdetail = new List<TaxDetail>();

            var data = db.COAs.Where(p => p.ParentID != 0).ToList();
            ViewBag.HeadId = new SelectList(data, "ID", "Name");
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            ViewBag.TaxId = new SelectList(lstTax, "Id", "TaxName");
            ViewBag.subTax = new SelectList(lsttaxdetail, "Id", "TaxName");
            ViewBag.SecTaxId = new SelectList(lstTax, "Id", "TaxName");
            ViewBag.SecsubTax = new SelectList(lsttaxdetail, "Id", "TaxName");
            return View();
        }

        // POST: /Accounts/AccountRegister/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HeadId,BillNo,billdate,VendorId,AnnualBudget,BudgetRelease,GrossAmount,IncomeTax,SST,NetAmount,Balance")] AccountRegister accountregister)
        {
            if (ModelState.IsValid)
            {
                db.AccountRegisters.Add(accountregister);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HeadId = new SelectList(db.COAs, "ID", "Code", accountregister.HeadId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", accountregister.VendorId);
            return View(accountregister);
        }

        // GET: /Accounts/AccountRegister/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountRegister accountregister = db.AccountRegisters.Find(id);
            if (accountregister == null)
            {
                return HttpNotFound();
            }
            ViewBag.HeadId = new SelectList(db.COAs, "ID", "Code", accountregister.HeadId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", accountregister.VendorId);
            return View(accountregister);
        }

        // POST: /Accounts/AccountRegister/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HeadId,BillNo,billdate,VendorId,AnnualBudget,BudgetRelease,GrossAmount,IncomeTax,SST,NetAmount,Balance")] AccountRegister accountregister)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountregister).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HeadId = new SelectList(db.COAs, "ID", "Code", accountregister.HeadId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", accountregister.VendorId);
            return View(accountregister);
        }


        public bool DeleteAccountDetail(int pid)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                var data = db.Database.ExecuteSqlCommand("deleteRegisterById @pId", param);
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
            Response.Redirect("~/Reports/RegisterReport.aspx?Id=" + Id);
            return View();

        }
    }
}
