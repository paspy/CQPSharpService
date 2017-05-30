using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CQPSharpService.Utility;
using NLog;

namespace CQPSharpService.Core {

    public static class CQ {

        private static object _syncRoot = new object();
        private static Logger nLogger = LogManager.GetCurrentClassLogger();

        #region CoolQ Code

        /// <summary>获取 @指定QQ 的操作代码。</summary>
        /// <param name="qqNumber">指定的QQ号码。
        /// <para>当该参数为-1时，操作为 @全部成员。</para></param>
        /// <returns>CQ @操作代码。</returns>
        public static string CQCode_At(long qqNumber) {
            return "[CQ:at,qq=" + (qqNumber == -1L ? "all" : qqNumber.ToString()) + "]";
        }

        /// <summary>获取 指定的emoji表情代码。</summary>
        /// <param name="id">emoji表情索引ID。</param>
        /// <returns>CQ emoji表情代码。</returns>
        public static string CQCode_Emoji(int id) {
            return "[CQ:emoji,id=" + id + "]";
        }


        /// <summary>获取 指定的表情代码。</summary>
        /// <param name="id">表情索引ID。</param>
        /// <returns>CQ 表情代码。</returns>
        public static string CQCode_Face(int id) {
            return "[CQ:face,id=" + id + "]";
        }

        /// <summary>获取 窗口抖动代码。</summary>
        /// <returns>CQ 窗口抖动代码。</returns>
        public static string CQCode_Shake() {
            return "[CQ:shake]";
        }

        /// <summary>获取 匿名代码。</summary>
        /// <param name="ignore">是否不强制。</param>
        /// <returns>CQ 匿名代码。</returns>
        public static string CQCode_Anonymous(bool ignore = false) {
            return "[CQ:anonymous" + (ignore ? ",ignore=true" : "") + "]";
        }

        /// <summary>获取 发送图片代码。</summary>
        /// <param name="fileName">图片路径。</param>
        /// <returns>CQ 发送图片代码。</returns>
        public static string CQCode_Image(string fileName) {
            return "[CQ:image,file=" + fileName + "]";
        }

        /// <summary>获取 发送音乐代码。</summary>
        /// <param name="id">音乐索引ID。</param>
        /// <returns>CQ 发送音乐代码。</returns>
        public static string CQCode_Music(int id) {
            return "[CQ:music,id=" + id + "]";
        }

        /// <summary>获取 发送语音代码。</summary>
        /// <param name="fileName">语音文件路径。</param>
        /// <returns>CQ 发送语音代码。</returns>
        public static string CQCode_Record(string fileName) {
            return "[CQ:record,file=" + fileName + "]";
        }

        /// <summary>获取 链接分享代码。</summary>
        /// <param name="url">链接地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="imageUrl">图片地址。</param>
        /// <returns>CQ 链接分享代码。</returns>
        public static string CQCode_ShareLink(string url, string title, string content, string imageUrl) {
            return string.Format("[CQ:share,url={0},title={1},content={2},image={3}]", url, title, content, imageUrl);
        }
        #endregion

        #region Send Message
        /// <summary>发送私聊消息。</summary>
        /// <param name="qqNumber">QQ号码。</param>
        /// <param name="message">私聊消息内容。</param>
        public static void SendPrivateMessage(long qqNumber, string message) {
            nLogger.Info(string.Format("[↑][私聊] QQ：{0} {1}", qqNumber, message));
            CQUDPProxy.GetInstance().SendMessage(string.Format("PrivateMessage {0} {1}", qqNumber, Convert.ToBase64String(Encoding.Default.GetBytes(message))));
        }

        /// <summary>发送群消息。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="message">群消息内容。</param>
        public static void SendGroupMessage(long groupNumber, string message) {
            nLogger.Info(string.Format("[↑][群聊] 群：{0} {1}", groupNumber, message));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupMessage {0} {1}", groupNumber, Convert.ToBase64String(Encoding.Default.GetBytes(message))));
        }

        /// <summary>发送讨论组消息。</summary>
        /// <param name="discussNumber">讨论组号码。</param>
        /// <param name="message">论组消息内容。</param>
        public static void SendDiscussMessage(long discussNumber, string message) {
            nLogger.Info(string.Format("[↑][讨论] QQ：{0} {1}", discussNumber, message));
            CQUDPProxy.GetInstance().SendMessage(string.Format("DiscussMessage {0} {1}", discussNumber, Convert.ToBase64String(Encoding.Default.GetBytes(message))));
        }

        /// <summary>发送赞。</summary>
        /// <param name="qqNumber">被操作的QQ。</param>
        /// <param name="times">次数，谨慎增加次数</param>
        public static void SendLike(long qqNumber, long times = 1) {
            nLogger.Info(string.Format("[↑][发赞] QQ：{0} 次数：{1}", qqNumber, times));
            CQUDPProxy.GetInstance().SendMessage(string.Format("Like {0} {1}", qqNumber, times));
        }
        #endregion

        #region Management 
        /// <summary>置群员移除。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="qqNumber">被操作的QQ号码。</param>
        /// <param name="refuse">是否拒绝再次加群。</param>
        public static void SetGroupMemberRemove(long groupNumber, long qqNumber, bool refuse = false) {
            nLogger.Info(string.Format("[↑][踢人] 群：{0} QQ：{1} 拒绝再申请：{2}", groupNumber, qqNumber, refuse ? "是" : "否"));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupKick {0} {1} {2}", groupNumber, qqNumber, refuse ? 1 : 0));
        }

        /// <summary>置群员禁言。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="qqNumber">被操作的QQ号码。</param>
        /// <param name="time">禁言时长（以秒为单位)</param>
        public static void SetGroupMemberBan(long groupNumber, long qqNumber, long time) {
            nLogger.Info(string.Format("[↑][禁言] 群：{0} QQ：{1} 时间：{2} 秒", groupNumber, qqNumber, time));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupBan {0} {1} {2}", groupNumber, qqNumber, time));
        }

        /// <summary>置群管理员。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="qqNumber">被操作的QQ号码。</param>
        /// <param name="admin">是否设置为管理员。</param>
        public static void SetGroupAdministrator(long groupNumber, long qqNumber, bool admin) {
            nLogger.Info(string.Format("[↑][管理] 群：{0} QQ：{1}", groupNumber, qqNumber, admin ? "提升为管理员" : "降为群员"));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupAdmin {0} {1} {2}", groupNumber, qqNumber, admin ? 1 : 0));
        }

        /// <summary>置全群禁言。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="gag">设置或关闭全群禁言。</param>
        public static void SetGroupWholeBan(long groupNumber, bool enable) {
            nLogger.Info(string.Format("[↑][禁言] 群：{0} QQ：{1}", groupNumber, enable ? "全员禁言" : "取消全员禁言"));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupWholeBan {0} {1}", groupNumber, enable ? 1 : 0));
        }

        /// <summary>置匿名群员禁言。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="anomymous">被操作的匿名成员名称。</param>
        /// <param name="time">禁言时长（以秒为单位)</param>
        public static void SetGroupAnonymousMemberBan(long groupNumber, string anomymous, long time) {
            nLogger.Info(string.Format("[↑][禁言] 群：{0} 匿名：{1} 时长：{2}", groupNumber, anomymous, time));
            CQUDPProxy.GetInstance().SendMessage(
                string.Format("GroupAnonymousBan {0} {1} {2}", groupNumber, Convert.ToBase64String(Encoding.Default.GetBytes(anomymous)), time));
        }

        /// <summary>置群匿名设置。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="allow">开启或关闭匿名功能。</param>
        public static void SetGroupAllowAnonymous(long groupNumber, bool enable) {
            nLogger.Info(string.Format("[↑][禁言] 群：{0} QQ：{1}", groupNumber, enable ? "开启匿名" : "关闭匿名"));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupAnonymous {0} {1}", groupNumber, enable ? 1 : 0));
        }

        /// <summary>置群成员名片</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="qqNumber">被操作的QQ号码。</param>
        /// <param name="newName">新的群名称。</param>
        public static void SetGroupNickName(long groupNumber, long qqNumber, string newName) {
            nLogger.Info(string.Format("[↑][名片] 群：{0} QQ：{1} {2}", groupNumber, qqNumber, newName));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupCard {0} {1} {2}", groupNumber, qqNumber, Convert.ToBase64String(Encoding.Default.GetBytes(newName))));
        }

        /// <summary>置群退出。</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="isdismiss">是否解散。</param>
        public static void SetGroupLeave(long groupNumber, bool isDismiss = false) {
            nLogger.Info(string.Format("[↑][退群] 群：{0} {1}", groupNumber, isDismiss ? "[解散]" : ""));
            CQUDPProxy.GetInstance().SendMessage(string.Format("GroupLeave {0} {1}", groupNumber, isDismiss ? 0 : 1));
        }

        /// <summary>置群成员专属头衔</summary>
        /// <param name="groupNumber">群号码。</param>
        /// <param name="qqNumber">被操作的QQ号码。</param>
        /// <param name="newName">头衔名称。</param>
        /// <param name="time">过期时间（以秒为单位）。</param>
        public static void SetGroupSpecialTitle(long groupNumber, long qqNumber, string newTitle, long duration) {
            nLogger.Info(string.Format("[↑][头衔] 群：{0} QQ：{1} {2} 时间：{3}", groupNumber, qqNumber, newTitle, duration));
            CQUDPProxy.GetInstance().SendMessage(
                string.Format("GroupSpecialTitle {0} {1} {2} {3}", groupNumber, qqNumber, Convert.ToBase64String(Encoding.Default.GetBytes(newTitle)), duration));
        }

        /// <summary>置讨论组退出。</summary>
        /// <param name="discussNumber">讨论组号码。</param>
        public static void SetDiscussLeave(long discussNumber) {
            nLogger.Info(string.Format("[↑][退组] 组：{0}", discussNumber));
            CQUDPProxy.GetInstance().SendMessage(string.Format("DiscussLeave {0}", discussNumber));
        }

        /// <summary>置好友添加请求。</summary>
        /// <param name="react">请求反馈标识。</param>
        /// <param name="reactType">反馈类型。</param>
        /// <param name="description">备注。</param>
        public static void SetFriendAddRequest(string react, CQReactType reactType, string description = "") {
            nLogger.Info(string.Format("[↑][请友] {0} {1} {2}", react, reactType, description));
            CQUDPProxy.GetInstance().SendMessage(
                string.Format("FriendAddRequest {0} {1} {2}",
                Convert.ToBase64String(Encoding.Default.GetBytes(react)),
                (int)reactType,
                Convert.ToBase64String(Encoding.Default.GetBytes(description))));
        }

        /// <summary>置群添加请求。</summary>
        /// <param name="react">请求反馈标识。</param>
        /// <param name="requestType">请求类型。</param>
        /// <param name="reactType">反馈类型。</param>
        /// <param name="reason">加群原因。</param>
        public static void SetGroupAddRequest(string react, CQRequestType requestType, CQReactType reactType, string reason = "") {
            nLogger.Info(string.Format("[↑][请群] {0} {1} {2} {3}", react, requestType, reactType, reason));
            CQUDPProxy.GetInstance().SendMessage(
                string.Format("GroupAddRequest {0} {1} {2} {3}",
                Convert.ToBase64String(Encoding.Default.GetBytes(react)),
                (int)requestType,
                (int)reactType,
                Convert.ToBase64String(Encoding.Default.GetBytes(reason))));
        }
        #endregion

        #region Retrieve Infomation
        /// <summary>取登录QQ。</summary>
        /// <returns>登录的QQ号码</returns>
        public static long GetLoginQQ() {
            try {
                lock (_syncRoot) {
                    nLogger.Info(string.Format("[↑][帐号] 取登录QQ"));
                    var qq = Convert.ToInt64(CQUDPProxy.GetInstance().GetStringMessage(string.Format("LoginQQ")));
                    nLogger.Info(string.Format("[↓][帐号] 登录QQ: {0}", qq));
                    return qq;
                }
            } catch (Exception e) {
                nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
            }
            return 0;
        }

        /// <summary>取登录昵称。</summary>
        /// <returns>登录的QQ号码昵称。</returns>
        public static string GetLoginName() {
            try {
                lock (_syncRoot) {
                    nLogger.Info(string.Format("[↑][帐号] 取登录昵称"));
                    var nickname = Encoding.Default.GetString(Convert.FromBase64String(CQUDPProxy.GetInstance().GetStringMessage(string.Format("LoginNick"))));
                    nLogger.Info(string.Format("[↓][帐号] 登录昵称: {0}", nickname));
                    return nickname;
                }
            } catch (Exception e) {
                nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
            }
            return string.Empty;
        }

        /// <summary>取Cookies。</summary>
        /// <returns>登录的Cookies。</returns>
        public static string GetCookies() {
            try {
                lock (_syncRoot) {
                    nLogger.Info(string.Format("[↑][帐号] 取Cookies"));
                    var cookie = Encoding.Default.GetString(Convert.FromBase64String(CQUDPProxy.GetInstance().GetStringMessage(string.Format("Cookies"))));
                    nLogger.Info(string.Format("[↑][帐号] Cookies: {0}", cookie));
                    return cookie;
                }
            } catch (Exception e) {
                nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
            }
            return string.Empty;
        }

        /// <summary>取CsrfToken。</summary>
        /// <returns>登录的CsrfToken。</returns>
        public static int GetCsrfToken() {
            try {
                lock (_syncRoot) {
                    nLogger.Info(string.Format("[↑][帐号] 取CsrfToken"));
                    var token = Convert.ToInt32(CQUDPProxy.GetInstance().GetStringMessage(string.Format("CsrfToken")));
                    nLogger.Info(string.Format("[↓][帐号] CsrfToken: {0}", token));
                    return token;
                }
            } catch (Exception e) {
                nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
            }
            return 0;
        }

        /// <summary>获取酷Q插件的目录。</summary>
        /// <returns></returns>
        public static string GetCQAppFolder() {
            try {
                lock (_syncRoot) {
                    nLogger.Info(string.Format("[↑][帐号] 取酷Q插件目录"));
                    var appDir = Encoding.Default.GetString(Convert.FromBase64String(CQUDPProxy.GetInstance().GetStringMessage(string.Format("AppDirectory"))));
                    nLogger.Info(string.Format("[↓][帐号] 酷Q插件目录: {0}", appDir));
                    return appDir;
                }
            } catch (Exception e) {
                nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
            }
            return string.Empty;
        }

        /// <summary>获取C#插件的应用目录。</summary>
        /// <returns>应用目录。</returns>
        public static string GetCSPluginsFolder() {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSharpPlugins");
        }

        /// <summary>
        /// 取群成员信息
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <param name="qq">QQ号</param>
        /// <param name="nocache">取缓存</param>
        /// <returns></returns>
        public static CQGroupMemberInfo GetGroupMemberInfo(long groupId, long qq, bool nocache = false) {
            try {
                lock (_syncRoot) {
                    nLogger.Info(string.Format("[↑][帐号] 取群成员信息"));
                    var data = CQUDPProxy.GetInstance().GetStringMessage(
                        string.Format("GroupMemberInfo {0} {1} {2}", groupId, qq, nocache ? 0 : 1)
                    );

                    var memberBytes = Convert.FromBase64String(data);
                    var info = new CQGroupMemberInfo();
                    var groupNumberBytes = new byte[8];

                    Array.Copy(memberBytes, 0, groupNumberBytes, 0, 8);
                    Array.Reverse(groupNumberBytes);
                    info.GroupId = BitConverter.ToInt64(groupNumberBytes, 0);

                    var qqNumberBytes = new byte[8];
                    Array.Copy(memberBytes, 8, qqNumberBytes, 0, 8);
                    Array.Reverse(qqNumberBytes);
                    info.Number = BitConverter.ToInt64(qqNumberBytes, 0);

                    var nameLengthBytes = new byte[2];
                    Array.Copy(memberBytes, 16, nameLengthBytes, 0, 2);
                    Array.Reverse(nameLengthBytes);
                    var nameLength = BitConverter.ToInt16(nameLengthBytes, 0);

                    var nameBytes = new byte[nameLength];
                    Array.Copy(memberBytes, 18, nameBytes, 0, nameLength);
                    info.NickName = Encoding.Default.GetString(nameBytes);

                    var cardLengthBytes = new byte[2];
                    Array.Copy(memberBytes, 18 + nameLength, cardLengthBytes, 0, 2);
                    Array.Reverse(cardLengthBytes);
                    var cardLength = BitConverter.ToInt16(cardLengthBytes, 0);

                    var cardBytes = new byte[cardLength];
                    Array.Copy(memberBytes, 20 + nameLength, cardBytes, 0, cardLength);
                    info.InGroupName = Encoding.Default.GetString(cardBytes);

                    var genderBytes = new byte[4];
                    Array.Copy(memberBytes, 20 + nameLength + cardLength, genderBytes, 0, 4);
                    Array.Reverse(genderBytes);
                    info.Gender = BitConverter.ToInt32(genderBytes, 0) == 0 ? "男" : " 女";

                    var ageBytes = new byte[4];
                    Array.Copy(memberBytes, 24 + nameLength + cardLength, ageBytes, 0, 4);
                    Array.Reverse(ageBytes);
                    info.Age = BitConverter.ToInt32(ageBytes, 0);

                    var areaLengthBytes = new byte[2];
                    Array.Copy(memberBytes, 28 + nameLength + cardLength, areaLengthBytes, 0, 2);
                    Array.Reverse(areaLengthBytes);
                    var areaLength = BitConverter.ToInt16(areaLengthBytes, 0);

                    var areaBytes = new byte[areaLength];
                    Array.Copy(memberBytes, 30 + nameLength + cardLength, areaBytes, 0, areaLength);
                    info.Area = Encoding.Default.GetString(areaBytes);

                    var addGroupTimesBytes = new byte[4];
                    Array.Copy(memberBytes, 30 + nameLength + cardLength + areaLength, addGroupTimesBytes, 0, 4);
                    Array.Reverse(addGroupTimesBytes);
                    info.JoinTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime().AddSeconds(BitConverter.ToInt32(addGroupTimesBytes, 0));

                    var lastSpeakTimesBytes = new byte[4];
                    Array.Copy(memberBytes, 34 + nameLength + cardLength + areaLength, lastSpeakTimesBytes, 0, 4);
                    Array.Reverse(lastSpeakTimesBytes);
                    info.LastSpeakingTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime().AddSeconds(BitConverter.ToInt32(lastSpeakTimesBytes, 0));

                    var levelNameLengthBytes = new byte[2];
                    Array.Copy(memberBytes, 38 + nameLength + cardLength + areaLength, levelNameLengthBytes, 0, 2);
                    Array.Reverse(levelNameLengthBytes);
                    var levelNameLength = BitConverter.ToInt16(levelNameLengthBytes, 0);

                    var levelNameBytes = new byte[levelNameLength];
                    Array.Copy(memberBytes, 40 + nameLength + cardLength + areaLength, levelNameBytes, 0, levelNameLength);
                    info.Level = Encoding.Default.GetString(levelNameBytes);

                    var authorBytes = new byte[4];
                    Array.Copy(memberBytes, 40 + nameLength + cardLength + areaLength + levelNameLength, authorBytes, 0, 4);
                    Array.Reverse(authorBytes);
                    var authority = BitConverter.ToInt32(authorBytes, 0);
                    info.Authority = authority.ToString();

                    var badBytes = new byte[4];
                    Array.Copy(memberBytes, 44 + nameLength + cardLength + areaLength + levelNameLength, badBytes, 0, 4);
                    Array.Reverse(badBytes);
                    info.HasBadRecord = BitConverter.ToInt32(badBytes, 0) == 1;

                    var titleLengthBytes = new byte[2];
                    Array.Copy(memberBytes, 48 + nameLength + cardLength + areaLength + levelNameLength, titleLengthBytes, 0,
                        2);
                    Array.Reverse(titleLengthBytes);
                    var titleLength = BitConverter.ToInt16(titleLengthBytes, 0);

                    var titleBytes = new byte[titleLength];
                    Array.Copy(memberBytes, 50 + nameLength + cardLength + areaLength + levelNameLength, titleBytes, 0,
                        titleLength);
                    info.Title = Encoding.Default.GetString(titleBytes);

                    var titleExpireBytes = new byte[4];
                    Array.Copy(memberBytes, 50 + nameLength + cardLength + areaLength + levelNameLength + titleLength,
                        titleExpireBytes, 0, 4);
                    Array.Reverse(titleExpireBytes);
                    info.TitleExpirationTime = BitConverter.ToInt32(titleExpireBytes, 0);

                    var modifyCardBytes = new byte[4];
                    Array.Copy(memberBytes, 54 + nameLength + cardLength + areaLength + levelNameLength + titleLength,
                        titleExpireBytes, 0, 4);
                    Array.Reverse(titleExpireBytes);
                    info.CanModifyInGroupName = BitConverter.ToInt32(modifyCardBytes, 0) == 1;

                    nLogger.Info(string.Format("[↓][帐号] 群成员: 昵称：{0} QQ：{1} 所属群：", info.NickName, info.Number, info.GroupId));
                    return info;
                }
            } catch (Exception e) {
                nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
                return null;
            }
        }

        public static List<CQGroupMemberInfo> GetGroupMemberList(long groupId) {
            lock (_syncRoot) {
                List<CQGroupMemberInfo> lst = null;
                try {
                    lst = new List<CQGroupMemberInfo>();
                    nLogger.Info(string.Format("[↑][帐号] 取群成员列表"));
                    var cachePath = Encoding.Default.GetString(
                        Convert.FromBase64String(CQUDPProxy.GetInstance().GetStringMessage(string.Format("GroupMemberList {0}", groupId))));
                    var data = File.ReadAllText(cachePath);

                    if (string.IsNullOrEmpty(data) || !ConvertStrToGroupMembersList(data, ref lst)) {
                        return null;
                    }
                    nLogger.Info(string.Format("[↓][帐号] 总共获得群员数：{0}", lst.Count));
                    return lst;
                } catch (Exception e) {
                    nLogger.Error(e, string.Format("[X][帐号] 异常: {0}", e.Message));
                    return null;
                }
            }
        }

        #endregion

        #region Helper

        private static bool ConvertStrToGroupMembersList(string source, ref List<CQGroupMemberInfo> lsGm) {
            Unpack u = new Unpack();
            if (source == string.Empty) return false;
            var data = Convert.FromBase64String(source);
            if (data == null || data.Length < 10) return false;
            u.SetData(data);
            var count = u.GetInt();
            for (int i = 0; i < count; i++) {
                if (u.Len() <= 0) return false;
                CQGroupMemberInfo gm = new CQGroupMemberInfo();
                if (!ConvertAnsiHexToGroupMembers(u.GetToken(), ref gm)) return false;
                lsGm.Add(gm);
            }
            return true;
        }

        /// <summary>
        /// 转换_ansihex到群成员信息
        /// </summary>
        /// <param name="source">源字节集</param>
        /// <param name="gm">群成员</param>
        /// <returns></returns>
        private static bool ConvertAnsiHexToGroupMembers(byte[] source, ref CQGroupMemberInfo gm) {
            if (source == null || source.Length < 40) return false;
            Unpack u = new Unpack();
            u.SetData(source);
            gm.GroupId = u.GetLong();
            gm.Number = u.GetLong();
            gm.NickName = u.GetLenStr();
            gm.InGroupName = u.GetLenStr();
            gm.Gender = u.GetInt() == 0 ? "男" : "女";
            gm.Age = u.GetInt();
            gm.Area = u.GetLenStr();
            gm.JoinTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime().AddSeconds(u.GetInt());
            gm.LastSpeakingTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime().AddSeconds(u.GetInt());
            gm.Level = u.GetLenStr();
            gm.Authority = u.GetInt().ToString();
            gm.HasBadRecord = (u.GetInt() == 1);
            gm.Title = u.GetLenStr();
            gm.TitleExpirationTime = u.GetInt();
            gm.IsInGroupNameModifiable = (u.GetInt() == 1);
            return true;
        }

        #endregion

    }
}
