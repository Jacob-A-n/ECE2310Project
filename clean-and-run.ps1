$proj = "c:\Users\aroho\Documents\ECE2310Project\ECE2310Project\ECE2310Project.csproj"
$exe  = "c:\Users\aroho\Documents\ECE2310Project\ECE2310Project\bin\Debug\ECE2310Project.exe"

Get-Process ECE2310Project -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1
dotnet build $proj
if ($LASTEXITCODE -eq 0) { Start-Process $exe }