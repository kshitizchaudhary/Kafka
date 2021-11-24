namespace EventSourcingDemo
{
    internal static class Extensions
    {
        public static int? ConvertToInt(this string value)
        {
            if (int.TryParse(value, out int intValue))
                return intValue;
            return null;
        }
    }
}
