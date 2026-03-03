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
    public partial class PateintReport : System.Web.UI.Page
    {
        HMSEntities db = new HMSEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            string searchText = string.Empty;
            if (Request.QueryString["MRNO"] != null)
            {
                searchText = Request.QueryString["MRNO"].ToString();

                var token = (from d in db.Pateints
                             join t in db.Tokens on d.Id equals t.PatientId
                             where d.MRNo == searchText
                             select new
                             {
                                 tokenNo = t.TokenNumber
                             }).FirstOrDefault();

                if (token == null)
                {
                    db.tokengenerate(searchText);
                }
            }         

            if (!IsPostBack)
            {
                ObjectResult<Report_pateint_Result> pateints = db.Report_pateint(searchText);
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Patient.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource ds = new ReportDataSource("DataSet1", pateints);
                ReportViewer1.LocalReport.DataSources.Add(ds);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();

            }
        }
    }
}