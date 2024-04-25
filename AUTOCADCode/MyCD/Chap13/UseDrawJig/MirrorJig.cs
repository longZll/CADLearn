using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace UseDrawJig
{
    class MirrorJig : DrawJig
    {
        private Point3d m_MirrorPt1, m_MirrorPt2;
        private Entity[] m_EntArr, m_EntCopyArr;
        private Matrix3d m_InverseMt = Matrix3d.Identity;

        // 初始化MirrorJig派生类
        public MirrorJig(Point3d mirrorPt1, Entity[] entArr, Entity[] entCopyArr)
        {
            m_MirrorPt2 = m_MirrorPt1 = mirrorPt1;
            m_EntArr = entArr;
            m_EntCopyArr = entCopyArr;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            for (int i = 0; i < m_EntCopyArr.Length; i++)
            {
                draw.Geometry.Draw(m_EntCopyArr[i]);
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Matrix3d mt = ed.CurrentUserCoordinateSystem;

            JigPromptPointOptions optJig = new JigPromptPointOptions("\n请指定镜像线第二点:");
            optJig.Cursor = CursorType.RubberBand;
            // 基点必须是WCS点. 
            Point3d wcsMirrorPt1 = m_MirrorPt1.TransformBy(mt);
            optJig.BasePoint = wcsMirrorPt1;
            optJig.UseBasePoint = true;

            PromptPointResult resJig = prompts.AcquirePoint(optJig);
            Point3d curPt = resJig.Value;

            if (m_MirrorPt2 != curPt)
            {
                m_MirrorPt2 = curPt;

                Matrix3d mirrorMt = Matrix3d.Mirroring(new Line3d(m_MirrorPt2, wcsMirrorPt1));
                for (int i = 0; i < m_EntCopyArr.Length; i++)
                {
                    m_EntCopyArr[i].TransformBy(m_InverseMt);
                    m_EntCopyArr[i].TransformBy(mirrorMt);
                }

                m_InverseMt = mirrorMt.Inverse();
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.NoChange;
            }
        }

        public void Unhighlight()
        {
            // 取消源对象的高亮状态.
            for (int i = 0; i < m_EntArr.Length; i++)
            {
                m_EntArr[i].Unhighlight();
            }
        }
    }
}
