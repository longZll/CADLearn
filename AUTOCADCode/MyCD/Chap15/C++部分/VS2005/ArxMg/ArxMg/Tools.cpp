#include "StdAfx.h"
#include "Tools.h"

namespace ahlzl
{
	Tools::Tools(void)
	{
	}

	// 创建直线
	ObjectId Tools::CreateLine(Point3d startPt, Point3d endPt)
	{
		AcGePoint3d ge_StartPt = GETPOINT3D(startPt);
		AcGePoint3d ge_EndPt = GETPOINT3D(endPt);
		AcDbLine* pLine = new AcDbLine(ge_StartPt, ge_EndPt);

		AcDbDatabase *pDb = acdbHostApplicationServices()->workingDatabase();

		AcDbBlockTable *pBt;
		pDb->getBlockTable(pBt, AcDb::kForRead);

		AcDbBlockTableRecord *pBtr;
		pBt->getAt(ACDB_MODEL_SPACE, pBtr, AcDb::kForWrite);

		AcDbObjectId entId;
		pBtr->appendAcDbEntity(entId, pLine);
		pBt->close();
		pBtr->close();
		pLine->close();

		return ToObjectId(entId);
	}
}