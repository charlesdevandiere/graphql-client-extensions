using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// Query of TSource interface
    /// </summary>
    public interface IQuery<TSource> where TSource : class
    {
        /// <summary>
        /// Gets the select list.
        /// </summary>
        List<object> SelectList { get; }

        /// <summary>
        /// Gets the arguments map.
        /// </summary>
        Dictionary<string, object> ArgumentsMap { get; }

        /// <summary>
        /// Gets the query name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the alias name.
        /// </summary>
        string AliasName { get; }

        /// <summary>
        /// Sets the Query Alias name. This is used in graphQL to allow
        /// multiple queries with the same endpoint (name) to be assembled
        /// into a batch like query. This will prefix the Name in the query.
        /// It will also be used for the Response name processing.
        /// Note that this can be applied to any sub-select as well. GraphQL will
        /// rename the query with the alias name in the response.
        /// </summary>
        /// <param name="alias">The alias name</param>
        /// <returns>Query</returns>
        IQuery<TSource> Alias(string alias);

        /// <summary>
        /// Add a field to the select list of the query.
        /// </summary>
        /// <typeparam name="TProperty">Field property type</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>IQuery{TSource}</returns>
        IQuery<TSource> Select<TProperty>(Expression<Func<TSource, TProperty>> selector);

        /// <summary>
        /// Add a field to the select list of the query.
        /// </summary>
        /// <param name="field">Field name</param>
        /// <returns>IQuery{TSource}</returns>
        IQuery<TSource> Select(string field);

        /// <summary>
        /// Generates a sub select query from child object property
        /// </summary>
        /// <typeparam name="TSubSource">Sub query source type</typeparam>
        /// <param name="selector">Child object property selector</param>
        /// <param name="buildSubQuery">Build sub query</param>
        IQuery<TSource> SubSelect<TSubSource>(
            Expression<Func<TSource, TSubSource>> selector,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> buildSubQuery)
            where TSubSource : class;

        /// <summary>
        /// Generates a sub select query
        /// </summary>
        /// <typeparam name="TSubSource">Sub query source type</typeparam>
        /// <param name="field">Field name</param>
        /// <param name="buildSubQuery">Build sub query</param>
        IQuery<TSource> SubSelect<TSubSource>(
            string field,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> buildSubQuery)
            where TSubSource : class;

        /// <summary>
        /// Sets up the Parameters part of the GraphQL query. This
        /// accepts a key and a where part that will go into the
        /// list for later construction into the query. The where part
        /// can be a simple primitive or complex object that will be
        /// evaluated.
        /// </summary>
        /// <param name="key">The Parameter Name</param>
        /// <param name="where">The value of the parameter, primitive or object</param>
        /// <returns></returns>
        IQuery<TSource> SetArgument(string key, object where);

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionary that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <throws>DuplicateKeyException and others</throws>
        IQuery<TSource> SetArguments(Dictionary<string, object> dict);

        /// <sumary>
        /// Sets arguments from object.
        /// </sumary>
        /// <typeparam name="TArguments">Arguments type</typeparam>
        /// <param name="arguments">Arguments object</param>
        IQuery<TSource> SetArguments<TArguments>(TArguments arguments) where TArguments : class;
        
        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <returns>The GraphQL Query String, without outer enclosing block</returns>
        /// <exception cref="ArgumentException">Must have a 'Name' specified in the Query</exception>
        /// <exception cref="ArgumentException">Must have a one or more 'Select' fields in the Query</exception>
        string Build();
    }
}
