# Overview

Microsoft SQL Server provider for `Silk.Data.SQL.Base`.

# Installing

`Silk.Data.SQL.SqlServer` is available as a NuGet package: https://www.nuget.org/packages/Silk.Data.SQL.SqlServer

You can install it from the NuGet package manager in Visual Studio or from command line with dotnet core:

~~~~
dotnet add package Silk.Data.SQL.SqlServer
~~~~


# Usage

To execute SQL statements just create an instance of `MSSqlDataProvider`, passing in the name of the file you wish to store your database in.

    var provider = new MSSqlDataProvider("hostname", "database", "username", "password");

## Executing Queries

Non-reader queries:

    provider.ExecuteNonReaderAsync(
        QueryExpression.Insert(
            "Accounts",
            new[] { "DisplayName" },
            new object[] { "John" },
            new object[] { "Jane" }
        )
    );

Queries with results need to be disposed:

    using (var queryResult = provider.ExecuteReader(
        QueryExpression.Select(
            new[] { Expression.Value("Hello World!") }
    )))
    {
        Assert.IsTrue(queryResult.HasRows);
        Assert.IsTrue(queryResult.Read());
        Assert.AreEqual("Hello World!", queryResult.GetString(0));
    }

## Raw SQL

A raw SQL expression is provided on the `MSSql` helper class.

    var rawSQL = MSSql.Raw("SELECT random()");

Raw SQL expressions are safe to be used within `TransactionExpression`:

    var transaction = QueryExpression.Transaction(
        MSSql.Raw("SELECT date()"),
        MSSql.Raw("SELECT time()")
    );

# License

`Silk.Data.SQL.SqlServer` is made available under the MIT license.