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
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using DotNetARX;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.AutoCAD.EditorInput;

namespace Wpfs
{
    /// <summary>
    /// WPFBinding.xaml 的交互逻辑
    /// </summary>
    public partial class BindingPanel : UserControl
    {
        ObservableCollection<Block> blocks=new ObservableCollection<Block>();
        public BindingPanel()
        {
            InitializeComponent();
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            GetBlocks();
        }
        private void comboBoxBlocks_DropDownClosed(object sender, EventArgs e)
        {
            GetBlocks();
        }
        void GetBlocks()
        {
            Document doc=AcadApp.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //根据组合框中选中的项设置块名
            string blockName=this.comboBoxBlocks.SelectedIndex == 0 ? "Door" : "Window";
            using (DocumentLock loc=doc.LockDocument())
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //查找当前文档中块名以blockName开头的块参照
                var blocks=from block in db.GetEntsInModelSpace<BlockReference>()
                           //条件：块参照的块名以blockName开头
                           where block.GetBlockName().StartsWith(blockName)
                           //设置一个中间变量，取值为块参照的SYM.属性（即门或窗的符号）
                           let SYM = block.ObjectId.GetAttributeInBlockReference("SYM.")
                           //根据符号对符合条件的块参照进行分组
                           group block by SYM into g
                           orderby g.Key //根据符号对组进行升序排序
                           select new    //创建一个新的匿名类型作为结果
                           {
                               Symbol = g.Key,
                               Width = g.First().ObjectId.GetAttributeInBlockReference("WIDTH"),
                               Height = g.First().ObjectId.GetAttributeInBlockReference("HEIGHT"),
                               Count = g.Count()
                           };
                //设置listViewBlocks的数据源以显示块的分组统计信息
                this.listViewBlocks.ItemsSource = blocks.ToList();
                trans.Commit();
            }
        }
    }
}
