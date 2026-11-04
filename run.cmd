@echo off
title RESPLICON // GLOBAL MULTI-AGENT CYBERSECURITY OSINT MATRIX
mode con: cols=110 lines=40
color 0b
cd /d "%~dp0"
if not exist "Resplicon.exe" (
    echo [!] Resplicon.exe not found. Running compilation script...
    call build.bat
)
Resplicon.exe %*
