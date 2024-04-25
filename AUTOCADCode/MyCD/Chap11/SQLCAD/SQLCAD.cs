using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace SQLCAD
{
    public class SQLCAD
    {
        //提取CAD中的员工名字AutoCAD->数据库
        [CommandMethod("Employee")]
        public void Employee()
        {
            Database db=HostApplicationServices.WorkingDatabase;            
            using (officeDataContext office=new officeDataContext())// 连接数据库
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //使用Linq获取模型空间中位于EMPLOYEE图层上的多行文本对象
                var employeesInCAD=from m in db.GetEntsInModelSpace<MText>()
                                   where m.Layer == "EMPLOYEE"
                                   select m;
                List<Employee> employees=new List<Employee>();//员工列表
                foreach (MText employee in employeesInCAD)//遍历表示员工的多行文本
                {
                    string[] name=employee.Text.Split('\n');//分离出员工的姓和名
                    //设置一个员工对象，包括姓、名、房间号
                    Employee newEmployee=new Employee { Last_Name = name[0], First_Name = name[1], Room_No = GetNearstRoom(employee) };
                    //添加提取的员工信息到员工列表
                    employees.Add(newEmployee);
                }
                //一次性添加AutoCAD提取的员工信息到Employee表中
                office.Employee.InsertAllOnSubmit(employees);
                office.SubmitChanges();//对数据库提交更改，完成数据的添加
                trans.Commit();
            }
        }

        // 获取离指定员工（由多行文本表示）最近的房间号（RMNUM块的NUMBER属性）
        public string GetNearstRoom(MText mtext)
        {
            Database db=HostApplicationServices.WorkingDatabase;
            // 获取表示房间的RMNUM块
            var roomBlocks=from b in db.GetEntsInModelSpace<BlockReference>()
                           where b.Name == "RMNUM"
                           select b;
            // 获取表示员工的多行文本的左下角点
            Point3d pos=mtext.GeometricExtents.MinPoint;
            // 获取离多行文本最近的RMNUM块
            var nearsetLeftTop=from r in roomBlocks
                               let blockTop = r.GeometricExtents.MinPoint
                               let left = blockTop.X - pos.X
                               let top = blockTop.Y - pos.Y
                               where left < 0 && top > 0
                               select r;
            var q=(from n in nearsetLeftTop
                   let dist = n.GeometricExtents.MinPoint.DistanceTo(pos)
                   where dist > 0
                   orderby dist
                   select n).First();
            //返回员工所在的房间号（离多行文本最近的RMNUM块的NUMBER属性）
            return q.ObjectId.GetAttributeInBlockReference("NUMBER");
        }

        //提取CAD中的房间号码、类型、句柄,AutoCAD->数据库
        [CommandMethod("Room")]
        public void Room()
        {
            Database db=HostApplicationServices.WorkingDatabase;            
            using (Transaction trans=db.TransactionManager.StartTransaction())
            using (officeDataContext office=new officeDataContext())
            {
                //使用Linq获取模型空间中的RMNUM块
                var roomBlocks=from b in db.GetEntsInModelSpace<BlockReference>()
                               where b.Name == "RMNUM"
                               select b;
                //用于存储NUMBER属性为整数（代表房间号）的RMNUM块
                List<Room> roomsHasNum=new List<Room>();
                //若房间上方有房间类型，则存储该房间号及房间类型
                var roomsNumAndType=new List<KeyValuePair<string, string>>();
                foreach (var room in roomBlocks) //循环遍历表示房间的RMNUM块
                {
                    //获取表示房间号的块属性NUMBER
                    string roomNumber=room.ObjectId.GetAttributeInBlockReference("NUMBER");
                    string roomType="Office"; //设置默认房间类型为Office                   
                    if (!roomNumber.IsInt()) //如果NUMBER属性不是整数，则块表示它下方的房间类型
                    {
                        var q=(from rb in roomBlocks
                               let dist = rb.Position.DistanceTo(room.Position) //当前RMNUM块与其它RMNUM块的距离
                               where dist > 0
                               orderby dist //按距离排序
                               select rb).First(); //选择距离最近的RMNUM块
                        //获取最近的有房间号的房间
                        var roomNumberNearst=q.ObjectId.GetAttributeInBlockReference("NUMBER");
                        var roomAndType=new KeyValuePair<string, string>(roomNumberNearst, roomNumber);//设置房间号及房间类型
                        roomsNumAndType.Add(roomAndType);//加入列表，供后面操作
                        continue; //由于RMNUM块为非数字的，只是提供房间类型，因此返回
                    }
                    //如果NUMBER属性是整数，则生成一个新房间，对房间号、实体句柄、房间类型进行赋值
                    var newRoom=new Room { Room_No = roomNumber, Entity_Handle = room.Handle.ToString(), Room_Type = roomType };
                    roomsHasNum.Add(newRoom); //加入房间列表
                }
                foreach (var roomOther in roomsNumAndType)//对具有特殊类型的房间进行遍历
                {
                    var q=from r in roomsHasNum
                          where r.Room_No == roomOther.Key
                          select r; //选择具有特殊类型的房间
                    if (q.Count() == 1)//
                    {
                        q.First().Room_Type = roomOther.Value;//设置房间类型
                    }
                }
                office.Room.InsertAllOnSubmit(roomsHasNum);//一次性添加AutoCAD提取的房间信息到Room表中
                office.SubmitChanges(); //对数据库提交更改，完成数据的添加
                trans.Commit();
            }
        }

        //增加一个新员工,并在AutoCAD中表示
        [CommandMethod("NewEmp")]
        public void AddNewEmployee()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Application.ShowModalDialog(new EmpForm());// 显示员工情况输入窗口
            using (Transaction trans=db.TransactionManager.StartTransaction())
            using (var office=new officeDataContext())// 连接SQL Server数据库
            {
                // 查找数据库中最后一个员工
                var lastEmp=office.Employee.OrderBy(e => e.Id).ToList().Last();
                ObjectId roomId=ObjectId.Null;// 表示房间的RUMNUM块的Id
                //新建一个多行文本对象表示员工，文本内容设置为查找到的员工的姓名
                MText employee=new MText();
                employee.Contents = lastEmp.First_Name + "\n" + lastEmp.Last_Name;
                //查找到的员工所在的房间（Room表）
                var room=(from r in office.Room
                          where r.Room_No == lastEmp.Room_No
                          select r).ToList();
                //如果找到员工所在的房间号，则将其句柄转化为ObjectId
                if (room.Count() == 1)
                    roomId = db.HandleToObjectId(room.First().Entity_Handle);
                else return;//未找到房间号则返回
                //获取房间所代表的块
                BlockReference roomBlock=(BlockReference)roomId.GetObject(OpenMode.ForRead);
                //员工文本放置在房间块的右下方
                employee.Location = roomBlock.GeometricExtents.MaxPoint.Add(new Vector3d(3, -3, 0));
                //设置员工文本的层号
                employee.Layer = "Employee";
                db.AddToModelSpace(employee);
                trans.Commit();
            }
        }
    }
}
