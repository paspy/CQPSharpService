using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQPSharpService.KancolleBot.Utility;
using NLog;

namespace CQPSharpService.KancolleBot {
    public sealed partial class KCBManager {
        private KCBConfig m_config;
        private Timer m_cacheTimer;

        internal static readonly Random Rnd = new Random();

        public static Logger nLog = LogManager.GetLogger("CQPSharpService.KancolleBot");

        public string AppDataPath;
        public string CQAppPath { get; private set; }
        public string CQRootPath => Directory.GetParent(Directory.GetParent(Directory.GetParent(CQAppPath).FullName).FullName).FullName;
        public string CQImagePath => Path.Combine(CQRootPath, @"data\image");

        public long SuperAdminQQ => m_config.SuperAdminQQ;
        public string RandomOrgToken => m_config.RandomOrgToken;
        public string TwitterConsumerKey => m_config.TwitterConsumerKey;
        public string TwitterConsumerSecret => m_config.TwitterConsumerSecret;
        public string TwitterAccessToken => m_config.TwitterAccessToken;
        public string TwitterAccessTokenSecret => m_config.TwitterAccessTokenSecret;
        public string WhatAnimeToken => m_config.WhatAnimeToken;

        public static KancolleAuth KCAuth { get; private set; }

        public static readonly List<string> SERVER_ADDRESS = new List<string>() {
               "203.104.209.71"  , // 横須賀鎮守府   
               "203.104.209.87"  , // 呉鎮守府        
               "125.6.184.16"    , // 佐世保鎮守府    
               "125.6.187.205"   , // 舞鶴鎮守府      
               "125.6.187.229"   , // 大湊警備府      
               "125.6.187.253"   , // トラック泊地    
               "125.6.188.25"    , // リンガ泊地      
               "203.104.248.135" , // ラバウル基地    
               "125.6.189.7"     , // ショートランド泊地
               "125.6.189.39"    , // ブイン基地
               "125.6.189.71"    , // タウイタウイ泊地 
               "125.6.189.103"   , // パラオ泊地
               "125.6.189.135"   , // ブルネイ泊地    
               "125.6.189.167"   , // 単冠湾泊地      
               "125.6.189.215"   , // 幌筵泊地        
               "125.6.189.247"   , // 宿毛湾泊地      
               "203.104.209.23"  , // 鹿屋基地        
               "203.104.209.39"  , // 岩川基地        
               "203.104.209.55"  , // 佐伯湾泊地      
               "203.104.209.102" , // 柱島泊地        
        };

        public static readonly List<string> SERVER_NAME = new List<string>() {
              /*203.104.209.71 */ "横須賀鎮守府",
              /*203.104.209.87 */ "呉鎮守府",
              /*125.6.184.16   */ "佐世保鎮守府",
              /*125.6.187.205  */ "舞鶴鎮守府",
              /*125.6.187.229  */ "大湊警備府",
              /*125.6.187.253  */ "トラック泊地",
              /*125.6.188.25   */ "リンガ泊地",
              /*203.104.248.135*/ "ラバウル基地",
              /*125.6.189.7    */ "ショートランド泊地",
              /*125.6.189.39   */ "ブイン基地",
              /*125.6.189.71   */ "タウイタウイ泊地",
              /*125.6.189.103  */ "パラオ泊地",
              /*125.6.189.135  */ "ブルネイ泊地",
              /*125.6.189.167  */ "単冠湾泊地",
              /*125.6.189.215  */ "幌筵泊地",
              /*125.6.189.247  */ "宿毛湾泊地",
              /*203.104.209.23 */ "鹿屋基地",
              /*203.104.209.39 */ "岩川基地",
              /*203.104.209.55 */ "佐伯湾泊地",
              /*203.104.209.102*/ "柱島泊地",
        };

        private const string DB_PATH = @"\database\kcb.db";

        // Singleton
        private KCBManager() { }
        ~KCBManager() { }
        public static KCBManager Instance {
            get {
                if (__instance == null) {
                    lock (__syncRoot) {
                        if (__instance == null)
                            __instance = new KCBManager();
                    }
                }
                return __instance;
            }
        }
        private static volatile KCBManager __instance;
        private static object __syncRoot = new object();
    }
}
