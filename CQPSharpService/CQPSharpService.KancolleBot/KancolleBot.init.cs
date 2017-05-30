using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQPSharpService.Core;
using CQPSharpService.KancolleBot.Modules;
using RandomOrgSharp;
using Yamool.CWSharp;
using System.Net;

namespace CQPSharpService.KancolleBot {
    public sealed partial class KancolleBot : CQAppAbstract {

        private string AppDataPath => Path.Combine(Path.GetDirectoryName(AssemblyPath), Path.GetFileNameWithoutExtension(AssemblyPath));

        private bool IsInit;
        private RandomJsonRPCClient RandomOrgClient;

        // Phrases processing
        private StandardTokenizer m_cwsStandard;
        private StopwordTokenizer m_cwsStopWord;

        // Modules
        private TwitterNotifier m_modTwitterNotifier;
        private KCUpdateNotifier m_modKCUpdateNotifier;

        public override void Initialize() {
            Name = "舰队收藏Q群机器人-天津风";
            Version = new Version("0.1.0");
            Author = "Paspy";
            Description = "给帅群员提供各种撒必死服务。";
        }

        /// <summary>
        /// 应用启动，完成插件线程、全局变量等自身运行所必须的初始化工作。
        /// </summary>
        public override void Startup() {
            IsInit = false;
            KCBManager.Instance.Init(AppDataPath);
            Task.Run(InitializeAsync);
        }

        private async Task InitializeAsync() {
            await Task.Run(() => {
                m_cwsStandard = new StandardTokenizer(new FileStream(Path.Combine(AppDataPath, @"CWSharp\default.dawg"), FileMode.Open, FileAccess.Read));
                m_cwsStopWord = new StopwordTokenizer(m_cwsStandard, File.ReadAllLines(Path.Combine(AppDataPath, @"CWSharp\default.stopwords")));
                KCBManager.nLog.Info("CWS initialized.");
            });
            await Task.Run(() => {
                RandomOrgClient = new RandomJsonRPCClient(KCBManager.Instance.RandomOrgToken);
                KCBManager.nLog.Info("RandomOrgClient initialized.");
            });
            await Task.Run(() => {
                m_modTwitterNotifier = new TwitterNotifier();
                KCBManager.nLog.Info("Module: TwitterNotifier initialized.");
            });
            await Task.Run(() => {
                m_modKCUpdateNotifier = new KCUpdateNotifier();
                KCBManager.nLog.Info("Module: KCUpdateNotifie initialized.");
            });
            IsInit = true;
        }

    }
}
