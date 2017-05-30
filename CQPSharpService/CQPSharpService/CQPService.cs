using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using CQPSharpService.Core;

namespace CQPSharpService {
    public partial class CQPService : ServiceBase {
        public CQPService() {
            InitializeComponent();

        }

        internal void TestStartupAndStop(string[] args) {
            OnStart(args);
            while (Console.ReadKey(false).Key != ConsoleKey.Escape) { }
            OnStop();
        }

        protected override void OnStart(string[] args) {
            CQUDPProxy.GetInstance().StartProxy();

        }

        protected override void OnStop() {

        }
    }
}
