using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using chap10;
using Autodesk.AutoCAD.Windows;


namespace XattachGEDI
{
    /// <summary>
    /// 添加外部参照GEDI类
    /// </summary>
    public class XattachGEDI
    {
        /// <summary>
        /// 添加外部参照GEDI
        /// </summary>
        [CommandMethod("xattachGEDI")]
        public void xattachGEDI()
        {
            ModalForm form=new ModalForm();
            Application.ShowModalDialog(form);
        }

    }
}
