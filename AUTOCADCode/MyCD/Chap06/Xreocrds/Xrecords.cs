using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using DotNetARX;
using Autodesk.AutoCAD.Geometry;

namespace Xrecords
{
    public class Xrecords
    {
        /// <summary>
        /// 添加扩展记录
        /// </summary>
        [CommandMethod("AddXrec")]
        public void AddXrec()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                ed.WriteMessage("\n请选择表示房间、椅子的块及员工姓名文本");
                ed.WriteMessage("\n1111111111");
                ed.WriteMessage("\n2222222222");

                List<Entity> ents=db.GetSelection();

                //LINQ过滤只块
                var blocks=from ent in ents
                           where ent is BlockReference
                           select ent;
                
                //过滤房间块
                var room=(from BlockReference b in blocks
                          where b.Name == "RMNUM"
                          select b).FirstOrDefault();
                
                //过滤椅子块并返回椅子数
                int chairs=(from BlockReference b in blocks
                            where b.Name == "CHAIR7"
                            select b).Count();
                
                //过滤表示员工姓名的多行文本
                var txt=(from ent in ents
                         where ent is MText
                         select ent).FirstOrDefault();
                //必须同时选中房间和椅子块
                if (room == null && txt == null) return;

                //获取表示房间号的块属性
                string roomNum=room.ObjectId.GetAttributeInBlockReference("NUMBER");

                //设置员工类型：房间中椅子数大于1的为管理人员，否则为普通员工
                string employeeType = chairs > 1 ? "管理人员" : "普通员工";

                //为所选文本添加员工类型和房间号的扩展记录
                TypedValueList values=new TypedValueList();
                values.Add(DxfCode.Text, employeeType);
                values.Add(DxfCode.Text, roomNum);
                txt.ObjectId.AddXrecord("员工", values);
                trans.Commit();
            }
        }

        /// <summary>
        /// 添加 有名对象字典项
        /// </summary>
        [CommandMethod("AddDict")]
        public static void AddDict()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //为图形添加命名对象字典：管理人员
                ObjectId id=db.AddNamedDictionary("管理人员");
                
                if (id.IsNull)//如果没有创建成功，则返回
                {
                    Application.ShowAlertDialog("已经有管理人员记录了");
                    return;
                }
                //向管理人员字典添加一条表示年薪的扩展记录
                TypedValueList values=new TypedValueList();
                values.Add(DxfCode.Real, 500000.0);
                id.AddXrecord("年薪", values);
                
                //为图形添加命名对象字典：普通员工员
                id = db.AddNamedDictionary("普通员工");
                if (id.IsNull)//如果没有创建成功，则返回
                {
                    Application.ShowAlertDialog("已经有普通员工记录了");
                    return;
                }

                //向普通员工字典添加一条表示年薪的扩展记录
                values.Clear();
                values.Add(DxfCode.Real, 100000.0);
                id.AddXrecord("年薪", values);

                trans.Commit();
            }
        }

        /// <summary>
        /// 显示上面代码添加的扩展记录以及有名对象字典项
        /// </summary>
        [CommandMethod("ListXrec")]
        public void ListXrec()
        {
            Document doc=Application.DocumentManager.MdiActiveDocument;
            Database db=doc.Database;
            Editor ed=doc.Editor;
            //提示用户选择放置表格的位置
            PromptPointResult pointResult = ed.GetPoint("\n请选择表放置的位置: ");
            if (pointResult.Status != PromptStatus.OK) return;

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //获取有名对象字典
                DBDictionary dicts=(DBDictionary)trans.GetObject(db.NamedObjectsDictionaryId, OpenMode.ForRead);
                
                //获取模型空间中所有的多行文本
                var mtexts=(from m in db.GetEntsInModelSpace<MText>()
                            let xrecord = m.ObjectId.GetXrecord("员工")   //获取名为员工的扩展记录
                            where xrecord != null               //只选择有员工扩展记录的文本
                            let Position = xrecord[0].Value.ToString()  //获取表示员工职位的扩展记录项
                            let RoomNumber = xrecord[1].Value.ToString()//获取表示员工房间的扩展记录项
                            let Name = m.Text.Replace("\r\n", " ")  //对文本表示的人名去除回车换行符
                            //通过表示职位的有名对象字典获取职位对应的年薪
                            let Salary = string.Format("{0:C0}", dicts.GetAt(Position).GetXrecord("年薪").First().Value)
                            orderby RoomNumber  //按房间号排序
                            
                            //生成一个包含房间号、职位、姓名和年薪的匿名类，并转化为列表
                            select new { RoomNumber, Position, Name, Salary }).ToList();
                //添加表格表头
                mtexts.Insert(0, new { RoomNumber = "员工明细表", Position = "", Name = "", Salary = "" });
                //添加表格标题
                mtexts.Insert(1, new { RoomNumber = "房间号", Position = "职位", Name = "姓名", Salary = "年薪" });
                
                Table tb = new Table();//新建表格对象
                tb.TableStyle = db.Tablestyle;  //设置表格的样式为数据库缺省样式
                
                //获取表格的行数和列数
                tb.NumRows = mtexts.Count();
                tb.NumColumns = 4;
                tb.SetRowHeight(3);         //行高
                tb.SetColumnWidth(15);      //列宽
                tb.SetTextHeight(1);        //文本高度
                //设置数据单元格的对奇方式为居中
                tb.SetAlignment(CellAlignment.MiddleCenter, RowType.DataRow);
                //将表格放置到用户选择的点
                tb.Position = pointResult.Value;
                
                //遍历多行文本集合，按行设置表格的内容
                for (int i = 0; i < mtexts.Count; i++)
                {
                    var mtext=mtexts[i];
                    tb.SetRowTextString(i, mtext.RoomNumber, mtext.Name, mtext.Position, mtext.Salary);
                }
                tb.GenerateLayout();    //根据当前表格样式更新表格
                db.AddToModelSpace(tb); //添加表格到模型空间
                trans.Commit();
            }
        }
    }
}
