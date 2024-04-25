using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Commands
{
    public class Commands
    {
        [CommandMethod("NetCommand")]
        public void NetCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //提示用户选择圆的圆心
            PromptPointResult result = ed.GetPoint("\n请选择圆心");
            if (result.Status != PromptStatus.OK) return; // 如果选择失败，返回
            Point3d pt = result.Value; // 获得用户选择的圆心            
            // 命令字符串，画一个半径为10的圆
            string cmd = string.Format("Circle {0},{1},{2} 10 ", pt.X, pt.Y, pt.Z);
            cmd += "Zoom E "; // 命令字符串，缩放视图以使圆可见
            // 获取NOMUTT系统变量，该变量用来控制是否禁止显示提示信息
            object nomutt = Application.GetSystemVariable("NOMUTT");
            // 命令字符串，表示在命令调用结束时，将NOMUTT设置为原始值
            cmd += "_NOMUTT " + nomutt.ToString() + " ";
            // 在命令调用之前，设置NOMUTT为1，表示禁止显示提示信息
            Application.SetSystemVariable("NOMUTT", 1);

            // 向AutoCAD发出cmd字符串所表示的命令
            doc.SendStringToExecute(cmd, true, false, false);
            // 显示对话框提示
            Application.ShowAlertDialog("创建圆成功！");
        }

        [CommandMethod("ComCommand",CommandFlags.Session)]
        public void ComCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // 获取FILEDIA系统变量，该变量用来表示是否显示文件导航对话框
            object filedia = Application.GetSystemVariable("FILEDIA");
            // 设置FILEDIA=0，表示不显示文件导航对话框
            Application.SetSystemVariable("FILEDIA", 0);

            string filename = "C:\\test.dwg"; // 要保存的文件名
            // 如果在C盘根目录下存在test.dwg，则删除
            if (File.Exists(filename)) File.Delete(filename);
            // 命令字符串，用来调用AutoCAD的另存为命令
            string cmd = "SaveAs\n\n" + filename + "\n";

            doc.SendCommand(cmd); // 调用COM的SendCommand函数，执行另存为命令
            // 恢复FILEDIA系统变量为原始值
            Application.SetSystemVariable("FILEDIA", filedia);
        }


        [CommandMethod("ARXCommand")]
        public void ARXCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // 提示用户选择要打断的对象兼作第一点
            PromptEntityResult entRes = ed.GetEntity("\n请选择要打断的对象");

            if (entRes.Status != PromptStatus.OK) return; // 若未选择，则返回
            
            // 选择打断的第二点
            PromptPointResult ptRes = ed.GetPoint("\n选择点");
            if (ptRes.Status != PromptStatus.OK) return; // 若未选择，则返回
            
            // ResultBuffer对象，用来存储命令、对象、点
            ResultBuffer rb = new ResultBuffer();
            rb.Add(new TypedValue(5005, "_Break"));         // 字符串，表示命令
            rb.Add(new TypedValue(5006, entRes.ObjectId));  // 实体的Id
            rb.Add(new TypedValue(5009, ptRes.Value));      // 打断的第二点

            ed.AcedCmd(rb); //执行AutoCAD的打断命令
            // 显示提示信息
            Application.ShowAlertDialog("打断对象成功！");
        }
    }
}

