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
    public partial class ReportWardBeds : System.Web.UI.Page
    {
        HMSEntities db = new HMSEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDDL();
            }
        }

        private void LoadDDL()
        {
            var data = (from d in db.Wards                        
                        select new
                        {
                            Id = d.Id,
                            Name = d.WardName
                        }).ToList();

            ddlWards.DataSource = data;
            ddlWards.DataTextField = "Name";
            ddlWards.DataValueField = "Id";
            ddlWards.DataBind();
        }

        private void BindReport()
        {            
            try
            {
                //ObjectResult<Report_BedAllotment_Result> dt = db.Report_BedAllotment(Convert.ToInt32(ddlWards.SelectedValue));
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/rptBedAllotment.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                //ReportDataSource ds = new ReportDataSource("DataSet1", dt);
                //ReportViewer1.LocalReport.DataSources.Add(ds);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReport();
        }
    }
}