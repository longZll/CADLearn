using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace Wpfs
{
    public class Command
    {
        internal static PaletteSet ps=null;
        [CommandMethod("ShowPalette")]
        public void ShowPalette()
        {
            if (ps == null)//如果面板没有被创建
            {
                //新建一个面板集对象
                ps = new PaletteSet("WPF面板示例", typeof(Command).GUID);
                //添加标题为“绑定”的面板项
                ps.AddVisual("绑定", new BindingPanel()); 
                //声明一个可以承载WPF元素的WinForm控件
                ElementHost host=new ElementHost();
                host.AutoSize = true;//WinForm控件可以自动调整大小
                host.Dock = DockStyle.Fill;//WinForm控件充满其容器窗口
                host.Child = new CollapsePanel();//设置要承载的WPF元素
                ps.Add("抽屉式面板", host);//添加标题为“抽屉式面板”的面板项
            }
            ps.Visible = true;//面板可见
        }
        [CommandMethod("ShowLayerWindow")]
        public static void ShowLayerWindow()
        {
            //新建一个WPF形式的自定义图层窗口
            LayerWindow dlg = new LayerWindow();
            //以非模式形式显示窗口
            AcadApp.ShowModelessWindow(dlg);            
        }        
    }
}
