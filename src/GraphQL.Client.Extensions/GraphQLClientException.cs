using System;
using System.Linq;
using GraphQL.Common.Response;

namespace GraphQL.Client.Extensions
{
    /// <summary>GraphQL Client Exception.</summary>
    public class GraphQLClientException : Exception
    {
        private const string baseMessage = "The GraphQL request returns errors.";

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphQLClientException" /> class.
        /// </summary>
        public GraphQLClientException() : base(baseMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphQLClientException" /> class.
        /// </summary>
        /// <param name="errors">The GraphQL errors.</param>
        public GraphQLClientException(GraphQLError[] errors)
            : base(
                baseMessage +
                Environment.NewLine +
                string.Join(Environment.NewLine, errors?.Select(errors => errors.Message)))
        {
            if (errors != null && errors.Length > 0)
            {
                this.Data.Add("Errors", errors);
            }
        }
    }
}
