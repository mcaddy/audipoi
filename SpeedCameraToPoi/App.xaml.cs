//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace SpeedCameraToPoi
{
    using System.Windows;
    using Mcaddy;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Configuration object
        /// </summary>
        private static Config configuration = new Config();

        /// <summary>
        /// Gets the configuration object
        /// </summary>
        public static Config Configuration { get => configuration; private set => configuration = value; }

        /// <summary>
        /// App Startup Event - Process the Command Line arguments
        /// </summary>
        /// <param name="sender">Sender Argument</param>
        /// <param name="e">Startup Event Arguments</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool auto = false;
            string targetDrive = string.Empty;

            // Parse Command Line
            for (int i = 0; i < e.Args.Length; i++)
            {
                switch (e.Args[i].ToLower())
                {
                    case "-auto":
                        auto = true;
                        break;
                    case "-targetdrive":
                        if (e.Args.Length > (i + 1))
                        {
                            targetDrive = e.Args[i + 1];
                        }

                        break;
                    default:
                        break;
                }
            }

            App.Current.Properties["targetDrive"] = targetDrive;
            App.Current.Properties["auto"] = auto;
        }
    }
}