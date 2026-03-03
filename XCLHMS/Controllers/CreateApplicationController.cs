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
    public class CreateApplicationController : Controller
    {
        private HMSEntities db = new HMSEntities();
        String _currentUrl = String.Empty;
   
        public ActionResult ManageCreateApplication(string id)
        {
            int ids = 0;
            
            int.TryParse(id, out ids);

            if(Convert.ToInt32(ids) > 0)
            { 
            var queryLondonCustomers = from menuLst in db.Menu_List
                                       where menuLst.menuID == ids
                                       select menuLst;

             var _menuParentID = queryLondonCustomers.ToList()[0].menuParentID;
            ViewBag.currentUrl  =_menuParentID;
            }
            else
            {
                 ViewBag.currentUrl  ="0";
            }

            return View(MenuList(ids));
        }

        public IEnumerable<XCLHMS.Models.Menu_ListGetByParentID_Result> MenuList(int parentid)
        {
            return db.Menu_ListGetByParentID(parentid).ToList();
        }



        [HttpPost]
        public ActionResult AddEditApplicationSave(CreateApplicationModel mod) 
        {
            Menu_List objMenu;
            int ?_hdnmenuId = 0;
            int? _hdnmenuParentId = 0;

            //string _text = Session["parentid"].ToString() ;// app.menuParentID.ToString();

            if (Session["parentid"]!=null)
            {
                _hdnmenuParentId = Convert.ToInt32(Session["parentid"].ToString());

            }
          
            if (Session["menuid"]!=null)
            {
                _hdnmenuId = Convert.ToInt32(Session["menuid"].ToString());
            }

            if (ModelState.IsValid)
            {
                //_hdnmenuId = _hdnmenuId;

                if (_hdnmenuId > 0)
                {
                    objMenu = db.Menu_List.Where(o => o.menuID == _hdnmenuId).SingleOrDefault();
                }
                else
                {
                    objMenu = new Menu_List();
                    objMenu.menuParentID = _hdnmenuParentId;//Convert.ToInt32(mod.menuParentID);
                }

                objMenu.menuName = mod.menuName.ToString();// frm["menuName"].ToString();
                objMenu.applicationPath = mod.applicationPath.ToString();// frm["applicationPath"].ToString();

                if (_hdnmenuId > 0)
                {

                    db.Entry(objMenu).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.Menu_List.Add(objMenu);
                    db.SaveChanges();
                    MenuAppCompanyMapp objMap = new MenuAppCompanyMapp();
                    objMap.menuID = objMenu.menuID;
                    db.MenuAppCompanyMapps.Add(objMap);
                    db.SaveChanges();
                    MenuApplicationPrivacy objPrivacy = new MenuApplicationPrivacy();
                    objPrivacy.menuID = objMenu.menuID;
                    objPrivacy.ViewToSuperAdmin = false;
                    objPrivacy.AllowAccess = true;
                    objPrivacy.AllowAssignment = true;
                    db.MenuApplicationPrivacies.Add(objPrivacy);
                    db.SaveChanges();
                }

                db.SaveChanges();




            }
            return RedirectToAction("ManageCreateApplication");


        }

        [HttpGet]
        public ActionResult AddEditApplication(String id, String parentid)
        {
            _currentUrl = Request.Url.ToString().ToLower();
            ViewBag.currentUrl = _currentUrl;
            int _id = 0;
            int _parentid = 0;
            CreateApplicationModel mod = new CreateApplicationModel();

              int ids = 0;
            int.TryParse(id, out ids);

            if (!String.IsNullOrEmpty(parentid) && (parentid != null) && (parentid != "null"))
            {
                Session["parentid"] = parentid.ToString();
                Session["menuid"] = null;
            }
            else
            {
                Session["parentid"] = null;
                Session["menuid"] = id.ToString();

            }
            

            
            Menu_List objMenu;
            int _hdMenuId = Convert.ToInt32(id);
            //if (_hdMenuId > 0)
            if (!String.IsNullOrWhiteSpace(id) && (id != null) && (id != "null"))
            {

                objMenu = db.Menu_List.Where(o => o.menuID == _hdMenuId).SingleOrDefault();
                mod.menuName = objMenu.menuName;
                mod.applicationPath = objMenu.applicationPath;

                mod.menuParentID = objMenu.menuParentID; // Convert.ToInt32(_parentid);

            }
            else
            {
                objMenu = new Menu_List();
                mod.menuName = objMenu.menuName;
                mod.applicationPath = objMenu.applicationPath;
                //mod.menuParentID = Convert.ToInt32(_parentid);
                mod.menuParentID = objMenu.menuParentID; // Convert.ToInt32(_parentid);

            }
            return View(mod);


        }

    }
}
