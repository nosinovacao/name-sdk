#if NET462
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;

namespace NAME.Tests.ConnectionStrings
{
    public static class ConfigurationManagerHelpers
    {
        public static NameValueCollection SetWritable(this NameValueCollection collection)
        {
            typeof(NameObjectCollectionBase).GetProperty("IsReadOnly", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(collection, false, null);
            return collection;
        }

        public static T SetWritable<T>(this T collection) where T : ConfigurationElementCollection
        {
            var fi = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            fi.SetValue(collection, false);
            return collection;
        }
    }
}
#endif