using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace FileOpener
{
    public class FileOpener
    {
        /// <summary>
        /// 已知DWG文件路径,打开DWG文件
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenDWGFile(string filePath)
        {
            DocumentCollection acDocMgr = Application.DocumentManager;
            Document acDoc = acDocMgr.Open(filePath, false);
            if (acDoc != null)
            {
                // 文件打开成功，向命令行输出提示消息
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n文件打开成功!");
            }
            else
            {
                // 文件打开失败，向命令行输出错误消息
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n文件打开失败!");
            }
        }



    }
}
