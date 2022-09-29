namespace BecomeJPEG
{
    internal static class Util
    {
        /// <summary>
        /// Attempts to parse an integer from a specified entry in a string array.
        /// </summary>
        /// <param name="i">the integer var that receives the result</param>
        /// <param name="arr">the string array</param>
        /// <param name="index">the index of the string you want to parse</param>
        /// <param name="defaultValue">the default value if index is invalid</param>
        internal static void ParseIntFromStrArr(ref int i, string[] arr, int index, int defaultValue)
        {
            if (index < arr.Length)
                if (!int.TryParse(arr[index], out i)) //if parse fails, return the default value.
                    i = defaultValue;
        }

        /// <summary>
        /// Attempts to parse a float from a specified entry in a string array.
        /// </summary>
        /// <param name="i">the float var that receives the result</param>
        /// <param name="arr">the string array</param>
        /// <param name="index">the index of the string you want to parse</param>
        /// <param name="defaultValue">the default value if index is invalid</param>
        internal static void ParseFloatFromStrArr(ref float i, string[] arr, int index, float defaultValue)
        {
            if (index < arr.Length)
                i = float.Parse(arr[index]);
            else
                i = defaultValue;
        }
    }
}
