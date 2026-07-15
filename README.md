# BetterShortcuts 🚀

A lightweight, high-performance Windows utility designed to clean up your desktop by customizing or completely hiding shortcut overlay arrows. 

Built with native C# and Windows Forms, **BetterShortcuts** runs entirely in memory without heavy frameworks, background services, or bloated installers. Perfect for clean, debloated Windows installations (like Windows 10 IoT Enterprise) and minimalist desktop setups.

---

## ✨ Features

* **Instant Arrow Removal:** Uses the native Windows modern image library (`imageres.dll,197`) to cleanly hide shortcut arrows—eliminating the classic "black box" cache glitch.
* **Legacy Support:** Option to use standard `shell32.dll` methods.
* **Custom Icon Customization:** Easily set your own `.ico` file to replace the default Windows shortcut arrow.
* **Self-Elevating Privileges:** Automatically requests Administrator permissions (UAC prompt) upon launch to safely modify system registry keys.
* **Automatic Explorer Refresh:** Silently restarts `explorer.exe` in the background when changes are applied, giving you instant visual updates.
* **Minimalist UI:** A clean, modern native Windows GUI. No external web-wrapper engines (Electron/WebView) or bloated frameworks.

---
## 💬 How do i make a custom icon?

simple. go to https://www.freeconvert.com/ico-converter and use the image of your liking, then Bettershortcuts will do the rest.

--
## 🛠️ Build from Source (Minimalist Compilation)

Since this utility is written entirely in native C#, you don't even need Visual Studio installed. You can compile it directly using the built-in compiler included in standard Windows .NET installations:

1. Clone or download the repository.
2. Open **PowerShell** in the source directory.
3. Run the following command:

```powershell
& "$env:SystemRoot\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /target:winexe /win32icon:BetterShortcuts.ico /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:BetterShortcuts.exe BetterShortcuts.cs
