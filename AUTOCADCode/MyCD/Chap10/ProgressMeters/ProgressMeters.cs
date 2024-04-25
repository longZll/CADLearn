using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace ProgessMeters
{
    public class ProgessMeters
    {
[CommandMethod("ProgressUtils")]
public void ProgressUtils()
{
    Database db=HostApplicationServices.WorkingDatabase;
            
    Editor ed=AcadApp.DocumentManager.MdiActiveDocument.Editor;
    //创建消息过滤类
    MessageFilter filter=new MessageFilter();
    //为程序添加消息过滤
    System.Windows.Forms.Application.AddMessageFilter(filter);
    bool esc=false;//标识是否按下了Esc键
    using (Transaction trans=db.TransactionManager.StartTransaction())
    {
        // 获取模型空间中所有的点对象
        var points=db.GetEntsInModelSpace<DBPoint>();
        //进度条开始工作并设置进度条前要显示的提示文字
        Utils.SetApplicationStatusBarProgressMeter("更改点的颜色", 0, points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            //设置进度
            Utils.SetApplicationStatusBarProgressMeter(i);
            points[i].UpgradeOpen();//切换点对象的状态为写
            points[i].ColorIndex = 2;//设置点对象的颜色为黄色            
            //如果用户按下了Esc键，则结束遍历
            if (filter.KeyName == Keys.Escape)
            {
                esc = true;
                break;
            }
            //让AutoCAD在长时间任务处理时仍然能接收消息
            System.Windows.Forms.Application.DoEvents();
        }
        Utils.RestoreApplicationStatusBar();//恢复状态栏
        if (esc) trans.Abort();//如果按下了Esc键，则放弃所有的更改
        else trans.Commit(); //否则程序能完成所有点的变色工作，提交事务处理
    }
    //移除对按键消息的过滤
    System.Windows.Forms.Application.RemoveMessageFilter(filter);
}
[CommandMethod("Progress")]
public void Progress()
{
    Database db=HostApplicationServices.WorkingDatabase;
    Editor ed=AcadApp.DocumentManager.MdiActiveDocument.Editor;
    //设置进度条对象
    ProgressMeter pm=new ProgressMeter();
    //创建消息过滤类
    MessageFilter filter=new MessageFilter();
    //为程序添加消息过滤
    System.Windows.Forms.Application.AddMessageFilter(filter);
    bool esc=false;//标识是否按下了Esc键
    using (Transaction trans=db.TransactionManager.StartTransaction())
    {
        // 获取模型空间中所有的点对象
        var points=db.GetEntsInModelSpace<DBPoint>();
        //进度条开始工作并设置进度条前要显示的提示文字
        pm.Start("更改点的颜色");
        //设置进度条需要更新的次数
        pm.SetLimit(points.Count);
        foreach (var point in points)//遍历点
        {
            pm.MeterProgress();//更新进度条
            point.UpgradeOpen();//切换点对象的状态为写
            point.ColorIndex = 1;//设置点对象的颜色为红色
            //如果用户按下了Esc键，则结束遍历
            if (filter.KeyName == Keys.Escape)
            {
                esc = true;
                break;
            }
            //让AutoCAD在长时间任务处理时仍然能接收消息
            System.Windows.Forms.Application.DoEvents();
        }
        pm.Stop();//停止进度条的更新
        if (esc) trans.Abort();//如果按下了Esc键，则放弃所有的更改
        else trans.Commit(); //否则程序能完成所有点的变色工作，提交事务处理
    }
    //移除对按键消息的过滤
    System.Windows.Forms.Application.RemoveMessageFilter(filter);
}
[CommandMethod("ProgressManager")]
public void TestProgressManager()
{
    Database db=HostApplicationServices.WorkingDatabase;
    Editor ed=AcadApp.DocumentManager.MdiActiveDocument.Editor;
    //创建消息过滤类
    MessageFilter filter=new MessageFilter();
    //为程序添加消息过滤
    System.Windows.Forms.Application.AddMessageFilter(filter);
    bool esc=false;//标识是否按下了Esc键
    //开始事务处理并新建一个进度条管理类
    using (Transaction trans=db.TransactionManager.StartTransaction())
    using (ProgressManager manager=new ProgressManager("更改点的颜色"))
    {
        // 获取模型空间中所有的点对象
        var points=db.GetEntsInModelSpace<DBPoint>();
        //设置进度条需要更新的次数，一般为循环次数
        manager.SetTotalOperations(points.Count);
        foreach (var point in points)//遍历点
        {
            manager.Tick();//进度条更新进度
            point.UpgradeOpen();//切换点对象的状态为写
            point.ColorIndex = 3;//设置点对象的颜色为绿色
            //如果用户按下了Esc键，则结束遍历
            if (filter.KeyName == Keys.Escape)
            {
                esc = true;
                break;
            }
        }
        if (esc) trans.Abort();//如果按下了Esc键，则放弃所有的更改
        else trans.Commit(); //否则程序能完成所有点的变色工作，提交事务处理
    }
    //移除对按键消息的过滤
    System.Windows.Forms.Application.RemoveMessageFilter(filter);
}
    }
}
