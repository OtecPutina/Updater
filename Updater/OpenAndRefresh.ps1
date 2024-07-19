$folderPath = '\\veyseloglu.az'
$explorerProcess = Start-Process explorer.exe $folderPath -PassThru

Start-Sleep -Seconds 2

$shell = New-Object -ComObject Shell.Application
$folder = $shell.Namespace($folderPath)
$folder.Self.InvokeVerb('Refresh')

Start-Sleep -Seconds 2

$processes = Get-Process | Where-Object { $_.MainWindowTitle -like "*$folderPath*" }
foreach ($proc in $processes) {
    $proc | Stop-Process -Force
}
