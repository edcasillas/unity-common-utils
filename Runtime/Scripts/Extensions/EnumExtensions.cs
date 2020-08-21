using System;
using System.Text.RegularExpressions;

namespace CommonUtils.Extensions {
	public static class EnumExtensions {
		public static int ToInt(this Enum value) => Convert.ToInt32(value);
		public static string ToText(this Enum value) => Regex.Replace(value.ToString(), "([A-Z0-9])([a-z]*)", " $1$2");
		public static TEnum ToEnumValue<TEnum>(this int i) where TEnum : Enum => (TEnum)Enum.Parse(typeof(TEnum), i.ToString());
	}
}