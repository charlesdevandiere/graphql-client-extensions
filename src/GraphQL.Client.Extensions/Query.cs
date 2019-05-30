using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// The Query Class is a simple class to build out graphQL
    /// style queries. It will build the parameters and field lists
    /// similar in a way you would use a SQL query builder to assemble
    /// a query. This will maintain the response for the query
    /// </summary>
    public class Query : IQuery
    {
        public List<object> SelectList { get; } = new List<object>();
        public Dictionary<string, object> WhereMap { get; } = new Dictionary<string, object>();
        public string QueryName { get; private set; }
        public string AliasName { get; private set; }
        public string QueryComment { get; private set; }
        public string RawQuery { get; private set; }
        public List<IQuery> BatchQueryList { get; } = new List<IQuery>();
        public IQueryStringBuilder Builder { get; } = new QueryStringBuilder();

        /// <summary>
        /// Clear the Query and anything related
        /// </summary>
        public void Clear()
        {
            // reset all member vars to clean state

            Builder.Clear();
            SelectList.Clear();
            WhereMap.Clear();
            QueryName = string.Empty;
            AliasName = string.Empty;
            QueryComment = string.Empty;
            RawQuery = string.Empty;
            BatchQueryList.Clear();
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
        public IQuery Raw(string rawQuery)
        {
            Clear();

            // scan string and find if the first non-whitespace
            // is a brace, if so we need to strip beginning and
            // ending. Otherwise leave it be.

            bool stripBraces = false;

            foreach (char c in rawQuery)
            {
                if (Char.IsWhiteSpace(c))
                {
                    continue;
                }

                // did we land on a open brace?

                if (c == '{')
                {
                    stripBraces = true;
                }

                break;
            }

            // Got the word to remove surrounding braces

            if (stripBraces)
            {
                rawQuery = rawQuery.Remove(rawQuery.IndexOf("{", StringComparison.Ordinal), 1);
                rawQuery = rawQuery.Remove(rawQuery.LastIndexOf("}", StringComparison.Ordinal), 1);
            }

            RawQuery = rawQuery;
            return this;
        }

        /// <summary>
        /// Sets the query Name
        /// </summary>
        /// <param name="queryName">The Query Name String</param>
        /// <returns>Query</returns>
        public IQuery Name(string queryName)
        {
            QueryName = queryName;

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
        /// <returns>Query</returns>
        public IQuery Alias(string alias = "")
        {
            AliasName = alias;

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
        /// <returns>Query</returns>
        public IQuery Comment(string comment = "")
        {
            QueryComment = comment;

            return this;
        }

        /// <summary>
        /// Add this list to the select part of the query. This
        /// accepts any type of list, but must be one of the types
        /// of data we support, primitives, lists, maps
        /// </summary>
        /// <param name="objectList">Generic List of select fields</param>
        /// <returns>Query</returns>
        public IQuery Select(IEnumerable<object> objectList)
        {
            SelectList.AddRange(objectList);

            return this;
        }

        /// <summary>
        /// Add a list of simple strings to the selection part of the
        /// query.
        /// </summary>
        /// <param name="selects">List of strings</param>
        /// <returns>Query</returns>
        public IQuery Select(params string[] selects)
        {
            SelectList.AddRange(selects);

            return this;
        }

        /// <summary>
        /// Adds a sub query to the list
        /// </summary>
        /// <param name="subSelect">A sub-selection, which can be just a query</param>
        /// <returns>Query</returns>
        /// <exception cref="ArgumentException"></exception>
        public IQuery Select(IQuery subSelect)
        {
            if (String.IsNullOrWhiteSpace(subSelect.QueryName))
            {
                throw new ArgumentException("Hey silly, sub-selections must have a `Name` attribute set");
            }

            SelectList.Add(subSelect);

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
        public IQuery SetArgument(string key, object where)
        {
            WhereMap.Add(key, where);

            return this;
        }

        /// <summary>
        /// Add a dict of key value pairs &lt;string, object&gt; to the existing where part
        /// </summary>
        /// <param name="dict">An existing Dictionary that takes &lt;string, object&gt;</param>
        /// <returns>Query</returns>
        /// <exception cref="ArgumentException">Dupe Key</exception>
        /// <exception cref="ArgumentNullException">Null Argument</exception>
        public IQuery SetArguments(Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> field in dict)
                WhereMap.Add(field.Key, field.Value);

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
        public IQuery Batch(IQuery query)
        {
            if (query != null)
            {
                BatchQueryList.Add(query);
            }

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

            StringBuilder strQuery = new StringBuilder();

            // If we have a RawQuery set, use that instead
            // of generating it since it can't mix with the
            // regular generation of a query, it's all or nothing
            // except for items in the batch list

            if (!string.IsNullOrWhiteSpace(RawQuery))
            {
                strQuery.Append(RawQuery);
            }
            else
            {
                // Build the regular query, validate here

                if (string.IsNullOrWhiteSpace(QueryName))
                {
                    throw new ArgumentException("Must have a `Name` specified in the Query");
                }

                // and also a select list

                if (SelectList.Count == 0)
                {
                    throw new ArgumentException("Must have a one or more `Select` fields in the Query");
                }

                Builder.Clear();
                strQuery.Append(Builder.Build(this));
            }

            // Now we have a query, check to see if we are batching

            foreach (IQuery batchQuery in BatchQueryList)
            {
                strQuery.Append(batchQuery.ToString());
            }

            return strQuery.ToString();
        }
    }
}
