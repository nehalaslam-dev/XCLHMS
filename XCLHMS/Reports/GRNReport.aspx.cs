using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCLHMS.Models;

namespace XCLHMS.Reports
{
    public partial class GRNReport : System.Web.UI.Page
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
                ObjectResult<GetGRNDetail_Result> grns = db.GetGRNDetail(Convert.ToInt32(searchText));// db.GetGRNDetail(Convert.ToInt32(searchText));
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/GRNReport.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource ds = new ReportDataSource("DataSet1", grns);
                ReportViewer1.LocalReport.DataSources.Add(ds);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();

            }
        }
    }
}