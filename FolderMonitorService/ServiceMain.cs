using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FolderMonitorService
{
    static class ServiceMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            MonitorService s = new MonitorService();
            s.OnDebug();

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new MonitorService() 
			};
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
