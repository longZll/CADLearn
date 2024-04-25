#include "StdAfx.h"
#include "MgTriangle.h"
#include "ZfgkTriangle.h"

namespace Zfgk
{
	namespace TriangleEntityDB
	{
		//////////////////////////////////////////////////////////////////////////
		// 托管类构造函数，这种形式直接创建内部封装的VC++实体
		MgTriangle::MgTriangle()
			:Autodesk::AutoCAD::DatabaseServices::Entity(new ZfgkTriangle(), true)
		{
			GetImpObj()->set_ManagedWrapper(this);
			//m_pMgTriangleModifiedHandler = NULL;
		}

		//////////////////////////////////////////////////////////////////////////
		// 托管类构造函数，这种形式将一个已经存在的非托管实体附着到当前托管实体
		MgTriangle::MgTriangle(System::IntPtr unmanagedPointer, bool autoDelete)
			: Autodesk::AutoCAD::DatabaseServices::Entity(unmanagedPointer, autoDelete)
		{
			//m_pMgTriangleModifiedHandler = NULL;
		}

		//////////////////////////////////////////////////////////////////////////
		// 自定义的构造函数
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
