using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.AutoCAD.ApplicationServices;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace Wpfs
{
    /// <summary>
    /// WpfPanel.xaml 的交互逻辑
    /// </summary>
    public partial class CollapsePanel : UserControl
    {
        public CollapsePanel()
        {
            InitializeComponent();
        }
        private void executeCommand(object sender, RoutedEventArgs e)
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Button button=sender as Button;
            string content=button.Content.ToString();
            switch (content)
            {
                case "直线":
                    doc.SendStringToExecute("_Line ", true, false, true);
                    break;
                case "多段线":
                    doc.SendStringToExecute("_PLine ", true, false, true);
                    break;
                case "矩形":
                    doc.SendStringToExecute("_Rectangle ", true, false, true);
                    break;
                case "圆":
                    doc.SendStringToExecute("_Circle ", true, false, true);
                    break;
                case "复制":
                    doc.SendStringToExecute("_Copy ", true, false, true);
                    break;
                case "删除":
                    doc.SendStringToExecute("_Erase ", true, false, true);
                    break;
                case "移动":
                    doc.SendStringToExecute("_Move ", true, false, true);
                    break;
                case "旋转":
                    doc.SendStringToExecute("_Rotate ", true, false, true);
                    break;
            }
        }
    }
}
