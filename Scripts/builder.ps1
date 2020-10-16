$svc = New-WebServiceProxy –Uri "http://localhost/taskmanager/BuildService.asmx?WSDL"
#$svc = New-WebServiceProxy –Uri "http://localhost:8311/BuildService.asmx?WSDL"
$svc.scheduledBuild()