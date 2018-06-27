using Silk.Data.SQL.Providers;
using Silk.Data.SQL.Queries;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Silk.Data.SQL.MSSQL
{
	public class MSSqlDataProvider : DataProviderCommonBase
	{
		public const string PROVIDER_NAME = "mssql";

		private readonly string _connectionString;

		public override string ProviderName => PROVIDER_NAME;

		public MSSqlDataProvider(string connectionString)
		{
			_connectionString = connectionString;
		}

		public MSSqlDataProvider(SqlConnectionStringBuilder connectionStringBuilder) :
			this(connectionStringBuilder.ConnectionString) { }

		public MSSqlDataProvider(string hostname, string database, string username, string password) :
			this(new SqlConnectionStringBuilder
			{
				InitialCatalog = database,
				DataSource = hostname,
				UserID = username,
				Password = password
			}) { }

		public override void Dispose()
		{
		}

		protected override DbConnection Connect()
		{
			var connection = new SqlConnection(_connectionString);
			connection.Open();
			return connection;
		}

		protected override async Task<DbConnection> ConnectAsync()
		{
			var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();
			return connection;
		}

		protected override IQueryConverter CreateQueryConverter()
		{
			return new MSSqlQueryConverter();
		}

#if DEBUG
		public override DbCommand CreateCommand(DbConnection connection, SqlQuery sqlQuery)
		{
			return base.CreateCommand(connection, sqlQuery);
		}
#endif
	}
}
