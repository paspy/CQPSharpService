using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQPSharpService.KancolleBot {
    class QQMemberInfo {
        public int Id { get; set; }
        public long QQNumber { get; set; }
        public Dictionary<long, bool> InGroupAdminMap { get; set; }
        public bool IsBot { get; set; }
        public int QueryCount { get; set; }
        public Dictionary<DateTime, string> QueryHistory { get; set; }
    }
}
