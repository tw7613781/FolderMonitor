using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.ServiceProcess;

namespace FolderMonitorGUI
{
    public partial class foldermonitorGUI : Form
    {

        string HomeDir = Path.GetDirectoryName(Application.ExecutablePath).Trim();

        public foldermonitorGUI()
        {
            InitializeComponent();
        }


        private Boolean check_parameters()
        {
            Boolean result = default(Boolean);

            if (!System.IO.Directory.Exists(this.HomeDir + "\\config"))
            {
                System.IO.Directory.CreateDirectory(this.HomeDir + "\\config");
                result = false;
            }
            else
            {

                if (System.IO.File.Exists(this.HomeDir + "\\config\\params.xml"))
                {
                    result = true;
                    XmlDocument parametersdoc = new XmlDocument();

                    try
                    {
                        parametersdoc.Load(this.HomeDir + "\\config\\params.xml");
                    }
                    catch
                    {
                        result = false;
                    }

                    if (result)
                    {
                        XmlNode PathParameters = parametersdoc.ChildNodes.Item(1).ChildNodes.Item(0);
                        this.textBox1.Text = PathParameters.Attributes.GetNamedItem("monitored").Value.Trim();
                        this.textBox1.Refresh();
                        this.textBox2.Text = PathParameters.Attributes.GetNamedItem("destination").Value.Trim();
                        this.textBox2.Refresh();
                    }

                    parametersdoc = null;


                }
                else
                {
                    result = false;
                }
            }

            return (result);
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (this.textBox1.Text.Trim().Length == 0)
            {
                e.Cancel = true;
            }
            else
            {
                if (!System.IO.Directory.Exists(this.textBox1.Text.Trim()))
                {
                    MessageBox.Show("The Monitored Path entered doesn't exist.", "Folder Monitor Interface");
                    e.Cancel = true;
                }
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if (this.textBox2.Text.Trim().Length == 0)
            {
                e.Cancel = true;
            }
            else
            {
                if (!System.IO.Directory.Exists(this.textBox2.Text.Trim()))
                {
                    MessageBox.Show("The Destination Path entered doesn't exist.", "Folder Monitor Interface");
                    e.Cancel = true;
                }
            }

        }

        private void Save_Parameter()
        {
            XmlDocument oparamsxml = new XmlDocument();

            XmlProcessingInstruction _xml_header = oparamsxml.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

            oparamsxml.InsertBefore(_xml_header, oparamsxml.ChildNodes.Item(0));

            XmlNode parameters = oparamsxml.CreateNode(XmlNodeType.Element, "Parameters", "");
            XmlNode path = oparamsxml.CreateNode(XmlNodeType.Element, "Path", "");

            XmlAttribute attribute = oparamsxml.CreateAttribute("monitored");
            attribute.Value = this.textBox1.Text.Trim();
            path.Attributes.Append(attribute);

            attribute = oparamsxml.CreateAttribute("destination");
            attribute.Value = this.textBox2.Text.Trim();
            path.Attributes.Append(attribute);

            parameters.AppendChild(path);
            oparamsxml.AppendChild(parameters);

            oparamsxml.Save(this.HomeDir + "\\config\\params.xml");
        }

        private void Notify_Changes()
        {
            ServiceController controller = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Folder Monitor Service");

            if (controller != null)
            {
                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    controller.Start();
                }

                if (controller.Status == ServiceControllerStatus.Running)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped);
                    controller.Start();
                }
            }
        }

        private void Stop_Service()
        {
            ServiceController controller = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Folder Monitor Service");
            if (controller != null)
            {
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    controller.Stop();
                }
            }
        }

        private void foldermonitorGUI_Load(object sender, EventArgs e)
        {
            if (!this.check_parameters())
            {
                this.textBox1.Text = "C:\\";
                this.textBox1.Refresh();
                this.textBox2.Text = "C:\\";
                this.textBox2.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Save_Parameter();
            this.Notify_Changes();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Stop_Service();
            this.Close();
        }
    }
}
