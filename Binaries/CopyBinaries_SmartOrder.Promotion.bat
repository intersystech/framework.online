@echo off

call xcopy *.dll C:\SmartOrder\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
call xcopy *.pdb C:\SmartOrder\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
call xcopy *.xml C:\SmartOrder\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
call xcopy *.xsd C:\SmartOrder\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
pause

@echo on
