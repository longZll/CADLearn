using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
namespace Plots
{
    public class Plots
    {
        [CommandMethod("ShowPlotForm")]
        public void ShowPlotForm()
        {
            PlotForm plotForm = new PlotForm();//新建对话框
            Application.ShowModelessDialog(plotForm);//显示无模式对话框
        }
    }
}
