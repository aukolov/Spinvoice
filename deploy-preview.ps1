$git = $env:ProgramFiles + "\Git\bin\git.exe"
$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
$publish = "Spinvoice.Preview\publish\"

$new_csproj = Get-Content .\Spinvoice.Preview\Spinvoice.Preview.csproj |
 foreach { $n = [regex]::match($_,'(?<=\<ApplicationRevision\>)(\d+)(?=\<\/ApplicationRevision\>)').groups[1].value;
 if ($n) {$_ -replace "$n", ([int32]$n+1)} else {$_}; }
Set-Content .\Spinvoice.Preview\Spinvoice.Preview.csproj $new_csproj -encoding UTF8

If (Test-Path $publish) { Remove-Item $publish -Force -Recurse }
& $msbuild Spinvoice.sln /t:publish /p:Configuration=Release /p:PublishDir=publish\

