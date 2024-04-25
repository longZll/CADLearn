using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;

namespace UseEntityJig
{
    class EllipseJig : EntityJig
    {
        // 声明全局变量.
        private Point3d m_CenterPt, m_MajorPt;
        private Vector3d m_Normal, m_MajorAxis;
        private int m_PromptCounter;
        private double m_OtherAxisLength, m_RadiusRatio;
        private double m_StartAng, m_EndAng, m_ang1, m_ang2;

        // 派生类的构造函数.
        public EllipseJig(Point3d center, Vector3d vec)
            : base(new Ellipse())
        {
            m_CenterPt = center;
            m_Normal = vec;
        }

        protected override bool Update()
        {
            if (m_PromptCounter == 0)
            {
                // 第一次拖拽时，椭圆的半径比为1，屏幕上显示的是一个圆.
                m_RadiusRatio = 1;
                m_MajorAxis = m_MajorPt - m_CenterPt;
                m_StartAng = 0;
                m_EndAng = 2 * Math.PI;
            }
            else if (m_PromptCounter == 1)
            {
                // 第二次拖拽时，修改了椭圆的半径比，屏幕上显示的是一个完整椭圆.
                m_RadiusRatio = m_OtherAxisLength / m_MajorAxis.Length;
            }
            else if (m_PromptCounter == 2)
            {
                // 第三次拖拽时，修改了椭圆的起初角度，屏幕上显示的是一个终止角度为360度的椭圆弧.
                m_StartAng = m_ang1;
            }
            else if (m_PromptCounter == 3)
            {
                // 第四次拖拽时，修改了椭圆的终止角度，屏幕上显示的是一个最终的椭圆弧.
                m_EndAng = m_ang2;
            }

            try
            {
                if (m_RadiusRatio < 1)
                    // 更新椭圆的参数.
                    ((Ellipse)(Entity)).Set(m_CenterPt, m_Normal, m_MajorAxis, m_RadiusRatio, m_StartAng, m_EndAng);
                else
                {
                    // 如另一条半轴长度超过椭圆弧长轴方向矢量的长度，则要重新定义椭圆弧长轴方向矢量的方向和长度.
                    Vector3d mMajorAxis2 = m_MajorAxis.RotateBy(0.5 * Math.PI, Vector3d.ZAxis).DivideBy(1 / m_RadiusRatio);
                    // 更新椭圆的参数.
                    ((Ellipse)(Entity)).Set(m_CenterPt, m_Normal, mMajorAxis2, 1 / m_RadiusRatio, m_StartAng, m_EndAng);
                }
            }
            catch
            {
                // 此处不需要处理.
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            if (m_PromptCounter == 0)
            {
                // 定义一个点拖动交互类.
                JigPromptPointOptions optJigPoint = new JigPromptPointOptions("\n请指定椭圆弧轴上一点");
                // 设置拖拽的光标类型.
                optJigPoint.Cursor = CursorType.RubberBand;
                // 设置拖动光标基点.
                optJigPoint.BasePoint = m_CenterPt;
                optJigPoint.UseBasePoint = true;
                // 用AcquirePoint函数得到用户输入的点.
                PromptPointResult resJigPoint = prompts.AcquirePoint(optJigPoint);
                Point3d curPt = resJigPoint.Value;

                if (curPt != m_MajorPt)
                {
                    // 保存当前点.
                    m_MajorPt = curPt;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }

                if (resJigPoint.Status == PromptStatus.Cancel)
                {
                    return SamplerStatus.Cancel;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            else if (m_PromptCounter == 1)
            {
                // 定义一个距离拖动交互类.
                JigPromptDistanceOptions optJigDis = new JigPromptDistanceOptions
                    ("\n请指定另一条半轴的长度");
                // 设置对拖拽的约束:禁止输入零和负值.
                optJigDis.UserInputControls = UserInputControls.NoZeroResponseAccepted |
                    UserInputControls.NoNegativeResponseAccepted;
                // 设置拖拽的光标类型.
                optJigDis.Cursor = CursorType.RubberBand;
                // 设置拖动光标基点.
                optJigDis.BasePoint = m_CenterPt;
                optJigDis.UseBasePoint = true;

                // 用AcquireDistance函数得到用户输入的距离值.
                PromptDoubleResult resJigDis = prompts.AcquireDistance(optJigDis);
                double radiusRatioTemp = resJigDis.Value;

                if (radiusRatioTemp != m_OtherAxisLength)
                {
                    // 保存当前距离值.
                    m_OtherAxisLength = radiusRatioTemp;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }

                if (resJigDis.Status == PromptStatus.Cancel)
                {
                    return SamplerStatus.Cancel;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            else if (m_PromptCounter == 2)
            {
                // 设置椭圆弧0度基准角.
                double baseAng;
                Vector2d mMajorAxis2d = new Vector2d(m_MajorAxis.X, m_MajorAxis.Y);
                if (m_RadiusRatio < 1)
                {
                    baseAng = mMajorAxis2d.Angle;
                }
                else
                {
                    baseAng = mMajorAxis2d.Angle + 0.5 * Math.PI;
                }

                // 设置系统变量“ANGBASE”.
                Application.SetSystemVariable("ANGBASE", baseAng);
                // 定义一个角度拖动交互类.
                JigPromptAngleOptions optJigAngle1 = new JigPromptAngleOptions("\n请指定椭圆弧的起始角度");
                // 设置拖拽的光标类型.
                optJigAngle1.Cursor = CursorType.RubberBand;
                // 设置拖动光标基点.
                optJigAngle1.BasePoint = m_CenterPt;
                optJigAngle1.UseBasePoint = true;

                // 用AcquireAngle函数得到用户输入的角度值.
                PromptDoubleResult resJigAngle1 = prompts.AcquireAngle(optJigAngle1);
                m_ang1 = resJigAngle1.Value;

                if (m_StartAng != m_ang1)
                {
                    // 保存当前角度值.
                    m_StartAng = m_ang1;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }

                if (resJigAngle1.Status == PromptStatus.Cancel)
                {
                    return SamplerStatus.Cancel;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            else if (m_PromptCounter == 3)
            {
                // 定义一个角度拖动交互类.
                JigPromptAngleOptions optJigAngle2 = new JigPromptAngleOptions("\n请指定椭圆弧的终止角度");
                // 设置拖拽的光标类型.
                optJigAngle2.Cursor = CursorType.RubberBand;
                // 设置拖动光标基点.
                optJigAngle2.BasePoint = m_CenterPt;
                optJigAngle2.UseBasePoint = true;

                // 用AcquireAngle函数得到用户输入的角度值.
                PromptDoubleResult resJigAngle2 = prompts.AcquireAngle(optJigAngle2);
                m_ang2 = resJigAngle2.Value;

                if (m_EndAng != m_ang2)
                {
                    // 保存当前角度值.
                    m_EndAng = m_ang2;
                }
                else
                {
                    return SamplerStatus.NoChange;
                }

                if (resJigAngle2.Status == PromptStatus.Cancel)
                {
                    return SamplerStatus.Cancel;
                }
                else
                {
                    return SamplerStatus.OK;
                }
            }
            else
            {
                return SamplerStatus.NoChange;
            }
        }

        // GetEntity函数用于得到派生类的实体.
        public Entity GetEntity()
        {
            return Entity;
        }

        // setPromptCounter过程用于控制不同的拖拽.
        public void setPromptCounter(int i)
        {
            m_PromptCounter = i;
        }
    }
}
