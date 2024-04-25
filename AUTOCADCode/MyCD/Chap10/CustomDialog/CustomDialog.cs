using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using chap10;
using Autodesk.AutoCAD.Windows;


namespace CustomDialog
{
    public class CustomDialog
    {
        [CommandMethod("ModalDialog")]
        public void ModalDialog()
        {
            ModalForm form=new ModalForm();
            Application.ShowModalDialog(form);
        }
    }
}
