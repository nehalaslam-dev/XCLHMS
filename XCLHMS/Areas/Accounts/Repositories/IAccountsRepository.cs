using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XCLHMS.Models;

namespace XCLHMS.Areas.Accounts.Repositories
{
    public interface IAccountsRepository
    {
        // COA
        IEnumerable<COA> GetAllCOA();
        COA GetCOAById(int id);
        void SaveCOA(COA model);
        void DeleteCOA(int id);

        // Budget
        IEnumerable<Budget> GetBudgets();
        void SaveBudget(Budget model);

        // Vendors
        IEnumerable<Vendors> GetAllVendors();
        void SaveVendor(Vendors model);

        // Cheque Register
        // Add more as needed...
    }

    public class AccountsRepository : IAccountsRepository
    {
        private readonly HMSEntities _db;

        public AccountsRepository()
        {
            _db = new HMSEntities();
        }

        public IEnumerable<COA> GetAllCOA()
        {
            return _db.COA.OrderBy(x => x.Code).ToList();
        }

        public COA GetCOAById(int id)
        {
            return _db.COA.Find(id);
        }

        public void SaveCOA(COA model)
        {
            if (model.ID == 0)
            {
                model.CreatedDate = DateTime.Now;
                _db.COA.Add(model);
            }
            else
            {
                _db.Entry(model).State = EntityState.Modified;
                model.ModifiedDate = DateTime.Now;
            }
            _db.SaveChanges();
        }

        public void DeleteCOA(int id)
        {
            var item = _db.COA.Find(id);
            if (item != null)
            {
                _db.COA.Remove(item);
                _db.SaveChanges();
            }
        }

        public IEnumerable<Budget> GetBudgets()
        {
            return _db.Budget.Include(b => b.COA).ToList();
        }

        public void SaveBudget(Budget model)
        {
            if (model.Id == 0)
                _db.Budget.Add(model);
            else
                _db.Entry(model).State = EntityState.Modified;
            
            _db.SaveChanges();
        }

        public IEnumerable<Vendors> GetAllVendors()
        {
            return _db.Vendors.ToList();
        }

        public void SaveVendor(Vendors model)
        {
            if (model.Id == 0)
                _db.Vendors.Add(model);
            else
                _db.Entry(model).State = EntityState.Modified;

            _db.SaveChanges();
        }
    }
}
