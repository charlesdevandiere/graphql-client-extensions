using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("GraphQL.Client.Extensions.UnitTests")]
namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// The Query Class is a simple class to build out graphQL
    /// style queries. It will build the parameters and field lists
    /// similar in a way you would use a SQL query builder to assemble
    /// a query. This will maintain the response for the query
    /// </summary>
    public class Query<TSource> : IQuery<TSource> where TSource : class
    {
        private readonly QueryOptions options;

        /// <summary>
        /// Gets the select list.
        /// </summary>
        public List<object> SelectList { get; } = new List<object>();

        /// <summary>
        /// Gets the arguments map.
        /// </summary>
        public Dictionary<string, object> ArgumentsMap { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the query name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the alias name.
        /// </summary>
        public string AliasName { get; private set; }
        
        /// <summary>
        /// Gets the query string builder.
        /// </summary>
        private IQueryStringBuilder QueryStringBuilder { get; set; } = new QueryStringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(string name, IQueryStringBuilder queryStringBuilder)
        {
            this.Name = name;
            this.QueryStringBuilder = queryStringBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(string name, QueryOptions options)
        {
            this.Name = name;
            this.options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{TSource}" /> class.
        /// </summary>
        public Query(string name, IQueryStringBuilder queryStringBuilder, QueryOptions options)
        {
            this.Name = name;
            this.QueryStringBuilder = queryStringBuilder;
            this.options = options;
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
        public IQuery<TSource> Alias(string alias)
        {
            this.AliasName = alias;

            return this;
        }

        /// <summary>
        /// Add a field to the select list of the query.
        /// </summary>
        /// <typeparam name="TProperty">Field property type</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> Select<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            PropertyInfo property = GetPropertyInfo(selector);
            string name = GetPropertyName(property);

            this.SelectList.Add(name);

            return this;
        }

        /// <summary>
        /// Add a field to the select list of the query.
        /// </summary>
        /// <param name="field">Field name</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> Select(string field)
        {
            this.SelectList.Add(field);

            return this;
        }

        /// <summary>
        /// Generates a sub select query from child object property
        /// </summary>
        /// <typeparam name="TSubSource">Sub query source type</typeparam>
        /// <param name="selector">Child object property selector</param>
        /// <param name="buildSubQuery">Build sub query</param>
        public IQuery<TSource> SubSelect<TSubSource>(
            Expression<Func<TSource, TSubSource>> selector,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> buildSubQuery)
            where TSubSource : class
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            if (buildSubQuery == null)
            {
                throw new ArgumentNullException(nameof(buildSubQuery));
            }

            PropertyInfo property = GetPropertyInfo(selector);
            string name = GetPropertyName(property);

            return SubSelect(name, buildSubQuery);
        }

        /// <summary>
        /// Generates a sub select query
        /// </summary>
        /// <typeparam name="TSubSource">Sub query source type</typeparam>
        /// <param name="field">Field name</param>
        /// <param name="buildSubQuery">Build sub query</param>
        public IQuery<TSource> SubSelect<TSubSource>(
            string field,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> buildSubQuery)
            where TSubSource : class
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException(nameof(field));
            }
            if (buildSubQuery == null)
            {
                throw new ArgumentNullException(nameof(buildSubQuery));
            }

            var query = new Query<TSubSource>(field, this.options);
            IQuery<TSubSource> subQuery = buildSubQuery.Invoke(query);

            this.SelectList.Add(subQuery);

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
        public IQuery<TSource> SetArgument(string key, object where)
        {
            this.ArgumentsMap.Add(key, where);

            return this;
        }

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionary that takes &lt;string, object&gt;</param>
        /// <returns>IQuery{TSource}</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        /// <exception cref="ArgumentNullException">Null Argument</exception>
        public IQuery<TSource> SetArguments(Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> field in dict)
            {
                this.ArgumentsMap.Add(field.Key, field.Value);
            }

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
                this.ArgumentsMap.Add(this.GetPropertyName(property), property.GetValue(arguments));
            }

            return this;
        }

        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <returns>The GraphQL Query String, without outer enclosing block</returns>
        /// <exception cref="ArgumentException">Must have a 'Name' specified in the Query</exception>
        /// <exception cref="ArgumentException">Must have a one or more 'Select' fields in the Query</exception>
        public string Build()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new ArgumentException("Must have a 'Name' specified in the Query");
            }
            if (SelectList.Count == 0)
            {
                throw new ArgumentException("Must have a one or more 'Select' fields in the Query");
            }

            this.QueryStringBuilder.Clear();

            return this.QueryStringBuilder.Build(this);
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
