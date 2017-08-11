using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPSharpService.KancolleBot {
    class KCBConfig {
        public KCBConfig() {
            RandomOrgToken =
            TwitterConsumerKey =
            TwitterConsumerSecret =
            TwitterAccessToken =
            TwitterAccessTokenSecret =
            DMMLoginId =
            DMMLoginPwd = 
            WhatAnimeToken = string.Empty;

        }
        public long SuperAdminQQ { get; set; }
        public string RandomOrgToken { get; set; }
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessTokenSecret { get; set; }
        public string DMMLoginId { get; set; }
        public string DMMLoginPwd { get; set; }
        #region To Do Stuff
        public string WhatAnimeToken { get; set; }

        #endregion
    }
}
