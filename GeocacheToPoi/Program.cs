//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="mcaddy">
//     All rights reserved
// </copyright>
//-----------------------------------------------------------------------
namespace Mcaddy.Audi
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    /// <summary>
    /// Audi POI Exporter
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Have we been asked to auto process
        /// </summary>
        private static bool auto = false;

        /// <summary>
        /// The Target Drive Passed on the command line
        /// </summary>
        private static string targetDrive = string.Empty;

        /// <summary>
        /// The GpxPath passed on the command line
        /// </summary>
        private static string gpxPath = string.Empty;

        /// <summary>
        /// Gets a value indicating whether we've been requested to auto download
        /// </summary>
        public static bool Auto { get => auto; private set => auto = value; }

        /// <summary>
        /// Gets the Target Drive passed on the command line
        /// </summary>
        public static string TargetDrive { get => targetDrive; private set => targetDrive = value; }

        /// <summary>
        /// Gets the GPX Path passed on the command line
        /// </summary>
        public static string GpxPath { get => gpxPath; private set => gpxPath = value; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command Line Arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Parse Command Line
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower(CultureInfo.CurrentCulture))
                {
                    case "-auto":
                        Auto = true;
                        break;
                    case "-target":
                        if (args.Length > (i + 1))
                        {
                            TargetDrive = args[i + 1].ToUpper();
                        }

                        break;
                    case "-gpxpath":
                        if (args.Length > (i + 1))
                        {
                            GpxPath = args[i + 1];
                        }

                        break;
                    default:
                        break;
                }
            }

            Application.Run(new MainForm());
        }
    }
}
