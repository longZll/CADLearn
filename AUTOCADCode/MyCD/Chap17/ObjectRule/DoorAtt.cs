using System.Collections.Generic;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.DatabaseServices;
using DotNetARX;
using Masterhe.OPM;
namespace ObjectRule
{
    public class DoorAtt : PropertyBase
    {
        SortedDictionary<string,double> doors=new SortedDictionary<string, double>();
        public override int PropertyCount
        {
            get
            {
                return 2;
            }
        }
        public override PropertyTypeEnum GetPropertyType(int nIndex)
        {
            //确定每个属性的类型
            switch (nIndex)
            {
                case 0:
                    return PropertyTypeEnum.PT_DropDownList;
                default:
                    return PropertyTypeEnum.PT_Normal;
            }
        }
        public override string GetCategoryName(int nIndex)
        {
            return "门属性";
        }
        public override bool IsPropertyReadOnly(int index)
        {
            return index == 1;
        }
        #region 必须重载的方法
        public override object GetCurrentValueData(int nIndex, DBObject pObj)
        {
            ObjectId id=pObj.ObjectId;
            TypedValueList xdata=pObj.GetXDataForApplication(Commands.appName);
            if (xdata == null) return null;
            switch (nIndex)
            {
                case 0:
                    return xdata[1].Value;
                default:
                    return xdata[2].Value;
            }
            
        }

        public override VarEnum GetCurrentValueType(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    return VarEnum.VT_BSTR;
                default:
                    return VarEnum.VT_R8;
            }
        }

        public override string GetDescription(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    return "指定门的类型";
                default:
                    return "指定门的价格";
            }
        }

        public override string GetDisplayName(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    return "类型";
                default:
                    return "价格";
            }
        }

        public override void SetCurrentValueData(int nIndex, DBObject pObj, object objData)
        {
            ObjectId id=pObj.ObjectId;
            TypedValueList xdata=pObj.GetXDataForApplication(Commands.appName);
            if (xdata == null) return;
            switch (nIndex)
            {
                case 0:
                    string doorType=objData.ToString();
                    xdata[1] = new TypedValue((int)DxfCode.ExtendedDataAsciiString, doorType);
                    xdata[2] = new TypedValue((int)DxfCode.ExtendedDataReal,Commands.doors[doorType]);
                    break;
            }
            using (Transaction trans=id.Database.TransactionManager.StartTransaction())
            {
                id.RemoveXData(Commands.appName);
                id.AddXData(Commands.appName, xdata);
                trans.Commit();
            }            
        }
        #endregion
        #region 为PropertyTypeEnum.PT_DropDownList重载的方法
        List<string> listDoorTypes = null;
        public override int GetNumPropertyValues(int nIndex)
        {
            if (listDoorTypes == null)
            {
                listDoorTypes = new List<string>();
                listDoorTypes.AddRange(new string[] { "木门","铁门","铝合金门"});
            }
            return listDoorTypes.Count;
        }
        public override string GetPropValueName(int nIndex, int lValueIndex)
        {
            return listDoorTypes[lValueIndex].ToString();
        }
        public override object GetPropValueData(int nIndex, int lValueIndex)
        {
            return listDoorTypes[lValueIndex];
        }
        #endregion
    }
}
