using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using WinApp = System.Windows.Application;
namespace Ribbons
{
    public class Ribbons
    {
        //调用资源字典
        ResourceDictionary resourceDictionary=(ResourceDictionary)WinApp.LoadComponent(new Uri("/Ribbons;component/RibbonDictionary.xaml", UriKind.Relative));
        [CommandMethod("AddRibbon")]
        public void AddRibbon()
        {
            RibbonControl control=ComponentManager.Ribbon;//获取Ribbon界面
            RibbonTab tab=new RibbonTab();//创建选项卡
            tab.Title = "C# Ribbon";//设置选项卡的标题
            tab.Id = "ID_TabMyRibbon";//设置选项卡的AutoCAD标识符
            control.Tabs.Add(tab);//将选项卡添加到Ribbon界面中
            addPanel(tab);//为选项卡添加Ribbon面板
            tab.IsActive = true;//设置当前活动选项卡
        }
        [CommandMethod("AddRibbonXAML")]
        public void AddRibbonXAML()
        {
            //获取由XAML定义的选项卡
            RibbonTab tab=resourceDictionary["TabXaml"] as RibbonTab;
            //查找Ribbon按钮并添加命令事件
            RibbonItemCollection items= tab.Panels[0].Source.Items;
            foreach (RibbonItem item in items)
            {
                if (item is RibbonButton)
                    ((RibbonButton)item).CommandHandler = new RibbonCommandHandler();
                else if (item is RibbonRowPanel)
                {
                    RibbonRowPanel row=(RibbonRowPanel)item;
                    foreach (RibbonItem rowItem in row.Items)
                    {
                        if (rowItem is RibbonButton)
                            ((RibbonButton)rowItem).CommandHandler = new RibbonCommandHandler();
                    }
                }
            }
            RibbonControl ribbonControl = ComponentManager.Ribbon;//获取Ribbon界面
            ribbonControl.Tabs.Add(tab);//将选项卡添加到Ribbon界面中
            ribbonControl.ActiveTab = tab;//设置当前活动选项卡
        }
        private void addPanel(RibbonTab tab)
        {
            //创建RibbonPanelSource对象，用来加入Ribbon元素（如按钮）
            RibbonPanelSource sourcePanel=new RibbonPanelSource();
            sourcePanel.Title = "演示";//Ribbon面板的标题
            RibbonPanel ribPanel=new RibbonPanel();//创建Ribbon面板
            ribPanel.Source = sourcePanel;//设置Ribbon面板的内容
            tab.Panels.Add(ribPanel);//将Ribbon面板添加到选项卡中
            RibbonButton buttonLine=new RibbonButton();//创建按钮
            buttonLine.Name = "直线";//按钮名称 
            buttonLine.Text = "直线";//按钮显示的文字
            buttonLine.ShowImage = true;//按钮显示图像
            buttonLine.ShowText = true;//按钮显示文字
            //设置按钮的大小图像
            buttonLine.Image = resourceDictionary["LineImage"] as BitmapImage;
            buttonLine.LargeImage = resourceDictionary["LineImage"] as BitmapImage;
            buttonLine.Size = RibbonItemSize.Large;//以大图像的形式表示按钮
            //按钮文字和图像的方向为竖直
            buttonLine.Orientation = System.Windows.Controls.Orientation.Vertical;
            buttonLine.CommandParameter = "Line ";//设置按钮的命令参数
            //设置按钮的命令处理程序
            buttonLine.CommandHandler = new RibbonCommandHandler();
            RibbonToolTip toolTipLine=new RibbonToolTip();//创建按钮提示
            toolTipLine.Command = "Line";//设置提示中显示的命令名
            toolTipLine.Title = "直线";//设置提示的标题
            toolTipLine.Content = "创建直线段";//设置提示的内容
            //设置提示的附加内容
            toolTipLine.ExpandedContent = "使用Line命令，可以创建一系列连续的直线段。每条线段都是可以单独进行编辑的直线对象。";
            //设置在提示附加内容下显示的图像
            toolTipLine.ExpandedImage = resourceDictionary["TooltipLineImage"] as BitmapImage;
            buttonLine.ToolTip = toolTipLine;//设置“直线”按钮的提示
            //创建“单行文字”按钮
            RibbonButton buttonText=new RibbonButton();
            buttonText.Name = "单行文字";
            buttonText.Text = "文字";
            buttonText.ShowImage = true;
            buttonText.LargeImage = resourceDictionary["TextImage"] as BitmapImage;
            buttonText.Image = resourceDictionary["TextImage"] as BitmapImage;
            buttonText.Size = RibbonItemSize.Large;
            buttonText.CommandParameter = "Text ";
            buttonText.CommandHandler = new RibbonCommandHandler();
            //创建“多行文字”按钮
            RibbonButton buttonMText=new RibbonButton();
            buttonMText.Name = "多行文字";
            buttonMText.Text = "文字";
            buttonMText.ShowImage = true;
            buttonMText.LargeImage = resourceDictionary["MTextImage"] as BitmapImage;
            buttonMText.Image = resourceDictionary["MTextImage"] as BitmapImage;
            buttonMText.Size = RibbonItemSize.Large;
            buttonMText.CommandParameter = "MText ";
            buttonMText.CommandHandler = new RibbonCommandHandler();
            //创建组合下拉按钮
            RibbonSplitButton splitButton=new RibbonSplitButton();
            splitButton.Text = "文字";
            splitButton.ShowText = true;
            splitButton.ShowImage = true;
            splitButton.Size = RibbonItemSize.Large;
            splitButton.Orientation = System.Windows.Controls.Orientation.Vertical;
            splitButton.Items.Add(buttonText);//添加“单行文本”按钮到组合下拉按钮
            splitButton.Items.Add(buttonMText);//添加“多行文本”按钮到组合下拉按钮
            //创建行面板
            RibbonRowPanel row1=new RibbonRowPanel();
            //创建“移动”按钮
            RibbonButton buttonMove=new RibbonButton
            {
                Name = "移动",
                Text = "移动",
                ShowImage = true,
                ShowText = true,
                Image = resourceDictionary["MoveImage"] as BitmapImage,
                Size = RibbonItemSize.Standard,
                CommandParameter = "Move ",
                CommandHandler = new RibbonCommandHandler(),
            };
            row1.Items.Add(buttonMove);//添加“移动”按钮到行面板
            row1.Items.Add(new RibbonRowBreak());//换行，这样可以使按钮多行排列
            //创建“复制”按钮
            RibbonButton buttonCopy=new RibbonButton
            {
                Name = "复制",
                Text = "复制",
                ShowImage = true,
                ShowText = true,
                Image = resourceDictionary["CopyImage"] as BitmapImage,
                Size = RibbonItemSize.Standard,
                CommandParameter = "Copy ",
                CommandHandler = new RibbonCommandHandler(),
            };
            row1.Items.Add(buttonCopy);
            row1.Items.Add(new RibbonRowBreak());
            //创建“拉伸”按钮
            RibbonButton buttonStretch=new RibbonButton
            {
                Name = "拉伸",
                Text = "拉伸",
                ShowImage = true,
                ShowText = true,
                Image = resourceDictionary["StretchImage"] as BitmapImage,
                Size = RibbonItemSize.Standard,
                CommandParameter = "Stretch ",
                CommandHandler = new RibbonCommandHandler()
            };
            row1.Items.Add(buttonStretch);
            row1.Items.Add(new RibbonRowBreak());
            //将“直线”按钮、组合下拉按钮、行面板添加到"演示"面板中
            sourcePanel.Items.Add(buttonLine);
            sourcePanel.Items.Add(splitButton);
            sourcePanel.Items.Add(row1);
        }
        public class RibbonCommandHandler : System.Windows.Input.ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;//确定此命令可以在其当前状态下执行
            }
            //当出现影响是否应执行该命令的更改时发生
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                //获取发出命令的按钮对象
                RibbonButton button = parameter as RibbonButton;
                //如果发出命令的不是按钮或按钮未定义命令参数，则返回
                if (button == null || button.CommandParameter == null) return;
                //根据按钮的命令参数，执行对应的AutoCAD命令
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                doc.SendStringToExecute(button.CommandParameter.ToString(), true, false, true);
            }
        }
    }
}
