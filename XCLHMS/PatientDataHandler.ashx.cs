using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using XCLHMS.Models;

namespace XCLHMS
{
    /// <summary>
    /// Summary description for PatientDataHandler
    /// </summary>
    public class PatientDataHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int displayLength;
            int displayStart;
            int sortCol;
            string sortDir;
            string search;
            if (context.Request["iDisplayLenght"] == null)
            {
                displayLength = 10;
            }
            else
            {
                displayLength = int.Parse(context.Request["iDisplayLenght"]);
            }

            if (context.Request["iDisplayStart"] == null)
            {
                displayStart = 0;
            }
            else
            {
                displayStart = int.Parse(context.Request["iDisplayStart"]);
            }

            if (context.Request["iSortCol_0"] == null)
            {
                sortCol = 0;
            }
            else
            {
                sortCol = int.Parse(context.Request["iSortCol_0"]);
            }

            if (context.Request["sSortDir_0"]==null)
            {
                sortDir = "desc";
            }
            else
            {
                sortDir = "desc";//context.Request["sSortDir_0"];
            }

            if (context.Request["sSearch"] == null)
            {
                search = string.Empty;
            }
            else
            {
                search = context.Request["sSearch"];
            }

            string cs = ConfigurationManager.ConnectionStrings["HMSDB"].ConnectionString;
            List<Pateint> lstPatients = new List<Pateint>();
            int filterCount = 0;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetPatient", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramDisplayLength = new SqlParameter()
                {
                    ParameterName = "@DisplayLength",
                    Value = displayLength
                };
                cmd.Parameters.Add(paramDisplayLength);

                SqlParameter paramDisplayStart = new SqlParameter()
                {
                    ParameterName = "@DisplayStart",
                    Value = displayStart
                };
                cmd.Parameters.Add(paramDisplayStart);

                SqlParameter paramSortCol = new SqlParameter()
                {
                    ParameterName = "@SortCol",
                    Value = sortCol
                };
                cmd.Parameters.Add(paramSortCol);

                SqlParameter paramSortDir = new SqlParameter()
                {
                    ParameterName = "@SortDir",
                    Value = sortDir
                };
                cmd.Parameters.Add(paramSortDir);

                SqlParameter paramSearch = new SqlParameter()
                {
                    ParameterName = "@Search",
                    Value = string.IsNullOrEmpty(search) ? null : search
                };
                cmd.Parameters.Add(paramSearch);

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Pateint pateint = new Pateint();
                    pateint.Id = Convert.ToInt32(rdr["Id"]);
                    filterCount = Convert.ToInt32(rdr["TotalCount"]);
                    pateint.NIC = rdr["NIC"].ToString();
                    pateint.MRNo = rdr["MRNO"].ToString();
                    pateint.Name = rdr["patientName"].ToString();
                    //pateint.Type = rdr["patientType"].ToString(); //Convert.ToInt32(rdr["patientTypeId"]);
                    pateint.Gender = rdr["Gender"].ToString();
                    pateint.Age = rdr["Age"].ToString();
                    pateint.BloodGroup = rdr["BloodGroup"].ToString();
                    pateint.ContactNo = rdr["ContactNo"].ToString();
                    pateint.AdmitDate = Convert.ToDateTime(rdr["AdmitDate"]);
                    lstPatients.Add(pateint);
                }
            } 

            var result = new
            {
                iTotalRecords = GetPatientTotalCount(),
                iTotalDisplayRecords = filterCount,
                aaData = lstPatients
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            context.Response.Write(js.Serialize(result));
        }

        private int GetPatientTotalCount()
        {
            int totalPatientCount = 0;
            string cs = ConfigurationManager.ConnectionStrings["HMSDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select count(*) from pateint", conn);
                conn.Open();
                totalPatientCount = (int)cmd.ExecuteScalar();
            }
            return totalPatientCount;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}