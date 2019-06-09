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
        public Query(string name, QueryOptions options)
        {
            this.Name = name;
            this.options = options;
            if (options?.QueryStringBuilderFactory != null)
            {
                this.QueryStringBuilder = options.QueryStringBuilderFactory();
            }
        }

        /// <summary>
        /// Sets the query alias name.
        /// </summary>
        /// <param name="alias">The alias name</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> Alias(string alias)
        {
            this.AliasName = alias;

            return this;
        }

        /// <summary>
        /// Adds a field to the query.
        /// </summary>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="selector">Field selector</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddField<TProperty>(Expression<Func<TSource, TProperty>> selector)
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
        /// Adds a field to the query.
        /// </summary>
        /// <param name="field">Field name</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddField(string field)
        {
            this.SelectList.Add(field);

            return this;
        }

        /// <summary>
        /// Adds a sub-object field to the query.
        /// </summary>
        /// <typeparam name="TSubSource">Sub-object type</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="build">Sub-object query building function</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddField<TSubSource>(
            Expression<Func<TSource, TSubSource>> selector,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> build)
            where TSubSource : class
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            if (build == null)
            {
                throw new ArgumentNullException(nameof(build));
            }

            PropertyInfo property = GetPropertyInfo(selector);
            string name = GetPropertyName(property);

            return AddField(name, build);
        }

        /// <summary>
        /// Adds a sub-list field to the query.
        /// </summary>
        /// <typeparam name="TSubSource">Sub-list object type</typeparam>
        /// <param name="selector">Field selector</param>
        /// <param name="build">Sub-object query building function</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddField<TSubSource>(
            Expression<Func<TSource, IEnumerable<TSubSource>>> selector,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> build)
            where TSubSource : class
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            if (build == null)
            {
                throw new ArgumentNullException(nameof(build));
            }

            PropertyInfo property = GetPropertyInfo(selector);
            string name = GetPropertyName(property);

            return AddField(name, build);
        }

        /// <summary>
        /// Adds a sub-object field to the query.
        /// </summary>
        /// <typeparam name="TSubSource">Sub-object type</typeparam>
        /// <param name="field">Field name</param>
        /// <param name="build">Sub-object query building function</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddField<TSubSource>(
            string field,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> build)
            where TSubSource : class
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException(nameof(field));
            }
            if (build == null)
            {
                throw new ArgumentNullException(nameof(build));
            }

            var query = new Query<TSubSource>(field, this.options);
            IQuery<TSubSource> subQuery = build.Invoke(query);

            this.SelectList.Add(subQuery);

            return this;
        }

        /// <summary>
        /// Adds a new argument to the query.
        /// </summary>
        /// <param name="key">Argument name</param>
        /// <param name="value">Value</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddArgument(string key, object value)
        {
            this.ArgumentsMap.Add(key, value);

            return this;
        }

        /// <summary>
        /// Adds arguments to the query.
        /// </summary>
        /// <param name="arguments">Dictionary argument</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddArguments(Dictionary<string, object> arguments)
        {
            foreach (KeyValuePair<string, object> argument in arguments)
            {
                this.ArgumentsMap.Add(argument.Key, argument.Value);
            }

            return this;
        }

        /// <sumary>
        /// Adds arguments to the query.
        /// </sumary>
        /// <typeparam name="TArguments">Arguments object type</typeparam>
        /// <param name="arguments">Arguments object</param>
        /// <returns>IQuery{TSource}</returns>
        public IQuery<TSource> AddArguments<TArguments>(TArguments arguments) where TArguments : class
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
