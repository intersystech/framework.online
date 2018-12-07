@echo off

call xcopy *.dll C:\Senu\Intersystech.SmartOrder.SignalRServer\CommonBinaries /F /C /R /Y
call xcopy *.pdb C:\Senu\Intersystech.SmartOrder.SignalRServer\CommonBinaries /F /C /R /Y
call xcopy *.xml C:\Senu\Intersystech.SmartOrder.SignalRServer\CommonBinaries /F /C /R /Y
call xcopy *.xsd C:\Senu\Intersystech.SmartOrder.SignalRServer\CommonBinaries /F /C /R /Y
pause

@echo on
