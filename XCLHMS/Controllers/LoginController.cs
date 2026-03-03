using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;
using System.Data.SqlClient;
using System.Data;
using XCLHMS.Helpers;


namespace XCLHMS.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(LoginViewModel model)
        {
            ModelState.Clear();
            return View(model);
        }



        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                String _userName = model.UserName;
                String _Password = model.Password;
                String _error = "";
                String _errorMsg = "";

                SqlConnection connection = new SqlConnection();
                DataSet dataSet = new DataSet();
                SqlCommand sqlCommand = new SqlCommand();
                DataTable dataTable = new DataTable();

                connection = DBase.GetConnection();
                sqlCommand = DBase.GetCommand("Employee_Passwordinfo", connection);
                sqlCommand.Parameters.AddWithValue("@userName", _userName);
                sqlCommand.Parameters.AddWithValue("@Password", _Password);

                dataSet = DBase.GetDataSetByCommand(sqlCommand, connection);
                //DataTable dt = dataSet.Tables[0];
                if (dataSet.Tables.Count > 0 && dataSet != null)
                {

                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        _error = dataSet.Tables[0].Rows[0]["error"].ToString();

                        if (_error == "0")
                        {
                            SessionHelper.EmployeeID = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Id"].ToString());
                            SessionHelper.UserName = dataSet.Tables[0].Rows[0]["Name"].ToString();
                            SessionHelper.Password = dataSet.Tables[0].Rows[0]["employeePassword"].ToString();
                            SessionHelper.IsSuperAdmin = Convert.ToInt32(dataSet.Tables[0].Rows[0]["superAdminCount"].ToString()) > 0 ? true : false;
                            return RedirectToAction("Index", "Home");

                        }
                        else
                        {
                            _errorMsg = dataSet.Tables[0].Rows[0]["errorMsg"].ToString();
                            ModelState.AddModelError("", _errorMsg);

                        }


                    }

                }
                else
                {

                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            
            return View(model);
        }
      

        public ActionResult LogOut()
        {

            SessionHelper.EmployeeID = 0;
            SessionHelper.UserName = "";
            SessionHelper.Password = "";
            SessionHelper.IsSuperAdmin = false;
            SessionHelper.CurrentMenus = null;
            Session["SideMenus"] = null;
            Session.Abandon();
            Session.Clear();

            return RedirectToAction("/Index");
        }


    }
}