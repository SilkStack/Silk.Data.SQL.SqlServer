using Silk.Data.SQL.MSSQL.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silk.Data.SQL.MSSQL
{
	public static class MSSql
	{
		public static MSSqlRawQueryExpression Raw(string sql)
		{
			return new MSSqlRawQueryExpression(sql);
		}
	}
}
