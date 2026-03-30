using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Areas.EmployeeModule.Controllers
{
    public class EmployeeController : Controller
    {
        private HMSEntities db = new HMSEntities();

        // GET: /EmployeeModule/Employee/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadGrid()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.Employees.Include(e => e.Designations).Include(e => e.EmployeeType).Include(d => d.Department).ToList();
            var final = (from f in data
                         select new
                         {
                             Id = f.Id,
                             designationName = f.Designations.Name,
                             departmentName = f.Department.DepartmentName,
                             empType = f.EmployeeType.EmpType,
                             Name = f.Name,
                             Nic = f.NIC,
                             empCode = f.EmployeeCode,
                             Grade = f.Grade,
                             Salary = f.Salary,
                             ContactNo = f.ContactNo,
                             AppDate = f.AppDate,
                             terminationdate = f.terminationdate,
                             Active = f.Active
                         });
            return Json(new { data = final }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmployeeModule/Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: /EmployeeModule/Employee/Create
        public ActionResult Create()
        {
            ViewBag.EmpTypeID = new SelectList(db.EmployeeTypes, "Id", "EmpType");
            ViewBag.DesignationID = new SelectList(db.Designations, "Id", "Name");
            ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "DepartmentName");
            return View();
        }

        // POST: /EmployeeModule/Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DesignationID,DepartmentId,EmpTypeId,Name,FName,employeeCode,NIC,Grade,Gender,Address,ContactNo,EmailAddress,AppDate,DateOfBirth,Salary,ContractExpireDate,terminationdate,Active,CreatedDate,ModifiedDate")] Employee employee, string Password)
        {
            if (ModelState.IsValid)
            {
                var empCode = db.Employees.OrderByDescending(x => x.Id).Take(1).Select(x => x.EmployeeCode).ToList().SingleOrDefault();
                if (empCode != null)
                {
                    string[] empCodeSplit = empCode.Split('-');
                    int maxEmpCode = Convert.ToInt32(empCodeSplit[1]) + 1;
                    employee.EmployeeCode = "E-" + maxEmpCode.ToString();
                }
                else
                {
                    employee.EmployeeCode = "E-1";
                }

                employee.CreatedDate = DateTime.Now;
                db.Employees.Add(employee);
                db.SaveChanges();


                SqlParameter p1 = new SqlParameter("@employeeID", employee.Id);
                SqlParameter p2 = new SqlParameter("@designationId", employee.DesignationID);
                SqlParameter p3 = new SqlParameter("@Password", Password);
                db.Database.ExecuteSqlCommand("InsertPasswordLog  @employeeID,@designationId,@Password", p1, p2, p3);
                return RedirectToAction("Index");
            }
            ViewBag.EmpTypeID = new SelectList(db.EmployeeTypes, "Id", "EmpType", employee.EmpTypeId);
            ViewBag.DesignationID = new SelectList(db.Designations, "Id", "Name", employee.DesignationID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "DepartmentName", employee.DepartmentId);

            return View(employee);
        }

        // GET: /EmployeeModule/Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            TempData["crtDate"] = employee.CreatedDate;
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpTypeID = new SelectList(db.EmployeeTypes, "Id", "EmpType", employee.EmpTypeId);
            ViewBag.DesignationID = new SelectList(db.Designations, "Id", "Name", employee.DesignationID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "DepartmentName", employee.DepartmentId);

            return View(employee);
        }

        // POST: /EmployeeModule/Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DesignationID,DepartmentId,EmpTypeId,Name,FName,employeeCode,NIC,Grade,Gender,Address,ContactNo,EmailAddress,AppDate,DateOfBirth,Salary,ContractExpireDate,terminationdate,Active,CreatedDate,ModifiedDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CreatedDate = Convert.ToDateTime(TempData["crtDate"]);
                employee.ModifiedDate = DateTime.Now;
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpTypeID = new SelectList(db.EmployeeTypes, "Id", "EmpType", employee.EmpTypeId);
            ViewBag.DesignationID = new SelectList(db.Designations, "Id", "Name", employee.DesignationID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "Id", "DepartmentName", employee.DepartmentId);

            return View(employee);
        }


        [HttpPost]
        public JsonResult Delete(int? Id)
        {
            try
            {
                Employee employee = db.Employees.Find(Id);
                if (Id == null)
                {
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                }
                db.Employees.Remove(employee);
                db.SaveChanges();
                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(data: "Exists", behavior: JsonRequestBehavior.AllowGet);
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

