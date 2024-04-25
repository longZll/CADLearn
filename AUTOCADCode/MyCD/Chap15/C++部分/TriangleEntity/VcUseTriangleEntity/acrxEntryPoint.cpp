// (C) Copyright 2002-2005 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

//-----------------------------------------------------------------------------
//----- acrxEntryPoint.h
//-----------------------------------------------------------------------------
#include "StdAfx.h"
#include "resource.h"
#include "..\TriangleEntityDB\ZfgkTriangle.h"

//-----------------------------------------------------------------------------
#define szRDS _RXST("Zfgk")

//-----------------------------------------------------------------------------
//----- ObjectARX EntryPoint
class CVcUseTriangleEntityApp : public AcRxArxApp {

public:
	CVcUseTriangleEntityApp () : AcRxArxApp () {}

	virtual AcRx::AppRetCode On_kInitAppMsg (void *pkt) {
		// TODO: Load dependencies here

		// You *must* call On_kInitAppMsg here
		AcRx::AppRetCode retCode =AcRxArxApp::On_kInitAppMsg (pkt) ;
		
		// TODO: Add your initialization code here

		return (retCode) ;
	}

	virtual AcRx::AppRetCode On_kUnloadAppMsg (void *pkt) {
		// TODO: Add your code here

		// You *must* call On_kUnloadAppMsg here
		AcRx::AppRetCode retCode =AcRxArxApp::On_kUnloadAppMsg (pkt) ;

		// TODO: Unload dependencies here

		return (retCode) ;
	}

	virtual void RegisterServerComponents () {
	}

	// ��ʵ����ӵ�ģ�Ϳռ�
	static AcDbObjectId PostToModelSpace( AcDbEntity *pEnt, AcDbDatabase *pDb = acdbHostApplicationServices()->workingDatabase() )
	{
		if (pEnt == NULL)
		{
			return AcDbObjectId::kNull;
		}

		if (!pDb)
		{
			pDb = acdbHostApplicationServices()->workingDatabase();
		}

		AcDbBlockTable *pBlkTbl;
		AcDbBlockTableRecord *pBlkTblRcd;
		Acad::ErrorStatus es = pDb->getBlockTable(pBlkTbl, AcDb::kForRead);
		if (es != Acad::eOk)
		{
			return AcDbObjectId::kNull;
		}

		es = pBlkTbl->getAt(ACDB_MODEL_SPACE, pBlkTblRcd, AcDb::kForWrite);
		if (es != Acad::eOk)
		{
			pBlkTbl->close();
			return AcDbObjectId::kNull;
		}
		pBlkTbl->close();

		AcDbObjectId entId;
		es = pBlkTblRcd->appendAcDbEntity(entId, pEnt);
		if (es != Acad::eOk)
		{
			pBlkTblRcd->close();
			delete pEnt;	// ���ʧ��ʱ��Ҫdelete
			pEnt = NULL;

			return AcDbObjectId::kNull;
		}

		pBlkTblRcd->close();
		pEnt->close();

		return entId;
	}

	// - ZfgkVUTE.AddTriangle command (do not rename)
	static void ZfgkVUTEAddTriangle(void)
	{
		ZfgkTriangle *pTriangle = new ZfgkTriangle(AcGePoint3d(0, 0, 0), AcGePoint3d(100, 0, 0), AcGePoint3d(100, 100, 30));
		PostToModelSpace(pTriangle);
	}
} ;

//-----------------------------------------------------------------------------
IMPLEMENT_ARX_ENTRYPOINT(CVcUseTriangleEntityApp)

ACED_ARXCOMMAND_ENTRY_AUTO(CVcUseTriangleEntityApp, ZfgkVUTE, AddTriangle, AddTriangle, ACRX_CMD_MODAL, NULL)
