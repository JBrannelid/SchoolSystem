public static class StringExtensions
{
    public static string SplitCamelCase(this string inputString)
    {
        string result = "";

        // This foreach loop is necessary for displaying Enum values as readable menu choices for the user.
        // It splits a CamelCase string (Enum names) into separate words by adding spaces before each uppercase letter.
        foreach (char character in inputString)
        {

            if (char.IsUpper(character) && result.Length > 0)
            {
                result += " "; 
            }

            result += character;
        }

        return result;
    }
}
