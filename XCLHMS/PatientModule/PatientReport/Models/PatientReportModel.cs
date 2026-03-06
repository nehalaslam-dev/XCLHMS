using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCLHMS.PatientModule.PatientReport.Models
{
    public class PatientReportModel
    {
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string DoctorName { get; set; }
        public string VisitDate { get; set; }
        public string ChallanNo { get; set; }
        public string Address { get; set; }
    }
}
