using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Xml;

namespace FolderMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        private string HomeDir = (new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory)).FullName.Trim();
        private FileSystemWatcher _fileWatcher;
        string monitoredFolder = "C:\\";
        string destinationPath = "C:\\";

        public MonitorService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            this.checkParameter();
            
            this.FolderMonitor();
            
        }

        protected override void OnStop()
        {
        }


        public void FolderMonitor()
        {
            _fileWatcher = new FileSystemWatcher(this.monitoredFolder);

            _fileWatcher.Created += new FileSystemEventHandler(onChange);
            _fileWatcher.Deleted += new FileSystemEventHandler(onChange);

            _fileWatcher.Renamed += new RenamedEventHandler(onRename);

            _fileWatcher.EnableRaisingEvents = true;

        }

        void onChange(object sender, FileSystemEventArgs e)
        {
            WatcherChangeTypes wct = e.ChangeType;
            this.Log(string.Format("{0} File {1}: {2}", DateTime.Now.ToString(), wct.ToString(), e.FullPath, e.Name));
        }

        void onRename(object sender, RenamedEventArgs e)
        {
            WatcherChangeTypes wct = e.ChangeType;
            this.Log(string.Format("{0} File {1}: From {2}, To {3}", DateTime.Now.ToString(), wct.ToString(), e.OldFullPath, e.FullPath));
        }

        public void checkParameter()
        {
            if (!System.IO.Directory.Exists(this.HomeDir + "\\config"))
            {
                System.IO.Directory.CreateDirectory(this.HomeDir + "\\config");
                return;
            }
            else
            {
                if (System.IO.File.Exists(this.HomeDir + "\\config\\params.xml"))
                {
                    Boolean docparsed = true;
                    XmlDocument parametersdoc = new XmlDocument();

                    try
                    {
                        parametersdoc.Load(this.HomeDir + "\\config\\params.xml");
                    }
                    catch (XmlException ex)
                    {
                        docparsed = false;
                    }

                    if (docparsed)
                    {
                        XmlNode PathParameters = parametersdoc.ChildNodes.Item(1).ChildNodes.Item(0);
                        this.monitoredFolder = PathParameters.Attributes.GetNamedItem("monitored").Value.Trim();
                        this.destinationPath = PathParameters.Attributes.GetNamedItem("destination").Value.Trim();
                        parametersdoc = null;
                    }
                    else
                        return;

                }
                else
                    return;
            }
        }

        public void Log(string message)
        {
            try
            {
                string _message = String.Format("{0} {1}", message, Environment.NewLine);
                File.AppendAllText(this.destinationPath + "\\logFile.txt", _message);
            }
            catch (Exception ex)
            {
                //Implement logging on next version.
            }
        }

    }
}
