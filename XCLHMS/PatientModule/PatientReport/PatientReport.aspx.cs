using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using XCLHMS.PatientModule.PatientReport.Data;

namespace XCLHMS.PatientModule.PatientReport
{
    public partial class PatientReport : System.Web.UI.Page
    {
        PatientReportDAL dal = new PatientReportDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set default dates to today's month
                txtStartDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                if (Request.QueryString["ChallanNo"] != null)
                {
                    string challanNo = Request.QueryString["ChallanNo"].ToString();
                    LoadReport(challanNo);
                }
                else
                {
                    // Optionally load default data
                    btnSearch_Click(null, null);
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime? start = string.IsNullOrEmpty(txtStartDate.Text) ? (DateTime?)null : DateTime.Parse(txtStartDate.Text);
            DateTime? end = string.IsNullOrEmpty(txtEndDate.Text) ? (DateTime?)null : DateTime.Parse(txtEndDate.Text);
            string type = ddlPatientType.SelectedValue;

            LoadReport(null, start, end, type);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = ReportViewer1.LocalReport.Render(
               "Excel", null, out mimeType, out encoding,
                out extension,
               out streamids, out warnings);

            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=PatientReport." + extension);
            Response.BinaryWrite(bytes);
            Response.End();
        }

        private void LoadReport(string challanNo = null, DateTime? startDate = null, DateTime? endDate = null, string patientType = null)
        {
            try
            {
                DataTable dt = dal.GetPatientReport(challanNo, startDate, endDate, patientType);

                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/PatientModule/PatientReport/Reports/PatientReport.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                
                ReportDataSource rds = new ReportDataSource("PatientReportDataset", dt);
                ReportViewer1.LocalReport.DataSources.Add(rds);
                
                ReportViewer1.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
                pnlError.Visible = true;
            }
        }
    }
}
