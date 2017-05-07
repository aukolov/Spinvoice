$git = $env:ProgramFiles + "\Git\bin\git.exe"
$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
$publish = "Spinvoice\publish\"

& $git checkout master
& $git pull
& $git merge dev

If (Test-Path $publish) { Remove-Item $publish -Force -Recurse }
& $msbuild Spinvoice.sln /t:publish /p:Configuration=Release /p:PublishDir=publish\

#& $git checkout dev