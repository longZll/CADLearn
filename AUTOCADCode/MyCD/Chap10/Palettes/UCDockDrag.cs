using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.Windows;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.ApplicationServices;
namespace Palettes
{
    public partial class UCDockDrag : UserControl
    {
        public UCDockDrag()
        {
            InitializeComponent();
        }

        private void comboBoxDock_SelectedIndexChanged(object sender, EventArgs e)
        {
            //根据组合框选择的项目分别设置面板的停靠方式
            switch (this.comboBoxDock.SelectedIndex)
            {
                case 0://底部
                    Palettes.ps.Dock = DockSides.Bottom;
                    break;
                case 1://左侧
                    Palettes.ps.Dock = DockSides.Left;
                    break;
                case 2://右侧
                    Palettes.ps.Dock = DockSides.Right;
                    break;
                case 3://顶部
                    Palettes.ps.Dock = DockSides.Top;
                    break;
                case 4://浮动
                    Palettes.ps.Dock = DockSides.None;
                    break;
            }
        }

        private void comboBoxDock_DropDown(object sender, EventArgs e)
        {
            //处理组合框无法选择的问题，浮动面板除外
            if (Palettes.ps.Dock != DockSides.None)
            {
                Palettes.ps.KeepFocus = true;
            }
        }

        private void comboBoxDock_DropDownClosed(object sender, EventArgs e)
        {
            if (Palettes.ps.Dock != DockSides.None)
            {
                Palettes.ps.KeepFocus = false;
            }
        }

        private void textBoxDrag_MouseMove(object sender, MouseEventArgs e)
        {
            //只执行左键移动操作，以表示进行了拖放操作
            if (Control.MouseButtons.Equals(MouseButtons.Left))
            {
                //必须锁定文档，否则会出错
                using (DocumentLock loc=AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    //文本框对象触发拖放事件，调用拖放操作事件处理函数，并传入文本框对象的Text属性供事件处理函数进行判断
                    AcadApp.DoDragDrop(this, this.textBoxDrag.Text, DragDropEffects.All, new MyDropTarget());
                }
            }
        }
    }
}
