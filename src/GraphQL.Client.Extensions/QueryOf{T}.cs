using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace GraphQL.Client.Extensions
{
    public class Query<TSource> : Query where TSource : class
    {
        public IQuery Select<TProperty>(Expression<Func<TSource, TProperty>> lambda)
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

            return this.Select(name);
        }

        public IQuery SubSelect<TSubSource>(Expression<Func<TSource, TSubSource>> lambda, Query<TSubSource> subQuery) where TSubSource : class
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

            return this.Select(subQuery);
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