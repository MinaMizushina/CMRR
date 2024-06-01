using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mina.Tools.CMRR
{
    /// <summary>
    /// Class for Entry point
    /// </summary>
    public class EntryPoint
    {

        private static Mutex mutex;

        /// <summary>
        /// Entry Point
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNewMutex;
            mutex = new Mutex(true, Resources.MUTEX_NAME, out createdNewMutex);

            try
            {
                // prevent multiple running
                if (!createdNewMutex)
                {
                    return;
                }

                var monitorForm = new MonitorForm();

                Application.Run(monitorForm);

            }
            finally
            {
                if(createdNewMutex)
                {
                    mutex.ReleaseMutex();
                }
            }

        }
    }
}
