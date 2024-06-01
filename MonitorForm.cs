using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mina.Tools.CMRR
{
    public partial class MonitorForm : Form
    {

        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        /// <summary>
        /// Clipboard Listner Message
        /// </summary>
        private const int WM_DRAWCLIPBOARD = 0x031D;

        /// <summary>
        /// Setting Loader
        /// </summary>
        SettingProvider settingProvider = null;

        /// <summary>
        /// Monitoring Flag
        /// </summary>
        bool isMonitoring = false;

        /// <summary>
        /// Notification Icon
        /// </summary>
        NotifyIcon notifyIcon = null;


        public MonitorForm()
        {
            InitializeComponent();

            settingProvider = new SettingProvider();
            AddTasktrayIcon();

        }

        /// <summary>
        /// Set form as not visible, and start monitoring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonitorForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            StartMonitoring();
        }

        /// <summary>
        /// Start monitoring (Add Clipboard Listner)
        /// </summary>
        private void StartMonitoring()
        {
            if(isMonitoring)
            {
                return;
            }

            AddClipboardFormatListener(this.Handle);
            isMonitoring = true;
        }

        /// <summary>
        /// Stop monitoring
        /// </summary>
        private void EndMonitoring()
        {
            if(!isMonitoring)
            {
                return;
            }

            RemoveClipboardFormatListener(this.Handle);
            isMonitoring = false;
        }

        /// <summary>
        /// Window Procedure (Overrided)
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DRAWCLIPBOARD)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        ClipboardOnPaint(GetClipboardText());
                    }
                }
                catch (InvalidOperationException)
                { 
                    // Clipboard locked and couldn't get type setting / nothing can do.
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Form termination
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            EndMonitoring();
        }

        /// <summary>
        /// Clipboard rewrite execution
        /// </summary>
        /// <param name="clipboardText"></param
        private void ClipboardOnPaint(string clipboardText)
        {
            foreach (var setting in settingProvider.Settings)
            {
                if (Regex.Match(clipboardText, setting.Match, RegexOptions.Singleline).Success)
                {
                    var newVal = Regex.Replace(clipboardText, setting.Match, setting.replaceTo, RegexOptions.Singleline);

                    EndMonitoring();

                    ClipboardClear();

                    ClipboardSetText(newVal);

                    StartMonitoring();

                    if (setting.notifySeconds > 0)
                    {
                        notifyIcon.ShowBalloonTip(setting.notifySeconds, setting.Name, CutString(clipboardText) + "\nto\n" + newVal, ToolTipIcon.Info);
                    }
                    if (!setting.goNext)
                    {
                        break;
                    }
                }
            }
        }

        private void AddTasktrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Resources.MainIcon;

            notifyIcon.Text = Resources.MENU_DISP_NAME;


            ContextMenu contextMenu = new ContextMenu();

            MenuItem mnuExit = new MenuItem(Resources.MENU_EXIT_APP);
            mnuExit.Click += new EventHandler(this.ExitMenu);

            contextMenu.MenuItems.Add(mnuExit);

            notifyIcon.ContextMenu = contextMenu;

            notifyIcon.Visible = true;

        }

        /// <summary>
        /// Terminate menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ExitMenu(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        /// <summary>
        /// Cut string for display
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string CutString(string text)
        {
            var max = int.Parse(Resources.BALLOON_STRING_MAX);
            if(text.Length > max)
            {
                return text.Substring(0, max);
            }
            return text;
        }

        /// <summary>
        /// Get text from clipboard
        /// </summary>
        /// <returns></returns>
        private string GetClipboardText()
        {
            // Retry 20 times (with 50ms interval) to get clipboard value, because sometimes clipboard is locked by another app.
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    return Clipboard.GetText();
                }
                catch (InvalidOperationException)
                {

                }
                System.Threading.Thread.Sleep(50);
            }

            return string.Empty;
        }

        private void ClipboardClear()
        {
            // retry 10 times to clear clipboard
            for (int i = 0; i < 20; i++)
            {
                try
                {

                    Clipboard.Clear();
                    break;
                }
                catch (InvalidOperationException)
                {

                }
                System.Threading.Thread.Sleep(50);
            }
        }

        private void ClipboardSetText(string text)
        {
            for(var i = 0; i < 20; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return;
                }
                catch(InvalidOperationException)
                {

                }
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
