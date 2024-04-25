using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using System.Drawing;
using Autodesk.AutoCAD.Geometry;
using DotNetARX;
namespace Palettes
{
    public class Palettes
    {
        internal static PaletteSet ps=null;
        static UCTreeView treeControl=null;
        static UCDockDrag dockControl=null;
        [CommandMethod("ShowPalette")]
        public void ShowPalette()
        {
            if (ps == null)//如果面板没有被创建
            {
                //新建一个面板对象，标题为"工作空间"
                ps = new PaletteSet("工作空间", typeof(Palettes).GUID);
                //添加标题为“门窗统计”的面板项（树形列表）
                treeControl = new UCTreeView();
                ps.Add("门窗统计", treeControl);
                //添加标题为“停靠及拖放”的面板项（组合框及文本框）
                dockControl = new UCDockDrag();
                ps.Add("停靠及拖放", dockControl);
                //添加文档打开事件
                AcadApp.DocumentManager.DocumentCreated += new DocumentCollectionEventHandler(DocumentManager_DocumentCreated);
                //添加文档关闭事件
                AcadApp.DocumentManager.DocumentDestroyed += new DocumentDestroyedEventHandler(DocumentManager_DocumentDestroyed);
                //添加面板装载事件，用于获取用户数据
                ps.Load += new PalettePersistEventHandler(ps_Load);
                //添加面板保存事件，在AutoCAD关闭时，保存用户数据
                ps.Save += new PalettePersistEventHandler(ps_Save);
            }
            ps.Visible = true;//面板可见
            updateTree();//填充树形列表
        }
        void ps_Save(object sender, PalettePersistEventArgs e)
        {
            //将文本框中的文本保存到配置文件中
            e.ConfigurationSection.WriteProperty("Drag", dockControl.textBoxDrag.Text);
        }
        void ps_Load(object sender, PalettePersistEventArgs e)
        {
            //如果配置文件中包含指定的用户数据，则将文本框内容设置为用户数据
            if (e.ConfigurationSection.Contains("Drag"))
                dockControl.textBoxDrag.Text = e.ConfigurationSection.ReadProperty("Drag", "test").ToString();
        }
        void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            updateTree();//有文档关闭，须重新更新树形列表
        }
        void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            updateTree();//有文档打开，须重新更新树形列表
        }
        private static void updateTree()
        {
            //清空树形列表中的内容
            treeControl.treeViewEnts.Nodes.Clear();
            //遍历AutoCAD中的文档
            foreach (Document doc in AcadApp.DocumentManager)
            {
                TreeNode nodeDoor = new TreeNode("门");//门节点
                nodeDoor.ImageIndex = 1;//门节点图标
                nodeDoor.SelectedImageIndex = 1;//门节点选中后的图标
                TreeNode nodeWindow = new TreeNode("窗");//窗节点
                nodeWindow.ImageIndex = 2;//窗节点图标
                nodeWindow.SelectedImageIndex = 2;//窗节点选中后的图标
                TreeNode[] nodes = new TreeNode[] { nodeDoor, nodeWindow };
                //定义一个以文档名称为标题的节点，该节点包含两个子节点：门、窗
                TreeNode nodeDoc=new TreeNode(doc.Name, nodes);
                nodeDoc.ImageIndex = 0;//文档节点图标
                //将文档节点添加到树形列表控件中
                treeControl.treeViewEnts.Nodes.Add(nodeDoc);
            }
        }
    }
    public class MyDropTarget : DropTarget
    {
        static string dropText;//拖放文本
        Database db=HostApplicationServices.WorkingDatabase;
        Document doc=AcadApp.DocumentManager.MdiActiveDocument;
        public override void OnDrop(DragEventArgs e)
        {
            //如果拖放的是字符串对象
            if (e.Data.GetDataPresent(typeof(string)))
            {
                //获取拖放的字符串
                dropText = (string)e.Data.GetData(typeof(string));
                //调用命令，在当前鼠标位置拖放的字符串
                string cmd=string.Format("Drop\n{0},{1},0\n", e.X, e.Y);
                doc.SendStringToExecute(cmd, false, false, false);
            }
        }
        [CommandMethod("Drop")]
        public void Drop()
        {
            Editor ed=doc.Editor;
            if (dropText != null)//如果有拖放文本
            {
                //提示输入文本要放置的位置
                PromptPointOptions opt=new PromptPointOptions("请输入文本放置的位置");
                PromptPointResult ppr=ed.GetPoint(opt);
                if (ppr.Status != PromptStatus.OK) return;
                Point3d pos=ppr.Value;//获取输入的位置（拖放操作的鼠标位置，为屏幕坐标）
                using (Transaction trans=db.TransactionManager.StartTransaction())
                {
                    DBText txt=new DBText();//新建文本对象
                    txt.TextString = dropText;//设置文本内容为拖放文本
                    //将屏幕坐标转换为AutoCAD的点坐标，再将该坐标值设置为文本的位置
                    txt.Position = ed.PointToWorld(new Point((int)pos.X, (int)pos.Y));
                    db.AddToModelSpace(txt);//将文本添加到模型空间
                    trans.Commit();
                }
                dropText = null;
            }
        }
    }
}
