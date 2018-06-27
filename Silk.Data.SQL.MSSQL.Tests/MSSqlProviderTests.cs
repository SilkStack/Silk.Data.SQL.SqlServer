using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.ProviderTests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silk.Data.SQL.MSSQL.Tests
{
	[TestClass]
	public class MSSqlProviderTests : SqlProviderTests
	{
		public override IDataProvider CreateDataProvider(string connectionString)
		{
			return new MSSqlDataProvider(connectionString);
		}

		public override void Dispose()
		{
			DataProvider.Dispose();
		}
	}
}
