using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CQPSharpService.Utility {
    internal class Unpack {

        private byte[] m_bin;
        private int location;

        /// <summary>
        /// 
        /// </summary>
        public void Empty() {
            m_bin = new byte[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetAll() {
            return m_bin.SubArray(location, m_bin.Length - location);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] GetBin(int len) {
            if (len <= 0) {
                return null;
            }
            var bin = m_bin.SubArray(location, len);
            location += len;
            return bin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte GetByte() {
            var bin = (byte)m_bin.SubArray(location, 1).GetValue(0);
            location += 1;
            return bin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetInt() {
            var bin = m_bin.SubArray(location, 4);
            location += 4;
            return bin.ToInt();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetLong() {
            var bin = m_bin.SubArray(location, 8);
            location += 8;
            return bin.ToLong();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public short GetShort() {
            var bin = m_bin.SubArray(location, 2);
            location += 2;
            return bin.ToShort(); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLenStr() {
            var len = GetShort();
            var bytes = GetBin(len);
            try {
                if (bytes == null) {
                    return string.Empty;
                }
                return Encoding.GetEncoding("GB2312").GetString(bytes);
            } catch (Exception e){
                Debug.WriteLine(e.Message);
                return "";
            }

            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetToken() {
            var len = GetShort();
            return GetBin(len);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Len() {
            return m_bin.Length - location;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void SetData(byte[] b) {
            m_bin = b;
            location = 0;
        }
    }
}
