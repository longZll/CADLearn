using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;

namespace CADDialog
{
    public class CADDialog
    {
        [CommandMethod("CADDialog")]
        public void ShowCADDialog()
        {
            //显示自定义的图层特性管理器模态对话框
            FormLayer form=new FormLayer();
            Application.ShowModalDialog(form);
        }
    }
}
