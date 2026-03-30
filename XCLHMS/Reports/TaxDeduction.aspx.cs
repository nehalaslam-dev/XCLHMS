using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCLHMS.Models;

namespace XCLHMS.Reports
{
    public partial class TaxDeduction : System.Web.UI.Page
    {
        HMSEntities db = new HMSEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHeadDDL();
                LoadVendorDDL();
                txtStart.Text = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                txtEnd.Text = DateTime.Now.ToString("MM/dd/yyyy");
            }
        }

        private void LoadHeadDDL()
        {
            var data = db.COAs.Where(x => x.ParentID != 0).Select(x => new { x.ID, x.Name }).ToList();
            ddlHead.DataSource = data;
            ddlHead.DataTextField = "Name";
            ddlHead.DataValueField = "ID";
            ddlHead.DataBind();
            ddlHead.Items.Insert(0, new ListItem("ALL", "0"));
        }

        private void LoadVendorDDL()
        {
            var data = db.Vendors.Select(x => new { x.Id, x.Name }).ToList();
            ddlVendor.DataSource = data;
            ddlVendor.DataTextField = "Name";
            ddlVendor.DataValueField = "Id";
            ddlVendor.DataBind();
            ddlVendor.Items.Insert(0, new ListItem("ALL", "0"));
        }

        private void BindReport()
        {
            try
            {
                DateTime sdate = DateTime.ParseExact(txtStart.Text, "MM/dd/yyyy", null);
                DateTime edate = DateTime.ParseExact(txtEnd.Text, "MM/dd/yyyy", null);
                int headId = Convert.ToInt32(ddlHead.SelectedValue);
                int vendorId = Convert.ToInt32(ddlVendor.SelectedValue);
                string billNo = txtBillNo.Text;

                var parameters = new List<object>();
                parameters.Add(sdate);
                parameters.Add(edate);

                string sql = @"
                    SELECT 
                        v.PaymentSection, v.NTN, v.CNIC, v.Name as VendorName, v.Code as City, v.Address, v.Status as TaxStatus,
                        a.GrossAmount as TaxableAmount, a.IncomeTax, a.SST as SRB, a.StampDuty, a.NetAmount,
                        c.Code as ObjectHead, a.BillNo, ch.ChequeNo, ch.Date as ChequeDate
                    FROM AccountRegister a
                    JOIN Vendors v ON a.VendorId = v.Id
                    JOIN COA c ON a.HeadId = c.ID
                    LEFT JOIN ChequeRegister ch ON a.BillNo = ch.BillNo
                    WHERE a.billdate >= {0} AND a.billdate <= {1}";
                
                int paramIdx = 2;
                if (headId > 0) {
                    sql += " AND a.HeadId = {" + paramIdx + "}";
                    parameters.Add(headId);
                    paramIdx++;
                }
                if (vendorId > 0) {
                    sql += " AND a.VendorId = {" + paramIdx + "}";
                    parameters.Add(vendorId);
                    paramIdx++;
                }
                if (!string.IsNullOrEmpty(billNo)) {
                    sql += " AND a.BillNo LIKE {" + paramIdx + "}";
                    parameters.Add("%" + billNo + "%");
                    paramIdx++;
                }

                sql += " ORDER BY a.billdate DESC";

                var data = db.Database.SqlQuery<TaxSheetDTO>(sql, parameters.ToArray()).ToList();

                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/TaxDeduction.rdlc");
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource ds = new ReportDataSource("DataSet1", data);
                ReportViewer1.LocalReport.DataSources.Add(ds);
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }

        public class TaxSheetDTO {
            public string PaymentSection { get; set; }
            public string NTN { get; set; }
            public string CNIC { get; set; }
            public string VendorName { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public string TaxStatus { get; set; }
            public decimal? TaxableAmount { get; set; }
            public decimal? IncomeTax { get; set; }
            public decimal? SRB { get; set; }
            public decimal? StampDuty { get; set; }
            public decimal? NetAmount { get; set; }
            public string ObjectHead { get; set; }
            public string BillNo { get; set; }
            public string ChequeNo { get; set; }
            public DateTime? ChequeDate { get; set; }
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            BindReport();
        }
    }
}