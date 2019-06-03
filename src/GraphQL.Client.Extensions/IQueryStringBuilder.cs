using System.Text;

namespace GraphQL.Client.Extensions
{
    /// <summary>
    /// Query string builder interface
    /// </summary>
    public interface IQueryStringBuilder
    {
        /// <summary>
        /// May need to look at the string as it's being built
        /// </summary>
        StringBuilder QueryString { get; }

        /// <summary>
        /// Clear the QueryStringBuilder and all that entails
        /// </summary>
        void Clear();

        /// <summary>
        /// Recurse an object which could be a primitive or more
        /// complex structure. This will return a string of the value
        /// at the current level. Recursion terminates when at a terminal
        /// (primitive).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMemberInSuper.Global
        string BuildQueryParam(object value);

        /// <summary>
        /// This take all parameter data
        /// and builds the string. This will look in the query and
        /// use the WhereMap for the list of data. The data can be
        /// most any type as long as it's one that we support. Will
        /// resolve nested structures
        /// </summary>
        /// <param name="query">The Query</param>
        void AddParams<TSource>(IQuery<TSource> query) where TSource : class;

        /// <summary>
        /// Adds fields to the query sting. This will use the SelectList
        /// structure from the query to build the GraphQL select list. This
        /// will recurse as needed to pick up sub-selects that can contain
        /// parameter lists.
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="indent">Indent characters, default 0</param>
        void AddFields<TSource>(IQuery<TSource> query, int indent = 0) where TSource : class;

        /// <summary>
        /// Build the entire query into a string. This will take
        /// the query object and build a graphql query from it. This
        /// returns the query, but not outer block. This is done so
        /// you can use the output to batch the queries
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="indent">Indent characters, default = 0</param>
        /// <returns>GraphQL query string without outer block</returns>
        string Build<TSource>(IQuery<TSource> query, int indent = 0) where TSource : class;
    }
}
