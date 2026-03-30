using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Controllers
{
    public class VendorManagementController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Database.SqlQuery<VendorDTO>(
                "SELECT Id, Name, Code, Address, ContactNo, Active, NTN, CNIC, Status, PaymentSection FROM Vendors"
            ).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public class VendorDTO {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Address { get; set; }
            public string ContactNo { get; set; }
            public bool? Active { get; set; }
            public string NTN { get; set; }
            public string CNIC { get; set; }
            public string Status { get; set; }
            public string PaymentSection { get; set; }
        }

        [HttpPost]
        public JsonResult Save(VendorDTO model)
        {
            try
            {
                if (model.Id == 0)
                {
                    db.Database.ExecuteSqlCommand(
                        "INSERT INTO Vendors (Name, Code, Address, ContactNo, Active, CreatedDate, NTN, CNIC, Status, PaymentSection) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                        model.Name, model.Code, model.Address, model.ContactNo, model.Active ?? true, DateTime.Now, model.NTN, model.CNIC, model.Status, model.PaymentSection
                    );
                }
                else
                {
                    db.Database.ExecuteSqlCommand(
                        "UPDATE Vendors SET Name={0}, Code={1}, Address={2}, ContactNo={3}, Active={4}, ModifiedDate={5}, NTN={6}, CNIC={7}, Status={8}, PaymentSection={9} WHERE Id={10}",
                        model.Name, model.Code, model.Address, model.ContactNo, model.Active ?? true, DateTime.Now, model.NTN, model.CNIC, model.Status, model.PaymentSection, model.Id
                    );
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
