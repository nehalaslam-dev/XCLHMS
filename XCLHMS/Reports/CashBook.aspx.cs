using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCLHMS.Models;

namespace XCLHMS.Reports
{
    public partial class Cashbook : System.Web.UI.Page
    {
        HMSEntities db = new HMSEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHeadDDL();
                LoadAccountDDL();
            }
        }

        private void LoadHeadDDL()
        {
            var data = (from d in db.COAs
                        where d.ParentID != 0
                        select new
                        {
                            Id = d.ID,
                            Name = d.Name
                        }).ToList();

            

            ddlHead.DataSource = data;
            ddlHead.DataTextField = "Name";
            ddlHead.DataValueField = "Id";
            ddlHead.DataBind();

            ddlHead.Items.Insert(0, new ListItem("ALL","0"));

        }

        private void LoadAccountDDL()
        {
            var data = (from d in db.Vendors
                        select new
                        {
                            Id = d.Id,
                            Name = d.Name
                        }).ToList();

            ddlAccountName.DataSource = data;
            ddlAccountName.DataTextField = "Name";
            ddlAccountName.DataValueField = "Id";
            ddlAccountName.DataBind();

            ddlAccountName.Items.Insert(0, new ListItem("ALL","0"));

        }


        private void BindReport()
        {
            DateTime @sdate = DateTime.ParseExact(txtStart.Text, "MM/dd/yyyy", null);
            DateTime @edate = DateTime.ParseExact(txtEnd.Text, "MM/dd/yyyy", null);
            try
            {
                ObjectResult<Report_CashBook_Result> dt = db.Report_CashBook(@sdate, @edate, Convert.ToInt32(ddlHead.SelectedValue), Convert.ToInt32(ddlAccountName.SelectedValue), txtBill.Text, txtCheque.Text);
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/CashBook.rdlc");
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

            //SqlConnection conn = new SqlConnection(db);
            //try
            //{
            //    SqlCommand cmd = new SqlCommand("Report_CashBook",db);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@sdate",Convert.ToDateTime(txtStart.Text));
            //    cmd.Parameters.AddWithValue("@edate", Convert.ToDateTime(txtEnd.Text));
            //    cmd.Parameters.AddWithValue("@headId", Convert.ToInt32(ddlHead.SelectedValue));
            //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //    adapter.Fill(dt);

            //    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/CashBook.rdlc");
            //    ReportViewer1.LocalReport.DataSources.Clear();
            //    ReportDataSource ds = new ReportDataSource("DataSet1", dt);
            //    ReportViewer1.LocalReport.DataSources.Add(ds);
            //    ReportViewer1.LocalReport.Refresh();
            //    ReportViewer1.DataBind();

            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}