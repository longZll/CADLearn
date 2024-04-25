using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace UCSTables
{
    public class UCSTables
    {
        [CommandMethod("NewUCS")]
        public void NewUCS()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=db.GetEditor();
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                ObjectId ucsId=db.AddUCS("NewUCS"); //添加名为NewUCS的UCS
                db.SetCurrentUCS("NewUCS");         //设置NewUCS为当前UCS
                trans.Commit();
            }
        }

        [CommandMethod("ModUCS")]
        public void ModUCS()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=db.GetEditor();
            //提示用户输入UCS的新原点
            PromptPointOptions opt=new PromptPointOptions("请指定UCS的新原点");
            PromptPointResult result=ed.GetPoint(opt);
            if (result.Status != PromptStatus.OK) return;//若用户未输入有效点，则返回

            Point3d newOrigin=result.Value;//获取用户输入的点
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                ObjectId ucsId=db.GetCurrentUCS();  //获取当前UCS
                ucsId.SetUCSOrigin(newOrigin);      //设置当前UCS的原点
                ucsId.RotateUCS(60, Vector3d.ZAxis);//将UCS绕Z轴旋转60度
                trans.Commit();
            }
        }

        [CommandMethod("UCSCir")]
        public void UCSCir()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=db.GetEditor();
            //创建一个圆心坐标（WCS)为(0,0,0)，半径为50的小圆
            Circle cir1=new Circle(Point3d.Origin, Vector3d.ZAxis, 50);
            //创建一个圆心坐标（WCS)为(0,0,0)，半径为80的大圆
            Circle cir2=new Circle(Point3d.Origin, Vector3d.ZAxis, 80);

            var mt=ed.CurrentUserCoordinateSystem;  //得到当前UCS矩阵
            cir2.TransformBy(mt);                   //对大圆实施矩阵变换

            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                db.AddToModelSpace(cir1, cir2);//将圆添加到模型空间
                trans.Commit();
            }
        }
    }
}
