using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;

namespace XCLHMS.Helpers
{
    public class SessionHelper
    {
        private const string _SideMenus = "SideMenus";
        private const string _EmployeeID = "EmployeeID";
        private const string _AuthToken = "AuthToken";
        private const string _CompanyID = "CompanyID";
        private const string _DbName = "DbName";
        private const string _UserName = "UserName";
        private const string _UserID = "UserID";
        private const string _employeePic = "employeePic";
        private const string _LandingURL = "LandingURL";
        private const string _IsSuperAdmin = "IsSuperAdmin";
        private const string _Password = "Password";


        public static string Encrypt(string toEncode)
        {
            var toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            var returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        //public static String GetPageSecurity()
        //{
        //    //String _pagePath = HttpContext.Current.Request.Url.AbsolutePath;
        //    //String _pagePath1 = HttpContext.Current.Request.Url.Query;
        //    //String _pagePath2 = HttpContext.Current.Request.Url.PathAndQuery;
        //    //String _pagePath3 = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);

        //    String _retValAdd = "";
        //    String _retValEdit = "";
        //    String _retValDelete = "";

        //    String _retvalue = "";

        //    String _controllerName = "";
        //    String _actionName = "";
        //    String _queryString = "";
        //    var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

        //    if (routeValues.ContainsKey("controller"))
        //    {

        //        _controllerName = routeValues["controller"].ToString();
        //    }

        //    if (routeValues.ContainsKey("action"))
        //    {

        //        _actionName = routeValues["action"].ToString();
        //    }
        //    if (routeValues.ContainsKey("id"))
        //    {

        //        _queryString = routeValues["id"].ToString();
        //    }

        //    SqlConnection connection = new SqlConnection();
        //    DataSet dataSet = new DataSet();
        //    SqlCommand sqlCommand = new SqlCommand();
        //    connection = DBase.GetConnection();
        //    sqlCommand = DBase.GetCommand("PageSecurity_SelectByPageName", connection);
        //    sqlCommand.Parameters.AddWithValue("@employeeID", SessionHelper.EmployeeID);
        //    sqlCommand.Parameters.AddWithValue("@pagePath", _controllerName);
        //    dataSet = DBase.GetDataSetByCommand(sqlCommand, connection);
        //    if (dataSet != null && dataSet.Tables.Count > 0)
        //    {

        //        DataTable dt = dataSet.Tables[0];


        //        _retValAdd = dataSet.Tables[0].Rows[0]["IsAdd"].ToString() == "True" ? "1" : "0";
        //        _retValEdit = dataSet.Tables[0].Rows[0]["IsEdit"].ToString() == "True" ? "1" : "0";
        //        _retValDelete = dataSet.Tables[0].Rows[0]["IsDelete"].ToString() == "True" ? "1" : "0";
        //        _retvalue = _retValAdd + "|" + _retValEdit + "|" + _retValDelete;

        //    }
        //    else
        //    {

        //        _retvalue = "1|1|1";

        //    }


        //    return _retvalue;
        //}

        public static String GetPageSecurity()
        {
            String _pagePath = HttpContext.Current.Request.Url.AbsolutePath;

            String _retValAdd = "";
            String _retValEdit = "";
            String _retValDelete = "";

            String _retvalue = "";

            String _controllerName = "";
            String _actionName = "";
            String _queryString = "";
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            var _areaName = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"];


            if (routeValues.ContainsKey("controller"))
            {

                _controllerName = routeValues["controller"].ToString();
            }

            if (routeValues.ContainsKey("action"))
            {

                _actionName = routeValues["action"].ToString();
            }
            if (routeValues.ContainsKey("id"))
            {

                _queryString = routeValues["id"].ToString();
            }



            if (routeValues.ContainsKey("area"))
            {

                _areaName = routeValues["area"].ToString();
            }

            _pagePath = "/" + _areaName + "/" + _controllerName + "/" + _actionName;

            SqlConnection connection = new SqlConnection();
            DataSet dataSet = new DataSet();
            SqlCommand sqlCommand = new SqlCommand();
            connection = DBase.GetConnection();
            sqlCommand = DBase.GetCommand("PageSecurity_SelectByPageName", connection);
            sqlCommand.Parameters.AddWithValue("@employeeID", SessionHelper.EmployeeID);
            sqlCommand.Parameters.AddWithValue("@pagePath", _pagePath);
            dataSet = DBase.GetDataSetByCommand(sqlCommand, connection);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {

                DataTable dt = dataSet.Tables[0];


                _retValAdd = dataSet.Tables[0].Rows[0]["IsAdd"].ToString() == "True" ? "1" : "0";
                _retValEdit = dataSet.Tables[0].Rows[0]["IsEdit"].ToString() == "True" ? "1" : "0";
                _retValDelete = dataSet.Tables[0].Rows[0]["IsDelete"].ToString() == "True" ? "1" : "0";
                _retvalue = _retValAdd + "|" + _retValEdit + "|" + _retValDelete;



            }
            else
            {

                _retvalue = "1|1|1";

            }


            return _retvalue;
        }
        public static string Decrypt(string encodeData)
        {

            var encodeDataAsBytes = System.Convert.FromBase64String(encodeData);
            var returnValue = Encoding.ASCII.GetString(encodeDataAsBytes);
            return returnValue;
        }
        public static int EmployeeID
        {
            get
            {
                if (HttpContext.Current.Session[_EmployeeID] == null)
                {
                    return 0;
                }
                return ((int)HttpContext.Current.Session[_EmployeeID]);
            }
            set { HttpContext.Current.Session[_EmployeeID] = value; }
        }
        public static IEnumerable<MenuModel> CurrentMenus
        {
            get
            {
                if (HttpContext.Current.Session[_SideMenus] == null)
                {


                    CurrentMenus = GetSideMenu();


                }

                return (IEnumerable<MenuModel>)HttpContext.Current.Session[_SideMenus];
            }
            set { HttpContext.Current.Session[_SideMenus] = value; }
        }


        public static int CompanyID
        {
            get
            {
                if (HttpContext.Current.Session[_CompanyID] == null)
                {
                    return 0;
                }
                return ((int)HttpContext.Current.Session[_CompanyID]);
            }
            set { HttpContext.Current.Session[_CompanyID] = value; }
        }
        public static string DbName
        {
            get
            {
                if (HttpContext.Current.Session[_DbName] == null)
                {
                    return null;
                }
                return ((string)HttpContext.Current.Session[_DbName]);
            }
            set { HttpContext.Current.Session[_DbName] = value; }
        }
        public static string UserName
        {
            get
            {
                if (HttpContext.Current.Session[_UserName] == null)
                {
                    return null;
                }
                return ((string)HttpContext.Current.Session[_UserName]);
            }
            set { HttpContext.Current.Session[_UserName] = value; }
        }

        public static string Password
        {
            get
            {
                if (HttpContext.Current.Session[_Password] == null)
                {
                    return null;
                }
                return ((string)HttpContext.Current.Session[Password]);
            }
            set { HttpContext.Current.Session[Password] = value; }
        }
        public static string EmployeePic
        {
            get
            {
                if (HttpContext.Current.Session[_employeePic] == null)
                {
                    return null;
                }
                return ((string)HttpContext.Current.Session[_employeePic]);
            }
            set { HttpContext.Current.Session[_employeePic] = value; }
        }
        public static string LandingURL
        {
            get
            {
                if (HttpContext.Current.Session[_LandingURL] == null)
                {
                    return null;
                }
                return ((string)HttpContext.Current.Session[_LandingURL]);
            }
            set { HttpContext.Current.Session[_LandingURL] = value; }
        }
        public static bool IsSuperAdmin
        {
            get
            {
                if (HttpContext.Current.Session[_IsSuperAdmin] == null)
                {
                    return false;
                }
                return ((bool)HttpContext.Current.Session[_IsSuperAdmin]);
            }
            set { HttpContext.Current.Session[_IsSuperAdmin] = value; }
        }


        public static IEnumerable<MenuModel> GetSideMenu()
        {
            int empID = SessionHelper.EmployeeID;
            List<MenuModel> menuList = new List<MenuModel>();

            if (SessionHelper.EmployeeID == 0)
            {
                HttpContext.Current.Response.Redirect("/Login/LogOut");

            }
            else
            {

                SqlConnection connection = new SqlConnection();
                DataSet dataSet = new DataSet();
                SqlCommand sqlCommand = new SqlCommand();
                DataTable dataTable = new DataTable();

                connection = DBase.GetConnection();
                sqlCommand = DBase.GetCommand("generateMenu", connection);
                sqlCommand.Parameters.AddWithValue("@empID", empID);

                dataSet = DBase.GetDataSetByCommand(sqlCommand, connection);
                DataTable dt = dataSet.Tables[0];

                using (System.Data.DataTable data = dt)
                {
                    var root = data.Select(string.Format("menuParentID=0 OR menuParentID IS NULL"));

                    foreach (var item in root)
                    {
                        MenuModel menu = new MenuModel()
                        {
                            menuId = item["menuID"] == DBNull.Value ? 0 : Convert.ToInt32(item["menuID"]),
                            Name = item["menuName"] == DBNull.Value ? "" : Convert.ToString(item["menuName"] ?? ""),
                            ParentId = item["menuParentID"] == DBNull.Value ? 0 : Convert.ToInt32(item["menuParentID"] ?? 0),
                            Path = item["applicationPath"] == DBNull.Value ? "" : Convert.ToString(item["applicationPath"] ?? ""),
                            ChildMenu = new List<MenuModel>()
                        };

                        foreach (var childItem in data.Select(string.Format("menuParentID={0}", menu.menuId)) ?? Enumerable.Empty<System.Data.DataRow>())
                        {
                            MenuModel childMenu = new MenuModel()
                            {
                                menuId = childItem["menuID"] == DBNull.Value ? 0 : Convert.ToInt32(childItem["menuID"]),
                                Name = childItem["menuName"] == DBNull.Value ? "" : Convert.ToString(childItem["menuName"] ?? ""),
                                ParentId = childItem["menuParentID"] == DBNull.Value ? 0 : Convert.ToInt32(childItem["menuParentID"] ?? 0),
                                Path = childItem["applicationPath"] == DBNull.Value ? "" : Convert.ToString(childItem["applicationPath"] ?? ""),


                                ChildMenu = new List<MenuModel>()
                            };

                            foreach (var childSubItem in data.Select(string.Format("menuParentID={0}", childMenu.menuId)) ?? Enumerable.Empty<System.Data.DataRow>())
                            {
                                MenuModel childSubMenu = new MenuModel()
                                {
                                    menuId = childSubItem["menuID"] == DBNull.Value ? 0 : Convert.ToInt32(childSubItem["menuID"]),
                                    Name = childSubItem["menuName"] == DBNull.Value ? "" : Convert.ToString(childSubItem["menuName"] ?? ""),
                                    ParentId = childSubItem["menuParentID"] == DBNull.Value ? 0 : Convert.ToInt32(childSubItem["menuParentID"] ?? 0),
                                    Path = childSubItem["applicationPath"] == DBNull.Value ? "" : Convert.ToString(childSubItem["applicationPath"] ?? ""),
                                    ChildMenu = new List<MenuModel>()
                                };
                                childMenu.ChildMenu.Add(childSubMenu);
                            }

                            menu.ChildMenu.Add(childMenu);
                        }
                        menuList.Add(menu);
                    }
                }
            }
            return menuList;
        }
    }







}