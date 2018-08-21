using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PYun;
using log4net;
using System.Reflection;
using log4net.Appender;
using log4net.Core;
using PYun.Events;
using log4net.Repository.Hierarchy;
using PYun.API;

namespace api_demo
{
    public partial class Form1 : Form, IAppender
    {

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Form1()
        {
            InitializeComponent();
            textBoxLogging.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            ((Hierarchy)LogManager.GetRepository()).Root.AddAppender(this);
        }

        private PYunAPI instance = null;

        private PYunAPI createPYunAPI()
        {
            if (instance == null)
            {
                instance = new PYunAPI(textBoxHost.Text, Convert.ToInt32(textBoxPort.Value));
                instance.Type = textBoxType.Text;
                // instance.IdleInterval = 1000;
                
                instance.ChannelEventHandler += new EventHandler<ChannelEventArgs>(ChannelEventHandler);
                instance.ServiceHandler = new PYunServiceAPIImpl();
            }
            return instance;
        }

        #region Event
        void ChannelEventHandler(object sender, ChannelEventArgs args)
        {
            switch (args.Type)
            {
                case ChannelEventType.AccessGranted:
                    Logging(args.Type.ToString(), Color.LightGreen);
                    break;
                case ChannelEventType.AccessDenied:
                    Logging(args.Type.ToString(), Color.Red);
                    break;
                case ChannelEventType.ChannelError:
                    Logging(args.Type.ToString(), Color.OrangeRed);
                    break;
                case ChannelEventType.ChannelClosed:
                    Logging(args.Type.ToString(), Color.Yellow);
                    break;
            }
        }

        private void delegateControl(ToolStripButton obj, bool enable)
        {
            Action<bool> actionDelegate = (x) =>
            {
                obj.Enabled = x;
            };

            textBoxLogging.Invoke(actionDelegate, enable);
        }

        #endregion

        # region Buttons EventHandlers...
        private void toolStripStartup_Click(object sender, EventArgs e)
        {
            Logging("Startup...", Color.LightGreen);
            PYunAPI instance = this.createPYunAPI();
            string uuid = textBoxUuid.Text;
            string mac = textBoxMac.Text;

            textBoxHost.Enabled = false;
            textBoxPort.Enabled = false;
            textBoxType.Enabled = false;
            textBoxMac.Enabled = false;
            textBoxUuid.Enabled = false;
            toolStripStartup.Enabled = false;
            toolStripShutdown.Enabled = true;
            toolStripRestart.Enabled = true;
            
            instance.Startup(uuid, mac);
        }

        private void toolStripShutdown_Click(object sender, EventArgs e)
        {
            Logging("Shutdown...", Color.Yellow);
            PYunAPI instance = this.createPYunAPI();
            instance.Shutdown();
            instance = null;
            textBoxHost.Enabled = true;
            textBoxPort.Enabled = true;
            textBoxType.Enabled = true;
            textBoxMac.Enabled = true;
            textBoxUuid.Enabled = true;

            toolStripStartup.Enabled = true;
            toolStripShutdown.Enabled = false;
            toolStripRestart.Enabled = false;
        }

        private void toolStripRestart_Click(object sender, EventArgs e)
        {
            Logging("Restart...", Color.Yellow);
            PYunAPI instance = this.createPYunAPI();
            instance.Restart();

            textBoxHost.Enabled = false;
            textBoxPort.Enabled = false;
            textBoxType.Enabled = false;
            textBoxMac.Enabled = false;
            textBoxUuid.Enabled = false;

            toolStripStartup.Enabled = false;
            toolStripShutdown.Enabled = true;
            toolStripRestart.Enabled = true;
        }

        private void toolStripClearLog_Click(object sender, EventArgs e)
        {
            if (textBoxLogging.InvokeRequired)
            {
                Action<string> actionDelegate = (x) =>
                {
                    textBoxLogging.Clear();
                };
                textBoxLogging.Invoke(actionDelegate);
            }
            else
            {
                textBoxLogging.Clear();
            }
        }
        #endregion

        #region Logging
        private void Logging(LoggingEvent loggingEvent)
        {
            if (textBoxLogging.InvokeRequired)
            {
                Action<LoggingEvent> actionDelegate = (x) =>
                {
                    textBoxLogging.AppendText(String.Format("{0} {1} - {2}{3}", DateTime.Now.ToString("HH:mm:ss.fff"), x.Level.Name, x.MessageObject.ToString(), Environment.NewLine));
                };
                textBoxLogging.Invoke(actionDelegate, loggingEvent);

            }
            else
            {
                textBoxLogging.AppendText(String.Format("{0} {1} - {2}{3}", DateTime.Now.ToString("HH:mm:ss.fff"), loggingEvent.Level.Name, loggingEvent.MessageObject.ToString(), Environment.NewLine));
            }
        }

        private void Logging(string payload, Color color)
        {
            if (textBoxLogging.InvokeRequired)
            {
                Action<string> actionDelegate = (x) =>
                {
                    textBoxLogging.SelectionColor = color;
                    textBoxLogging.AppendText(String.Format("{0} {1}{2}", DateTime.Now.ToString("HH:mm:ss.fff"), payload, Environment.NewLine));
                    textBoxLogging.SelectionColor = textBoxLogging.ForeColor;
                };
                textBoxLogging.Invoke(actionDelegate, payload);
            }
            else
            {
                textBoxLogging.SelectionColor = color;
                    textBoxLogging.AppendText(String.Format("{0} {1}{2}", DateTime.Now.ToString("HH:mm:ss.fff"), payload, Environment.NewLine));
                textBoxLogging.SelectionColor = textBoxLogging.ForeColor;
            }

        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            this.Logging(loggingEvent);
        }
        #endregion
    }
}
