using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CQPSharpService.Utility;
using Newtonsoft.Json.Linq;

namespace CQPSharpService.Core {
    /// <summary>
    /// 封装非酷Q官方提供的方法静态类。
    /// </summary>
    public static class CQE {

        /// <summary>获取可以直接被C#使用的登录QQ的CookieCollection对象。</summary>
        /// <param name="domain">此 Cookie 与其关联的域。</param>
        /// <returns>登录QQ的 <see cref="T:System.Net.CookieCollection" /> 对象。</returns>
        public static CookieCollection GetCookies(string domain) {
            CookieCollection cookieCollection = new CookieCollection();
            string cookies = CQ.GetCookies();
            char[] chArray1 = new char[1] { ';' };
            foreach (string str in cookies.Split(chArray1)) {
                char[] chArray2 = new char[1] { '=' };
                string[] strArray = str.Split(chArray2);
                if (strArray.Length == 2) {
                    Cookie cookie = new Cookie(strArray[0].Trim(), HttpHelper.UrlDecode(strArray[1].Trim()), "/", domain);
                    cookieCollection.Add(cookie);
                }
            }
            return cookieCollection;
        }

        /// <summary>取QQ昵称。</summary>
        /// <param name="qqNumber">QQ号码。</param>
        /// <returns>昵称。</returns>
        public static string GetQQName(long qqNumber) {
            CQLogger.GetInstance().AddLog(string.Format("[↓][昵称] QQ：{0}", qqNumber));
            string sourceString = HttpHelper.Get(
                string.Format("http://r.pengyou.com/fcg-bin/cgi_get_portrait.fcg?uins={0}&get_nick=1&_=1438937421131", qqNumber),
                "r.pengyou.com", Encoding.GetEncoding("GB2312"), DecompressionMethods.None);
            if (sourceString.Contains("error"))
                return "暂无昵称";
            string[] midStrings = sourceString.GetMidStrings(",\"", "\",");
            if (midStrings.Length > 0)
                return midStrings[0];
            return "暂无昵称";
        }

        /// <summary>
        /// 取登陆QQ所在群的列表
        /// </summary>
        /// <returns></returns>
        public static List<CQGroupInfo> GetGroupList() {
            List<CQGroupInfo> lst = new List<CQGroupInfo>();
            try {
                CQLogger.GetInstance().AddLog("[↓][帐号] 取群列表");
                string url = "http://qun.qq.com/cgi-bin/qun_mgr/get_group_list";
                string token = CQ.GetCsrfToken().ToString();
                var postData = new Dictionary<string, string>() {
                    { "bkn", CQ.GetCsrfToken().ToString() }
                };
                var cookies = GetCookies("qun.qq.com");
                string sourceString =
                    HttpHelper.Post(
                        url, postData, "http://qun.qq.com/member.html", ref cookies);
                var strReg = "{\"gc\":([1-9][0-9]{4,10}),\"gn\":\"(.*?)\",\"owner\":([1-9][0-9]{4,10})}";
                Regex reg = new Regex(strReg);
                MatchCollection matches = reg.Matches(sourceString);
                foreach (Match match in matches) {
                    dynamic g = JToken.Parse(match.Value);
                    var gInfo = new CQGroupInfo() { Name = HttpUtility.HtmlDecode((string)g.gn), Number = (long)g.gc, Owner = (long)g.owner };
                    lst.Add(gInfo);
                    CQLogger.GetInstance().AddLog(string.Format("Name: {0}, Number: {1}, Owner: {2}", gInfo.Name, gInfo.Number, gInfo.Owner));
                }

            } catch (Exception e) {
                CQLogger.GetInstance().AddLog("GetGroupList " + e.Message);
                lst = null;
            }

            return lst;
        }
    }
}
