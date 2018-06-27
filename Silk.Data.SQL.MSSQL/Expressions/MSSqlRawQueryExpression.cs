using Silk.Data.SQL.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silk.Data.SQL.MSSQL.Expressions
{
	public class MSSqlRawQueryExpression : QueryExpression
	{
		public string SqlText { get; }

		public override ExpressionNodeType NodeType => ExpressionNodeType.Query;

		public MSSqlRawQueryExpression(string sqlText)
		{
			SqlText = sqlText;
		}
	}
}
