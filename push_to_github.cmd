@echo off
setlocal enabledelayedexpansion
title RESPLICON // GITHUB REPOSITORY DEPLOYMENT
color 0B

echo.
echo ==============================================================
echo [RESPLICON] GITHUB REPOSITORY DEPLOYMENT WIZARD
echo ==============================================================
echo.

:: Ensure MinGit or Git is available
set GIT_EXE=git
where git >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    if exist "%LOCALAPPDATA%\MinGit\cmd\git.exe" (
        set GIT_EXE="%LOCALAPPDATA%\MinGit\cmd\git.exe"
    ) else (
        echo [!] Git was not detected in PATH or MinGit directory.
        pause
        exit /b 1
    )
)

echo [OK] Using Git engine: %GIT_EXE%
echo.

:: Verify git repository status
%GIT_EXE% status >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [*] Initializing local Git repository on branch 'main'...
    %GIT_EXE% init -b main
    %GIT_EXE% add .
    %GIT_EXE% commit -m "feat: initial commit of Resplicon Multi-Agent OSINT Matrix"
)

echo --------------------------------------------------------------
echo Step 1: Create a NEW empty repository on GitHub:
echo         https://github.com/new
echo         (Do NOT initialize with a README, .gitignore, or license)
echo --------------------------------------------------------------
echo.

set /p REPO_URL="Enter your GitHub Repository URL (e.g. https://github.com/YourUsername/Resplicon.git): "

if "%REPO_URL%"=="" (
    echo [!] No repository URL provided. Aborting.
    pause
    exit /b 1
)

:: Configure origin remote
%GIT_EXE% remote remove origin >nul 2>&1
%GIT_EXE% remote add origin %REPO_URL%

echo.
echo [*] Remote 'origin' configured to: %REPO_URL%
echo [*] Pushing 'main' branch to GitHub...
echo.

%GIT_EXE% push -u origin main

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==============================================================
    echo [SUCCESS] RESPLICON DEPLOYED TO GITHUB SUCCESSFULLY!
    echo ==============================================================
    echo Repository: %REPO_URL%
) else (
    echo.
    echo [!] Push failed. Ensure you have authenticated with your GitHub credentials.
    echo     You can generate a Personal Access Token (PAT) at:
    echo     https://github.com/settings/tokens
)

echo.
pause
