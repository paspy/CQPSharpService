using System;

namespace CQPSharpService.Core {
    /// <summary>
    /// 表示一个群成员的信息。
    /// </summary>
    public sealed class CQGroupMemberInfo {

        /// <summary>
        /// 此群成员在其个人资料上所填写的年龄。
        /// </summary>
        /// <returns> </returns>
        public int Age { get; set; }

        /// <summary>
        /// 此群成员在其个人资料上所填写的区域。
        /// </summary>
        /// <returns></returns>
        public string Area { get; set; }

        /// <summary>
        /// 此群成员的身份。
        /// 1/普通成员 2/管理员 3/群主
        /// </summary>
        /// <returns></returns>
        public string Authority {
            get {
                switch (authority) {
                    case 1:
                        return "普通成员";
                    case 2:
                        return "管理员";
                    case 3:
                        return "群主";
                    default:
                        return "Error";
                }
            }
            set {
                authority = int.Parse(value);
            }
        }
        private int authority;

        /// <summary>
        /// 指示此群成员是否能够修改所有群成员名片的值。
        /// </summary>
        /// <returns><see cref="bool"/> </returns>
        public bool CanModifyInGroupName { get; set; }

        /// <summary>
        /// 此群成员在其个人资料上所填写的性别。
        /// </summary>
        /// <returns></returns>
        public string Gender { get; set; }

        /// <summary>
        /// 此群成员的群名片。
        /// </summary>
        /// <returns></returns>
        public string InGroupName { get; set; }

        /// <summary>
        /// 此群成员的头衔。
        /// </summary>
        /// <returns></returns>
        public string Title { get; set; }

        /// <summary>
        /// 此群成员所在群号。
        /// </summary>
        /// <returns><see cref="long"/> </returns>
        public long GroupId { get; set; }

        /// <summary>
        /// 指示此群成员是否有不良记录的值。
        /// </summary>
        /// <returns><see cref="bool"/> </returns>
        public bool HasBadRecord { get; set; }

        /// <summary>
        /// 头衔过期时间。
        /// </summary>
        /// <returns> </returns>
        public int TitleExpirationTime { get; set; }

        /// <summary>
        /// 此群成员的入群时间。
        /// </summary>
        /// <returns><see cref="DateTime"/> </returns>
        public DateTime JoinTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int JoinTimeInt { get; set; }

        /// <summary>
        /// 此群成员最后发言日期。
        /// </summary>
        /// <returns><see cref="DateTime"/> </returns>
        public DateTime LastSpeakingTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LastSpeakingTimeInt { get; set; }


        /// <summary>
        /// 此群成员的群内等级。
        /// </summary>
        /// <returns></returns>
        public string Level { get; set; }

        /// <summary>
        /// 此群成员的昵称。
        /// </summary>
        /// <returns></returns>
        public string NickName { get; set; }

        /// <summary>
        /// 此群成员的QQ号码。
        /// </summary>
        /// <returns><see cref="long"/> </returns>
        public long Number { get; set; }

        /// <summary>
		/// 是否允许修改群名片
		/// </summary>
		public bool IsInGroupNameModifiable { get; set; }
    }
}
