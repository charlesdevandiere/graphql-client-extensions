using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace GraphQL.Client.Extensions
{
    public class Query<TSource> : Query, IQuery<TSource> where TSource : class
    {
        public new IQuery<TSource> Raw(string rawQuery)
        {
            base.Raw(rawQuery);

            return this;
        }

        public new IQuery<TSource> Name(string queryName)
        {
            base.Name(queryName);

            return this;
        }
        
        public new IQuery<TSource> Alias(string alias)
        {
            base.Alias(alias);

            return this;
        }

        public new IQuery<TSource> Comment(string comment)
        {
            base.Comment(comment);

            return this;
        }

        public IQuery<TSource> Select<TProperty>(Expression<Func<TSource, TProperty>> lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException(nameof(lambda));
            }

            PropertyInfo property = GetPropertyInfo(lambda);
            if (!TryGetJsonPropertyName(property, out string name))
            {
                name = property.Name;
            }

            base.Select(name);

            return this;
        }

        public new IQuery<TSource> Where(string key, object where)
        {
            base.Where(key, where);

            return this;
        }

        public new IQuery<TSource> Where(Dictionary<string, object> dict)
        {
            base.Where(dict);

            return this;
        }

        public new IQuery<TSource> Batch(IQuery query)
        {
            base.Batch(query);

            return this;
        }

        public IQuery<TSource> SubSelect<TSubSource>(Expression<Func<TSource, TSubSource>> lambda, IQuery<TSubSource> subQuery) where TSubSource : class
        {
            if (lambda == null)
            {
                throw new ArgumentNullException(nameof(lambda));
            }
            if (subQuery == null)
            {
                throw new ArgumentNullException(nameof(subQuery));
            }

            PropertyInfo property = GetPropertyInfo(lambda);
            if (!TryGetJsonPropertyName(property, out string name))
            {
                name = property.Name;
            }
            subQuery.Name(name);

            base.Select(subQuery);

            return this;
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

        private static bool TryGetJsonPropertyName(PropertyInfo property, out string name)
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
                    name = (attribute as JsonPropertyAttribute).PropertyName;

                    return true;
                }
            }

            name = null;

            return false;
        }
    }
}