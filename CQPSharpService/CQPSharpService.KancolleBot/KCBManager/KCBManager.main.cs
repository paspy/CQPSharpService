using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

using LiteDB;
using Newtonsoft.Json;
using CQPSharpService.Core;
using CQPSharpService.Utility;
using CQPSharpService.KancolleBot.Utility;

namespace CQPSharpService.KancolleBot {

    public sealed partial class KCBManager {

        public void Init(string dataPath) {
            try {
                AppDataPath = dataPath;
                CQAppPath = CQ.GetCQAppFolder();
                Directory.CreateDirectory(AppDataPath);
                Directory.CreateDirectory(Path.GetDirectoryName(AppDataPath + DB_PATH));
                if (!File.Exists(Path.Combine(dataPath, "config.json"))) {
                    File.WriteAllText(Path.Combine(dataPath, "config.json"),
                        JsonConvert.SerializeObject(new KCBConfig(), Formatting.Indented), new UTF8Encoding(false));

                }
                var configText = File.ReadAllText(Path.Combine(dataPath, "config.json"), new UTF8Encoding(false));
                m_config = JsonConvert.DeserializeObject<KCBConfig>(
                    configText, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
                if (SuperAdminQQ == 0) {
                    throw new Exception("Config Not Set");
                }
                using (var db = new LiteDatabase(AppDataPath + DB_PATH)) {
                    if (!db.CollectionExists("qq_group_info")) {
                        var qqGroupCol = db.GetCollection<QQGroupInfo>("qq_group_info");
                        qqGroupCol.EnsureIndex(x => x.GroupNumber, true);
                        qqGroupCol.EnsureIndex(x => x.InclBroadcasting);
                    }
                    if (!db.CollectionExists("qq_member_info")) {
                        var qqMemberCol = db.GetCollection<QQMemberInfo>("qq_member_info");
                        qqMemberCol.EnsureIndex(x => x.QQNumber, true);
                        qqMemberCol.EnsureIndex(x => x.QueryCount);
                        qqMemberCol.EnsureIndex(x => x.IsBot);
                    }
                }

                m_cacheTimer = new Timer(UpdateGroupInfo, null, 1000, Timeout.Infinite);

            } catch (Exception e) {
                nLog.Error(e, e.Message);
                throw;
            }
        }

        public void BroadcastToGroup(string msg) {
            lock (__syncRoot) {
                using (var db = new LiteDatabase(AppDataPath + DB_PATH)) {
                    var col = db.GetCollection<QQGroupInfo>("qq_group_info");
                    var bcLst = col.Find(Query.EQ("InclBroadcasting", true)).ToList();
                    foreach (var g in bcLst) {
                        CQ.SendGroupMessage(g.GroupNumber, msg + "\n[自动群发]");
                        Thread.Sleep(50);
                        //CQ.SendPrivateMessage(SuperAdminQQ, msg + "\n[自动群发]");
                        Debug.WriteLine(DateTime.Now + " | " + msg);
                    }
                }
            }
        }

        public string GetCoolQImageCode(string path) {
            var fileExt = Path.GetExtension(path);
            byte[] data = null;
            if (fileExt == ".png" || fileExt == ".jpg" || fileExt == ".gif") {
                if (path.StartsWith("http")) {
                    using (var image = HttpHelper.GetImage(path))
                    using (MemoryStream imgStream = new MemoryStream()) {
                        image.Save(imgStream, image.RawFormat);
                        data = imgStream.ToArray();
                    }
                } else if (File.Exists(path)) {
                    data = File.ReadAllBytes(path);
                } else {
                    return string.Empty;
                }
                var destFilename = data.ToMD5String() + fileExt;
                var destFilePath = Path.Combine(CQImagePath, destFilename);
                if (!File.Exists(destFilePath))
                    File.WriteAllBytes(Path.Combine(CQImagePath, destFilename), data);
                return CQ.CQCode_Image(destFilename);
            }
            return string.Empty;
        }

        public void AdminLog(string msg, bool notifySuperAdmin = false) {
            if (notifySuperAdmin) {
                nLog.Warn(msg);
                CQ.SendPrivateMessage(SuperAdminQQ, msg);
            } else {
                nLog.Info(msg);
            }
        }

        private void UpdateGroupInfo(object target) {
            var lst = CQE.GetGroupList();
            using (var db = new LiteDatabase(AppDataPath + DB_PATH)) {
                var col = db.GetCollection<QQGroupInfo>("qq_group_info");
                using (var trans = db.BeginTrans()) {
                    foreach (var g in lst) {
                        if (!col.Exists(Query.EQ("GroupNumber", g.Number))) {
                            QQGroupInfo gInfo = new QQGroupInfo() {
                                GroupName = g.Name,
                                GroupNumber = g.Number,
                                InclBroadcasting = true
                            };
                            col.Insert(gInfo);
                        }
                    }
                    var colList = col.FindAll().ToList();
                    var removedGroups = colList.Select(x => x.GroupNumber).Except(lst.Select(y => y.Number)).ToList();
                    foreach (var gNumber in removedGroups) {
                        col.Delete(Query.EQ("GroupNumber", gNumber));
                    }
                    trans.Commit();
                }
            }

            m_cacheTimer.Change(3600000, Timeout.Infinite);
        }


    }
}
