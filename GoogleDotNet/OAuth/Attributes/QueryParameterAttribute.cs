using System;

namespace GoogleDotNet.OAuth.Attributes
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class QueryParameterAttribute : Attribute
    {
        public string Name { get; set; }

        public QueryParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
