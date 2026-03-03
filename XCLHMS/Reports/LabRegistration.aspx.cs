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

    public partial class LabRegistration : System.Web.UI.Page
    {
        HMSEntities db = new HMSEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            string searchText = string.Empty;
            if (Request.QueryString["regno"] != null)
            {
                searchText = Request.QueryString["regno"].ToString();
            }
            if (!IsPostBack)
            {
                ObjectResult<GetLabRecordByRegNo_Result> acc = db.GetLabRecordByRegNo(searchText);
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/LabRegistration.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource ds = new ReportDataSource("DataSet1", acc);
                ReportViewer1.LocalReport.DataSources.Add(ds);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();

            }
        }
    }
}