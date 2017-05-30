using System;

namespace CQPSharpService.Core {

    /// <summary>群信息。</summary>
    public class CQGroupInfo {
        /// <summary>获取或设置群号码。</summary>
        public long Number { get; set; }

        /// <summary>获取或设置群名称。</summary>
        public string Name { get; set; }

        public long Owner { get; set; }
    }
}
