@echo off

call xcopy *.dll C:\Senu\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
call xcopy *.pdb C:\Senu\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
call xcopy *.xml C:\Senu\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
call xcopy *.xsd C:\Senu\Intersystech.SmartOrderPR.Online\CommonBinaries /F /C /R /Y
pause

@echo on
