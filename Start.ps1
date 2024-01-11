# Replace with the path to your EXE
$exePathOnvifAPI = "C:\Ptz\OnvifAPI\OnvifAPI.exe"
$exePathONVIFPTZControl = "C:\Ptz\ONVIFPTZControl\ONVIFPTZControl.exe"


# Execute the EXE
Start-Process -FilePath $exePathOnvifAPI -Wait
Start-Process -FilePath $exePathONVIFPTZControl -Wait


# Replace with the directory path
$directoryPath = "C:\Users\User\build\build>"

# CMD command to run after changing directory
$cmdCommand = "serve -l 3000"

# Change the current directory
Set-Location -Path $directoryPath

# Execute the CMD command
Invoke-Expression "cmd.exe /c $cmdCommand"