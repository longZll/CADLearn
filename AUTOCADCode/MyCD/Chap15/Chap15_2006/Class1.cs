using System.Runtime.InteropServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace Chap18_2006
{
    public class Class1
    {
        // arx文件必须放置在debug目录或AutoCAD搜索目录中
	// 将测试.dwg放置在C盘根目录下
        [DllImport("ahlzlArxMg.arx", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, EntryPoint = "InsertDwg")]
        public static extern int InsertDwg(string fileName, string blockName,
            Point3d insertPt, out ObjectId entId);

        [CommandMethod("test1")]
        public void MyTest1()
        {
            ObjectId noCensusRefId;
            InsertDwg("c:\\测试.dwg",           // 改成你的路径
                "我的图块",                     // 块名
                new Point3d(10.0, 8.0, 0.0),   // 插入点
                out noCensusRefId);            // 块参照Id  
        }
    }
}
