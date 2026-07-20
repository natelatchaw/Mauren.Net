using System;

namespace Mauren.Net
{
    public static class UriExtensions
    {
        /// <summary>
        /// The default seperator for Uri paths.
        /// </summary>
        private const Char PathSeperator = '/';

        public static Uri AppendPath(this Uri uri, String path)
        {
            // Initialize a UriBuilder with the provided Uri
            UriBuilder builder = new(uri);
            // Trim path seperators from the provided Uri's path
            String parentPath = builder.Path.Trim(PathSeperator);
            // Trim path seperators from the provided path
            String childPath = path.Trim(PathSeperator);
            // Set the path to the joined path segments
            builder.Path = String.Join(PathSeperator, parentPath, childPath);
            // Return the builder's Uri
            return builder.Uri;
        }
    }
}
