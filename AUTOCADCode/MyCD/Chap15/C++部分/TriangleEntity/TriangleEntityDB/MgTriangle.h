#pragma once

using namespace System;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::DatabaseServices;

class ZfgkTriangle;

namespace Zfgk
{
	namespace TriangleEntityDB
	{
		// Declare a delegate which represents the event handler for our SquareModified event
		//public __delegate void MgTriangleModifiedHandler(ObjectId pObjectId);

		// 定义一个从Entity继承的托管类，其中封装了ZfgkTriangle类的多个属性
		[Autodesk::AutoCAD::Runtime::Wrapper("ZfgkTriangle")]
		public __gc class MgTriangle :  public Autodesk::AutoCAD::DatabaseServices::Entity
		{
		public:
			MgTriangle(void);
			MgTriangle(Point3d pt1, Point3d pt2, Point3d pt3);

		public private:
			MgTriangle(System::IntPtr unmanagedPointer, bool autoDelete);

			// 获得非托管实体指针
			inline ZfgkTriangle* GetImpObj()
			{
				// UnmanagedObject()是一个AutoCAD托管包装方法，它提供了一种直接访问VC++中ARX实体的方法
				return static_cast<ZfgkTriangle*>(UnmanagedObject.ToPointer());
			}

		public:
			// 获得所有顶点
			void GetVerts([Runtime::InteropServices::Out] Autodesk::AutoCAD::Geometry::Point3dCollection*& verts);

			// 设置某个顶点的位置
			void SetVertAt(int index, Point3d point);

			// 三角形面积
			__property double get_Area();


			// Here are the methods defied to support exposure of our event.
// 			__event void add_MgTriangleModified // Called to add listeners
// 				( MgTriangleModifiedHandler* pMgTriangleModified );
// 			__event void remove_MgTriangleModified // Called to remove listeners.
// 				( MgTriangleModifiedHandler* pMgTriangleModified );			
// 
// 		private public:
// 			// Called to send the TriangleModified event to all attached managed listeners.
// 			__event void raise_MgTriangleModified( Autodesk::AutoCAD::DatabaseServices::ObjectId pObjectId );			
// 
// 		private:
// 			// Here is our member delegate which represents the event for this object.
// 			MgTriangleModifiedHandler* m_pMgTriangleModifiedHandler;
		};
	}
}
