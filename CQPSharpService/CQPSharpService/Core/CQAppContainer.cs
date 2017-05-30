using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CQPSharpService.Core {

    public class CQAppContainer {

        private NLog.Logger nLogger = NLog.LogManager.GetCurrentClassLogger();

        private List<CQAppAbstract> _apps;

        public List<CQAppAbstract> Apps {
            get {
                lock (__syncRoot)
                    return _apps;
            }
        }


        private CQAppContainer() {
            LoadApps();
        }

        private static volatile CQAppContainer _instance;
        private static object __syncRoot = new object();
        public static CQAppContainer GetInstance() {
            lock (__syncRoot) {
                if (_instance == null)
                    _instance = new CQAppContainer();
            }
            return _instance;
        }

        private void LoadApps() {
            lock (__syncRoot) {
                _apps = new List<CQAppAbstract>();
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSharpPlugins");
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                } else {
                    foreach (string file in Directory.GetFiles(path, "*.dll")) {
                        bool loadingStatus = true;
                        try {
                            var ts = Assembly.Load(File.ReadAllBytes(file)).GetTypes();

                            foreach (Type type in ts) {
                                if (type.IsClass && !type.IsNotPublic && ((IEnumerable<Type>)type.GetInterfaces()).Select(s => s.Name).Contains("ICQAssembly")) {
                                    CQAppAbstract instance1 = (CQAppAbstract)Activator.CreateInstance(type, null);
                                    instance1.AssemblyPath = file;
                                    instance1.RunningStatus = loadingStatus;
                                    instance1.Initialize();
                                    _apps.Add(instance1);
                                    if (!loadingStatus)
                                        instance1.Name = "[*]" + instance1.Name;
                                    else
                                        instance1.Startup();

                                }
                            }
                        } catch {
                        }
                    }
                }
            }
        }
    }
}
