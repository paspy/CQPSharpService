using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

using CQPSharpService.Core;

namespace CQPSharpService.Utility {

    public static class HttpHelper {


        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="userAgent">User-Agent HTTP标头。</param>
        /// <param name="accept">Accept HTTP标头。</param>
        /// <param name="timeout">超时时间。</param>
        /// <param name="header">HTTP 标头。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, string referer, string userAgent, string accept, int timeout, WebHeaderCollection header, CookieCollection cookies, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            string str = string.Empty;
            try {
                HttpWebRequest httpWebRequest;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.CheckValidationResult);
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    httpWebRequest.ProtocolVersion = HttpVersion.Version10;
                } else
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Referer = referer;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = timeout;
                httpWebRequest.Accept = accept;
                if (cookies != null) {
                    httpWebRequest.CookieContainer = new CookieContainer();
                    httpWebRequest.CookieContainer.Add(cookies);
                }
                if (header != null)
                    httpWebRequest.Headers = header;
                httpWebRequest.AutomaticDecompression = decompression;
                str = HttpHelper.GetResponseString((HttpWebResponse)httpWebRequest.GetResponse(), encoding);
            } catch {
            }
            return str;
        }

        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="header">HTTP 标头。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, string referer, WebHeaderCollection header, CookieCollection cookies, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Get(url, referer, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", 30000, header, cookies, encoding, decompression);
        }

        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, string referer, CookieCollection cookies, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Get(url, referer, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", 30000, (WebHeaderCollection)null, cookies, encoding, decompression);
        }

        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, string referer, CookieCollection cookies, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Get(url, referer, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", 30000, (WebHeaderCollection)null, cookies, Encoding.UTF8, decompression);
        }

        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, string referer, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Get(url, referer, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", 30000, (WebHeaderCollection)null, (CookieCollection)null, encoding, decompression);
        }

        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, string referer, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Get(url, referer, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", 30000, (WebHeaderCollection)null, (CookieCollection)null, Encoding.UTF8, decompression);
        }

        /// <summary>向HTTP服务器发送Get请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Get(string url, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Get(url, "", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", 30000, (WebHeaderCollection)null, (CookieCollection)null, Encoding.UTF8, decompression);
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="userAgent">User-Agent HTTP标头。</param>
        /// <param name="accept">Accept HTTP标头。</param>
        /// <param name="timeout">超时时间。</param>
        /// <param name="header">HTTP 标头。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, string referer, string userAgent, string accept, int timeout, WebHeaderCollection header, ref CookieCollection cookies, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            string str = string.Empty;
            try {
                HttpWebRequest httpWebRequest;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpHelper.CheckValidationResult);
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    httpWebRequest.ProtocolVersion = HttpVersion.Version10;
                } else
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                httpWebRequest.UserAgent = userAgent;
                httpWebRequest.Referer = referer;
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = timeout;
                httpWebRequest.Accept = accept;
                if (cookies != null) {
                    httpWebRequest.CookieContainer = new CookieContainer();
                    httpWebRequest.CookieContainer.Add(cookies);
                }
                if (header != null)
                    httpWebRequest.Headers = header;
                httpWebRequest.AutomaticDecompression = decompression;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                if (parameters != null && parameters.Count > 0) {
                    StringBuilder stringBuilder = new StringBuilder();
                    int num = 0;
                    foreach (string key in parameters.Keys) {
                        stringBuilder.AppendFormat("{0}={1}", (object)key, (object)parameters[key]);
                        if (num != parameters.Keys.Count - 1)
                            stringBuilder.Append("&");
                        ++num;
                    }
                    httpWebRequest.ContentLength = (long)stringBuilder.ToString().Length;
                    byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                    using (Stream requestStream = httpWebRequest.GetRequestStream())
                        requestStream.Write(bytes, 0, bytes.Length);
                }
                var responese = (HttpWebResponse)httpWebRequest.GetResponse();
                str = HttpHelper.GetResponseString(responese, encoding);
                cookies = responese.Cookies;
            } catch (Exception e) {
                var log = NLog.LogManager.GetCurrentClassLogger();
                log.Error(e, e.Message);
            }
            return str;
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="header">HTTP 标头。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, string referer, WebHeaderCollection header, ref CookieCollection cookies, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Post(url, parameters, referer, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", 30000, header, ref cookies, encoding, decompression);
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, string referer, ref CookieCollection cookies, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Post(url, parameters, referer, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", 30000, (WebHeaderCollection)null, ref cookies, encoding, decompression);
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="cookies">Cookies。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, string referer, ref CookieCollection cookies, DecompressionMethods decompression = DecompressionMethods.None) {
            return HttpHelper.Post(url, parameters, referer, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", 30000, (WebHeaderCollection)null, ref cookies, Encoding.UTF8, decompression);
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="encoding">文本编码。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, string referer, Encoding encoding, DecompressionMethods decompression = DecompressionMethods.None) {
            CookieCollection cookies = null;
            return HttpHelper.Post(url, parameters, referer, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", 30000, (WebHeaderCollection)null, ref cookies, encoding, decompression);
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="referer">参照页。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, string referer, DecompressionMethods decompression = DecompressionMethods.None) {
            CookieCollection cookies = null;
            return HttpHelper.Post(url, parameters, referer, "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", 30000, (WebHeaderCollection)null, ref cookies, Encoding.UTF8, decompression);
        }

        /// <summary>向HTTP服务器发送Post请求。</summary>
        /// <param name="url">请求地址。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="decompression">加密方式。</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> parameters, DecompressionMethods decompression = DecompressionMethods.None) {
            CookieCollection cookies = null;
            return HttpHelper.Post(url, parameters, "", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", 30000, (WebHeaderCollection)null, ref cookies, Encoding.UTF8, decompression);
        }

        /// <summary>
        /// 获取图像信息
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <returns>Image对象</returns>
        public static Stream GetImageStream(string url) {
            Stream s = null;
            try {
                using (WebClient client = new WebClient()) {
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    s = client.OpenRead(new Uri(url));
                }
            } catch (Exception e) {

                throw e;
            }
            return s;
        }

        /// <summary>获取图像信息</summary>
        /// <param name="url">Url地址</param>
        /// <returns>Image对象</returns>
        public static Image GetImage(string url) {
            Image image = null;
            try {
                Stream stream = GetImageStream(url);
                image = Image.FromStream(stream);
                stream.Close();
            } catch (Exception e) {

                throw e;
            }
            return image;
        }

        /// <summary>Url编码数据。</summary>
        /// <param name="data">要编码的数据。</param>
        /// <returns>编码后的数据</returns>
        public static string UrlEncode(string data) {
            return HttpUtility.UrlEncode(data);
        }

        /// <summary>Url解码。</summary>
        /// <param name="data">要解码的数据。</param>
        /// <returns>解码后的数据。</returns>
        public static string UrlDecode(string data) {
            return HttpUtility.UrlDecode(data);
        }

        /// <summary>获取请求的数据。</summary>
        /// <param name="webResponse"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static string GetResponseString(HttpWebResponse webResponse, Encoding encoding) {
            if (webResponse.ContentEncoding.ToLower().Contains("gzip")) {
                using (GZipStream gzipStream = new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress)) {
                    using (StreamReader streamReader = new StreamReader((Stream)gzipStream, encoding))
                        return streamReader.ReadToEnd();
                }
            } else if (webResponse.ContentEncoding.ToLower().Contains("deflate")) {
                using (DeflateStream deflateStream = new DeflateStream(webResponse.GetResponseStream(), CompressionMode.Decompress)) {
                    using (StreamReader streamReader = new StreamReader((Stream)deflateStream, encoding))
                        return streamReader.ReadToEnd();
                }
            } else {
                using (Stream responseStream = webResponse.GetResponseStream())
                    return new StreamReader(responseStream, encoding).ReadToEnd();
            }
        }

        /// <summary>验证证书</summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) {
            return true;
        }
    }
}
