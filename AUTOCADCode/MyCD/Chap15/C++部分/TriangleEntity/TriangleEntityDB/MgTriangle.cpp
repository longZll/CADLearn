#include "StdAfx.h"
#include "MgTriangle.h"
#include "ZfgkTriangle.h"

namespace Zfgk
{
	namespace TriangleEntityDB
	{
		//////////////////////////////////////////////////////////////////////////
		// �й��๹�캯����������ʽֱ�Ӵ����ڲ���װ��VC++ʵ��
		MgTriangle::MgTriangle()
			:Autodesk::AutoCAD::DatabaseServices::Entity(new ZfgkTriangle(), true)
		{
			GetImpObj()->set_ManagedWrapper(this);
			//m_pMgTriangleModifiedHandler = NULL;
		}

		//////////////////////////////////////////////////////////////////////////
		// �й��๹�캯����������ʽ��һ���Ѿ����ڵķ��й�ʵ�帽�ŵ���ǰ�й�ʵ��
		MgTriangle::MgTriangle(System::IntPtr unmanagedPointer, bool autoDelete)
			: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
		{
			//m_pMgTriangleModifiedHandler = NULL;
		}

		//////////////////////////////////////////////////////////////////////////
		// �Զ���Ĺ��캯��
		MgTriangle::MgTriangle(Point3d pt1, Point3d pt2, Point3d pt3)
			:Autodesk::AutoCAD::DatabaseServices::Entity(new ZfgkTriangle(GETPOINT3D(pt1), GETPOINT3D(pt2), GETPOINT3D(pt3)), true)
		{
			GetImpObj()->set_ManagedWrapper(this);
			//m_pMgTriangleModifiedHandler = NULL;
		}

		void MgTriangle::GetVerts( [Runtime::InteropServices::Out] Autodesk::AutoCAD::Geometry::Point3dCollection*& verts )
		{
			verts = new Point3dCollection();
			AcGePoint3dArray verts3d;
			GetImpObj()->GetVerts(verts3d);
			for (int i = 0; i < verts3d.length(); i++)
			{
				verts->Add(ToPoint3d(verts3d[i]));
			}
		}

		void MgTriangle::SetVertAt( int index, Point3d point )
		{
			GetImpObj()->SetVertAt(index, GETPOINT3D(point));
		}

		double MgTriangle::get_Area()
		{
			return GetImpObj()->GetArea();
		}

		//////////////////////////////////////////////////////////////////////////
		// Method to add a listener to the SquareModified event
// 		void MgTriangle::add_MgTriangleModified ( MgTriangleModifiedHandler* pMgTriangleModified )
// 		{
// 			m_pMgTriangleModifiedHandler = static_cast< MgTriangleModifiedHandler* >
// 				(Delegate::Combine( m_pMgTriangleModifiedHandler, pMgTriangleModified ));
// 		}
// 		//////////////////////////////////////////////////////////////////////////
// 		// Method to remove a listener to the SquareModified event
// 		void MgTriangle::remove_MgTriangleModified ( MgTriangleModifiedHandler* pMgTriangleModified )
// 		{
// 			m_pMgTriangleModifiedHandler = static_cast< MgTriangleModifiedHandler* >
// 				(Delegate::Remove( m_pMgTriangleModifiedHandler, pMgTriangleModified ));
// 		}
// 		//////////////////////////////////////////////////////////////////////////
// 		// Method to send the SquareModified event to all attached listeners.
// 		void MgTriangle::raise_MgTriangleModified( Autodesk::AutoCAD::DatabaseServices::ObjectId pObjectId )
// 		{
// 			if( m_pMgTriangleModifiedHandler != NULL )
// 				m_pMgTriangleModifiedHandler->Invoke(pObjectId);
// 		}
	}
}
