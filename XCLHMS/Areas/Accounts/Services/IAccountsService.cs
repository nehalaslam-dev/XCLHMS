using System;
using System.Collections.Generic;
using System.Linq;
using XCLHMS.Models;
using XCLHMS.Areas.Accounts.Repositories;

namespace XCLHMS.Areas.Accounts.Services
{
    public interface IAccountsService
    {
        // COA
        IEnumerable<COA> GetAllCOA();
        COA GetCOAById(int id);
        void SaveCOA(COA model);
        void DeleteCOA(int id);

        // Calculations for Budget
        // Calculations for Cash Book
    }

    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _repo;

        public AccountsService(IAccountsRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<COA> GetAllCOA()
        {
            return _repo.GetAllCOA();
        }

        public COA GetCOAById(int id)
        {
            return _repo.GetCOAById(id);
        }

        public void SaveCOA(COA model)
        {
            // Add validation here if needed
            _repo.SaveCOA(model);
        }

        public void DeleteCOA(int id)
        {
            _repo.DeleteCOA(id);
        }

        // --- CALCULATION HELPER ---
        public decimal CalculateClosingBalance(decimal opening, decimal totalReceipts, decimal totalPayments)
        {
            return opening + totalReceipts - totalPayments;
        }

        public decimal CalculateNetAmount(decimal gross, decimal incomeTax, decimal sst, decimal stampDuty)
        {
            return gross - (incomeTax + sst + stampDuty);
        }
    }
}
