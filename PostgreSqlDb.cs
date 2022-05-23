using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Npgsql;

namespace BoilerPlateWeb
{
    public class PostgreSqlDb
    {
        private static string GetConnectionString()
        {
            return "Server=10.10.20.180;Port=5432;User Id=postgres;Password=Icecool1;Database=postgres;";
        }

        public static DataSet Execute(string command)
        {
            NpgsqlCommand sqlCommand = new NpgsqlCommand(command, new NpgsqlConnection(GetConnectionString()));
            sqlCommand.CommandTimeout = 700;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public static DataSet GetData(string procName, List<NpgsqlParameter> parameters)
        {
            NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(procName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandTimeout = 700;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds);

            connection.Close();
            connection.Dispose();

            return ds;
        }

        public static DataSet GetData(string commandText)
        {
            NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);
            command.CommandTimeout = 700;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds);

            connection.Close();
            connection.Dispose();

            if (DBHelper.HasData(ds))
            {
                return ds;
            }

            return null;
        }

        public static object GetValue(string procName, string paramName, string value)
        {
            NpgsqlParameter parameter = new NpgsqlParameter(paramName, value);
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            parameters.Add(parameter);
            
            try
            {
                return GetData(procName, parameters).Tables[0].Rows[0][0];
            }
            catch (Exception er)
            {

            }
            return null;
        }

        public static object GetValue(string commandText, string field)
        {
            NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString());
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(commandText, connection);

            object value = null;

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Read();

                int ordinal = reader.GetOrdinal(field);
                value = reader.GetValue(ordinal);
                reader.Close();
            }
            catch (Exception er)
            {

            }

            connection.Close();

            return value;
        }

        //For single row.
        public static DataRow GetValues(string procName, string paramName, string value)
        {
            NpgsqlParameter parameter = new NpgsqlParameter(paramName, value);
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            parameters.Add(parameter);

            try
            {
                return GetData(procName, parameters).Tables[0].Rows[0];
            }
            catch (Exception er)
            {

            }
            return null;
        }
    }
}