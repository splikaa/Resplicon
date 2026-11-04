@echo off
setlocal
echo ==============================================================
echo [RESPLICON] COMPILING GLOBAL MULTI-AGENT CYBERSECURITY OSINT MATRIX...
echo ==============================================================

set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist %CSC% (
    echo [ERROR] C# Compiler not found at %CSC%
    exit /b 1
)

%CSC% /nologo /optimize+ /platform:anycpu /target:exe /r:System.Web.Extensions.dll /out:Resplicon.exe src\Program.cs src\Engine\AnsiConsole.cs src\Engine\CollaborationEngine.cs src\Engine\DeviceProfileManager.cs src\Engine\AiSlopDetector.cs src\Agents\AgentBase.cs src\Agents\SupervisorAgent.cs src\Agents\FlowAgent.cs src\Agents\HeadSecurityAgent.cs src\Agents\ResearcherAgent.cs src\Agents\DocumentationAgent.cs src\Workspace\VirtualWorkspace.cs src\Workspace\HtmlWorkspaceGenerator.cs src\Modules\OsintModules.cs src\Modules\GlobalPhoneEngine.cs src\Modules\PhishingInvestigationEngine.cs src\Modules\TechStackDetector.cs src\Modules\UserProfileDiscoveryEngine.cs

set COMPILE_ERR=%ERRORLEVEL%
if %COMPILE_ERR% EQU 0 (
    echo.
    echo ==============================================================
    echo [SUCCESS] Resplicon.exe COMPILED SUCCESSFULLY!
    echo ==============================================================
    copy /y Resplicon.exe OSINT-HQ.exe >nul 2>&1
    echo Executable: Resplicon.exe
    exit /b 0
) else (
    echo.
    echo [ERROR] Compilation failed with error code %COMPILE_ERR%
    exit /b %COMPILE_ERR%
)
