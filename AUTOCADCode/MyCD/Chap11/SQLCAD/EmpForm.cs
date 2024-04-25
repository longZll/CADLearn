using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLCAD
{
    public partial class EmpForm : Form
    {
        public EmpForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs ex)
        {
            using (var office=new officeDataContext())//连接SQLServer数据库
            {
                //生成一个新的员工对象
                var newEmp=new Employee
                {
                    Last_Name = this.textBoxLastName.Text,
                    First_Name = this.textBoxFirstName.Text,
                    Room_No = this.textBoxRoomNumber.Text
                };
                office.Employee.InsertOnSubmit(newEmp); //将新员工添加到Employee表中
                office.SubmitChanges();  //提交更改到数据库
            }
            this.Dispose();//关闭窗体，返回到AutoCAD中
        }
    }
}
