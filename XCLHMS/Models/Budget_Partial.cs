using System;

namespace XCLHMS.Models
{
    public partial class Budget
    {
        // New budget fields requested for Accounts Module
        public decimal? BudgetEstimates { get; set; }
        public decimal? RevisedBudget { get; set; }
        public decimal? AdditionalBudget { get; set; }
        public decimal? SupplementaryBudget { get; set; }
        public decimal? TotalRevisedBudget { get; set; }
        public decimal? ReAppropriationPositive { get; set; }
        public decimal? ReAppropriationNegative { get; set; }
        public decimal? TotalRevisedBudgetAfterReApp { get; set; }
        public string FiscalYear { get; set; }
    }
}
