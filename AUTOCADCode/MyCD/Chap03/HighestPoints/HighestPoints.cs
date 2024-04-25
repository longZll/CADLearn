using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace HighestPoints
{
    public class HighestPoints
    {
        [CommandMethod("HighestPoints")]
        public void GetHighestPoints()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //获取模型空间中所有的点，用于LINQ的数据源
                var dbpoints = db.GetEntsInModelSpace<DBPoint>();
                //按Z值降序排列点，选择最大的100个点，并强制执行查询
                var highestPoints = (from p in dbpoints
                                     orderby p.Position.Z descending
                                     select p.Position).Take(100).ToList();
                //对LINQ查询的结果进行循环，依次在命令行输出最高的100个点的值
                for (int i = 0; i < highestPoints.Count; i++)
                {
                    ed.WriteMessage("\n {0} : {1} ", i, highestPoints[i]);
                }
                trans.Commit();
            }
        }
    }
}

