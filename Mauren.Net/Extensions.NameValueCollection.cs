using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Mauren.Net
{
    public static class NameValueCollectionExtensions
    {
        public static NameValueCollection Append(this NameValueCollection nameValueCollection, KeyValuePair<String?, String[]?> keyValuePair) => nameValueCollection
            .Append(keyValuePair.Key, keyValuePair.Value);

        public static NameValueCollection Append(this NameValueCollection nameValueCollection, String? key, String[]? value)
        {
            // Set value to empty array if null
            value ??= Enumerable.Empty<String>().ToArray();
            return new(nameValueCollection)
            {
                [key] = String.Join(',', value),
            };
        }
    }
}
