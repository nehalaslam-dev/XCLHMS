using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCLHMS.Helpers;
using XCLHMS.Models;

namespace XCLHMS.Reports
{
    public partial class TestResult : System.Web.UI.Page
    {
        HMSEntities db = new HMSEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            string searchText = string.Empty;
            if (Request.QueryString["Id"] != null)
            {
                searchText = Request.QueryString["Id"].ToString();
            }
            if (!IsPostBack)
            {
                //SessionHelper.UserName;
                ObjectResult<Report_GetTestResult_Result> grns = db.Report_GetTestResult(Convert.ToInt32(searchText), SessionHelper.UserName);
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/TestResult.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource ds = new ReportDataSource("DataSet1", grns);
                ReportViewer1.LocalReport.DataSources.Add(ds);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();

            }
        }
    }
}