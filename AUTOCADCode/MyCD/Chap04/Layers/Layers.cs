using System.Linq;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using DotNetARX;
namespace Layers
{
    public class Layers
    {
        /// <summary>
        /// 新建图层,并且设置新添加的图层为当前层
        /// </summary>
        [CommandMethod("CreateLayer")]
        public void CreateLayer()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            Editor ed=Application.DocumentManager.MdiActiveDocument.Editor;
            string layerName="";    //用来存储图层名
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                while (layerName == "")//如果用户没有输入图层名，则循环
                {
                    //提示用户输入图层名
                    PromptResult pr=ed.GetString("请输入图层名称");
                    if (pr.Status != PromptStatus.OK) return;
                    try
                    {
                        //校验---验证输入字符串是否符合符号表命名规则
                        SymbolUtilityServices.ValidateSymbolName(pr.StringResult, false);
                        layerName = pr.StringResult;//图层名
                        //添加名为layerName的图层
                        if (db.AddLayer(layerName) != ObjectId.Null)
                        {
                            ///提示用户输入图层颜色
                            PromptIntegerResult pir=ed.GetInteger("请输入图层的颜色值");
                            if (pir.Status != PromptStatus.OK) return;
                            //设置图层的颜色
                            db.SetLayerColor(layerName, (short)pir.Value);
                            //设置新添加的图层为当前层
                            db.SetCurrentLayer(layerName);
                            break;//添加图层成功，跳出循环
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        //捕捉到异常,说明传入图层名不合法
                        ed.WriteMessage(ex.Message + "\n" + "输入的图层名称不合法(不能包含< > /等字符'),请重新输入");
                    }
                }

                trans.Commit();
            }
        }

        /// <summary>
        /// 删除当前图形中所有颜色为红色的图层
        /// </summary>
        [CommandMethod("DelRedLayer")]
        public void DelRedLayer()
        {
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //获取当前图形中所有颜色为红色的图层层名
                List<string> redLayers=(from layer in db.GetAllLayers()
                               where layer.Color == Color.FromColorIndex(ColorMethod.ByAci, 1)
                               select layer.Name).ToList();
                //删除红色的图层
                redLayers.ForEach(layer => db.DeleteLayer(layer));
                trans.Commit();
            }
        }
    }
}
