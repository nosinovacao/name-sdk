using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAME.Json
{
    internal class SimpleJsonPathValueSystem : IJsonPathValueSystem
    {

        public bool HasMember(object value, string member)
        {
            if (value is JsonClass)
                return (value as JsonClass)[member] != null;
            if (value is JsonArray)
            {
                int index = this.ParseInt(member, -1);
                return index >= 0 && index < (value as JsonArray).Count;
            }

            return false;
        }

        public IEnumerable<string> GetMembers(object value)
        {
            var valueObject = value as JsonClass;
            if (valueObject != null)
            {
                foreach (KeyValuePair<string, JsonNode> item in valueObject)
                {
                    yield return item.Key;
                }
            }
        }

        public object GetMemberValue(object value, string member)
        {
            if (value is JsonClass)
                return (value as JsonClass)[member];

            if (value is JsonArray)
            {
                int index = this.ParseInt(member, -1);
                return (value as JsonArray)[index];
            }

            return null;
        }

        public bool IsArray(object value)
        {
            return value is JsonArray;
        }

        public bool IsObject(object value)
        {
            return value is JsonClass;
        }

        public bool IsPrimitive(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return value is JsonClass || value is JsonArray ? false : true;
        }

        private int ParseInt(string s, int defaultValue)
        {
            int result;
            return int.TryParse(s, out result) ? result : defaultValue;
        }
    }
}
