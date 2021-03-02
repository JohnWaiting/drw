::%SYSTEMROOT%\System32\inetsrv\appcmd stop apppool /apppool.name:"drw.srv"
dotnet publish --configuration Release
::%SYSTEMROOT%\System32\inetsrv\appcmd start apppool /apppool.name:"drw.srv"
::pause