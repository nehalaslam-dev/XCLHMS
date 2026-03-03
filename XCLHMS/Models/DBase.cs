using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace XCLHMS.Models
{
    public class DBase
    {
        public static SqlParameterCollection GetParameterCollectionConstructor()
        {
            return (SqlParameterCollection)typeof(SqlParameterCollection).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);
        }

        static SqlConnection _connection = new SqlConnection();

        //public static SqlConnection GetConnection(string DBName)
        public static SqlConnection GetConnection(string DBName)
        {
            if (string.IsNullOrEmpty(DBName))
            {
                HttpContext.Current.Response.Redirect("/Login/Index", true);
            }

            string _connectringString = ConfigurationManager.ConnectionStrings[DBName].ConnectionString;
            _connection = new SqlConnection(_connectringString);
            _connection.Open();
            return _connection;
        }

        public static SqlConnection GetConnection()
        {
            string _connectringString = ConfigurationManager.ConnectionStrings["HMSDB"].ConnectionString;
            _connection = new SqlConnection(_connectringString);
            _connection.Open();
            return _connection;
        }
        public static SqlDataReader ExecuteDataset(SqlCommand sqlCommand, SqlConnection connection, string CmdText, SqlParameterCollection sqlParameters)
        {

            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = CmdText;
            sqlCommand.CommandTimeout = 9999;
            foreach (SqlParameter p in sqlParameters)
            {
                sqlCommand.Parameters.AddWithValue(p.ParameterName, p.Value);
            }

            //  Create a new instance of SQL DataAdapter and fill DataSet
            SqlDataReader dr = sqlCommand.ExecuteReader();

            //DataSet ds = new DataSet();
            //da.Fill(ds);
            //connection.Close();
            return dr;
        }

        public static DataSet GetDataSet(string connectionString, SqlConnection connection)
        {
            DataSet _dataSet = new DataSet();
            SqlDataAdapter _dataAdapter = new SqlDataAdapter(connectionString, connection);
            _dataAdapter.Fill(_dataSet);
            connection.Close();
            return _dataSet;
        }

        public static DataSet GetDataSetByCommand(SqlCommand command, SqlConnection connection)
        {
            DataSet _dataSet = new DataSet();
            SqlDataAdapter _dataAdapter = new SqlDataAdapter();
            _dataAdapter.SelectCommand = command;
            _dataAdapter.Fill(_dataSet);
            connection.Close();
            return _dataSet;
        }

        public static SqlCommand GetCommand(string connectionString, SqlConnection connection)
        {
            SqlCommand _command = new SqlCommand(connectionString, connection);
            _command.CommandType = CommandType.StoredProcedure;
            return _command;
        }

        public static string GetCommandExecuteScalar(string connectionString, SqlConnection connection)
        {
            SqlCommand _command = new SqlCommand(connectionString, connection);
            _command.CommandType = CommandType.StoredProcedure;
            string returnID = _command.ExecuteScalar().ToString();
            connection.Close();
            return returnID;
        }

        public static SqlDataReader GetDataReder(SqlCommand command)
        {
            SqlDataReader _sqlDataReder = command.ExecuteReader(CommandBehavior.SingleRow);
            return _sqlDataReder;
        }
    }
}