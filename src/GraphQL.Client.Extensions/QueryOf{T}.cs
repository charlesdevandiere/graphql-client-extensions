using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// The Query Class is a simple class to build out graphQL
    /// style queries. It will build the parameters and field lists
    /// similar in a way you would use a SQL query builder to assemble
    /// a query. This will maintain the response for the query
    /// </summary>
    public class Query<TSource> : Query, IQuery<TSource> where TSource : class
    {
        private readonly QueryOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(string name)
        {
            this.Name(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(QueryOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(string name, QueryOptions options)
        {
            this.Name(name);
            this.options = options;
        }

        /// <summary>
        /// Accepts a string and will use this as the query. Setting
        /// this will override any other settings and ignore any
        /// validation checks. If the string is empty it will be
        /// ignored and the existing query builder actions will be
        /// at play.
        ///
        /// NOTE : This will strip out any leading or trailing braces if found
        ///
        /// WARNING : Calling this will clear all other query elements.
        ///
        /// </summary>
        /// <param name="rawQuery">The full valid query to be sent to the endpoint</param>
        public new IQuery<TSource> Raw(string rawQuery)
        {
            base.Raw(rawQuery);

            return this;
        }

        /// <summary>
        /// Sets the query Name
        /// </summary>
        /// <param name="queryName">The Query Name String</param>
        /// <returns>IQuery{TSource}</returns>
        public new IQuery<TSource> Name(string queryName)
        {
            base.Name(queryName);

            return this;
        }

        /// <summary>
        /// Sets the Query Alias name. This is used in graphQL to allow
        /// multiple queries with the same endpoint (name) to be assembled
        /// into a batch like query. This will prefix the Name as specified.
        /// Note that this can be applied to any sub-select as well. GraphQL will
        /// rename the query with the alias name in the response.
        /// </summary>
        /// <param name="alias">The alias name</param>
        /// <returns>IQuery{TSource}</returns>
        public new IQuery<TSource> Alias(string alias)
        {
            base.Alias(alias);

            return this;
        }

        /// <summary>
        /// Add a comment to the Query. This will take a simple string comment
        /// and add it to the Select block in the query. GraphQL formatting will
        /// be automatically added. Multi-line comments can be done with the
        /// '\n' character and it will be automatically converted into a GraphQL
        /// multi-line comment
        /// </summary>
        /// <param name="comment">The comment string</param>
        /// <returns>IQuery{TSource}</returns>
        public new IQuery<TSource> Comment(string comment)
        {
            base.Comment(comment);

            return this;
        }

        /// <summary>
        /// Add this property to the select part of the query. This
        /// accepts any property of source object
        /// </summary>
        /// <typeparam name="TProperty">Select property type</typeparam>
        /// <param name="lambda">Property selector to build select fields</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> Select<TProperty>(Expression<Func<TSource, TProperty>> lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException(nameof(lambda));
            }

            PropertyInfo property = GetPropertyInfo(lambda);
            string name = GetPropertyName(property);

            base.Select(name);

            return this;
        }

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
        public new IQuery<TSource> SetArgument(string key, object where)
        {
            base.SetArgument(key, where);

            return this;
        }

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionary that takes &lt;string, object&gt;</param>
        /// <returns>IQuery{TSource}</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        /// <exception cref="ArgumentNullException">Null Argument</exception>
        public new IQuery<TSource> SetArguments(Dictionary<string, object> dict)
        {
            base.SetArguments(dict);

            return this;
        }

        /// <sumary>
        /// Sets arguments from object.
        /// </sumary>
        /// <typeparam name="TArguments">Arguments type</typeparam>
        /// <param name="arguments">Arguments object</param>
        public IQuery<TSource> SetArguments<TArguments>(TArguments arguments) where TArguments : class
        {
            PropertyInfo[] properties = typeof(TArguments).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                this.WhereMap.Add(this.GetPropertyName(property), property.GetValue(arguments));
            }

            return this;
        }

        /// <summary>
        /// Add additional queries to the request. These
        /// will get bundled in as additional queries. This
        /// will affect the query that is executed in the Get()
        /// method and the ToString() output, but will not
        /// change any items specific to this query. Each
        /// query will individually be calling it's ToString()
        /// to get the query to be batched. Use Alias names
        /// where appropriate if calling the same Query multiple times.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>IQuery</returns>
        public new IQuery<TSource> Batch(IQuery query)
        {
            base.Batch(query);

            return this;
        }

        /// <summary>
        /// Generates a subselect query from child oject property
        /// </summary>
        /// <typeparam name="TSubSource">Sub query source type</typeparam>
        /// <param name="lambda">Child object property selector</param>
        /// <param name="buildSubQuery">Build sub query</param>
        public IQuery<TSource> SubSelect<TSubSource>(Expression<Func<TSource, TSubSource>> lambda, Func<IQuery<TSubSource>, IQuery<TSubSource>> buildSubQuery) where TSubSource : class
        {
            if (lambda == null)
            {
                throw new ArgumentNullException(nameof(lambda));
            }
            if (buildSubQuery == null)
            {
                throw new ArgumentNullException(nameof(buildSubQuery));
            }

            PropertyInfo property = GetPropertyInfo(lambda);
            string name = GetPropertyName(property);

            var query = new Query<TSubSource>(this.options);
            query.Name(name);
            IQuery<TSubSource> subQuery = buildSubQuery.Invoke(query);

            base.Select(subQuery);

            return this;
        }

        /// <summary>
        /// Gets the string representation of the GraphQL query. This does some
        /// MINOR checking and will toss if not formatted correctly
        /// </summary>
        /// <returns>The GraphQL Query String, without outer enclosing block</returns>
        /// <exception cref="ArgumentException">Dupe Key, empty or missing parts</exception>
        public override string ToString()
        {
            return base.ToString();
        }

        private static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TSource, TProperty>> lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException(nameof(lambda));
            }

            MemberExpression member = lambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{lambda.ToString()}' body is not member expression.");
            }

            PropertyInfo propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Expression '{lambda.ToString()}' not refers to a property.");
            }

            Type type = typeof(TSource);
            if (type != propertyInfo.ReflectedType && !type.IsSubclassOf(propertyInfo.ReflectedType))
            {
                throw new ArgumentException($"Expression '{lambda.ToString()}' refers to a property that is not from type {type}.");
            }

            return propertyInfo;
        }

        /// <summary>
        /// Tries to get property name from JSON property attribute or from optional formater.
        /// </summary>
        /// <param name="property">Property</param>
        /// <returns>String</returns>
        private string GetPropertyName(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            Attribute attribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute));

            if (attribute != null)
            {
                if (!string.IsNullOrEmpty((attribute as JsonPropertyAttribute).PropertyName))
                {
                    return (attribute as JsonPropertyAttribute).PropertyName;
                }
            }

            if (this.options?.Formater != null)
            {
                return this.options.Formater.Invoke(property.Name);
            }

            return property.Name;
        }
    }
}
