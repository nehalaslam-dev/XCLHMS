using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCLHMS.Models;

namespace XCLHMS.Reports
{
    public partial class IssuanceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFrom.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd");

                // optional: run default report
                // BindReport();
            }
        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        private void BindReport()
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(txtFrom.Text, out fromDate) || !DateTime.TryParse(txtTo.Text, out toDate))
            {
                // show message to user
                return;
            }

            // Ensure inclusive toDate (end of day)
            //toDate = toDate.Date.AddDays(1).AddTicks(-1);

            // Fetch data from EF (or use stored proc)
            List<IssuanceViewModel> data = GetIssuanceData(fromDate, toDate);
            if (data.Count == 0)
            {
                // Show blank report instead of throwing error
                ReportViewer1.Visible = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No data found for the selected dates!');", true);
                return;
            }
            else
            {
                ReportViewer1.Visible = true;
            }

            // Prepare local report
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/IssuanceDateWise.rdlc"); // put rdlc in ~/Reports
            var rds = new ReportDataSource("IssuanceDS", data); // IssuanceDS must match RDLC dataset name
            ReportViewer1.LocalReport.DataSources.Add(rds);

            string headVal = data.Select(x => x.Head).Where(s => !string.IsNullOrEmpty(s)).Distinct().FirstOrDefault() ?? "N/A";

            // ✅ Build date range string
            string dateRange = $"{fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}";

            // Parameters
            var p1 = new ReportParameter("FromDate", fromDate.ToString("dd/MM/yyyy"));
            var p2 = new ReportParameter("ToDate", toDate.ToString("dd/MM/yyyy"));
            var pHead = new ReportParameter("ReportHead", headVal);
            var pDateRange = new ReportParameter("ReportDateRange", dateRange);
            ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });

            ReportViewer1.LocalReport.Refresh();
        }

        // Example viewmodel - simple DTO to match report fields
        private List<IssuanceViewModel> GetIssuanceData(DateTime fromDate, DateTime toDate)
        {
            using (var db = new HMSEntities())
            {
                System.Diagnostics.Debug.WriteLine($"Date range: {fromDate} to {toDate}");

                // Normalize dates
                DateTime fromOnly = fromDate.Date;
                DateTime toOnly = toDate.Date.AddDays(1); // keep as next day (exclusive)
                                                          // ✅ next day (no AddTicks)

                // Fetch directly from DB with filtering
                var data = (from d in db.Issuances
                            join p in db.Products on d.ProductId equals p.Id into productGroup
                            from p in productGroup.DefaultIfEmpty()
                            where d.Date != null &&
      System.Data.Entity.DbFunctions.TruncateTime(d.Date) >= fromOnly &&
      System.Data.Entity.DbFunctions.TruncateTime(d.Date) <= toDate

                            select new IssuanceViewModel
                            {
                                SNO = d.SNO,
                                Head = d.Head,
                                Qty = d.Qty,
                                Date = d.Date,
                                ProductName = p != null ? p.Name : ""
                            }).ToList();

                System.Diagnostics.Debug.WriteLine($"Fetched {data.Count} records from DB between {fromOnly:d} and {toOnly.AddDays(-1):d}");
                return data;
            }
        }

        public class IssuanceViewModel
        {
            public int SNO { get; set; }
            public string Head { get; set; }
            public string ProductName { get; set; }
            public decimal? Qty { get; set; }
            public DateTime? Date { get; set; }
        }
    }
}