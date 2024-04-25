using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using Autodesk.AutoCAD.Windows;
namespace CUIExample
{
    public class CUIExample
    {
        //设置CUI文件的名字，将其路径设置为当前运行目录
        string cuiFile = Tools.GetCurrentPath() + "\\MyCustom.cui";
        string menuGroupName = "MyCustom";//菜单组名
        //获得活动文档
        Document activeDoc = Application.DocumentManager.MdiActiveDocument;
        public CUIExample()
        {
            //添加程序退出时事件处理
            Application.QuitWillStart += new EventHandler(Application_QuitWillStart);
        }
        void Application_QuitWillStart(object sender, EventArgs e)
        {
            //由于触发此事件前文档已关闭，所以需通过模板重建，以便命令能够执行
            Document doc = Application.DocumentManager.Add("acadiso.dwt");
            //获取FILEDIA系统变量的值
            object oldFileDia = Application.GetSystemVariable("FILEDIA");
            //设置FILEDIA=0，禁止显示文件对话框，这样可以通过程序输入文件名
            Application.SetSystemVariable("FILEDIA", 0);
            //获取主CUI
            CustomizationSection mainCs = doc.GetMainCustomizationSection();
            //如果存在指定的局部CUI文件，则进行卸载
            if (mainCs.PartialCuiFiles.Contains(cuiFile))
                doc.Editor.PostCommand("cuiunload " + menuGroupName + " ");
            //恢复FILEDIA系统变量的值
            Application.SetSystemVariable("FILEDIA", oldFileDia);
        }
        [CommandMethod("AddMenu")]
        public void AddMenu()
        {
            string currentPath = Tools.GetCurrentPath();//当前运行目录
            //装载局部CUI文件，若不存在，则创建
            CustomizationSection cs = activeDoc.AddCui(cuiFile, menuGroupName);
            //添加表示绘制直线、多段线、矩形和圆的命令宏            
            cs.AddMacro("直线", "^C^C_Line ", "ID_MyLine", "创建直线段:   LINE", currentPath + "\\Image\\Line.BMP");
            cs.AddMacro("多段线", "^C^C_Pline ", "ID_MyPLine", "创建二维多段线:  PLINE", currentPath + "\\Image\\Polyline.BMP");
            cs.AddMacro("矩形", "^C^C_Rectang ", "ID_MyRectang", "创建矩形多段线:  RECTANG", currentPath + "\\Image\\Rectangle.BMP");
            cs.AddMacro("圆", "^C^C_circle ", "ID_MyCircle", "用指定半径创建圆:   CIRCLE", currentPath + "\\Image\\Circle.BMP");
            //添加表示复制、删除、移动及旋转操作的命令宏
            cs.AddMacro("复制", "^C^CCopy ", "ID_MyCopy", "复制对象:   COPY", currentPath + "\\Image\\Copy.BMP");
            cs.AddMacro("删除", "^C^CErase ", "ID_MyErase", "从图形删除对象:   ERASE", currentPath + "\\Image\\Erase.BMP");
            cs.AddMacro("移动", "^C^CMove ", "ID_MyMove", "将对象在指定方向上平移指定的距离:  MOVE", currentPath + "\\Image\\Move.BMP");
            cs.AddMacro("旋转", "^C^CRotate ", "ID_MyRotate", "绕基点旋转对象:  ROTATE", currentPath + "\\Image\\Rotate.BMP");
            //设置用于下拉菜单别名的字符串集合
            StringCollection sc = new StringCollection();
            sc.Add("MyPop1");
            //添加名为“我的菜单”的下拉菜单，如果已经存在，则返回null
            PopMenu myMenu = cs.MenuGroup.AddPopMenu("我的菜单", sc, "ID_MyMenu");
            if (myMenu != null)//如果“我的菜单”还没有被添加，则添加菜单项
            {
                //从上到下为“我的菜单”添加绘制直线、多段线、矩形和圆的菜单项
                myMenu.AddMenuItem(-1, null, "ID_MyLine");
                myMenu.AddMenuItem(-1, null, "ID_MyPLine");
                myMenu.AddMenuItem(-1, null, "ID_MyRectang");
                myMenu.AddMenuItem(-1, null, "ID_MyCircle");
                myMenu.AddSeparator(-1);//为菜单添加一分隔条
                //添加一个名为“修改”的子菜单
                PopMenu menuModify = myMenu.AddSubMenu(-1, "修改", "ID_MyModify");
                //从上到下为“修改”子菜单添加复制、删除、移动及旋转操作的菜单项
                menuModify.AddMenuItem(-1, null, "ID_MyCopy");
                menuModify.AddMenuItem(-1, null, "ID_MyErase");
                menuModify.AddMenuItem(-1, null, "ID_MyMove");
                menuModify.AddMenuItem(-1, null, "ID_MyRotate");
            }
            cs.LoadCui();//必须装载CUI文件，才能看到添加的菜单
        }
        [CommandMethod("AddToolbar")]
        public void AddToolbar()
        {
            //装载局部CUI文件，若不存在，则创建
            CustomizationSection cs = activeDoc.AddCui(cuiFile, menuGroupName);
            //添加名为“我的工具栏”的工具栏
            Toolbar barDraw = cs.MenuGroup.AddToolbar("我的工具栏");
            //为“我的工具栏”添加绘制直线、多段线、矩形和圆的按钮
            barDraw.AddToolbarButton(-1, "直线", "ID_MyLine");
            barDraw.AddToolbarButton(-1, "多段线", "ID_MyPLine");
            barDraw.AddToolbarButton(-1, "矩形", "ID_MyRectang");
            barDraw.AddToolbarButton(-1, "圆", "ID_MyCircle");
            //添加名为“修改工具栏”的工具栏，用于弹出式工具栏
            Toolbar barModify = cs.MenuGroup.AddToolbar("修改工具栏");
            //为“修改工具栏”添加复制、删除、移动及旋转操作的按钮
            ToolbarButton buttonCopy = barModify.AddToolbarButton(-1, "复制", "ID_MyCopy");
            ToolbarButton buttonErase = barModify.AddToolbarButton(-1, "删除", "ID_MyErase");
            ToolbarButton buttonMove = barModify.AddToolbarButton(-1, "移动", "ID_MyMove");
            ToolbarButton buttonRotate = barModify.AddToolbarButton(-1, "旋转", "ID_MyRotate");
            //将“修改工具栏”附着到“我的工具栏”的最后
            barDraw.AttachToolbarToFlyout(-1, barModify);
            cs.LoadCui();//必须装载CUI文件，才能看到添加的菜单
        }
        [CommandMethod("AddDoubleClick")]
        public void AddDoubleClick()
        {
            //装载局部CUI文件，若不存在，则创建
            CustomizationSection cs = activeDoc.AddCui(cuiFile, menuGroupName);
            //添加表示双击多段线动作的命令宏  
            MenuMacro macro = cs.AddMacro("多段线 - 双击", "^C^C_DoubleClickPline ", "ID_PlineDoubleClick", "调用自定义命令", null);
            //创建双击动作
            DoubleClickAction action = new DoubleClickAction(cs.MenuGroup, "优化多段线", -1);
            action.ElementID = "EID_mydblclick";//双击动作的标识号
            //设置双击动作的对象为多段线
            action.DxfName = RXClass.GetClass(typeof(Polyline)).DxfName;
            //创建一个双击命令对象，指定双击对象时执行的命令宏
            DoubleClickCmd cmd = new DoubleClickCmd(action, macro);
            action.DoubleClickCmd = cmd;//指定双击动作的命令对象
            cs.LoadCui();//必须装载CUI文件，才能看到添加的菜单
        }
        [CommandMethod("DoubleClickPline")]
        public void DoubleClickPline()
        {
            Application.ShowAlertDialog("你双击了多段线！");
        }
        [CommandMethod("AddDefaultContextMenu")]
        public void AddDefaultContextMenu()
        {
            //定义一个ContextMenuExtension对象，用于表示快捷菜单
            ContextMenuExtension contextMenu = new ContextMenuExtension();
            contextMenu.Title = "我的快捷菜单";//设置快捷菜单的标题
            MenuItem mi = new MenuItem("复制");//添加名为"复制"的菜单项
            //为"复制"菜单项添加单击事件
            mi.Click += new EventHandler(mi_Click);
            contextMenu.MenuItems.Add(mi);//将"复制"菜单项添加到快捷菜单中
            mi = new MenuItem("删除");//添加名为"删除"的菜单项
            //为"删除"菜单项添加单击事件
            mi.Click += new EventHandler(mi_Click);
            contextMenu.MenuItems.Add(mi);//将"删除"菜单项添加到快捷菜单中
            //为应用程序添加定义的快捷菜单
            Application.AddDefaultContextMenuExtension(contextMenu);
        }
        void mi_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;//获取发出命令的快捷菜单项
            //根据快捷菜单项的名字，分别调用对应的命令
            if (mi.Text == "复制")
                activeDoc.SendStringToExecute("_Copy ", true, false, true);
            else if (mi.Text == "删除")
                activeDoc.SendStringToExecute("_Erase ", true, false, true);
        }
        [CommandMethod("AddObjectContextMenu")]
        public void AddObjectContextMenu()
        {
            //定义一个ContextMenuExtension对象，用于表示快捷菜单
            ContextMenuExtension contextMenu = new ContextMenuExtension();
            //添加一个名为"统计个数"的菜单项，用于在AutoCAD命令行上显示所选择实体的个数
            MenuItem miCircle = new MenuItem("统计个数");
            //为"统计个数"菜单项添加单击事件，事件处理函数为调用自定义的Count命令
            miCircle.Click += delegate(object sender, EventArgs e)
            {
                activeDoc.SendStringToExecute("_Count ", true, false, false);
            };
            contextMenu.MenuItems.Add(miCircle);//将"统计个数"菜单项添加到快捷菜单中
            //获得实体所属的RXClass类型
            RXClass rx = RXClass.GetClass(typeof(Entity));
            //为实体对象添加定义的快捷菜单
            Application.AddObjectContextMenuExtension(rx, contextMenu);
        }
        [CommandMethod("Count", CommandFlags.UsePickSet)]
        public void CountEnts()
        {
            Editor ed = activeDoc.Editor;
            PromptSelectionResult result = ed.SelectImplied();
            if (result.Status == PromptStatus.OK)
                ed.WriteMessage("共选择了" + result.Value.Count + "个实体\n");
        }
    }
}
