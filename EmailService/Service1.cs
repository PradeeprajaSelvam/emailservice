using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;
using Timer = System.Timers.Timer;
using log4net;

namespace EmailService  
{
    /// <summary>
    /// Windows service that periodically triggers email sending.
    /// </summary>
    /// <remarks>
    /// This service creates and starts a <see cref="System.Timers.Timer"/> in the constructor.
    /// The timer is configured to raise the <see cref="Timer.Elapsed"/> event every 300000 ms (5 minutes).
    /// The <see cref="OnTimer"/> handler calls <c>SendEmail.SendEmailAsync()</c>.
    ///
    /// Notes:
    /// - <see cref="Timer.Elapsed"/> is raised on a ThreadPool thread. Ensure that the implementation
    ///   of <c>SendEmail.SendEmailAsync()</c> is thread-safe and handles exceptions appropriately.
    /// - Currently the timer is scoped to the constructor. Consider promoting it to a field if you
    ///   need to stop/dispose it explicitly in <see cref="OnStop(string[])"/>.
    /// - <c>SendEmail.SendEmailAsync()</c> is invoked without awaiting; this behaves like fire-and-forget.
    ///   If you require strict sequencing or guaranteed completion before the next tick or shutdown,
    ///   refactor to await the task and handle cancellation/timeouts.
    /// </remarks>
    public partial class EmailService : ServiceBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(EmailService));

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor calls <c>InitializeComponent()</c> and starts a <see cref="Timer"/>.
        /// The timer interval is set to 300000 ms (5 minutes). A commented alternative of 1.2e+6 (20 minutes)
        /// is preserved from the original source.
        /// </remarks>
        public EmailService()
        {
            InitializeComponent();
            SendEmail.SendEmailAsync();

            Timer timer = new Timer
            {
                Interval = 300000 //5 minutes
                //1.2e+6 // 20 minutes
            };
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        /// <summary>
        /// Called by the Service Control Manager when a Start command is sent to the service.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the service.</param>
        /// <remarks>
        /// The service's timer is started in the constructor. Place additional startup logic here
        /// if initialization must happen after the service start event.
        /// </remarks>
        protected override void OnStart(string[] args)
        {
            _logger.Info("EmailService starting...");
        }

        /// <summary>
        /// Called by the Service Control Manager when a Stop command is sent to the service.
        /// </summary>
        /// <remarks>
        /// Add cleanup and resource disposal logic here (for example, stop and dispose timers,
        /// cancel background work, flush logs). Because the timer in this implementation is local
        /// to the constructor, there is currently nothing to stop from this method.
        /// Consider storing the timer in a field so it can be stopped and disposed here.
        /// </remarks>
        protected override void OnStop()
        {
            _logger.Info("EmailService stopping...");
        }

        /// <summary>
        /// Handler invoked when the configured timer interval elapses.
        /// </summary>
        /// <param name="sender">The timer that raised the event.</param>
        /// <param name="args">Elapsed event arguments.</param>
        /// <remarks>
        /// This method currently calls <c>SendEmail.SendEmailAsync()</c>. The method is executed
        /// on a ThreadPool thread. Preserve the existing TODO comment and ensure exception handling
        /// and concurrency control are implemented inside the called method or add try/catch here
        /// to prevent unhandled exceptions from terminating the process.
        /// </remarks>
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                _logger.Debug("Timer elapsed event triggered.");
                SendEmail.SendEmailAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred in OnTimer:", ex);
            }
        }

    }
}
