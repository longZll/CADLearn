using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AppStarter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //获取系统中安装的AutoCAD路径列表
            List<string> locations = GetAutoCADLocations();
            foreach (string location in locations)
            {
                //在列表框中显示系统中安装的AutoCAD
                string installedCADs=location.Split('\\').Last();
                this.listBoxAutoCAD.Items.Add(installedCADs);
            }
            this.listBoxAutoCAD.SelectedIndex = 0;//设置列表框的选定项

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            List<string> locations = GetAutoCADLocations();//获取系统中安装的AutoCAD路径列表
            //获取列表框中选择的AutoCAD版本所代表的序号
            string location=locations[this.listBoxAutoCAD.SelectedIndex];
            //启动列表框所选版本的AutoCAD
            System.Diagnostics.Process.Start(location + "\\acad.exe");
            this.Dispose();//销毁窗体
        }

        public static List<string> GetAutoCADLocations()
        {
            //用于存储系统中安装的AutoCAD路径列表
            List<string> locations=new List<string>();
            // 获取HKEY_LOCAL_MACHINE键
            RegistryKey keyLocalMachine = Registry.LocalMachine;
            // 打开AutoCAD所属的注册表键:HKEY_LOCAL_MACHINE\Software\Autodesk\AutoCAD
            RegistryKey keyAutoCAD =keyLocalMachine.OpenSubKey("Software\\Autodesk\\AutoCAD");
            //获得表示系统中安装的各版本的AutoCAD注册表键
            string[] cadVersions=keyAutoCAD.GetSubKeyNames();
            foreach (string cadVersion in cadVersions)
            {
                //打开特定版本的AutoCAD注册表键
                RegistryKey keyCADVersion =keyAutoCAD.OpenSubKey(cadVersion);
                //获取表示各语言版本的AutoCAD注册表键值
                string[] cadNames=keyCADVersion.GetSubKeyNames();
                foreach (string cadName in cadNames)
                {
                    if (cadName.EndsWith("804"))//中文版本
                    {
                        //打开中文版本的AutoCAD所属的注册表键
                        RegistryKey keyCADName =keyCADVersion.OpenSubKey(cadName);
                        //获取AutoCAD的安装路径
                        string location=keyCADName.GetValue("Location").ToString();
                        locations.Add(location);//将路径添加到列表中
                    }
                }
            }
            return locations;//返回系统中安装的AutoCAD路径列表
        }
    }
}
