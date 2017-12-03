# youtube-center
A desktop client for youtube (disconnected from your google profile). Import from your main profile and manage subscriptions within the application.

![Demo](https://i.imgur.com/j3PIXyC.gif)

**\* [Find the download link for the latest version here.](https://github.com/dukemiller/youtube-center/releases/latest)**  

---

### Build 
**APIs**  

To build and run this with full functionality, you will have to modify [the api keys container](youtube-center/Classes/ApiKeys.cs) with your own keys before compiling.  

\- [YouTube API](https://console.developers.google.com/apis/)  

**Requirements:** [nuget.exe](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe) on PATH, Visual Studio 2017 and/or C# 7.0 Roslyn Compiler  
**Optional:** Devenv (Visual Studio 2017) on PATH  

```
git clone https://github.com/dukemiller/youtube-center.git
cd youtube-center
nuget install youtube-center\packages.config -OutputDirectory packages
```  

**Building with Devenv (CLI):** ``devenv youtube-center.sln /Build``  
**Building with Visual Studio:**  Open (Ctrl+Shift+O) "youtube-center.sln", Build Solution (Ctrl+Shift+B)

A "youtube-center.exe" artifact will be created in the parent youtube-center folder.
