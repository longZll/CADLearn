using ahlzl;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace WrapperTest
{
    public class WrapperTest
    {
        [CommandMethod("DrawLine")]
        public void DrawLineTest()
        {
            ObjectId entId = Tools.CreateLine
                (new Point3d(0.0, 0.0, 0.0), new Point3d(8.0, 3.0, 0.0));
        }
    }
}
