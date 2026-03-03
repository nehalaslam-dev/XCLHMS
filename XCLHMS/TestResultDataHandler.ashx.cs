using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using XCLHMS.Models;

namespace XCLHMS
{
    /// <summary>
    /// Summary description for TestResultDataHandler
    /// </summary>
    public class TestResultDataHandler : IHttpHandler
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

            if (context.Request["sSortDir_0"] == null)
            {
                sortDir = "desc";
            }
            else
            {
                sortDir = "desc";
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
            List<TestResult> lstTestResult = new List<TestResult>();
            int filterCount = 0;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetTestResult", conn);
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
                    TestResult testResult = new TestResult();
                    testResult.Id = Convert.ToInt32(rdr["Id"]);
                    filterCount = Convert.ToInt32(rdr["TotalCount"]);
                    //testResult.labname = rdr["labname"].ToString();
                    //testResult.MRNo = rdr["MRNO"].ToString();
                    //testResult.pateintname = rdr["pateintname"].ToString();
                    //testResult.categoryname = rdr["categoryname"].ToString(); //Convert.ToInt32(rdr["patientTypeId"]);
                    testResult.regNo = rdr["RegNo"].ToString();
                    //testResult.NIC = rdr["NIC"].ToString();
                    testResult.TestDate = Convert.ToDateTime(rdr["TestDate"]);
                    testResult.DeliveryDate = Convert.ToDateTime(rdr["DeliveryDate"]);
                    lstTestResult.Add(testResult);

                }
            }

            var result = new
            {
                iTotalRecords = GetTestResultTotalCount(),
                iTotalDisplayRecords = filterCount,
                aaData = lstTestResult
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            context.Response.Write(js.Serialize(result));
        }

        private int GetTestResultTotalCount()
        {
            int totalResultCount = 0;
            string cs = ConfigurationManager.ConnectionStrings["HMSDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select count(*) from testresult", conn);
                conn.Open();
                totalResultCount = (int)cmd.ExecuteScalar();
            }
            return totalResultCount;
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