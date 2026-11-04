; =====================================================================
; اسکریپت ساخت فایل نصب برنامه Industrial Sensor Monitor
; ساخت با Inno Setup 6
;   ISCC.exe installer\Setup.iss /DMyAppVersion=1.0.0
; ---------------------------------------------------------------------
; توجه: برنامه به SQL Server (LocalDB یا Express) نیاز دارد، اما این
; بسته نصبی، SQL Server را همراه خود ندارد. در صورت نبود LocalDB
; به کاربر پیام راهنما نمایش داده می‌شود.
; =====================================================================

#define MyAppName "Industrial Sensor Monitor"
#define MyAppNameFa "سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی"
#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#define MyAppPublisher "Industrial Sensor Monitor"
#define MyAppExeName "IndustrialSensorMonitor.exe"
#define MySetupFileName "IndustrialSensorMonitor-Setup-x64"
#ifndef MyPublishDir
  #define MyPublishDir "..\publish\win-x64"
#endif

[Setup]
AppId={{8C3A8B1E-5C1D-4E0B-9C2B-6E9F0A1D7C42}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://github.com/hoseinsedaqat/Industrial-SensorMonitor
AppSupportURL=https://github.com/hoseinsedaqat/Industrial-Sensor-Monitor/issues
AppUpdatesURL=https://github.com/hoseinsedaqat/Industrial-Sensor-Monitor/releases
VersionInfoVersion={#MyAppVersion}
VersionInfoDescription={#MyAppName} Setup
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
DefaultDirName={autopf}\IndustrialSensorMonitor
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=no
AllowNoIcons=yes
OutputDir=..\dist
OutputBaseFilename={#MySetupFileName}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName={#MyAppName} {#MyAppVersion}
UninstallDisplayIcon={app}\{#MyAppExeName}
MinVersion=10.0
DisableDirPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
; فایل اجرایی و فایل تنظیمات (این دو باید کنار هم باشند)
Source: "{#MyPublishDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyPublishDir}\appsettings.json"; DestDir: "{app}"; Flags: ignoreversion
; مستندات و اسکریپت دیتابیس (از ریشه مخزن خوانده می‌شوند)
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Database\InitDatabase.sql"; DestDir: "{app}\Database"; Flags: ignoreversion
; در صورت وجود فایل‌های همراه اضافی در خروجی انتشار
Source: "{#MyPublishDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MyPublishDir}\*.json"; DestDir: "{app}"; Excludes: "appsettings.json"; Flags: ignoreversion skipifsourcedoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppNameFa}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppNameFa}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
Filename: "{app}\Database\InitDatabase.sql"; Description: "مشاهده اسکریپت ایجاد دیتابیس"; Flags: shellexec postinstall skipifsilent unchecked

[UninstallDelete]
; فقط فایل‌های خود برنامه حذف می‌شوند.
; هیچ فایل دیتابیسی در مسیر نصب وجود ندارد و اطلاعات SQL Server دست‌نخورده باقی می‌ماند.
Type: filesandordirs; Name: "{app}\Database"

[Code]
const
  LocalDbRegistryPath = 'SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions';
  SqlExpressServiceKey = 'SYSTEM\CurrentControlSet\Services\MSSQL$SQLEXPRESS';

{ آیا SQL Server LocalDB روی سیستم نصب است؟ }
function IsLocalDbInstalled(): Boolean;
begin
  Result := RegKeyExists(HKLM, LocalDbRegistryPath)
         or RegKeyExists(HKLM32, LocalDbRegistryPath);
end;

{ آیا سرویس SQL Server Express روی سیستم نصب است؟ }
function IsSqlExpressInstalled(): Boolean;
var
  DisplayName: String;
begin
  Result := RegQueryStringValue(HKLM, SqlExpressServiceKey, 'DisplayName', DisplayName);
end;

{ نمایش هشدار در صورت نبود پیش‌نیاز دیتابیس (نصب متوقف نمی‌شود) }
function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := '';
  if (not IsLocalDbInstalled()) and (not IsSqlExpressInstalled()) then
  begin
    if MsgBox('SQL Server LocalDB یا SQL Server Express روی این سیستم پیدا نشد.' + #13#10 +
              'این برنامه برای ذخیره اطلاعات به یکی از این دو نیاز دارد.' + #13#10 + #13#10 +
              'آیا ادامه نصب را می‌پذیرید؟ (پس از نصب باید SQL Server Express LocalDB را نصب کرده و سپس فایل InitDatabase.sql را اجرا کنید.)',
              mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := 'نصب متوقف شد. ابتدا SQL Server Express LocalDB را نصب کنید.';
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    MsgBox('برنامه حذف شد اما اطلاعات دیتابیس شما حذف نشده است.' + #13#10 +
           'دیتابیس IndustrialSensorDb و جدول SensorData در SQL Server باقی می‌مانند.',
           mbInformation, MB_OK);
  end;
end;
