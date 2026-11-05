; =====================================================================
; اسکریپت ساخت فایل نصب برنامه Industrial Sensor Monitor
; ساخت با Inno Setup 6
;   ISCC.exe installer\Setup.iss /DMyAppVersion=1.3.1
; ---------------------------------------------------------------------
; این نسخه اطلاعات را در یک فایل JSON محلی ذخیره می‌کند و به هیچ
; SQL Server یا سرویس دیگری نیاز ندارد.
; فایل داده در مسیر زیر قرار می‌گیرد و با حذف برنامه پاک نمی‌شود:
;   %LOCALAPPDATA%\IndustrialSensorMonitor\Data\sensors.json
; =====================================================================

#define MyAppName "Industrial Sensor Monitor"
#define MyAppNameFa "سامانه ثبت و مدیریت اطلاعات سنسورهای صنعتی"
#ifndef MyAppVersion
  #define MyAppVersion "1.3.1"
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
AppSupportURL=https://github.com/hoseinsedaqat/Industrial-SensorMonitor/issues
AppUpdatesURL=https://github.com/hoseinsedaqat/Industrial-SensorMonitor/releases
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
SetupIconFile=..\src\IndustrialSensorMonitor\Assets\app.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
; فایل اجرایی و فایل تنظیمات (این دو باید کنار هم باشند)
Source: "{#MyPublishDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyPublishDir}\appsettings.json"; DestDir: "{app}"; Flags: ignoreversion
; مستندات (از ریشه مخزن خوانده می‌شود)
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
; در صورت وجود فایل‌های همراه اضافی در خروجی انتشار
Source: "{#MyPublishDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MyPublishDir}\*.json"; DestDir: "{app}"; Excludes: "appsettings.json"; Flags: ignoreversion skipifsourcedoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppNameFa}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "{#MyAppNameFa}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[Code]
{ مسیر فایل داده کاربر (بیرون از مسیر نصب، تا با حذف برنامه پاک نشود) }
function GetUserDataPath(): String;
begin
  Result := ExpandConstant('{localappdata}\IndustrialSensorMonitor\Data\sensors.json');
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    { اشاره کوچک به محل ذخیره اطلاعات، بدون ایجاد هیچ فایلی در Program Files }
    Log('Data file location: ' + GetUserDataPath());
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    MsgBox('برنامه حذف شد اما اطلاعات شما حذف نشده است.' + #13#10 + #13#10 +
           'فایل داده در این مسیر باقی می‌ماند:' + #13#10 +
           GetUserDataPath(),
           mbInformation, MB_OK);
  end;
end;
