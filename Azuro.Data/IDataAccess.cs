using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Azuro.Data
{
   /// <summary>
   /// IDataAccess provides an abstraction of all database interactions for
   /// DataObject. This enables the Data Access Layer to be database agnostic,
   /// as long as there is an implementation of IDataAccess for the targeted database.
   /// </summary>
   public interface IDataAccess
   {
      IDbConnection CreateConnection();

      /// <summary>
      /// Gets or sets the connection string. 
      /// </summary>
      string ConnectionString { get; set; }

      /// <summary>
      /// The implementation of this return the character used by the implemented
      /// database's SQL engine to mark a parameter.
      /// </summary>
      char ParameterPlaceholder { get; }

      /// <summary>
      /// Derive the parameter information from the database.
      /// </summary>
      /// <param name="sqlCommand">The SQL command that parameters must be derived for.</param>
      Dictionary<string, IDataParameter> DeriveParameters( IDbConnection connection, string sqlCommand );

      /// <summary>
      /// Create a command object for the database implementation.
      /// </summary>
      /// <param name="sqlCommand">The SQL command for the command object.</param>
      /// <returns>An IDbCommand for the specific database implementation.</returns>
      IDbCommand CreateCommand( IDbConnection connection, string sqlCommand );

      /// <summary>
      /// Create a parameter for the specified database implementation.
      /// </summary>
      /// <returns>An IDataParameter.</returns>
      IDataParameter CreateParameter();

      /// <summary>	
      /// Execute a command to create an IDataReader.
      /// </summary>
      /// <param name="cmd">The command object for which to return a reader.</param>
      /// Most often this should be set to true, as readers keep the connection open.</param>
      /// <returns>An IDataReader.</returns>
      IDataReader ExecuteReader( IDbConnection connection, IDbCommand cmd );

      /// <summary>
      /// Returns a dataset using the specified command.
      /// </summary>
      /// <param name="cmd">The command object for which to return a dataset.</param>
      /// <returns>A DataSet.</returns>
      DataSet ExecuteDataSet( IDbConnection connection, IDbCommand cmd );

      /// <summary>
      /// Execute any command. This is used most often for executing non-data queries, like Insert and Update.
      /// </summary>
      /// <param name="cmd">The command object to execute.</param>
      /// <returns>The number of rows affected by the command.</returns>
      int Execute( IDbConnection connection, IDbCommand cmd );

      /// <summary>
      /// Execute a query and return the first column of the first row.
      /// </summary>
      /// <param name="cmd">The command object to execute.</param>
      /// <returns>An object representing the first column of the first row.</returns>
      object ExecuteScalar( IDbConnection connection, IDbCommand cmd );
   }
}
