using System;
using System.Runtime.InteropServices;
using System.Security;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using System.Text;
namespace PInvokeTest
{
    [SuppressUnmanagedCodeSecurity]
    public class PInvokeTest
    {
        #region P/Invoke全局函数
        [DllImport("acad.exe", EntryPoint = "?acedGetUserFavoritesDir@@YAHPA_W@Z", CharSet = CharSet.Auto)]
        private static extern bool acedGetUserFavoritesDir([MarshalAs(UnmanagedType.LPWStr)] StringBuilder szFavoritesDir);
        public bool GetUserFavoritesDir(StringBuilder favoritesDir)
        {
            return acedGetUserFavoritesDir(favoritesDir);
        }
        #endregion

        #region P/Invoke对象函数
        [DllImport("acdb17.dll", CallingConvention = CallingConvention.ThisCall, CharSet = CharSet.Auto, EntryPoint = "?originPoint@AcDbHatch@@QBE?AVAcGePoint2d@@XZ")]
        private static extern Point2d originPoint(IntPtr hatch);

        //获取填充的原点
        public static Point2d GetOrigin(Hatch hatch)
        {
            return originPoint(hatch.UnmanagedObject);
        }

        // 设置一个指向setOriginPoint函数的委托，并指定委托的调用约定为调用对象函数
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate ErrorStatus setOriginPoint(IntPtr hatch, ref Point2d pt);
        public static void SetOrigin(Hatch hatch, Point2d pt)
        {
            // 32位和64位setOriginPoint函数的重整名称
            string func32 = "?setOriginPoint@AcDbHatch@@QAE?AW4ErrorStatus@Acad@@ABVAcGePoint2d@@@Z";
            string func64 = "?setOriginPoint@AcDbHatch@@QEAA?AW4ErrorStatus@Acad@@AEBVAcGePoint2d@@@Z";
            // 获取表示setOriginPoint函数的委托
            setOriginPoint delegateFunc = (setOriginPoint)PInvoke.GetDelegateForFunction(true, func32, func64, typeof(setOriginPoint));
            // 调用委托，执行setOriginPoint函数
            delegateFunc(hatch.UnmanagedObject, ref pt);
        }
        #endregion

        [CommandMethod("PInvoke1",CommandFlags.Defun)]
        public void PInvoke1()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            StringBuilder dir = new StringBuilder(256);
            if (GetUserFavoritesDir(dir) && dir.Length > 0)
                ed.WriteMessage("\nWindows收藏夹是" + dir);
        }

        [CommandMethod("PInvoke2")]
        public void PInvoke2()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // 提示用户选择一个填充对象
            PromptEntityOptions opt = new PromptEntityOptions("\n请选择一个填充");
            opt.SetRejectMessage("必须是填充对象");
            opt.AddAllowedClass(typeof(Hatch), false);
            PromptEntityResult result = ed.GetEntity(opt);
            // 如果用户未选择，则返回
            if (result.Status != PromptStatus.OK) return;
            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                ObjectId id = result.ObjectId;// 填充对象的Id
                // 打开填充对象
                Hatch hatch = (Hatch)id.GetObject(OpenMode.ForRead);
                Point2d origin = GetOrigin(hatch); // 获取填充的原点
                ed.WriteMessage(origin.ToString()); //在命令行上显示原点值 
                trans.Commit(); // 提交事务处理
            }
        }

        [CommandMethod("PInvoke3")]
        public void PInvoke3()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // 提示用户选择一个填充对象
            PromptEntityOptions opt = new PromptEntityOptions("\n请选择一个填充");
            opt.SetRejectMessage("必须是填充对象");
            opt.AddAllowedClass(typeof(Hatch), false);
            PromptEntityResult result = ed.GetEntity(opt);
            // 如果用户未选择，则返回
            if (result.Status != PromptStatus.OK) return;
            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                ObjectId id = result.ObjectId;// 填充对象的Id
                // 由于要修改填充对象的原点，因此将其打开为写的状态
                Hatch hatch = (Hatch)id.GetObject(OpenMode.ForWrite);
                SetOrigin(hatch, new Point2d(10, 10)); // 设置填充的原点
                trans.Commit(); // 提交事务处理
            }
        }
    }
}
