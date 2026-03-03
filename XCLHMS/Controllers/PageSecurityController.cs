using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XCLHMS.Models;
using System.Web.Script.Serialization;

namespace XCLHMS.Controllers
{
    public class PageSecurityController : Controller
    {
        //
        // GET: /Rate/
        private HMSEntities db = new HMSEntities();
        //onlineinternetposEntities db = new onlineinternetposEntities();
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ManagePageSecurity(FormCollection fc)
        {

            String isAdd = fc["isAdd"];
            String isEdit = fc["isEdit"];
            String isDelete = fc["isDelete"];
            Int32 selectedDesigOREmpTypeid = Convert.ToInt32(fc["selectedDesigOREmpTypeid"]);
            String selectedDesigOREmpType = fc["selectedDesigOREmpType"];
            Int32 selectedMenuid = Convert.ToInt32(fc["selectedMenuid"]);
            String selectedMenuText = fc["selectedMenuText"];
            
            bool _isAddvalue;
            bool _isEditvalue;
            bool _isDeletevalue;
            
            

            Int32 _employeeID = 0;
            Int32 _designationID = 0;


            if (isAdd == "true,false")
            {
                isAdd = "1";
                _isAddvalue= true;
            }
            else
            {
                isAdd = "0";
                _isAddvalue= false;
            }

            if (isEdit == "true,false")
            {
                isEdit = "1";
                _isEditvalue = true;
            }
            else
            {
                isEdit = "0";
                _isEditvalue = false;
            }

            if (isDelete == "true,false")
            {
                _isDeletevalue = true;
                isDelete = "1";
            }
            else
            {
                _isDeletevalue = false;
                isDelete = "0";
            }


            if (selectedDesigOREmpType.ToLower() == "employee")
            {
                _employeeID = Convert.ToInt32(selectedDesigOREmpTypeid);
                _designationID = 0;
            }
            else
            {
                _employeeID = 0;
                _designationID = Convert.ToInt32(selectedDesigOREmpTypeid);
            }

            int _retval = db.PageSecurity_Insert(selectedMenuid, selectedMenuText, _isAddvalue, _isEditvalue, _isDeletevalue, _employeeID, _designationID);
            return RedirectToAction("ManagePageSecurity");
        }

        [HttpGet]
        public ActionResult ManagePageSecurity(string seltype, string selName)
        {
            return View("");  

        }


        [HttpPost]
        public String GetSelectedAddEditDeleteValues(string pageID, string employeeID, string designationID)
        {
            String _defaultvalue = "isAdd,0|isEdit,0|isDelete,0";
            String _retVal = "";
            var _retValule = db.PageSecurity_Select(Convert.ToInt32(pageID), Convert.ToInt32(employeeID), Convert.ToInt32(designationID)).FirstOrDefault();
            
            if (_retValule!=null && _retValule.Length > 0)
            {
                _retVal = _retValule.ToString();
                
            }
            else
            {
                _retVal = _defaultvalue;
            }
           
            return _retVal;
        }

        [HttpPost]
        public JsonResult Type_Selected(string id)
        {

            IEnumerable<XCLHMS.Models.GetDesignationAndEmpIdsForAssignApplication_Result> rateLst = db.GetDesignationAndEmpIdsForAssignApplication(id);
            JsonResult result = new JsonResult();
            result.Data = rateLst;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }



        [HttpPost]
        public JsonResult GetAllApplications(string id)
        {
            //IEnumerable<XCLHMS.Models.GetAll_Menu_List_Result> menuLst = db.GetAll_Menu_List();//RateList(id);
            IEnumerable<GetAll_Menu_List_OnlyChild_Result> menuLst = db.GetAll_Menu_List_OnlyChild();//RateList(id);
            JsonResult result = new JsonResult();
            result.Data = menuLst;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }




    }
}