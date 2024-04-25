;
; Creaed by AcadNetAddinWizard on 2012年6月1日星期五 at 00:02:57.
;
; It is used to open a test drawing, load the assembly, and run a test command if applicable.
; The variables are set automatically by AcadNetAddinWizard during project creation and can be changed later.
;
(setq aawDwgPath "")		; The drawing path. No tail slash, either forward or backward.
(setq aawDwgName "")	; The drawing name. No matter whether upper or lower.
(setq aawCmdToCall "OptCommand")		; The commond name to call right after the assembly is loaded and the drawing opened.
(setq aawNetToLoad "F:/CADLearn/AUTOCADCode/MyCD/Chap01/outPutPath/AcadNetAddinCS1.dll")		

; The assembly to load.

(Defun OpenDwg ( file / )
	(vl-load-com)
	(vla-activate (vla-open (vla-get-documents (vlax-get-acad-object)) file))
)
  
(Defun S::StartUp ( / dwgNameVar dwgFullName)
	(setq dwgNameVar (strcase (getvar "DwgName") 0) )
	(setq dwgFullName (strcat aawDwgPath '"/" aawDwgName) )
	(if ( /= dwgNameVar (strcase aawDwgName 0) )	; To avoid reentry!
		(if (FindFile dwgFullName)
			(OpenDwg dwgFullName)
			(progn
				(if (/= "" aawCmdToCall)
					(Command "NetLoad" aawNetToLoad aawCmdToCall)
					(Command "NetLoad" aawNetToLoad)
				)
			)					
		)
		(progn
			(if (/= "" aawCmdToCall)
				(Command "NetLoad" aawNetToLoad aawCmdToCall)
				(Command "NetLoad" aawNetToLoad)
			)
		)					
	)	
)
