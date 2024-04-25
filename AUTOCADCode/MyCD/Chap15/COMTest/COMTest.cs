using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using System.Reflection;
namespace COMTest
{
    public class COMTest
    {
        [CommandMethod("ComMethod1")]
        public void ComMethod1()
        {
            //获取Application的COM对象
            AcadApplication appCom= Application.AcadApplication as AcadApplication;
            //定义缩放窗口的两个点
            Point3d pt1=Point3d.Origin;
            Point3d pt2=new Point3d(100, 100, 0);
            //调用ZoomWindow函数缩放窗口
            appCom.ZoomWindow(pt1.ToArray(), pt2.ToArray());
        }
        [CommandMethod("ComMethod2")]
        public void ComMethod2()
        {
            //获取Application的COM对象
            Type comType = Type.GetTypeFromHandle(Type.GetTypeHandle(Application.AcadApplication));
            Point3d pt1=Point3d.Origin;
            Point3d pt2=new Point3d(100, 100, 0);
            //通过后期绑定的方式调用ZoomWindow函数缩放窗口
            comType.InvokeMember("ZoomWindow", BindingFlags.InvokeMethod,
                null, Application.AcadApplication, new object[] { pt1.ToArray(), pt2.ToArray() });
        }
        [CommandMethod("ComProp1")]
        public void ComProp1()
        {
            //获取Application的COM对象
            AcadApplication appCom= Application.AcadApplication as AcadApplication;
            //获取Preferences对象
            AcadPreferences Preferences=appCom.Preferences;
            //获取光标大小
            int cursorSize=Preferences.Display.CursorSize;
            Application.ShowAlertDialog("当前光标大小为:" + cursorSize);
            Preferences.Display.CursorSize = 10;//设置光标大小
        }
        [CommandMethod("ComProp2")]
        public void ComProp2()
        {
            //获取光标大小
            int cursorSize=(int)GetProperties("Display", "CursorSize");
            Application.ShowAlertDialog("当前光标大小为:" + cursorSize);
            SetProperties("Display", "CursorSize", 5);//设置光标大小
        }
        public object GetProperties(string projectName, string propertyName)
        {
            try
            {
                //获取Preferences对象(COM类）
                Type AcadPreferences = Type.GetTypeFromHandle(Type.GetTypeHandle(Application.Preferences));
                //通过后期绑定的方式调用Preferences对象的projectName属性
                object obj = AcadPreferences.InvokeMember(projectName, BindingFlags.GetProperty, null, Application.Preferences, null);
                //获取ProjectName属性对应的COM类
                Type AcadPreferencesUnknown = Type.GetTypeFromHandle(Type.GetTypeHandle(obj));
                //获取ProjectName属性对应的COM类的propertyName属性
                return (object)AcadPreferencesUnknown.InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
            }
            catch
            {
                return null;
            }
        }
        public bool SetProperties(string projectName, string propertyName, object Value)
        {
            try
            {
                //获取Preferences对象(COM类）
                Type AcadPreferences = Type.GetTypeFromHandle(Type.GetTypeHandle(Application.Preferences));
                //通过后期绑定的方式调用Preferences对象的projectName属性
                object obj = AcadPreferences.InvokeMember(projectName, BindingFlags.GetProperty, null, Application.Preferences, null);
                //获取ProjectName属性对应的COM类
                Type AcadPreferencesUnknown = Type.GetTypeFromHandle(Type.GetTypeHandle(obj));
                //设置projectName属性对应的COM类的propertyName属性
                AcadPreferencesUnknown.InvokeMember(propertyName, BindingFlags.SetProperty, null, obj, new object[1] { Value });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
