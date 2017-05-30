using System.Text.RegularExpressions;

namespace CQPSharpService.Utility {
    /// <summary>文本操作类。</summary>
    public static class StringHelper {
        /// <summary>通过正则表达式获取源字符串中所有匹配的起始和结束字符串之间的内容。</summary>
        /// <param name="sourceString">源字符串。</param>
        /// <param name="startString">起始字符串。</param>
        /// <param name="endString">结束字符串。</param>
        /// <returns>所有匹配的字符串数组，无匹配时返回Null。</returns>
        public static string[] GetMidStrings(this string sourceString, string startString, string endString) {
            MatchCollection matchCollection = new Regex("(?<=(" + startString + "))[.\\s\\S]*?(?=(" + endString + "))", RegexOptions.Multiline | RegexOptions.Singleline).Matches(sourceString);
            if (matchCollection.Count <= 0)
                return (string[])null;
            string[] strArray = new string[matchCollection.Count];
            for (int index = 0; index < matchCollection.Count; ++index)
                strArray[index] = matchCollection[index].Value;
            return strArray;
        }
    }
}
