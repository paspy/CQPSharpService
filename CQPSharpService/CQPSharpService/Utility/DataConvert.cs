using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPSharpService.Utility {
    /// <summary>
    /// Base64编码解码
    /// </summary>
    internal static class DataConvert {

        private static Encoding _encode = Encoding.ASCII;
        private static char[] HEX_CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToBase64(this byte[] source) => Convert.ToBase64String(source);

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static byte[] FromBase64(this string result) => Convert.FromBase64String(result);

        /// <summary>  
        /// 将字节数组转换为HEX形式的字符串, 使用指定的间隔符  
        /// </summary>  
        public static string ByteToHex(this byte[] buf, string separator) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buf.Length; i++) {
                if (i > 0) {
                    sb.Append(separator);
                }
                sb.Append(HEX_CHARS[buf[i] >> 4]).Append(HEX_CHARS[buf[i] & 0x0F]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static long ToLong(this byte[] bytes) {
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int ToInt(this byte[] bytes) {
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static short ToShort(this byte[] bytes) {
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int ToByte(this byte[] bytes) {
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] ToBin(this int num) {
            var bytes = BitConverter.GetBytes(num);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] ToBin(this long num) {
            var bytes = BitConverter.GetBytes(num);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] ToBin(this short num) {
            var bytes = BitConverter.GetBytes(num);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>  
        /// 从此实例检索子数组  
        /// </summary>  
        /// <param name="source">要检索的数组</param>  
        /// <param name="startIndex">起始索引号</param>  
        /// <param name="length">检索最大长度</param>  
        /// <returns>与此实例中在 startIndex 处开头、长度为 length 的子数组等效的一个数组</returns>  
        public static byte[] SubArray(this byte[] source, int startIndex, int length) {
            if (startIndex < 0 || startIndex > source.Length || length < 0) {
                throw new ArgumentOutOfRangeException();
            }
            var Destination = startIndex + length <= source.Length ? source.Skip(startIndex).Take(length).ToArray() : source.Skip(startIndex).ToArray();
            return Destination;
        }

        /// <summary>
        /// 翻转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Flip(this string str, int len) {
            int f = 0; --len;
            StringBuilder sb = new StringBuilder(str);
            while (f < len) {
                var p = sb[len];
                sb[len] = str[f];
                sb[f] = p;
                ++f; --len;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetProperties<T>(this T t) {
            string tStr = string.Empty;
            if (t == null) {
                return tStr;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0) {
                return tStr;
            }
            foreach (System.Reflection.PropertyInfo item in properties) {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String")) {
                    tStr += string.Format("{0}:{1},", name, value);
                } else {
                    GetProperties(value);
                }
            }
            return tStr;
        }
    }
}
