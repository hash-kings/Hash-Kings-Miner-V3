!include x64.nsh
!include WinVer.nsh

BrandingText "${BASE_BRAND_NAME} ${VERSION}"
ShowInstDetails nevershow
SpaceTexts none
!ifdef BUILD_UNINSTALLER
  ShowUninstDetails nevershow
!endif
FileBufSize 64
Name "${PRODUCT_NAME}"

!define APP_EXECUTABLE_FILENAME "${PRODUCT_FILENAME}.exe"
!define UNINSTALL_FILENAME "Uninstall ${PRODUCT_FILENAME}.exe"

!macro check64BitAndSetRegView
  ${IfNot} ${AtLeastWin7}
    MessageBox MB_OK "$(win7Required)"
    Quit
  ${EndIf}

  !ifdef APP_64
    ${If} ${RunningX64}
      SetRegView 64
    ${Else}
      MessageBox MB_OK|MB_ICONEXCLAMATION "$(x64WinRequired)"
      Quit
    ${EndIf}
  !endif
!macroend

# avoid exit code 2
!macro quitSuccess
  SetErrorLevel 0
  Quit
!macroend

!macro setLinkVars
  # old desktop shortcut (could exist or not since the user might has selected to delete it)
  ReadRegStr $oldShortcutName SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" ShortcutName
  ${if} $oldShortcutName == ""
    StrCpy $oldShortcutName "${PRODUCT_FILENAME}"
  ${endIf}
  StrCpy $oldDesktopLink "$DESKTOP\$oldShortcutName.lnk"

  # new desktop shortcut (will be created/renamed in case of a fresh installation or if the user haven't deleted the initial one)
  StrCpy $newDesktopLink "$DESKTOP\${SHORTCUT_NAME}.lnk"

  ReadRegStr $oldMenuDirectory SHELL_CONTEXT "${INSTALL_REGISTRY_KEY}" MenuDirectory
  ${if} $oldMenuDirectory == ""
    StrCpy $oldStartMenuLink "$SMPROGRAMS\$oldShortcutName.lnk"
  ${else}
    StrCpy $oldStartMenuLink "$SMPROGRAMS\$oldMenuDirectory\$oldShortcutName.lnk"
  ${endIf}

  # new menu shortcut (will be created/renamed in case of a fresh installation or if the user haven't deleted the initial one)
  !ifdef MENU_FILENAME
    StrCpy $newStartMenuLink "$SMPROGRAMS\${MENU_FILENAME}\${SHORTCUT_NAME}.lnk"
  !else
    StrCpy $newStartMenuLink "$SMPROGRAMS\${SHORTCUT_NAME}.lnk"
  !endif
!macroend

# this will delete all but configs, backups, internals and miner_plugins
!macro deleteUserData_KeepNonGeneralSettings
  Delete "$INSTDIR\*.*"

  RMDir /R "$INSTDIR\NVIDIA"
  RMDir /R "$INSTDIR\OpenCL"
  RMDir /R "$INSTDIR\logs"
  RMDir /R "$INSTDIR\common"
  RMDir /R "$INSTDIR\plugins_packages"

  DeleteRegKey HKCU "SOFTWARE\Nicehash"
!macroend

# this will delete everything
!macro deleteUserData_All
  RMDir /R $INSTDIR
  DeleteRegKey HKCU "SOFTWARE\Nicehash"
!macroend
