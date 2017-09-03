::Usage : DeploySQL.bat <Script folder> <DatabaseIPPort> <DBUser> <DBPassword> <DBName> <DBInstanceName>
@ECHO off
CLS
DEL .\Commit_SQL_Deploy.bat /F /Q
GOTO _chkenv

:_chkenv
::Configure Debug Information Option
SET IS_4_DEBUG=TRUE
SET IS_TEST_ENV=FALSE
::SET IS_TEST_ENV=TRUE

SET SHOW_DEBUG_TIMESTAMP=TRUE
SET TIMESTAMP=NULL
IF %SHOW_DEBUG_TIMESTAMP%==TRUE @SET TIMESTAMP=[%DATE% %TIME% CPBU] -

::Test executability of the designated SQL Command Line Tool 
SET CPBU_SQLEXEBIN=OSQL.EXE
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Target SQL Command Line Tool : %CPBU_SQLEXEBIN%

%CPBU_SQLEXEBIN% -?

IF %ERRORLEVEL% EQU 9009 @ECHO _chkenv Error Return Code =%ERRORLEVEL% && GOTO _error_osql_not_fount
IF %ERRORLEVEL% EQU 1 @ECHO _chkenv Error Return Code =%ERRORLEVEL% && GOTO _setenv

:_setenv
CLS

SET CPBU_CURRENTDIR=%CD%
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Current Directory : %CPBU_CURRENTDIR%
SET CPBU_PROJDIR=%CPBU_CURRENTDIR%
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Project Directory : %CPBU_PROJDIR%
SET CPBU_SQLDBDIR=%CPBU_PROJDIR%%1%
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% SQL CENTRAL Script Directory : %CPBU_SQLCENTRALDIR%

SET CPBU_SCRIPTFOLDER=%CPBU_PROJDIR%\%1
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Source Database Script Folder : %CPBU_SCRIPTFOLDER%
SET CPBU_DBIPFQDN=%2
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Target Database IP or FQDN : %CPBU_DBIPFQDN%
SET CPBU_DBUSRUSN=%3
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Database Access Username : %CPBU_DBUSRUSN%
SET CPBU_DBUSRPWD=%4
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Database Access Password : %CPBU_DBUSRPWD%
SET DBNAME=%5
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Database Name : %DBNAME%
SET CPBU_DBINSTNAME=%6

SET CPBU_LOGFILENAME=.\CPBU_SQLDEPLOY.LOG
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Deployment Output File Name : %CPBU_LOGFILENAME%

IF %CPBU_DBINSTNAME%_TEST==_TEST SET CPBU_DBINSTNAME=MSSQLSERVER
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Target Instance Name : %CPBU_DBINSTNAME%

SET CPBU_DBSVR=%CPBU_DBIPFQDN%\%CPBU_DBINSTNAME%
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% Target Database Server\Instance : %CPBU_DBSVR%

::SET CPBU_SQLCONNCMD=%CPBU_SQLEXEBIN% -S %CPBU_DBSVR% -U %CPBU_DBUSRUSN% -P %CPBU_DBUSRPWD% -o %CPBU_LOGFILENAME% -i 
SET CPBU_SQLCONNCMD=%CPBU_SQLEXEBIN% -S %CPBU_DBIPFQDN% -U %CPBU_DBUSRUSN% -P %CPBU_DBUSRPWD%  -o %CPBU_LOGFILENAME% -i
IF %IS_4_DEBUG%==TRUE @ECHO %TIMESTAMP% SQL Connection Command : %CPBU_SQLCONNCMD% [SQL Input File Name]

ECHO.
ECHO _setenv Error Return Code =%ERRORLEVEL%

IF %IS_TEST_ENV%==TRUE GOTO _unsetenv
IF %IS_TEST_ENV%==FALSE GOTO _script_generation


:_unsetenv
ECHO.
ECHO _unsetenv b4 unsetenv Error Return Code =%SQLDeployResult%

SET ErrRtnB4UnSetEnv=%SQLDeployResult%

SET IS_4_DEBUG=
SET IS_TEST_ENV=
SET SHOW_TIMESTAMP=
SET TIMESTAMP=
SET CPBU_CURRENTDIR=
SET CPBU_PROJDIR=
SET CPBU_SQLBUDIR=
SET CPBU_SQLOPDIR=
SET CPBU_DBIPFQDN=
SET CPBU_DBUSRUSN=
SET CPBU_DBUSRPWD=
SET CPBU_DBNAME=
SET CPBU_DBINSTNAME=
SET CPBU_SQLEXEBIN=
SET CPBU_LOGFILENAME=
SET CPBU_DBSVR=
SET CPBU_SQLCONNCMD=

IF %ErrRtnB4UnSetEnv% EQU 0 SET ERRORLEVEL=0 && SET ErrRtnB4UnSetEnv=
ECHO.
ECHO _unsetenv Error Return Code =%SQLDeployResult%
GOTO _exit

:_script_generation
::Prepare and Generate SQL Inject Script for the Database
ECHO.
IF %IS_4_DEBUG%==TRUE ECHO.
::for /f %%i in ('dir /b "%CPBU_SCRIPTFOLDER%\32_indexes\*.sql"') do @ECHO ECHO. >> .\Commit_SQL_Deploy.bat && ECHO ECHO Processing object for %%i ...... >> .\Commit_SQL_Deploy.bat && ECHO %CPBU_SQLCONNCMD% "%CPBU_SCRIPTFOLDER%\32_indexes\%%i" -d %DBNAME% >> Commit_SQL_Deploy.bat && ECHO TYPE %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat && ECHO FIND "Line" %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat &&  ECHO IF %ERRORLEVEL% EQU 0 EXIT 1 >> Commit_SQL_Deploy.bat && ECHO.  >> Commit_SQL_Deploy.bat
IF %IS_4_DEBUG%==TRUE ECHO.
for /f %%i in ('dir /b "%CPBU_SCRIPTFOLDER%\40_views\*.sql"') do @ECHO ECHO. >> .\Commit_SQL_Deploy.bat && ECHO ECHO Processing object for %%i ...... >> .\Commit_SQL_Deploy.bat && ECHO %CPBU_SQLCONNCMD% "%CPBU_SCRIPTFOLDER%\40_views\%%i" -d %DBNAME% >> Commit_SQL_Deploy.bat && ECHO TYPE %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat && ECHO FIND "Line" %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat &&  ECHO IF %ERRORLEVEL% EQU 0 EXIT 1 >> Commit_SQL_Deploy.bat && ECHO.  >> Commit_SQL_Deploy.bat
IF %IS_4_DEBUG%==TRUE ECHO.
for /f %%i in ('dir /b "%CPBU_SCRIPTFOLDER%\50_functions\*.sql"') do @ECHO ECHO. >> .\Commit_SQL_Deploy.bat && ECHO ECHO Processing object for %%i ...... >> .\Commit_SQL_Deploy.bat && ECHO %CPBU_SQLCONNCMD% "%CPBU_SCRIPTFOLDER%\50_functions\%%i" -d %DBNAME% >> Commit_SQL_Deploy.bat && ECHO TYPE %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat && ECHO FIND "Line" %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat &&  ECHO IF %ERRORLEVEL% EQU 0 EXIT 1 >> Commit_SQL_Deploy.bat && ECHO.  >> Commit_SQL_Deploy.bat
IF %IS_4_DEBUG%==TRUE ECHO.
for /f %%i in ('dir /b "%CPBU_SCRIPTFOLDER%\60_stored_procedures\*.sql"') do @ECHO ECHO. >> .\Commit_SQL_Deploy.bat && ECHO ECHO Processing object for %%i ...... >> .\Commit_SQL_Deploy.bat && ECHO %CPBU_SQLCONNCMD% "%CPBU_SCRIPTFOLDER%\60_stored_procedures\%%i" -d %DBNAME% >> Commit_SQL_Deploy.bat && ECHO TYPE %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat && ECHO FIND "Line" %CPBU_LOGFILENAME% >> Commit_SQL_Deploy.bat &&  ECHO IF %ERRORLEVEL% EQU 0 EXIT 1 >> Commit_SQL_Deploy.bat && ECHO.  >> Commit_SQL_Deploy.bat
IF %IS_4_DEBUG%==TRUE ECHO.

DEL /Q /F sed*.*
ECHO.
ECHO Begin replacing string ERRORLEVEL to %%ERRORLEVEL%%...
"%ProgramFiles%\GNUWin32\bin\sed.exe" -i s/ERRORLEVEL/"%%ERRORLEVEL%%"/g  .\Commit_SQL_Deploy.bat
ECHO String replace completed.
ECHO EXIT 0 >> .\Commit_SQL_Deploy.bat
ECHO.
ECHO _script_generation Error Return Code = %ERRORLEVEL%
GOTO _commit_deployment

:_commit_deployment

%WinDir%\system32\cmd.exe /C .\Commit_SQL_Deploy.bat
SET SQLDeployResult=%ERRORLEVEL%

::DEL /Q /F .\Commit_SQL_Deploy.bat
DEL /Q /F sed*.*
DEL /Q /F sed*
DEL /Q /F %CPBU_LOGFILENAME%

ECHO.
ECHO _commit_deployment Error Return Code = %SQLDeployResult%
GOTO _unsetenv

:_error_osql_not_fount
CLS
IF %SHOW_DEBUG_TIMESTAMP%==TRUE ECHO.
IF %SHOW_DEBUG_TIMESTAMP%==TRUE ECHO %TIMESTAMP% 
ECHO.
ECHO The Microsoft (R) SQL Server Command Line Tool cannot be found.
ECHO Please try again.
ECHO.
ECHO _error_osql_not_fount Error Return Code =%ERRORLEVEL%
GOTO _unsetenv

:_exit
ECHO.
ECHO This is the end of SQL deployment procedure.
ECHO.
IF %SQLDeployResult% NEQ 0 EXIT 1
SET SQLDeployResult=
exit 0
