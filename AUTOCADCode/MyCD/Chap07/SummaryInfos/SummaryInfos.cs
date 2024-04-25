using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace SummaryInfos
{
    public class SummaryInfos
    {
[CommandMethod("AddSummaryInfo")]
public void AddSummaryInfo()
{
    Database db=HostApplicationServices.WorkingDatabase;
    //如果当前图形包含摘要信息，则返回
    if (db.HasSummaryInfo()) return;
    //新建一个摘要信息管理器
    var info=new DatabaseSummaryInfoBuilder(db.SummaryInfo);
    info.Author = "ObjectARX编程站";//作者
    info.Comments = "书配套Dwg文件";//注释
    info.HyperlinkBase = "http://www.objectarx.net";//超链接地址
    info.Keywords = "ObjectARX";//关键字
    info.LastSavedBy = "才鸟";//最近编辑者
    info.RevisionNumber = "V1.0";//修订次数
    info.Subject = "二次开发";//主题
    info.Title = "AutoCAD .NET开发基础与实例教程";//标题
    info.CustomProperties.Add("Size", "A4");//自定义属性
    info.CustomProperties.Add("Language", "Chinese");//自定义属性            
    db.SummaryInfo=info.ToDatabaseSummaryInfo();//保存摘要信息            
}
[CommandMethod("EditSummaryInfo")]
public void EditSummaryInfo()
{
    Database db=HostApplicationServices.WorkingDatabase;
    string propKey="Size";//自定义属性名
    //如果不存在名为Size的自定义属性，则返回
    if(!db.HasCustomProperty(propKey)) return;
    //新建一个摘要信息管理器
    var info=new DatabaseSummaryInfoBuilder(db.SummaryInfo);
    info.CustomProperties["Size"] = "A3";//自定义属性值
    db.SummaryInfo = info.ToDatabaseSummaryInfo();//保存摘要信息
}
[CommandMethod("ShowTime")]
public void ShowTime()
{
    Database db=HostApplicationServices.WorkingDatabase;
    Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
    //在命令行输出文件的各种时间
    ed.WriteMessage("文件创建时间为："+db.CreationTime()+"\n");
    ed.WriteMessage("文件修改时间为：" + db.ModifyTime() + "\n");
    ed.WriteMessage("文件总编辑时间为：" + Math.Floor(db.TotalEditTime().TotalMinutes) + "分钟\n");
}
    }
}
