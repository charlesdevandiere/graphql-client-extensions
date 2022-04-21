using System;
using GraphQL.Common.Response;
using Xunit;

namespace GraphQL.Client.Extensions.UnitTests;

public class GraphQLClientExceptionTest
{
    [Fact]
    public void TestDefaultConstructor()
    {
        GraphQLClientException exception = new();

        Assert.Equal("The GraphQL request returns errors.", exception.Message);
    }

    [Fact]
    public void TestConstructor()
    {
        GraphQLError[] errors = new[]
        {
                new GraphQLError
                {
                    Message = "First error.",
                    Locations = new GraphQLLocation[] { new GraphQLLocation { Line = 1, Column = 2 } }
                },
                new GraphQLError
                {
                    Message = "Second error.",
                    Locations = new GraphQLLocation[] { new GraphQLLocation { Line = 1, Column = 2 } }
                }
        };
        GraphQLClientException exception = new(errors);

        Assert.Equal($"The GraphQL request returns errors.{Environment.NewLine}First error.{Environment.NewLine}Second error.", exception.Message);
        Assert.Equal(errors, exception.Data["Errors"]);
    }
}
