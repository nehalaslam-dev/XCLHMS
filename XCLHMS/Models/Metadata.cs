using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XCLHMS.Models
{
    public class OrganizationMetadata
    {
        [Required(ErrorMessage = "Name required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Code required.")]
        public string Code { get; set; }
        public string Address { get; set; }
        [RegularExpression(@"^((\+92-?)|0)?[0-9]{10}$", ErrorMessage = "Mobile format is not valid.")]
        public string Phone { get; set; }
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email id is not valid.")]
        public string Email { get; set; }

        [Display(Name = "Active")]
        public Nullable<bool> isActive { get; set; }
    }

    public class PatientTypeMetaData
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Pateint type required.")]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProductCategoryMetaData
    {
        [Required(ErrorMessage = "Category name required.")]
        public string Name { get; set; }
    }

    public class EmployeeMetaData
    {
        [Required(ErrorMessage = "Name required.")]
        public string Name { get; set; }
        [Display(Name = "Father Name")]
        public string FName { get; set; }
        [Required(ErrorMessage = "Gender required.")]
        public string Gender { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "NIC must be numeric.")]
        public string NIC { get; set; }
        [Required(ErrorMessage = "Appointment date required.")]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppDate { get; set; }
        [Display(Name = "Termination Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> terminationdate { get; set; }
        [Display(Name = "Contact")]
        [RegularExpression(@"^((\+92-?)|0)?[0-9]{10}$", ErrorMessage = "Mobile format is not valid.")]
        public string ContactNo { get; set; }
        [Required(ErrorMessage = "Salary required.")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Salary must be in decimal.")]
        public Nullable<decimal> Salary { get; set; }

        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email id is not valid.")]
        public string EmailAddress { get; set; }
    }

    public class ProductMetaData
    {
        [Required(ErrorMessage = "Item name required.")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Item code required.")]
        //public string Code { get; set; }
        //[Required(ErrorMessage = "Qty required.")]
        //public Nullable<int> Qty { get; set; }

    }

    public class VendorMetaData
    {
        [Required(ErrorMessage = "Vendor name required.")]
        public string Name { get; set; }
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter valid email id.")]
        public string Email { get; set; }
    }

    public class PateintMetaData
    {
        // [Required(ErrorMessage = "NIC required.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "NIC must be numeric.")]
        [Remote("IsNicExists", "Pateint", HttpMethod = "POST", ErrorMessage = "NIC already exists.")]
        public string NIC { get; set; }
        [Display(Name = "MR No")]
        public string MRNo { get; set; }
        [Required(ErrorMessage = "Pateint name required.")]
        public string Name { get; set; }
        [Display(Name = "Father Name")]
        public string FName { get; set; }
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Age must be numeric.")]
        [Range(1, 99, ErrorMessage = "Age must be between 1 to 99")]
        public string Age { get; set; }


    }

    public class PatientAppointmentMetaData
    {
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Appointment date required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppDate { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Appointment time required.")]
        public Nullable<System.TimeSpan> AppTime { get; set; }
    }

    public class COAMetaData
    {
        [Required(ErrorMessage = "Account code required.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Head name required.")]
        public string Name { get; set; }
    }

    public class PatientPrescriptionMetaData
    {
        [Required(ErrorMessage = "Qty required.")]
        public Nullable<int> Qty { get; set; }
    }


    public class StockMetaData
    {
        [Required(ErrorMessage = "date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StockDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MfgDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        [Required(ErrorMessage = "Qty required.")]
        //[Range(0, 100000, ErrorMessage = "Please enter valid Qty between 0 to 100000.")]
        public Nullable<int> QtyOut { get; set; }
    }

    public class BedsMetaData
    {
        [Required(ErrorMessage = "Bed number is required.")]
        public string BedNumber { get; set; }
    }

    public class LabsMetaData
    {
        [Required(ErrorMessage = "Lab name required.")]
        public string Name { get; set; }
    }


    public class ShiftPeriodMetaData
    {
        [Required(ErrorMessage = "Shift name required.")]
        public string Name { get; set; }
    }

    public class DutyShiftMetaData
    {
        [Required(ErrorMessage = "Duty shift required.")]
        public string Name { get; set; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "start time required.")]
        public Nullable<System.TimeSpan> StartTime { get; set; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "End time required.")]
        public Nullable<System.TimeSpan> EndTime { get; set; }
    }

    public class EmployeeDutyRoasterMetaData
    {
        [Required(ErrorMessage = "Duty shift required.")]
        public int ShiftId { get; set; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Time in required.")]
        public Nullable<System.TimeSpan> InTime { get; set; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Time out required.")]
        public Nullable<System.TimeSpan> OutTime { get; set; }
    }

    public class DesignationMetaData
    {
        [Required(ErrorMessage = "Designation name required.")]
        public string Name { get; set; }
    }

    public class DepartmentMetaData
    {
        [Required(ErrorMessage = "Code required.")]
        public string DeptCode { get; set; }
        [Required(ErrorMessage = "Department name required.")]
        public string DepartmentName { get; set; }
    }

    public class TestCategoryMetaData
    {
        [Required(ErrorMessage = "Category name required.")]
        public string Name { get; set; }
    }

    public class TestMetaData
    {
        [Required(ErrorMessage = "Test name required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "NormalRange required.")]
        public string NormalRange { get; set; }

        [Required(ErrorMessage = "Test Unit required.")]
        public string Unit { get; set; }
    }

    public class TestUnitMetaData
    {
        [Required(ErrorMessage = "Unit name required.")]
        public string UnitName { get; set; }
    }

    public class AccountRegisterMetaData
    {
        [Required(ErrorMessage = "Bill no required.")]
        public string BillNo { get; set; }

        [Required(ErrorMessage = "Bill date required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> billdate { get; set; }

        [Required(ErrorMessage = "Budget required.")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Budget must be in decimal.")]
        public Nullable<decimal> BudgetRelease { get; set; }
        public Nullable<decimal> IncomeTax { get; set; }

        public Nullable<decimal> SST { get; set; }
    }

    public class PharmacyMetaData
    {
        [Required(ErrorMessage = "Date required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> IssuedDate { get; set; }

        [Required(ErrorMessage = "Qty required.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Qty must be numeric.")]
        public Nullable<int> QtyIssue { get; set; }
    }

    public class EmpTypeMetaData
    {
        [Display(Name = "Employee Type")]
        [Required(ErrorMessage = "Emp Type required.")]
        public string EmpType { get; set; }
    }

    public class TaxesMetaData
    {
        [Required(ErrorMessage = "Name required.")]
        public string TaxName { get; set; }

        [Required(ErrorMessage = "Values required.")]
        public Nullable<decimal> TaxValues { get; set; }
    }

    public class SemanAnalysisMetaData
    {
        [Required(ErrorMessage = "Date required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> TestDate { get; set; }
    }

    public class CustomersMetaData
    {
        [Required(ErrorMessage = "Name required.")]
        public string Name { get; set; }
    }

    public class WardsMetaData
    {
        [Required(ErrorMessage = "Name required.")]
        public string WardName { get; set; }
    }

    [MetadataType(typeof(AUMetadata))]
    public partial class AU
    {
    }

    public class AUMetadata
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Description is required")]
        //public string Description { get; set; }

        [Required(ErrorMessage = "Created Date is required")]
        public Nullable<DateTime> CreatedDate { get; set; }
    }

    [MetadataType(typeof(DosageMetadata))]
    public partial class Dosage
    {
    }

    public class DosageMetadata
    {
        [Required(ErrorMessage = "Dosage Name is required")]
        public string DosageName { get; set; }

        //[Required(ErrorMessage = "Description is required")]
        //public string Description { get; set; }
    }
}
