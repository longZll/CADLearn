using System;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using Autodesk.AutoCAD.EditorInput;
namespace StatusBars
{
    public class StatusBars
    {
        [CommandMethod("CreateAppPane")]
        public void AddApplicationPane()
        {
            //定义一个程序窗格对象
            Pane appPaneButton = new Pane();
            //设置窗格的属性
            appPaneButton.Enabled = true;
            appPaneButton.Visible = true;
            //设置窗格初始状态是弹出的
            appPaneButton.Style = PaneStyles.Normal;
            //设置窗格的标题
            appPaneButton.Text = "程序窗格";
            //显示窗格的提示信息
            appPaneButton.ToolTipText = "欢迎进行入.net的世界！";
            //添加MouseDown事件，当鼠标被按下时运行
            appPaneButton.MouseDown += OnAppMouseDown;
            //把窗格添加到AutoCAD的状态栏区域
            Application.StatusBar.Panes.Add(appPaneButton);
        }
        void OnAppMouseDown(object sender, StatusBarMouseDownEventArgs e)
        {
            //获取窗格按钮对象
            Pane paneButton = (Pane)sender;
            string alertMessage;
            //如果点击的不是鼠标左键，则返回
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                return;
            }
            //切换窗格按钮的状态
            if (paneButton.Style == PaneStyles.PopOut)//如果窗格按钮是弹出的，则切换为凹进
            {
                paneButton.Style = PaneStyles.Normal;
                alertMessage = "程序窗格按钮被按下";
            }
            else
            {
                paneButton.Style = PaneStyles.PopOut;
                alertMessage = "程序窗格按钮没有被按下";
            }
            //更新状态栏以反映窗格按钮的状态变化
            Application.StatusBar.Update();
            //显示反映窗格按钮变化的信息
            Application.ShowAlertDialog(alertMessage);
        }
        [CommandMethod("StatusBarBalloon")]
        public void StatusBarBalloon()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Editor ed=doc.Editor;
            //提示用户输入需要改变颜色的对象
            ObjectId id=ed.GetEntity("请选择需要改变颜色的对象").ObjectId;
            TrayItem trayItem=new TrayItem();//新建一个托盘项目
            trayItem.ToolTipText = "改变对象的颜色";//托盘项目的提示字符
            //托盘项目的图标
            trayItem.Icon = doc.StatusBar.TrayItems[0].Icon;
            //将托盘项目添加到AutoCAD的状态栏区域
            Application.StatusBar.TrayItems.Add(trayItem);
            //新建一个气泡通知窗口
            TrayItemBubbleWindow window=new TrayItemBubbleWindow();
            window.Title = "改变对象的颜色";//气泡窗口的标题
            window.HyperText = "对象颜色改为红色";//气泡窗口的链接文本
            window.Text = "点击改变对象的颜色";//气泡窗口的描述文本
            window.IconType = IconType.Information;//气泡窗口的图标类型
            trayItem.ShowBubbleWindow(window);//在托盘上显示气泡窗口
            Application.StatusBar.Update();//更新状态栏
            //注册气泡窗口关闭事件
            window.Closed += (sender, e) =>
            {
                //如果用户点击了气泡窗口中的链接，则设置对象的颜色为红色
                if (e.CloseReason == TrayItemBubbleWindowCloseReason.HyperlinkClicked)
                {
                    using (doc.LockDocument())
                    using (Transaction trans=doc.TransactionManager.StartTransaction())
                    {
                        Entity ent=(Entity)trans.GetObject(id, OpenMode.ForWrite);
                        ent.ColorIndex = 1;
                        trans.Commit();
                    }
                }
                //气泡窗口关闭后，将托盘从状态栏删除
                Application.StatusBar.TrayItems.Remove(trayItem);
                Application.StatusBar.Update();//更新状态栏
            };
        }
    }
}
