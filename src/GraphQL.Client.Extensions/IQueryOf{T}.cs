using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphQL.Client.Extensions
{
    public interface IQuery<TSource> : IQuery where TSource : class
    {
        /// <summary>
        /// Accepts a string and will use this as the query. Setting
        /// this will override any other settings and ignore any
        /// validation checks. If the string is empty it will be
        /// ignored and the existing query builder actions will be
        /// at play.
        ///
        /// WARNING : Calling this will clear all other query elements.
        ///
        /// </summary>
        /// <param name="rawQuery">The full valid query to be sent to the endpoint</param>
        // ReSharper disable once UnusedMethodReturnValue.Global
        new IQuery<TSource> Raw(string rawQuery);

        /// <summary>
        /// Sets the query Name
        /// </summary>
        /// <param name="queryName">The Query Name String</param>
        /// <returns>Query</returns>
        new IQuery<TSource> Name(string queryName);

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
        new IQuery<TSource> Alias(string alias);

        /// <summary>
        /// Add a comment to the Query. This will take a simple string comment
        /// and add it to the Select block in the query. GraphQL formatting will
        /// be automatically added. Multi-line comments can be done with the
        /// '\n' character and it will be automatically converted into a GraphQL
        /// multi-line comment
        /// </summary>
        /// <param name="comment">The comment string</param>
        /// <returns>Query</returns>
        new IQuery<TSource> Comment(string comment);

        /// <summary>
        /// Add this list to the select part of the query. This
        /// accepts any type of list, but must be one of the types
        /// of data we support, primitives, lists, maps
        /// </summary>
        /// <param name="objectList">Generic List of select fields</param>
        /// <returns>Query</returns>
        IQuery<TSource> Select<TProperty>(Expression<Func<TSource, TProperty>> lambda);

        /// <summary>
        /// Adds a sub query to the list
        /// </summary>
        /// <param name="lambda">The name of the sub query</param>
        /// <param name="buildeSubSelect">The sub query builder</param>
        /// <returns>Query</returns>
        /// <exception cref="ArgumentException"></exception>
        IQuery<TSource> SubSelect<TSubSource>(
            Expression<Func<TSource, TSubSource>> lambda,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> buildeSubSelect)
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
        new IQuery<TSource> Where(string key, object where);

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionary that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <throws>DuplicateKeyException and others</throws>
        new IQuery<TSource> Where(Dictionary<string, object> dict);

        /// <summary>
        /// Add additional queries to the request. These
        /// will get bundled in as additional queries. This
        /// will affect the query that is executed in the Get()
        /// method and the ToString() output, but will not
        /// change any items specific to this query. Each
        /// query will individually be calling it's ToString()
        /// to get the query to be batched. Use Alias names
        /// where appropriate
        /// </summary>
        /// <param name="query"></param>
        /// <returns>IQuery</returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        new IQuery<TSource> Batch(IQuery query);
    }
}
