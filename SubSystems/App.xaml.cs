using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Shell;
using APMTools;


namespace APM_SubSystems
{
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "GhanaatApplication";

        [STAThread]
        public static void Main()
        {
            // if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            //  {
            var application = new App();

            application.InitializeComponent();
            application.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(application_DispatcherUnhandledException);
            application.Run();

            //   SingleInstance<App>.Cleanup();
            // }
            // else
            //     APMTools.Messages.ErrorMessage("نسخۀ دیگری از برنامه در حال اجرا می باشد");
        }

        static void application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Messages.ExceptionMessage(e.Exception);
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion
    }
}
