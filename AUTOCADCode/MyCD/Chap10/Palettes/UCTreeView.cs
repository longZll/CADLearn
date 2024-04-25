using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace Palettes
{
    public partial class UCTreeView : UserControl
    {
        public UCTreeView()
        {
            InitializeComponent();
            this.treeViewEnts.ImageList = imageListNode;//设置TreeView使用的图像列表
        }
        private void treeViewEnts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //选中节点的根节点名称
            string rootName=findRoot(e.Node).Text;
            //如果根节点名称与活动文档的名称不相同，则需要切换活动文档
            if (rootName != AcadApp.DocumentManager.MdiActiveDocument.Name)
            {
                //查找文档名为选中节点名的文档
                var docs=from Document doc in AcadApp.DocumentManager
                         where doc.Name == rootName
                         select doc;
                if (docs.Count() == 1)//如果找到，则切换活动文档
                    AcadApp.DocumentManager.MdiActiveDocument = docs.First();
            }
            switch (e.Node.Text)//判断选中节点的标签文本
            {
                case "门":
                    GetBlocksFromDwg("Door");//统计Door块个数并填充DataGridView
                    break;
                case "窗":
                    GetBlocksFromDwg("Window");//统计Window块个数并填充DataGridView
                    break;
                default://其它的标签文本，DataGridView的内容设为空
                    this.dataGridViewEnts.DataSource = null;
                    break;
            }
        }
        private void GetBlocksFromDwg(string blockName)
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            using (DocumentLock loc=db.GetDocument().LockDocument())
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
                               符号 = g.Key,
                               宽度 = g.First().ObjectId.GetAttributeInBlockReference("WIDTH"),
                               高度 = g.First().ObjectId.GetAttributeInBlockReference("HEIGHT"),
                               个数 = g.Count()
                           };
                //设置DataGridView的数据源以显示块的分组统计信息
                this.dataGridViewEnts.DataSource = blocks.ToList();
                trans.Commit();
            }
        }
        TreeNode findRoot(TreeNode node)
        {
            if (node.Parent == null) return node;
            else return findRoot(node.Parent);
        }
    }
}
