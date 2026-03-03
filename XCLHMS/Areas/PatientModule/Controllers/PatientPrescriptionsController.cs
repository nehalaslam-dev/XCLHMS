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

namespace XCLHMS.Areas.PatientModule.Controllers
{
    public class PatientPrescriptionsController : Controller
    {
        private HMSEntities db = new HMSEntities();

        public ActionResult List()
        {
            ViewBag.ProductID = new SelectList(db.Products.Where(x => x.CategoryId == 8), "Id", "Name");
            return View();
        }

        public ActionResult LoadGrid()
        {
            ObjectResult<GetPateintAppointments_Result> list = db.GetPateintAppointments();
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult savePresc(string PatientRecordId, PatientPrescription[] pres)
        {
            string result = "Error! Record Is Not Complete!";
            bool flag = false;
            if (PatientRecordId != null && pres != null)
            {
                flag = DeleteRecord(Convert.ToInt32(PatientRecordId));
                if (flag == true)
                {
                    foreach (var item in pres)
                    {
                        PatientPrescription p = new PatientPrescription();
                        p.PatientRecordId = Convert.ToInt32(PatientRecordId);
                        p.ProductId = item.ProductId;
                        p.Qty = item.Qty;
                        p.Comments = item.Comments;
                        p.CreatedDate = DateTime.Now;
                        UpdateAppointmentStatus(Convert.ToInt32(PatientRecordId));
                        db.PatientPrescriptions.Add(p);
                    }
                    db.SaveChanges();
                    result = "Success! Record saved!";
                }
                else
                {
                    result = "Error! Record is not Updated!";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void UpdateAppointmentStatus(int PatientRecordId)
        {
            var record = (from x in db.PateintAppointments where x.Id == PatientRecordId select x).Single();
            record.AppStatus = "Visited";
            db.SaveChanges();
        }

        public bool DeleteRecord(int pid)
        {
            try
            {
                SqlParameter param = new SqlParameter("@pId", pid);
                var data = db.Database.ExecuteSqlCommand("DeletePrescriptionById @pId", param);
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

        public ActionResult GetPrescriptionById(string pId)
        {
            List<dynamic> list = new List<dynamic>();
            ObjectResult<GetPrescriptionById_Result> result = db.GetPrescriptionById(Convert.ToInt32(pId));
            foreach (var item in result)
            {
                var medicineId = item.medicineId;
                var medicineName = item.medicineName;
                var Qty = item.Qty;
                var comments = item.comments;

                list.Add(new
                {
                    medicineId = medicineId,
                    medicineName = medicineName,
                    Qty = Qty,
                    comments = comments

                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }

    }
}
