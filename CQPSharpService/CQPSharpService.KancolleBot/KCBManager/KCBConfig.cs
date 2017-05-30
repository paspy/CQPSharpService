using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPSharpService.KancolleBot {
    class KCBConfig {
        public long SuperAdminQQ { get; set; }
        public string RandomOrgToken { get; set; }
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }
        public string TwitterAccessToken { get; set; }
        public string TwitterAccessTokenSecret { get; set; }

        #region To Do Stuff
        public string WhatAnimeToken { get; set; }

        #endregion
    }
}
