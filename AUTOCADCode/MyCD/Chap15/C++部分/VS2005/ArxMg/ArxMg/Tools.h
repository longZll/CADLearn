#pragma once

using namespace System;
using namespace Autodesk::AutoCAD::DatabaseServices;
using namespace Autodesk::AutoCAD::Geometry;
using namespace Autodesk::AutoCAD::Runtime;

namespace ahlzl
{
	public ref class Tools
	{
	public:
		Tools(void);
		// ����ֱ��
		static ObjectId CreateLine(Point3d startPt, Point3d endPt); 
	};
}

