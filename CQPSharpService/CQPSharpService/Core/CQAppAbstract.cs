﻿using System;

namespace CQPSharpService.Core {
    /// <summary>CQ应用抽象类，自定义应用请继承此类。</summary>
    public abstract class CQAppAbstract : ICQAssembly {
        /// <summary>获取或设置插件名称。</summary>
        /// <value>插件名称字符串。</value>
        /// <return>插件名称字符串。</return>
        public virtual string Name { get; set; }

        /// <summary>获取或设置插件版本。</summary>
        /// <value>插件版本信息。</value>
        /// <return>插件版本信息。</return>
        public virtual Version Version { get; protected set; }

        /// <summary>获取或设置插件作者。</summary>
        /// <value>插件作者名称。</value>
        /// <return>插件作者名称。</return>
        public virtual string Author { get; protected set; }

        /// <summary>获取或设置插件说明。</summary>
        /// <value>插件说明描述信息。</value>
        /// <return>插件说明描述信息。</return>
        public virtual string Description { get; protected set; }

        /// <summary>获取程序集路径。</summary>
        public string AssemblyPath { get; internal set; }

        /// <summary>是否运行该插件。</summary>
        public bool RunningStatus { get; set; }

        /// <summary>应用初始化，用来初始化应用的基本信息。</summary>
        public virtual void Initialize() {
            Name = "Null";
            Version = new Version("1.0.0.0");
            Author = "UnKnown";
            Description = "";
        }

        /// <summary>应用启动，完成插件线程、全局变量等自身运行所必须的初始化工作。</summary>
        public virtual void Startup() { }

        /// <summary>Type=21 私聊消息。</summary>
        /// <param name="subType">子类型，11/来自好友 1/来自在线状态 2/来自群 3/来自讨论组。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public virtual void PrivateMessage(int subType, long sendTime, long fromQQ, string msg, int font) { }

        /// <summary>Type=2 群消息。</summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="fromAnonymous">来源匿名者。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public virtual void GroupMessage(int subType, long sendTime, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font) { }

        /// <summary>Type=4 讨论组消息。</summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromDiscuss">来源讨论组。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public virtual void DiscussMessage(int subType, long sendTime, long fromDiscuss, long fromQQ, string msg, int font) { }

        /// <summary>Type=11 群文件上传事件。</summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="file">上传文件信息。</param>
        public virtual void GroupUpload(int subType, long sendTime, long fromGroup, long fromQQ, string file) { }

        /// <summary>Type=101 群事件-管理员变动。</summary>
        /// <param name="subType">子类型，1/被取消管理员 2/被设置管理员。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public virtual void GroupAdmin(int subType, long sendTime, long fromGroup, long beingOperateQQ) { }

        /// <summary>Type=102 群事件-群成员减少。</summary>
        /// <param name="subType">子类型，1/群员离开 2/群员被踢 3/自己(即登录号)被踢。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public virtual void GroupMemberDecrease(int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ) { }

        /// <summary>Type=103 群事件-群成员增加。</summary>
        /// <param name="subType">子类型，1/管理员已同意 2/管理员邀请。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public virtual void GroupMemberIncrease(int subType, long sendTime, long fromGroup, long fromQQ, long beingOperateQQ) { }

        /// <summary>Type=201 好友事件-好友已添加。</summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        public virtual void FriendAdded(int subType, long sendTime, long fromQQ) { }

        /// <summary>Type=301 请求-好友添加。</summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        public virtual void RequestAddFriend(int subType, long sendTime, long fromQQ, string msg, string responseFlag) { }

        /// <summary>Type=302 请求-群添加。</summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        public virtual void RequestAddGroup(int subType, long sendTime, long fromGroup, long fromQQ, string msg, string responseFlag) { }

        /// <summary>打开设置窗口。</summary>
        public virtual void OpenSettingForm() { }
    }
}
