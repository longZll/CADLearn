using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
namespace AutoNumBlock
{
    /// <summary>
    /// 用来管理自动编号的类
    /// </summary>
    public class NumManager
    {
        public int BaseNum { get; set; }//起始编号
        public int Increment { get; set; }//编号增量
        public int MaxNum { get; set; }//最大编号
        public bool IsRowDirection { get; set; }//是否按行进行编号
        public string Prefix { get; set; }//编号前缀
        public string Suffix { get; set; }//编号后缀
        public string BlockName { get; set; }//自动编号使用的块名
        public string AttName { get; set; }//自动编号使用的块属性
        public string LayerName { get; set; }//自动编号所在的层名
        //存储编号及其对应的块参照的Id的有序字典对象
        public SortedDictionary<int, ObjectId> Numbers { get; set; }
        public NumManager()
        {
            BaseNum = Increment = 1;//起始编号及增量设为1
            IsRowDirection = true;//按行进行编号
            //初始化有序字典对象
            Numbers = new SortedDictionary<int, ObjectId>();
            Database db=HostApplicationServices.WorkingDatabase;
            using (Transaction trans=db.TransactionManager.StartTransaction())
            {
                //将自动编号放置在当前图层上
                LayerTableRecord ltr=(LayerTableRecord)trans.GetObject(db.Clayer, OpenMode.ForRead);
                LayerName = ltr.Name;
                trans.Commit();
            }
        }
    }
}
