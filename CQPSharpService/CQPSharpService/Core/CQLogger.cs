using System;

namespace CQPSharpService.Core {

    public class CQLogger {

        private static CQLogger _instance;
        private static object __syncRoot = new object();

        public event EventHandler<CQLogEventArgs> NewLogWrite;

        private CQLogger() { }

        public static CQLogger GetInstance() {
            if (_instance == null) {
                lock (__syncRoot) {
                    if (_instance == null)
                        _instance = new CQLogger();
                }
            }
            return _instance;
        }

        internal void AddLog(string logMessage) {
            if (NewLogWrite == null)
                return;
            string str = string.Format("[{0}] {1}", DateTime.Now, logMessage);
            NewLogWrite(this, new CQLogEventArgs() {
                LogMessage = str
            });
        }
    }

    public class CQLogEventArgs : EventArgs {

        /// <summary>日志发生的时间。</summary>
        public DateTime LogTime { get; set; }

        /// <summary>日志来源。</summary>
        public string LogSource { get; set; }

        /// <summary>日志类型。</summary>
        public string LogType { get; set; }

        /// <summary>日志信息。</summary>
        public string LogMessage { get; set; }
    }

}
