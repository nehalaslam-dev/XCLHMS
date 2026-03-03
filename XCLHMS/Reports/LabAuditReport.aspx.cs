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
    public partial class LabAuditReport : System.Web.UI.Page
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
            var data = (from d in db.Employees
                        where d.DesignationID == 13
                        select new
                        {
                            Id = d.Id,
                            Name = d.Name
                        }).ToList();

            ddlOperator.DataSource = data;
            ddlOperator.DataTextField = "Name";
            ddlOperator.DataValueField = "Id";
            ddlOperator.DataBind();
        }

        private void BindReport()
        {
            DateTime @sdate = DateTime.ParseExact(txtStart.Text, "MM/dd/yyyy", null);
            DateTime @edate = DateTime.ParseExact(txtEnd.Text, "MM/dd/yyyy", null);
            try
            {
                ObjectResult<GetOperatorDetailById_Result> dt = db.GetOperatorDetailById(Convert.ToInt32(ddlOperator.SelectedValue), @sdate, @edate);
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Report_Operator.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource ds = new ReportDataSource("DataSet1", dt);
                ReportViewer1.LocalReport.DataSources.Add(ds);
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