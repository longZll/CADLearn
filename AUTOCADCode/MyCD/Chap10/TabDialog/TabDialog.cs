using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using DotNetARX;
namespace TabDialog
{
    public class TabDialog
    {
        static Document doc=Application.DocumentManager.MdiActiveDocument;
        static Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
        //声明一个用于属性网格中要显示属性的对象
        static Drawing dwg=null;
        [CommandMethod("CreateNewOptionsTab")]
        public void CreateNewOptionsTab()
        {
            //当DisplayingOptionDialog事件被触发时（即选项对话框显示），运行对应的处理函数
            Application.DisplayingOptionDialog += new TabbedDialogEventHandler(Application_DisplayingOptionDialog);
        }
        private void OnOK()
        {
            SaveDwg();//保存在属性网格中所做的更改，并关闭选项对话框
        }


        private void OnCancel()
        {
            LoadDwg();//取消在属性网格中所做的更改，并关闭选项对话框
        }
        private void OnHelp()
        {
            //这里你可以添加一些帮助说明性的程序            
        }
        private void OnApply()
        {
            SaveDwg();//保存在属性网格中所做的更改，且不关闭选项对话框
        }
        void Application_DisplayingOptionDialog(object sender, TabbedDialogEventArgs e)
        {
            //如果没有为属性网格指定对象，则指定对象
            if (dwg == null) LoadDwg();
            //创建一个自定义控件的实例，这里使用的是显示个人信息的自定义控件
            TabControl tab=new TabControl();
            //设置对象属性窗口中需要显示的对象
            tab.propertyGridOwner.SelectedObject = dwg;
            //更新对象属性窗口
            tab.propertyGridOwner.Update();
            //为控件的确定、取消、帮助、应用按钮添加动作
            TabbedDialogExtension tabExtension=new TabbedDialogExtension(
                tab,
                new TabbedDialogAction(OnOK),
                new TabbedDialogAction(OnCancel),
                new TabbedDialogAction(OnHelp),
                new TabbedDialogAction(OnApply));
            //将控件作为标签页添加到选项对话框
            e.AddTab("自定义", tabExtension);
        }

        private static void LoadDwg()
        {
            dwg = new Drawing();//创建一个用于属性网格中要显示属性的对象
            //根据程序的配置文件内容设置当前图形的作者信息
            dwg.Name = Properties.Settings.Default.Name;
            dwg.Age = Convert.ToInt32(Properties.Settings.Default.Age);
            dwg.City = (CityName)Enum.Parse(typeof(CityName), Properties.Settings.Default.City);
            //设置属性窗口的颜色为当前图形模型空间的背景颜色
            dwg.BackgroundColor = Preferences.Display.GraphicsWinModelBackgrndColor;
        }
        private static void SaveDwg()
        {
            //根据属性网格中的属性更新程序的配置文件中
            Properties.Settings.Default.Name = dwg.Name;
            Properties.Settings.Default.Age = dwg.Age;
            Properties.Settings.Default.City = dwg.City.ToString();
            //设置当前图形模型空间的颜色为属性窗口指定的颜色
            Preferences.Display.GraphicsWinModelBackgrndColor = dwg.BackgroundColor;
            Properties.Settings.Default.Save();//保存配置文件
        }
    }
}
