using Silk.Data.SQL.Expressions;
using Silk.Data.SQL.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silk.Data.SQL.MSSQL
{
	public class MSSqlQueryConverter : QueryConverterCommonBase
	{
		protected override string ProviderName => MSSqlDataProvider.PROVIDER_NAME;

		protected override string AutoIncrementSql => "IDENTITY(1,1)";

		public MSSqlQueryConverter()
		{
			ExpressionWriter = new MSSqlQueryWriter(Sql, this);
		}

		protected override string GetDbDatatype(SqlDataType sqlDataType)
		{
			switch (sqlDataType.BaseType)
			{
				case SqlBaseType.Guid: return "uniqueidentifier";
				case SqlBaseType.TinyInt: return "tinyint";
				case SqlBaseType.SmallInt: return "smallint";
				case SqlBaseType.Int: return "int";
				case SqlBaseType.BigInt: return "bigint";
				case SqlBaseType.Float: return $"float({sqlDataType.Parameters[0]})";
				case SqlBaseType.Bit: return "bit";
				case SqlBaseType.Decimal: return $"decimal({sqlDataType.Parameters[0]}, {sqlDataType.Parameters[1]})";
				case SqlBaseType.Date: return "date";
				case SqlBaseType.Time: return "time";
				case SqlBaseType.DateTime: return "datetime2";
				case SqlBaseType.Text: return sqlDataType.Parameters.Length > 0 ? $"nvarchar({sqlDataType.Parameters[0]})" : "nvarchar(max)";
				case SqlBaseType.Binary: return "binary";
			}
			throw new System.NotSupportedException($"SQL data type not supported: {sqlDataType.BaseType}.");
		}

		protected override string QuoteIdentifier(string schemaComponent)
		{
			if (schemaComponent == "*") return "*";
			return $"[{schemaComponent}]";
		}

		protected override void WriteFunctionToSql(QueryExpression queryExpression)
		{
			switch (queryExpression)
			{
				case RandomFunctionExpression randomFunctionExpression:
					Sql.Append($"CAST(FLOOR(RAND() * {int.MaxValue}) AS BIGINT)");
					return;
				case TableExistsVirtualFunctionExpression tableExistsExpression:
					Sql.Append($@"IF EXISTS (SELECT 1 
						FROM [INFORMATION_SCHEMA].[TABLES] 
						WHERE [TABLE_TYPE] = 'BASE TABLE' 
						AND [TABLE_CATALOG] = DB_NAME()
						AND [TABLE_NAME] = '{tableExistsExpression.Table.TableName}') 
						SELECT 1 ELSE SELECT 0");
					return;
				case LastInsertIdFunctionExpression lastInsertIdFunctionExpression:
					Sql.Append("CAST(@@IDENTITY AS INT)");
					return;
			}
			base.WriteFunctionToSql(queryExpression);
		}

		private class MSSqlQueryWriter : QueryWriter
		{
			public new MSSqlQueryConverter Converter { get; }

			public MSSqlQueryWriter(StringBuilder sql, MSSqlQueryConverter converter) : base(sql, converter)
			{
				Converter = converter;
			}

			protected override void VisitQuery(QueryExpression queryExpression)
			{
				var isSubQuery = QueryDepth > 0;

				switch (queryExpression)
				{
					case SelectExpression select:
						QueryDepth++;
						if (isSubQuery)
						{
							Sql.Append("(");
						}

						Sql.Append("SELECT ");
						VisitExpressionGroup(select.Projection, ExpressionGroupType.Projection);
						if (select.From != null)
						{
							Sql.Append(" FROM ");
							Visit(select.From);
						}
						if (select.Joins != null && select.Joins.Length > 0)
						{
							VisitExpressionGroup(select.Joins, ExpressionGroupType.Joins);
						}
						if (select.WhereConditions != null)
						{
							Sql.Append(" WHERE ");
							Visit(select.WhereConditions);
						}
						if (select.GroupConditions != null && select.GroupConditions.Length > 0)
						{
							Sql.Append(" GROUP BY ");
							VisitExpressionGroup(select.GroupConditions, ExpressionGroupType.GroupByClauses);
						}
						if (select.HavingConditions != null)
						{
							Sql.Append(" HAVING ");
							Visit(select.HavingConditions);
						}
						if (select.OrderConditions != null && select.OrderConditions.Length > 0)
						{
							Sql.Append(" ORDER BY ");
							VisitExpressionGroup(select.OrderConditions, ExpressionGroupType.OrderByClauses);
						}
						else if (select.Offset != null || select.Limit != null)
						{
							Sql.Append(" ORDER BY (SELECT NULL) ");
						}
						if (select.Offset != null)
						{
							Sql.Append(" OFFSET ");
							Visit(select.Offset);
							Sql.Append(" ROWS ");
						}
						if (select.Limit != null)
						{
							if (select.Offset == null)
								Sql.Append(" OFFSET 0 ROWS FETCH FIRST ");
							else
								Sql.Append(" FETCH NEXT ");
							Visit(select.Limit);
							Sql.Append(" ROWS ONLY ");
						}

						QueryDepth--;
						if (isSubQuery)
						{
							Sql.Append(") ");
						}
						else
						{
							Sql.Append("; ");
						}
						break;
					default:
						base.VisitQuery(queryExpression);
						break;
				}
			}
		}
	}
}
