using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
namespace CQPSharpService.KancolleBot.Utility {
    internal static class Helper {

        public static bool HashEquals(this byte[] my, byte[] other) {
            byte[] firstHash = MD5.Create().ComputeHash(my);
            byte[] secondHash = MD5.Create().ComputeHash(other);
            for (int i = 0; i < firstHash.Length; i++) {
                if (firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }

        public static byte[] ToArray(this Stream s) {
            var memoryStream = new MemoryStream();
            s.CopyTo(memoryStream);
            return memoryStream.ToArray();

        }

        public static DateTime ToTokyoTime(this DateTime origin) {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(origin, "Tokyo Standard Time");
        }

        public static string ToMD5String(this byte[] input) {
            using (MD5 md5 = MD5.Create()) {
                var hashBytes = md5.ComputeHash(input);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }
}
