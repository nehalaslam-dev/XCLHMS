using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace XCLHMS.Models
{
    // Comprehensive partial class to extend auto-generated HMSEntities
    // This file acts as a bridge and extension context to avoid modifying auto-generated files.
    public partial class HMSEntities : DbContext
    {
        // New tables added for Accounts Module
        public virtual DbSet<Accounts_ChequeRegister> Accounts_ChequeRegisters { get; set; }
        public virtual DbSet<Accounts_ExpenditureStatement> Accounts_ExpenditureStatements { get; set; }

        // New tables for Stock Inventory (if not in EDMX)
        public virtual DbSet<Brands> Brands { get; set; }
        public virtual DbSet<Manufacture> Manufactures { get; set; }
        public virtual DbSet<Issuance> Issuances { get; set; }
        public virtual DbSet<AU> AUs { get; set; }
        public virtual DbSet<Dosage> Dosages { get; set; }

        // --- PLURALIZATION BRIDGE (Aliases for Legacy Code) ---
        public virtual DbSet<AccountLedger> AccountLedgers => Set<AccountLedger>();
        public virtual DbSet<AccountLedgerDetail> AccountLedgerDetails => Set<AccountLedgerDetail>();
        public virtual DbSet<AccountRegister> AccountRegisters => Set<AccountRegister>();
        public virtual DbSet<COA> COAs => Set<COA>();
        public virtual DbSet<TaxDetail> TaxDetails => Set<TaxDetail>();
        public virtual DbSet<Budget> Budgets => Set<Budget>();
        public virtual DbSet<TaxConfiguration> TaxConfigurations => Set<TaxConfiguration>();
        public virtual DbSet<EmployeeAllowance> EmployeeAllowances => Set<EmployeeAllowance>();
        public virtual DbSet<EmployeeType> EmployeeTypes => Set<EmployeeType>();
        public virtual DbSet<Employee> Employees => Set<Employee>();
        public virtual DbSet<Department> Departments => Set<Department>();
        public virtual DbSet<EmployeeDutyRoaster> EmployeeDutyRoasters => Set<EmployeeDutyRoaster>();
        public virtual DbSet<EmployeeRole> EmployeeRoles => Set<EmployeeRole>();
        public virtual DbSet<Pateint> Pateints => Set<Pateint>();
        public virtual DbSet<BedManagement> BedManagements => Set<BedManagement>();
        public virtual DbSet<GRN> GRNs => Set<GRN>();
        public virtual DbSet<GRNDetail> GRNDetails => Set<GRNDetail>(); 
        public virtual DbSet<PO> POes => Set<PO>();
        public virtual DbSet<PODetail> PODetails => Set<PODetail>();
        public virtual DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public virtual DbSet<IR> IRs => Set<IR>();
        public virtual DbSet<IRDetails> IRDetail_Alias => Set<IRDetails>(); 
        public virtual DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
        public virtual DbSet<Token> Tokens => Set<Token>();
        public virtual DbSet<Stock> Stocks => Set<Stock>(); 
        public virtual DbSet<Test> Tests => Set<Test>();
        public virtual DbSet<LabRegistration> LabRegistrations => Set<LabRegistration>();
        public virtual DbSet<LabTest> LabTests => Set<LabTest>();
        public virtual DbSet<TestCategory> TestCategories => Set<TestCategory>();
        public virtual DbSet<TestResult> TestResults => Set<TestResult>();
        public virtual DbSet<TestResultDetails> TestResultDetail_Alias => Set<TestResultDetails>();
        public virtual DbSet<TestUnit> TestUnits => Set<TestUnit>();
        public virtual DbSet<PostMartam> PostMartams => Set<PostMartam>();
        public virtual DbSet<MLC> MLCs => Set<MLC>();
        public virtual DbSet<PatientType> PatientTypes => Set<PatientType>();
        public virtual DbSet<MenuAppCompanyMapp> MenuAppCompanyMapps => Set<MenuAppCompanyMapp>();
        public virtual DbSet<MenuApplicationPrivacy> MenuApplicationPrivacies => Set<MenuApplicationPrivacy>();
        public virtual DbSet<Organization> Organizations => Set<Organization>();

        // Pluralization helpers if legacy code expects plural names for new module
        public virtual DbSet<Brands> Brand => Set<Brands>();
        public virtual DbSet<Manufacture> Manufacture => Set<Manufacture>();
    }

    // Temporary Models for new tables
    public partial class Accounts_ChequeRegister
    {
        public int Id { get; set; }
        public string ChequeNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string BillNo { get; set; }
        public string VendorName { get; set; }
        public int? ObjectHeadId { get; set; }
        public bool IsCleared { get; set; }
        public DateTime? ClearedDate { get; set; }
        public string Status { get; set; }
    }

    public partial class Accounts_ExpenditureStatement
    {
        public int Id { get; set; }
        public int? HeadId { get; set; }
        public string FiscalYear { get; set; }
        public string MonthName { get; set; }
        public decimal ExpenditureAmount { get; set; }
    }

    // --- NAVIGATION PROPERTY ALIASES ---
    public partial class TaxDetail {
        public virtual taxes tax { get => taxes; set => taxes = value; }
    }
    public partial class Employee {
        public virtual Designations Designation { get => Designations; set => Designations = value; }
    }
    public partial class EmployeeDutyRoaster {
        public virtual DutyShifts DutyShift { get => DutyShifts; set => DutyShifts = value; }
        public virtual ShiftPeriods ShiftPeriod { get => ShiftPeriods; set => ShiftPeriods = value; }
    }
    public partial class GRN {
        public virtual ICollection<Stock> Stocks { get => Stock; set => Stock = value; }
    }
    public partial class IR {
        public virtual Customers Customer { get => Customers; set => Customers = value; }
        public virtual ICollection<Stock> Stocks { get => Stock; set => Stock = value; }
    }
    public partial class IRDetails {
        public DateTime? CreatedDate { get; set; }
    }
    public partial class PO {
        public virtual Vendors Vendor { get => Vendors; set => Vendors = value; }
        public virtual ICollection<PODetail> PODetails { get => PODetail; set => PODetail = value; }
    }
    public partial class PODetail {
        public virtual Products Product { get => Products; set => Products = value; }
        public int? AUId { get; set; }
        public int? DosageId { get; set; }
        public int? BrandId { get; set; }
        public int? ManufactureId { get; set; }
        public string Remarks { get; set; }
        public string Tender_ { get; set; }
        public virtual AU AU { get; set; }
        public virtual Dosage Dosage { get; set; }
        public virtual Brands Brand { get; set; }
        public virtual Manufacture Manufacture { get; set; }
    }
    public partial class Products {
        public int? AUId { get; set; }
        public int? DosageId { get; set; }
        public int? BrandId { get; set; }
        public int? ManufactureId { get; set; }
        public virtual AU AU { get; set; }
        public virtual Dosage Dosage { get; set; }
        public virtual Brands Brand { get; set; }
        public virtual Manufacture Manufacture { get; set; }
    }
    public partial class LabTest {
        public virtual Labs Lab { get => Labs; set => Labs = value; }
    }
    public partial class SemanAnalysis {
        public virtual Labs Lab { get => Labs; set => Labs = value; }
    }

    // --- TYPE ALIASES (Legacy Compatibility) ---
    public partial class tax : taxes { }
    public partial class DutyShift : DutyShifts { }
    public partial class ShiftPeriod : ShiftPeriods { }
    public partial class Bed : Beds { }
    public partial class Ward : Wards { }
    public partial class Customer : Customers { }
    public partial class Product : Products { }
    public partial class Vendor : Vendors { }
    public partial class Lab : Labs { }
    public partial class SemanAnalysi : SemanAnalysis { }
    public partial class PateintAppointment : PateintAppointments { }
    public partial class Designation : Designations { }
    // Note: Stock : Products inheritance removed as they are separate entity types and was causing conversion errors.
    // public partial class Stock : Products { }

    // --- ENTITY NAVIGATION EXTENSIONS ---
    public partial class DutyShifts
    {
        public virtual ShiftPeriods ShiftPeriod { get => ShiftPeriods; set => ShiftPeriods = value; }
    }
    public partial class Beds
    {
        public virtual Wards Ward { get => Wards; set => Wards = value; }
    }
}
