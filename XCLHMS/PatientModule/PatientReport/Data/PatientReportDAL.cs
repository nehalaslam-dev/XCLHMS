using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace XCLHMS.PatientModule.PatientReport.Data
{
    public class PatientReportDAL
    {
        string connString = ConfigurationManager.ConnectionStrings["HMSDB"].ConnectionString;

        public DataTable GetPatientReport(string challanNo = null, DateTime? startDate = null, DateTime? endDate = null, string patientType = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetPatientReport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChallanNo", (object)challanNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object)endDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PatientType", (object)patientType ?? DBNull.Value);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}
