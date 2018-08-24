using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Windows.Forms;

namespace TextEditor
{
    class Program : MarshalByRefObject
    {
        static MainApp m_mainApp;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var mutex = new Mutex(false, Application.ProductName);
            if (mutex.WaitOne(0, false))
            {
                ChannelServices.RegisterChannel(new IpcServerChannel(Application.ProductName), true);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(Program), "Program", WellKnownObjectMode.Singleton);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(m_mainApp = new MainApp(args));
            }
            else
            {
                ChannelServices.RegisterChannel(new IpcClientChannel(), true);
                RemotingConfiguration.RegisterWellKnownClientType(typeof(Program), "ipc://" + Application.ProductName + "/Program");
                new Program().StartupNextInstance(args);
            }
            mutex.Close();
        }

        public void StartupNextInstance(string[] args)
        {
            m_mainApp.Invoke((Action)delegate
            {
                if (args.Length == 0)
                {
                    m_mainApp.NewTabView();
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        m_mainApp.OpenTabView(args[i]);
                    }
                }
            });
        }
    }
}
