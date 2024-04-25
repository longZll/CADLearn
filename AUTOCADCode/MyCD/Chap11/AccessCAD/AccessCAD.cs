using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace AccessCAD
{
    public class AccessCAD
    {
        //各部门的人员用不同颜色表示
        [CommandMethod("ColorDepartment")]
        public void ColorDepartment()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //新建一个指向db_sample.mdb数据库的数据集
            using (db_samplesDataSet ds=new db_samplesDataSet())
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 新建一个窗体，以添加表示员工部门的颜色图例
                FormEmployee empForm = new FormEmployee();
                Application.ShowModelessDialog(empForm);// 显示窗体
                // 创建一个用于Employee表的DataAdapter，用于检索数据和填充Employee表
                var empAdapter=new db_samplesDataSetTableAdapters.EmployeeTableAdapter();
                empAdapter.Fill(ds.Employee);// 填充数据集中的Employee表
                // 从数据库中选择Employee表，建立员工姓名和部门(Department)的新序列
                var employeesInDB = from e in ds.Employee
                                    select new { Name = e.First_Name + "\r\n" + e.Last_Name, Department = e.Department };
                // 获取CAD图形中表示员工姓名的多行文本
                var employeesInCAD = db.GetEntsInModelSpace<MText>().Where(e => e.Layer == "EMPLOYEE");
                // 将CAD中的员工（实体）与数据库中的员工（部门）连接成一个大表
                var employees = from emp1 in employeesInCAD
                                join emp2 in employeesInDB on emp1.GetText() equals emp2.Name
                                select new { Entity = emp1, Department = emp2.Department };
                //对员工按照部门(Department)进行分组
                var employeesToGroup = (from emp in employees
                                        group emp by emp.Department into g
                                        select g).ToList();
                // 对部门进行遍历
                for (int i = 0; i < employeesToGroup.Count(); i++)
                {
                    //从员工组成窗体的图表中选择部门的颜色
                    var colorWindows = (from p in empForm.chartEmployee.Series[0].Points
                                        where p.LegendText == employeesToGroup[i].Key
                                        select p.Color).First();
                    //将Windows颜色值转化为AutoCAD颜色值
                    var colorCAD = Autodesk.AutoCAD.Colors.Color.FromColor(colorWindows);
                    //对部门中的员工进行遍历
                    foreach (var employee in employeesToGroup[i])
                    {
                        employee.Entity.UpgradeOpen(); //打开表示员工的实体为写的状态
                        employee.Entity.Color = colorCAD; //设置员工实体的颜色为对应部门的颜色
                    }
                }
                trans.Commit();
            }
        }


        [CommandMethod("ComputerNoRoom")]
        public void ComputerNoRoom()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //新建一个指向db_sample.mdb数据库的数据集
            using (var ds=new db_samplesDataSet())
            {
                // 创建一个用于Computer表的DataAdapter，用于检索数据和填充Computer表
                var computerAdapter=new db_samplesDataSetTableAdapters.ComputerTableAdapter();
                // 创建一个用于Room表的DataAdapter，用于检索数据和填充Room表
                var roomAdapter=new db_samplesDataSetTableAdapters.RoomTableAdapter();
                //填充数据集中的Computer表
                computerAdapter.Fill(ds.Computer);
                //填充数据集中的Room表
                roomAdapter.Fill(ds.Room);
                //查找Room表中没有的房间号
                var computerNoRoom=(from c in ds.Computer
                                    select c.Room_No).Distinct().Except(
                        from r in ds.Room
                        select r.Room_No);
                foreach (var roomNo in computerNoRoom)
                {
                    var newRoom=ds.Room.NewRoomRow();  //新建一个Room行
                    //房间号设置为Computer表中的房间号
                    newRoom.Room_No = roomNo;
                    ds.Room.AddRoomRow(newRoom); //添加新行到Room表中
                    //在AutoCAD命令行输出添加新行的提示信息
                    ed.WriteMessage("Computer表中的" + roomNo + "被添加到Room表。\n");
                }
                roomAdapter.Update(ds.Room); //提交更新到数据库                
            }
        }
    }
}
