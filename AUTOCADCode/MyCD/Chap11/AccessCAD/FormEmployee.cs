using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AccessCAD
{
    public partial class FormEmployee : Form
    {
        //表示部门颜色的序列
        internal Color[] colors = new Color[] { Color.Red, Color.Yellow, Color.Green, Color.Cyan, Color.Blue, Color.Magenta, Color.Orange, Color.Brown, Color.Gray };
        public FormEmployee()
        {
            InitializeComponent();

        }

        private void FormEmployee_Load(object sender, EventArgs e)
        {
            //新建一个指向db_sample.mdb数据库的数据集
            using (db_samplesDataSet ds=new db_samplesDataSet())
            {
                // 创建一个用于Employee表的DataAdapter，用于检索数据和填充Employee表
                var empAdapter=new db_samplesDataSetTableAdapters.EmployeeTableAdapter();
                //填充数据集中的Employee表
                empAdapter.Fill(ds.Employee);
                //按照部门对员工进行分组，生成一个部门名和人数的新序列，并按人数降序排列
                var emps=(from emp in ds.Employee
                          group emp by emp.Department into g
                          select new { Department = g.Key, Count = g.Count() }).OrderByDescending(p => p.Count).ToList();
                //遍历部门
                for (int i = 0; i < emps.Count(); i++)
                {

                    //加入部门人数到饼图中
                    var point = this.chartEmployee.Series[0].Points.Add(emps[i].Count);
                    //设置图例文字
                    point.LegendText = emps[i].Department;
                    point.Color = colors[i]; //设置序列的颜色
                }

                //绑定到Datagrid
                this.dataGridViewEmployee.DataSource = emps;
                this.dataGridViewEmployee.Columns[0].HeaderText = "部门";
                this.dataGridViewEmployee.Columns[1].HeaderText = "员工数";
            }
        }
    }
}
