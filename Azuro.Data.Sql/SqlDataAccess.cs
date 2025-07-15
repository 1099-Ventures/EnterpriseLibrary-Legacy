using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections.Generic;

using Azuro.Data;

namespace Azuro.Data.Sql
{
    public class SqlDataAccess : IDataAccess
    {
        private string m_connectionString;

        public SqlDataAccess( string connectionString )
        {
            m_connectionString = connectionString;
        }

        /// <summary>
        /// Create a SqlConnection object.
        /// </summary>
        /// <returns>A new SqlConnection.</returns>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection( ConnectionString );
        }

        private SqlCommand CreateCommand( string sqlCommand, SqlConnection conn )
        {
            return new SqlCommand( sqlCommand, conn );
        }

        /// <summary>
        /// Open the specified connection.
        /// </summary>
        /// <param name="conn">The connection to open.</param>
        /// <returns>True if the connection could be opened.</returns>
        private bool Open( SqlConnection conn )
        {
            Close( conn );
            conn.Open();
            return (IsConnected( conn ));
        }

        /// <summary>
        /// Close the specified connection.
        /// </summary>
        /// <param name="conn">The connection to close.</param>
        private void Close( SqlConnection conn )
        {
            if( conn != null )
            {
                if( IsConnected( conn ) )
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Method to test the connected state of a SqlConnection.
        /// </summary>
        /// <param name="conn">The SqlConnection to test.</param>
        /// <returns>True if it is connected, else false.</returns>
        private bool IsConnected( SqlConnection conn )
        {
            return (conn != null) ? (conn.State == ConnectionState.Open) : false;
        }

        #region IDataAccess Members

        public string ConnectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        public char ParameterPlaceholder
        {
            get { return '@'; }
        }

        public Dictionary<string, IDataParameter> DeriveParameters( IDbConnection connection, string sqlCommand )
        {
            Dictionary<string, IDataParameter> parameters;
            SqlConnection conn = (SqlConnection)connection;

            SqlCommand cmd = new SqlCommand( sqlCommand, conn );
            cmd.CommandType = CommandType.StoredProcedure;

            if( Open( conn ) )
            {
                SqlCommandBuilder.DeriveParameters( cmd );

                parameters = new Dictionary<string, IDataParameter>( StringComparer.InvariantCultureIgnoreCase );

                foreach( SqlParameter p in cmd.Parameters )
                {
                    parameters.Add( p.ParameterName, p );
                }
            }
            else
            {
                parameters = new Dictionary<string, IDataParameter>();
            }

            return parameters;
        }

        public IDbCommand CreateCommand( IDbConnection connection, string sqlCommand )
        {
            return CreateCommand( sqlCommand, (SqlConnection)connection );
        }

        public IDataParameter CreateParameter()
        {
            return new SqlParameter();
        }

        public IDataReader ExecuteReader( IDbConnection connection, IDbCommand cmd )
        {
            SqlConnection conn = (SqlConnection)connection;

            cmd.Connection = conn;

            if( Open( conn ) )
            {
                IDataReader reader = cmd.ExecuteReader( CommandBehavior.CloseConnection );
                return reader;
            }

            return null;
        }

        public DataSet ExecuteDataSet( IDbConnection connection, IDbCommand cmd )
        {
            SqlConnection conn = (SqlConnection)connection;

            DataSet ds = new DataSet( cmd.CommandText );
            SqlDataAdapter da = new SqlDataAdapter( (SqlCommand)cmd );
            if( Open( conn ) )
            {
                da.Fill( ds );
                Close( conn );
            }
            return ds;
        }

        public int Execute( IDbConnection connection, IDbCommand cmd )
        {
            SqlConnection conn = (SqlConnection)connection;

            object retval = 0;
            if( Open( conn ) )
            {
                retval = cmd.ExecuteNonQuery();
                Close( conn );
            }
            return(int) retval;
        }

        /// <summary>
        /// Returns the first column of the first row in a result set.
        /// Useful for when you wish to return a count, a single value, 
        /// or check that a record exists in the db.
        /// </summary>
        /// <param name="cmd">The database command to execute.</param>
        /// <returns>An object containing the value or null.</returns>
        public object ExecuteScalar( IDbConnection connection, IDbCommand cmd )
        {
            SqlConnection conn = (SqlConnection)connection;

            object retVal = null;
            if( Open( conn ) )
            {
                retVal = cmd.ExecuteScalar();
                Close( conn );
            }
            return (retVal == System.DBNull.Value) ? null : retVal;
        }

        #endregion
    }
}
