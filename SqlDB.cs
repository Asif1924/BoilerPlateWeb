﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace BoilerPlateWeb
{
    public class SqlDB
    {
        private static string GetConnectionString()
        {
            return "Server = asl-app2; Database = reporting; User Id = sa; Password = Icecool1; Connection Timeout=600";
        }

        private static string GetConnectionStringProduction()
        {
            return "Server = apps; Database = reporting; User Id = sa; Password = access; Connection Timeout=600";
        }

        public static DataSet GetData(string command)
        {
            SqlCommand sqlCommand = new SqlCommand(command, new SqlConnection(GetConnectionString()));
            sqlCommand.CommandTimeout = 700;
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public static DataSet GetDataProduction(string command)
        {
            SqlCommand sqlCommand = new SqlCommand(command, new SqlConnection(GetConnectionStringProduction()));
            sqlCommand.CommandTimeout = 700;
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public static DataSet GetData(string procName, List<SqlParameter> parameters = null)
        {
            SqlConnection connection = new SqlConnection(GetConnectionString());
            connection.Open();

            SqlCommand command = new SqlCommand(procName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandTimeout = 700;

            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds);

            connection.Close();
            connection.Dispose();

            return ds;
        }

        public static DataSet GetDataProduction(string procName, List<SqlParameter> parameters = null, string connectionString = null)
        {
            string connString = "";
            if (connectionString == null)
            {
                connString = GetConnectionStringProduction();
            }
            else
            {
                connString = connectionString;
            }
            SqlConnection connection = new SqlConnection(connString);
            connection.Open();

            SqlCommand command = new SqlCommand(procName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }
            command.CommandTimeout = 700;

            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds);

            connection.Close();
            connection.Dispose();

            return ds;
        }

        public static void Execute(string commandText)
        {
            SqlConnection connection = new SqlConnection(GetConnectionString());
            connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}