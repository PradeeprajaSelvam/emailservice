using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;

namespace EmailService
{
    /// <summary>
    /// Hosts the application's entry point for the Windows Service.
    /// </summary>
    /// <remarks>
    /// This static <c>Program</c> class is responsible for creating the array of
    /// <see cref="ServiceBase"/> instances that the Service Control Manager (SCM)
    /// will run. For this project a single service, <c>EmailService</c>, is registered.
    ///
    /// Notes for developers:
    /// - When debugging from Visual Studio, you may prefer to run the service logic
    ///   directly (for example, by adding a debug-only entry point) because the SCM
    ///   does not host services while debugging by default.
    /// - The call to <see cref="ServiceBase.Run(ServiceBase[])"/> hands control to the
    ///   SCM and blocks until the service(s) stop(s).
    /// </remarks>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks>
        /// Builds the array of services to run and invokes <see cref="ServiceBase.Run(ServiceBase[])"/>.
        /// The array can contain one or more service instances; the SCM will manage all listed services.
        /// </remarks>
        static void Main()
        {
            string logPath = @"C:\Logs";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            
            XmlConfigurator.Configure();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EmailService()
            };
            ServiceBase.Run(ServicesToRun);

        }
    }
}
