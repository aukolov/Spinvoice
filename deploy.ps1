$git = $env:ProgramFiles + "\Git\bin\git.exe"
$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
$publish = "Spinvoice\publish\"

& $git checkout master
& $git pull
& $git merge dev

$new_csproj = Get-Content Spinvoice.csproj |
 foreach { $n = [regex]::match($_,'(?<=\<ApplicationRevision\>)(\d+)(?=\<\/ApplicationRevision\>)').groups[1].value;
 if ($n) {$_ -replace "$n", ([int32]$n+1)} else {$_}; }
Set-Content Spinvoice.csproj $new_csproj -encoding UTF8

If (Test-Path $publish) { Remove-Item $publish -Force -Recurse }
& $msbuild Spinvoice.sln /t:publish /p:Configuration=Release /p:PublishDir=publish\

& $git add .
& $git commit -m "New release"
& $git push
& $git checkout dev
& $git merge master
& $git push