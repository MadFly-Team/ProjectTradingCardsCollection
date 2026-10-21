[Setup]
AppName="MTG Card Database Viewer"
AppVersion=1.0.0 AppPublisher="Neil Beresford"
DefaultDirName="{pf}\MTGCardViewer"
DefaultGroupName="MTG Card Database Viewer"
OutputDir=installer\output
Compression=lzma
SolidCompression=yes
[Files]
Source: dist\*; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
[Icons]
Name: "{group}\MTG Card Database Viewer"; Filename: "{app}\ProjectMTGCollector.exe"
Name: "{userdesktop}\MTG Card Database Viewer"; Filename: "{app}\ProjectMTGCollector.exe"
[Run]
Filename: "{app}\ProjectMTGCollector.exe"; Description: "Launch MTG Card Database Viewer"; Flags: nowait postinstall skipifsilent
[UninstallDelete]
Type: filesandordirs; Name: "{app}"
[UninstallRun] 
Filename: "{app}\unins000.exe"; Parameters: "/silent"
[Messages]
WelcomeLabel1=Welcome to the MTG Card Database Viewer Setup Wizard. This will install the MTG Card Database Viewer application on your computer. Please click Next to continue.
ReadyLabel1=The MTG Card Database Viewer is ready to be installed. Click Install to begin the installation.
