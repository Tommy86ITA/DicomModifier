; ============================================================
;  DicomModifier - Inno Setup Script
;  Distribuzione: framework-dependent, win-x64
;  Richiede: .NET 8 Desktop Runtime (x64)
;
;  Per compilare:
;    iscc.exe Installer\DicomModifier.iss
;  oppure usa build-installer.ps1 dalla root del repo.
; ============================================================

#define AppName      "DICOM Import & Edit"
#define AppExeName   "DicomModifier.exe"
#define AppPublisher "Thomas Amaranto"
#define AppURL       "https://github.com/Tommy86ITA/DicomModifier"
#define AppVersion   GetVersionNumbersString("..\publish\DicomModifier.exe")
#define PublishDir   "..\publish"

[Setup]
AppId={{A3F7B2C1-4D5E-4F6A-9B0C-D1E2F3A4B5C6}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\DicomModifier
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE.md
OutputDir=.\output
OutputBaseFilename=DicomModifier_Setup_{#AppVersion}
SetupIconFile=..\Resources\server_output.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Crea un'icona sul desktop"; GroupDescription: "Icone aggiuntive:"; Flags: unchecked

[Files]
; Tutti i file pubblicati.
; Config.json e DBpsw.json vengono esclusi perché creati/gestiti dall'app.
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "Config.json,DBpsw.json"

; Guida utente
Source: "..\Help\UserGuide.pdf"; DestDir: "{app}\Help"; Flags: ignoreversion

[Icons]
; Menu Start
Name: "{group}\{#AppName}"; FileName: "{app}\{#AppExeName}"; IconFilename: "{app}\{#AppExeName}"
Name: "{group}\Disinstalla {#AppName}"; FileName: "{uninstallexe}"

; Desktop opzionale
Name: "{autodesktop}\{#AppName}"; FileName: "{app}\{#AppExeName}"; IconFilename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
; Avvia l'app al termine dell'installazione
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent

[Code]
// ---------------------------------------------------------------
//  Controlla la presenza di .NET 8 Desktop Runtime (x64).
//  Se non è installato, chiede all'utente se aprire la pagina
//  di download Microsoft.
// ---------------------------------------------------------------
function IsDotNet8Installed(): Boolean;
var
  key: string;
  versionName: string;
begin
  key := 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App';

  // Controlla la chiave specifica del Desktop Runtime 8.x.
  Result := RegQueryStringValue(HKLM, key, '8.0.0', versionName);

  // Fallback: controlla la presenza della cartella runtime.
  if not Result then
    Result := DirExists(ExpandConstant('{pf64}\dotnet\shared\Microsoft.WindowsDesktop.App\8.0.0'));

  // Fallback più permissivo: cartella Microsoft.WindowsDesktop.App presente.
  if not Result then
    Result := DirExists(ExpandConstant('{pf64}\dotnet\shared\Microsoft.WindowsDesktop.App'));
end;

function InitializeSetup(): Boolean;
var
  answer: Integer;
begin
  Result := True;

  if not IsDotNet8Installed() then
  begin
    answer := MsgBox(
      '.NET 8 Desktop Runtime (x64) non è installato sul PC.' + #13#10 +
      '{#AppName} richiede .NET 8 per funzionare.' + #13#10#13#10 +
      'Vuoi aprire la pagina di download Microsoft adesso?' + #13#10 +
      '(Installa il runtime e poi rilancia questo setup.)',
      mbConfirmation,
      MB_YESNO);

    if answer = IDYES then
      ShellExec(
        'open',
        'https://dotnet.microsoft.com/download/dotnet/8.0/runtime?initial-os=windows',
        '',
        '',
        SW_SHOWNORMAL,
        ewNoWait,
        answer);

    Result := False;
  end;
end;