using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CQPSharpService.Core {

    internal static class CQMessageAnalysis {

        private static object _syncRoot = new object();

        private static NLog.Logger nLogger = NLog.LogManager.GetLogger("CoolQMessage");

        public static void Analyze(string data) {
            lock (_syncRoot) {
                string[] splitMsg = data.Split(' ');
                int font = 0;
                long int64_1 = 0, int64_2 = 0, int64_3 = 0;
                string decodedStr1 = string.Empty;
                string decodedStr2 = string.Empty;
                long sendTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                var AppList = CQAppContainer.GetInstance().Apps;
                try {
                    string msgPrefix = splitMsg[0];
                    int subType = Convert.ToInt32(splitMsg[1]);
                    switch (msgPrefix) {
                        case "PrivateMessage":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            decodedStr1 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[3]));
                            nLogger.Info(string.Format("[↓][私聊] QQ：{0} {1}", int64_1, decodedStr1));
                            foreach (var app in AppList)
                                app.PrivateMessage(subType, sendTime, int64_1, decodedStr1, font);
                            break;
                        case "GroupMessage":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            decodedStr1 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[4]));
                            decodedStr2 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[5]));
                            nLogger.Info(string.Format("[↓][群聊] 群：{0} QQ：{1}{2} {3}", int64_1, int64_2, decodedStr1, decodedStr2));
                            foreach (var app in AppList)
                                app.GroupMessage(subType, sendTime, int64_1, int64_2, decodedStr1, decodedStr2, font);
                            break;
                        case "DiscussMessage":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            decodedStr1 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[4]));
                            nLogger.Info(string.Format("[↓][讨论] 组：{0} QQ：{1} {2}", int64_1, int64_2, decodedStr1));
                            foreach (var app in AppList)
                                app.DiscussMessage(subType, sendTime, int64_1, int64_2, decodedStr1, font);
                            break;
                        case "GroupUpload":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            decodedStr1 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[4]));
                            nLogger.Info(string.Format("[↓][上传] 群：{0} QQ：{1} {2}", int64_1, int64_2, decodedStr1));
                            foreach (var app in AppList)
                                app.GroupUpload(subType, sendTime, int64_1, int64_2, decodedStr1);
                            break;
                        case "GroupAdmin":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            nLogger.Info(string.Format("[↓][管理] 群：{0} QQ：{1}", int64_1, int64_2));
                            foreach (var app in AppList)
                                app.GroupAdmin(subType, sendTime, int64_1, int64_2);
                            break;
                        case "GroupMemberDecrease":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            int64_3 = Convert.ToInt64(splitMsg[4]);
                            nLogger.Info(string.Format("[↓][减员] 群：{0} QQ：{1} 目标QQ：{2}", int64_1, int64_2, int64_3));
                            foreach (var app in AppList)
                                app.GroupMemberDecrease(subType, sendTime, int64_1, int64_2, int64_3);
                            break;
                        case "GroupMemberIncrease":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            int64_3 = Convert.ToInt64(splitMsg[4]);
                            nLogger.Info(string.Format("[↓][增员] 群：{0} QQ：{1} 目标QQ：{2}", int64_1, int64_2, int64_3));
                            foreach (var app in AppList)
                                app.GroupMemberIncrease(subType, sendTime, int64_1, int64_2, int64_3);
                            break;
                        case "FriendAdded":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            nLogger.Info(string.Format("[↓][加友] QQ：{0}", int64_1));
                            foreach (var app in AppList)
                                app.FriendAdded(subType, sendTime, int64_1);
                            break;
                        case "RequestAddFriend":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            decodedStr1 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[3]));
                            decodedStr2 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[4]));
                            nLogger.Info(string.Format("[↓][请友] QQ：{0} {1} [{2}]", int64_1, decodedStr1, decodedStr2));
                            foreach (var app in AppList)
                                app.RequestAddFriend(subType, sendTime, int64_1, decodedStr1, decodedStr2);
                            break;
                        case "RequestAddGroup":
                            int64_1 = Convert.ToInt64(splitMsg[2]);
                            int64_2 = Convert.ToInt64(splitMsg[3]);
                            decodedStr1 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[4]));
                            decodedStr2 = Encoding.Default.GetString(Convert.FromBase64String(splitMsg[5]));
                            nLogger.Info(string.Format("[↓][请群] 群：{0} QQ：{1} {2} [{3}]", int64_1, int64_2, decodedStr1, decodedStr2));
                            foreach (var app in AppList)
                                app.RequestAddGroup(subType, sendTime, int64_1, int64_2, decodedStr1, decodedStr2);
                            break;
                        default:
                            break;
                    }
                } catch (Exception e) {
                    nLogger.Error(e, string.Format("[X][异常] {0}", data));
                }
            }
        }

    }
}
