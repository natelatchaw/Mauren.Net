using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Mauren.Net.Test
{
    internal class MockEndpoint : IEndpoint
    {
        public MockEndpoint()
        {
            PathSegments = new List<String>();
            QueryParameters = new NameValueCollection();
        }

        public List<String> PathSegments { get; set; }
        public NameValueCollection QueryParameters { get; set; }
    }
}
