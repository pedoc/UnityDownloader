using System.IO;

namespace UnityDownloader;

public class EditorItem
{
    public string Version { get; set; }
    public string Hash { get; set; }

    public string VersionMajor => Version.Split('.')[0];
    public string VersionMinor => Version.Split('.')[1];

    public List<EditorComponent> Win64 { get; set; }
    public List<EditorComponent> Linux { get; set; }
    public List<EditorComponent> Mac { get; set; }
    public List<EditorComponent> MacArm64 { get; set; }

    public string ReleaseDate { get; set; }


    public void FillMacOsEditorComponent()
    {

    }

    public void FillMacOsArm64EditorComponent()
    {

    }    
    
    public void FillLinuxEditorComponent()
    {

    }

    public void FillWin64EditorComponent()
    {
        Win64.Add(new EditorComponent()
        {
            Name = "UnityInstaller",
            DownloadUrl = $"https://download.unity3d.com/download_unity/{Hash}/UnityDownloadAssistant-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Android Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Android-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "iOS Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-iOS-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "tvOS Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-AppleTV-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "visionOS Build Support",
            //https://download.unity3d.com/download_unity/8e9b8558c41a/TargetSupportInstaller/UnitySetup-VisionOS-Support-for-Editor-2022.3.46f1.exe
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-VisionOS-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Linux Build Support (IL2CPP)",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Linux-IL2CPP-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Linux Build Support (Mono)",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Linux-Mono-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Linux Dedicated Server Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Linux-Server-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Mac Build Support (Mono)",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Mac-Mono-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Mac Dedicated Server Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Mac-Server-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Universal Windows Platform Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Universal-Windows-Platform-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "WebGL Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-WebGL-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Windows Build Support (IL2CPP)",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Windows-IL2CPP-Support-for-Editor-{Version}.exe"
        });
        Win64.Add(new EditorComponent()
        {
            Name = "Windows Dedicated Server Build Support",
            DownloadUrl =
                $"https://download.unity3d.com/download_unity/{Hash}/TargetSupportInstaller/UnitySetup-Windows-Server-Support-for-Editor-{Version}.exe"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "BuiltInShaders",
            DownloadUrl = $"https://download.unity3d.com/download_unity/{Hash}/builtin_shaders-{Version}.zip"
        });

        Win64.Add(new EditorComponent()
        {
            Name = "Documentation",
            DownloadUrl =
                $"https://cloudmedia-docs.unity3d.com/docscloudstorage/{VersionMajor}.{VersionMinor}/UnityDocumentation.zip"
        });

        foreach (var editorComponent in Win64)
        {
            editorComponent.Version = Version;
        }
    }
}

public class EditorComponent
{
    public string Name { get; set; }
    public string FileName => Path.GetFileName(DownloadUrl);

    public string DownloadUrl { get; set; }

    public string DownloadProgress { get; set; } = "";
    public string Version { get; set; }
}