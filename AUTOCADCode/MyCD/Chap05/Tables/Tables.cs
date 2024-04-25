using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Tables
{
    public class Tables
    {
        [CommandMethod("Table2008")]
        public static void AddTable2008()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;

            //对模型空间中的块参照进行统计
            var blocks=(from b in db.GetEntsInModelSpace<BlockReference>()
                        group b by b.Name into g            //按块名进行分组
                        let id = g.First().BlockTableRecord //获取块参照所在的块表记录
                        orderby g.Count() descending        //按块参照的个数降序排列
                        //返回分组结果，生成一个拥有块名、个数、块表记录Id 的匿名类
                        select new { Name = g.Key, Number = g.Count(), Id = id }).ToList();
            
            //提示用户输入表格插入点
            PromptPointResult ppr=ed.GetPoint("\n请选择表格插入点:");
            if (ppr.Status != PromptStatus.OK) return;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //添加一个名为ColorTable的表格样式
                ObjectId styleId=AddTableStyle("ColorTable");
                
                //新建一个表格
                Table table=new Table();
                table.TableStyle = styleId;//表格样式
                table.Position = ppr.Value;//表格位置
                table.SetSize(blocks.Count + 2, 4);//设置表格的行数和列数
                table.SetRowHeight(3); //表格行高为3
                table.SetColumnWidth(15);//表格列宽为15
                table.SetTextString(0, 0, "块统计情况表2008");//表格标题
                //表格表头
                table.SetTextString(1, 0, "序号");
                table.SetTextString(1, 1, "名字");
                table.SetTextString(1, 2, "数量");

                //Table.Cells[1, 2].TextString("数量");

                table.SetTextString(1, 3, "缩略图");
                int i=2;

                //将块参照的统计结果放入到表格中
                foreach (var block in blocks)
                {
                    table.SetTextString(i, 0, (i - 1).ToString());
                    table.SetTextString(i, 1, block.Name);
                    table.SetTextString(i, 2, block.Number.ToString());
                    
                    //设置块预览图
                    table.SetBlockTableRecordId(i, 3, block.Id, true);
                    i++;
                }
                db.AddToModelSpace(table);//将表格添加到模型空间
                trans.Commit();
            }
        }
        [CommandMethod("Table2012")]
        public static void AddTable2012()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;

            //PromptPointResult ppr=ed.GetPoint("\n请选择表格插入点:");
            //if (ppr.Status != PromptStatus.OK) return;

            var blocks = (from b in db.GetEntsInModelSpace<BlockReference>()
                        group b by b.Name into g
                        let id = g.First().BlockTableRecord
                        orderby g.Count() descending
                        select new { Name = g.Key, Number = g.Count(), Id = id }).ToList();
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                ObjectId styleId=AddTableStyle("ColorTable");
                Table table=new Table();
                table.TableStyle = styleId;

                //table.Position = ppr.Value;

                table.Position = new Autodesk.AutoCAD.Geometry.Point3d(0,0,0);

                //table.Position = Point3d().Origin;


                table.SetSize(blocks.Count + 2, 4);
                table.SetRowHeight(3);
                table.SetColumnWidth(15);

                table.Cells[0, 0].TextString = "块统计情况表2012";
                table.Cells[1, 0].TextString = "序号";
                table.Cells[1, 1].TextString = "名字";
                table.Cells[1, 2].TextString = "数量";
                table.Cells[1, 3].TextString = "缩略图";
                int i=2;

                foreach (var block in blocks)
                {
                    //设置单元格的文本内容
                    table.Cells[i, 0].TextString = (i - 1).ToString();
                    table.Cells[i, 1].TextString = block.Name;
                    table.Cells[i, 2].TextString = block.Number.ToString();
                    table.Cells[i, 3].Contents.Add(); //添加空白内容以添加下面的块参照
                    table.Cells[i, 3].Contents[0].BlockTableRecordId = block.Id;
                    i++;
                }
                db.AddToModelSpace(table);
                trans.Commit();
            }
        }

        //为当前图形添加一个新的表格样式
        public static ObjectId AddTableStyle(string style)
        {
            ObjectId styleId; // 存储表格样式的Id
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                // 打开表格样式字典
                DBDictionary dict=(DBDictionary)db.TableStyleDictionaryId.GetObject(OpenMode.ForRead);
                
                if (dict.Contains(style)) // 如果存在指定的表格样式
                    styleId = dict.GetAt(style); // 获取表格样式的Id
                else
                {
                    TableStyle ts=new TableStyle(); // 新建一个表格样式
                    // 设置表格的标题行为灰色
                    ts.SetBackgroundColor(Color.FromColorIndex(ColorMethod.ByAci, 8), (int)RowType.TitleRow);
                    // 设置表格所有行的外边框的线宽为0.30mm
                    ts.SetGridLineWeight(LineWeight.LineWeight030, (int)GridLineType.OuterGridLines, TableTools.AllRows);
                    // 不加粗表格表头行的底部边框
                    ts.SetGridLineWeight(LineWeight.LineWeight000, (int)GridLineType.HorizontalBottom, (int)RowType.HeaderRow);
                    // 不加粗表格数据行的顶部边框
                    ts.SetGridLineWeight(LineWeight.LineWeight000, (int)GridLineType.HorizontalTop, (int)RowType.DataRow);
                    // 设置表格中所有行的文本高度为1
                    ts.SetTextHeight(1, TableTools.AllRows);
                    // 设置表格中所有行的对齐方式为正中
                    ts.SetAlignment(CellAlignment.MiddleCenter, TableTools.AllRows);
                    dict.UpgradeOpen();//切换表格样式字典为写的状态
                    
                    // 将新的表格样式添加到样式字典并获取其Id
                    styleId = dict.SetAt(style, ts);
                    // 将新建的表格样式添加到事务处理中
                    trans.AddNewlyCreatedDBObject(ts, true);
                    trans.Commit();
                }
            }
            return styleId; // 返回表格样式的Id
        }
    }
}
