using System;
using GraphQL.Common.Response;
using Xunit;

namespace GraphQL.Client.Extensions.UnitTests
{
    public class GraphQLClientExceptionTest
    {
        [Fact]
        public void TestDefaultConstructor()
        {
            var exception = new GraphQLClientException();

            Assert.Equal("The GraphQL request returns errors.", exception.Message);
        }

        [Fact]
        public void TestConstructor()
        {
            var errors = new GraphQLError[]
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
            var exception = new GraphQLClientException(errors);

            Assert.Equal($"The GraphQL request returns errors.{Environment.NewLine}First error.{Environment.NewLine}Second error.", exception.Message);
            Assert.Equal(errors, exception.Data["Errors"]);
        }
    }
}
