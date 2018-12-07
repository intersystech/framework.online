@echo off

call xcopy *.dll C:\CloudTimeCard\Intersystech.CloudTimeCard.Online\CommonBinaries /F /C /R /Y
call xcopy *.xml C:\CloudTimeCard\Intersystech.CloudTimeCard.Online\CommonBinaries /F /C /R /Y
call xcopy *.xsd C:\CloudTimeCard\Intersystech.CloudTimeCard.Online\CommonBinaries /F /C /R /Y
pause

@echo on
