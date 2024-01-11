@echo off

REM Replace with the path to your EXE
start "" "C:\Ptz\OnvifAPI\OnvifAPI.exe"
start "" "C:\Ptz\ONVIFPTZControl\ONVIFPTZControl.exe"

REM Replace with the directory path
cd "C:\Users\User\build\build"

REM CMD command to run after changing directory
serve -l 3000