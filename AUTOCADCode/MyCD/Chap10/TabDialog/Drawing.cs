using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TabDialog
{
    public enum CityName
    {
        上海,
        北京,
        广州,
        其它
    }
    public class Drawing
    {
        [Category("基本信息"), Description("设置作者姓名"), DisplayName("姓名")]
        public string Name { get; set; }
        [Category("基本信息"), Description("设置作者年龄"), DisplayName("年龄")]
        public int Age { get; set; }
        [Category("基本信息"), Description("设置作者居住的城市"), DisplayName("居住城市")]
        public CityName City { get; set; }
        [Category("AutoCAD相关"), Description("设置AutoCAD的背景颜色"), DisplayName("背景颜色")]
        public System.Drawing.Color BackgroundColor { get; set; }
    }
}
