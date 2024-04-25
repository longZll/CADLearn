using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace UseDrawJig
{
    class ElRecJig : DrawJig
    {
        public Ellipse m_Ellipse;
        private Polyline m_PolyLine;
        private Point3d m_Pt1, m_Pt2;

        // 派生类的构造函数.
        public ElRecJig(Point3d pt1, Ellipse ellipse, Polyline polyline)
        {
            m_Pt1 = pt1;
            m_Ellipse = ellipse;
            m_PolyLine = polyline;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            draw.Geometry.Draw(m_Ellipse);
            draw.Geometry.Draw(m_PolyLine);
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;

            JigPromptPointOptions optJigPoint = new JigPromptPointOptions
                ("\n请指定矩形的另一角点:");
            optJigPoint.UserInputControls =
                UserInputControls.Accept3dCoordinates
                | UserInputControls.NoZeroResponseAccepted
                | UserInputControls.NoNegativeResponseAccepted;
            optJigPoint.Cursor = CursorType.Crosshair;

            // 基点必须是WCS点. 
            optJigPoint.BasePoint = m_Pt1.TransformBy(mt);
            optJigPoint.UseBasePoint = true;

            PromptPointResult resJigPoint = prompts.AcquirePoint(optJigPoint);
            Point3d tempPt = resJigPoint.Value;

            if (resJigPoint.Status == PromptStatus.Cancel)
            {
                return SamplerStatus.Cancel;
            }

            if (m_Pt2 != tempPt)
            {
                m_Pt2 = tempPt;
                // 将WCS点转化为UCS点.
                Point3d ucsPt2 = m_Pt2.TransformBy(mt.Inverse());

                // 更新矩形参数.
                m_PolyLine.Normal = Vector3d.ZAxis;
                m_PolyLine.Elevation = 0;
                m_PolyLine.SetPointAt(0, new Point2d(m_Pt1.X, m_Pt1.Y));
                m_PolyLine.SetPointAt(1, new Point2d(ucsPt2.X, m_Pt1.Y));
                m_PolyLine.SetPointAt(2, new Point2d(ucsPt2.X, ucsPt2.Y));
                m_PolyLine.SetPointAt(3, new Point2d(m_Pt1.X, ucsPt2.Y));
                m_PolyLine.TransformBy(mt);

                // 更新椭圆参数.
                Point3d cenPt = MidPoint(m_Pt1.TransformBy(mt), m_Pt2);
                Point3d majorPt = new Point3d(ucsPt2.X, m_Pt1.Y, 0);
                Vector3d vecX = MidPoint(majorPt, ucsPt2).TransformBy(mt) - cenPt;

                try
                {
                    if (Math.Abs(ucsPt2.X - m_Pt1.X) < 0.0000001 |
                        Math.Abs(ucsPt2.Y - m_Pt1.Y) < 0.0000001)
                    {
                        m_Ellipse = new Ellipse(Point3d.Origin, Vector3d.ZAxis,
                            new Vector3d(0.00000001, 0, 0), 1, 0, 0);
                    }
                    else
                    {
                        double radiusRatio = Math.Abs((ucsPt2.X - m_Pt1.X) /
                            (ucsPt2.Y - m_Pt1.Y));

                        if (radiusRatio < 1)
                        {
                            majorPt = new Point3d(m_Pt1.X, ucsPt2.Y, 0);
                            vecX = MidPoint(majorPt, ucsPt2).TransformBy(mt) - cenPt;
                        }
                        else
                        {
                            radiusRatio = 1 / radiusRatio;
                        }
                        m_Ellipse.Set(cenPt, mt.CoordinateSystem3d.Zaxis, vecX,
                            radiusRatio, 0, 2 * Math.PI);
                    }
                }
                catch { }
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.NoChange;
            }
        }

        private Point3d MidPoint(Point3d pt1, Point3d pt2)
        {
            return new Point3d((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2,
                (pt1.Z + pt2.Z) / 2);
        }
    }
}



