using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace UseEntityJig
{
    class CircleJig : EntityJig
    {
        private Point3d m_CenterPt;
        public double m_Radius = 100.0;

        // 派生类的构造函数.
        public CircleJig(Vector3d normal)
            : base(new Circle())
        {
            ((Circle)Entity).Center = m_CenterPt;
            ((Circle)Entity).Normal = normal;
            ((Circle)Entity).Radius = m_Radius;
        }

        protected override bool Update()
        {
            ((Circle)Entity).Center = m_CenterPt;
            ((Circle)Entity).Radius = m_Radius;
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            // 定义一个点拖动交互类.
            JigPromptPointOptions optJig = new JigPromptPointOptions
                ("\n请指定圆的圆心或用右键修改半径");
         
            optJig.Keywords.Add("100");
            optJig.Keywords.Add("200");
            optJig.Keywords.Add("300");
            optJig.UserInputControls = UserInputControls.Accept3dCoordinates;

            // 用AcquirePoint函数得到用户输入的点.
            PromptPointResult resJigDis = prompts.AcquirePoint(optJig);
            Point3d curPt = resJigDis.Value;

            if (resJigDis.Status == PromptStatus.Cancel)
            {
                return SamplerStatus.Cancel;
            }

            if (resJigDis.Status == PromptStatus.Keyword)
            {
                switch (resJigDis.StringResult)
                {
                    case "100":
                        m_Radius = 100;
                        return SamplerStatus.NoChange;
                    case "200":
                        m_Radius = 200;
                        return SamplerStatus.NoChange;
                    case "300":
                        m_Radius = 300;
                        return SamplerStatus.NoChange;
                }
            }

            if (m_CenterPt != curPt)
            {
                // 保存当前点.
                m_CenterPt = curPt;
                return SamplerStatus.OK;
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
    }
}

