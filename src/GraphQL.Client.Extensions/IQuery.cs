using System;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// Query interface
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <returns>The GraphQL Query String, without outer enclosing block</returns>
        /// <exception cref="ArgumentException">Must have a 'Name' specified in the Query</exception>
        /// <exception cref="ArgumentException">Must have a one or more 'Select' fields in the Query</exception>
        string Build();
    }
}
