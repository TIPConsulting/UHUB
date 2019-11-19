using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// Object extension methods
    /// </summary>
    public static class ObjectExtensions
    {
        class NullableValueProvider : IValueProvider
        {
            private readonly object _defaultValue;
            private readonly IValueProvider _underlyingValueProvider;


            public NullableValueProvider(MemberInfo memberInfo, Type underlyingType)
            {
                _underlyingValueProvider = new DynamicValueProvider(memberInfo);
                _defaultValue = Activator.CreateInstance(underlyingType);
            }

            public void SetValue(object target, object value)
            {
                _underlyingValueProvider.SetValue(target, value);
            }

            public object GetValue(object target)
            {
                return _underlyingValueProvider.GetValue(target) ?? _defaultValue;
            }
        }

        class SpecialContractResolver : DefaultContractResolver
        {
            protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    var pi = (PropertyInfo)member;
                    if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return new NullableValueProvider(member, pi.PropertyType.GetGenericArguments().First());
                    }
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    var fi = (FieldInfo)member;
                    if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        return new NullableValueProvider(member, fi.FieldType.GetGenericArguments().First());
                }

                return base.CreateMemberValueProvider(member);
            }
        }


        /// <summary>
        /// Convert object to formatted JSON.  Multiline easy-read format
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToFormattedJSON(this object obj)
        {

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.QuoteName = false;
                    writer.QuoteChar = '"';
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    writer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    writer.Indentation = 5;
                    writer.IndentChar = ' ';

                    JsonSerializer ser = new JsonSerializer
                    {
                        DefaultValueHandling = DefaultValueHandling.Include,
                        NullValueHandling = NullValueHandling.Include
                    };
                    ser.Serialize(writer, obj);
                }
            }

            return sb.ToString();
        }


        public static string GetFieldName<T, K>(Expression<Func<T, K>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }
    }
}
