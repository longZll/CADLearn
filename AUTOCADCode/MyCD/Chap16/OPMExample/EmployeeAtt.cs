using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Masterhe.OPM;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.DatabaseServices;
using DotNetARX;
using Autodesk.AutoCAD.ApplicationServices;
namespace OPMExample
{
    public class EmployeeAtt : PropertyBase
    {
        //设置属性个数，不重载就是一个
        public override int PropertyCount
        {
            get { return 2; }
        }
        public override PropertyTypeEnum GetPropertyType(int nIndex)
        {
            //确定每个属性的类型
            switch (nIndex)
            {
                //员工类型（以下拉列表表示）
                case 0: return PropertyTypeEnum.PT_DropDownList;
                default: return PropertyTypeEnum.PT_Normal;//年薪
            }
        }
        //将属性定义为一个新的组
        public override string GetCategoryName(int nIndex)
        {
            return "员工类别";
        }
        //设置年薪属性为只读属性
        public override bool IsPropertyReadOnly(int index)
        {
            return index == 1;
        }
        #region 必须重载的方法
        //获取当前的属性值
        public override object GetCurrentValueData(int nIndex, DBObject pObj)
        {
            //获取当前选择对象的扩展数据（"EMPLOYEE"）
            TypedValueList xdata=pObj.GetXDataForApplication(Commands.appName);
            //若没有对应的扩展数据，则返回
            if (xdata == null) return null;
            switch (nIndex)
            {
                case 0: return xdata[1].Value;//员工的职位
                default: return xdata[2].Value;//员工的年薪
            }
        }
        //获取当前的属性类别
        public override VarEnum GetCurrentValueType(int nIndex)
        {
            switch (nIndex)
            {
                case 0: return VarEnum.VT_BSTR;//员工职位的类别：string
                default: return VarEnum.VT_R8;  //员工年薪的类别：double
            }
        }
        //获取当前的属性描述
        public override string GetDescription(int nIndex)
        {
            switch (nIndex)
            {
                case 0: return "指定员工的职位";
                default: return "指定员工的年薪";
            }
        }
        //获取属性的显示名称
        public override string GetDisplayName(int nIndex)
        {
            switch (nIndex)
            {
                case 0: return "职位";
                default: return "年薪";
            }
        }
        //设置当前的属性值
        public override void SetCurrentValueData(int nIndex, DBObject pObj, object objData)
        {
            ObjectId id=pObj.ObjectId;
            //获取当前选择对象的扩展数据（"EMPLOYEE"）
            TypedValueList xdata=pObj.GetXDataForApplication(Commands.appName);
            //若没有对应的扩展数据，则返回
            if (xdata == null) return;
            switch (nIndex)
            {
                case 0:
                    string empType=objData.ToString();//员工类型
                    //设置员工的类型和对应的年薪
                    xdata[1] = new TypedValue((int)DxfCode.ExtendedDataAsciiString, empType);
                    xdata[2] = new TypedValue((int)DxfCode.ExtendedDataReal, Commands.employees[empType]);
                    break;
            }
            //修改扩展数据以对应属性值的更改
            using (Transaction trans=id.Database.TransactionManager.StartTransaction())
            {
                id.RemoveXData(Commands.appName);
                id.AddXData(Commands.appName, xdata);
                trans.Commit();
            }
        }
        #endregion
        #region 为PropertyTypeEnum.PT_DropDownList重载的方法
        //员工类型列表
        List<string> listEmpTypes = null;
        //获取下拉列表形式的属性可以取值的个数
        public override int GetNumPropertyValues(int nIndex)
        {
            if (listEmpTypes == null)
            {
                listEmpTypes = new List<string>();
                listEmpTypes.AddRange(new string[] { "普通员工", "管理人员", "董事长" });
            }
            return listEmpTypes.Count;
        }
        //获取与下拉列表索引对应的属性名
        public override string GetPropValueName(int nIndex, int lValueIndex)
        {
            return listEmpTypes[lValueIndex].ToString();
        }
        //获取与下拉列表索引对应的属性值
        public override object GetPropValueData(int nIndex, int lValueIndex)
        {
            return listEmpTypes[lValueIndex];
        }
        #endregion
    }
}
