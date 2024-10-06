using System.ComponentModel;
using System.Reflection;

namespace Ticket.Helper
{
    public static class EnumHelper
    {
        /// <summary>
        /// Get description attribute from enum value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        /// <summary>
        /// Get value from enum description
        /// </summary>
        /// <param name="value"></param>
        // <returns></returns>
        public static T? GetEnumValueFromDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);
            FieldInfo[] fields = type.GetFields();
            var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false),
                        (f, a) => new {
                            Field = f,
                            Att = a
                        }).Where(a => ((DescriptionAttribute)a.Att).Description == description).SingleOrDefault();

            return field == null ? default(T) : (T?)field.Field.GetRawConstantValue();
        }

        public static Dictionary<string, int> EnumOrder(Enum value)
        {
            var result = new Dictionary<string, int>();
            foreach (var item in value.GetType().GetEnumValues())
            {
                result.Add((string)item, (int)item);
            }

            return result;
        }
    }
}
