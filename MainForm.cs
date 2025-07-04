using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Downloader;
using Newtonsoft.Json;
using PuppeteerSharp;
using Serilog;
using UnityDownloader.Properties;
using static UnityDownloader.VersionRecord;
using Timer = System.Windows.Forms.Timer;

// ReSharper disable LocalizableElement

// ReSharper disable AsyncApostle.ConfigureAwaitHighlighting

#pragma warning disable SYSLIB0014

namespace UnityDownloader;

public partial class MainForm : XtraForm
{
    public MainForm()
    {
        InitializeComponent();
    }


    private void ShowMessage(string msg)
    {
        if (InvokeRequired)
        {
            Invoke(() => { ShowMessage(msg); });
        }
        else
        {
            memTxt.Text += $"[{DateTime.Now:yyyyMMdd-HH:mm:ss}]" + msg + Environment.NewLine;

            Log.Logger.Information(msg);
        }
    }

    private void ShowMessage(string prefix, string msg)
    {
        ShowMessage($"[{prefix}] {msg}");
    }

    const string EditorJSONFile = "editor.json";

    private async Task<bool> GenerateEditorJsonFileAsync()
    {
        //const string url = "https://unity3d.com/get-unity/download/archive";
        // var html = DownloadContent(url);
        // if (string.IsNullOrEmpty(html))
        // {
        //     ShowMessage("拉取u3d版本列表失败," + url);
        //     return false;
        // }
        IBrowser browser = null;
        IPage page = null;
        try
        {
            var browserFetcher = new BrowserFetcher();
            var proxyAddress = txtProxy.Text.Trim();
            var hasProxy = proxyAddress.Length > 0;
            if (hasProxy)
            {
                browserFetcher.WebProxy = new WebProxy(proxyAddress);
            }

            //124.0.6367.201
            var installedBrowser = await browserFetcher.DownloadAsync();
            var args = new List<string>();
            LaunchOptions launchOptions = new LaunchOptions()
            {
                Headless = false,
                ExecutablePath = installedBrowser.GetExecutablePath(),
                Args = []
            };
            if (hasProxy)
            {
                args.Add("--proxy-server=\"" + proxyAddress + "\"");
            }

            args.Add("--user-data-dir=userdata-alt");
            args.Add("--disk-cache-dir=cache-alt");

            if (args.Count > 0)
            {
                launchOptions.Args = args.Select(i => i = " " + i).ToArray();
            }

            browser = await Puppeteer.LaunchAsync(launchOptions);
            page = (await browser.PagesAsync())[0];
            await page.SetUserAgentAsync(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36 Edg/128.0.0.0");
            var targetAddress = txtEditorJson.Text.Trim();
            await page.GoToAsync(targetAddress, WaitUntilNavigation.DOMContentLoaded);

            var elements = await page.QuerySelectorAllAsync(txtSelector.Text.Trim());
            //await Task.Delay(TimeSpan.FromSeconds(5));
            await page.WaitForSelectorAsync(txtSelector.Text.Trim(), new WaitForSelectorOptions()
            {
                Visible = true
            });

            if (!page.Url.StartsWith(targetAddress))
            {
                ShowMessage($"{targetAddress} 已被重定向到 {page.Url},请检查代理是否生效");
                return false;
            }


            const int maxRetryCount = 3;
            int retryCount = 1;
            do
            {
                if (elements.Length <= 0)
                {
                    ShowMessage($"未抓取到任何有效元素,正在重试第 {retryCount} 次");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    elements = await page.QuerySelectorAllAsync(txtSelector.Text.Trim());
                }
                else
                {
                    break;
                }
            } while (retryCount++ <= maxRetryCount);

            if (retryCount > maxRetryCount && elements.Length <= 0)
            {
                return false;
            }

            List<VersionRecord> versions = new();
            foreach (var elementHandle in elements)
            {
                VersionRecord vr = new();
                var webMajorVersionText = await elementHandle.GetInnerTextAsync();
                ShowMessage($"正在抓取 {webMajorVersionText}");
                await elementHandle.ClickAsync();
                await Task.Delay(TimeSpan.FromSeconds(3));

                var tableElement = await page.QuerySelectorAsync("table");
                retryCount = 1;
                do
                {
                    if (tableElement == null)
                    {
                        ShowMessage(webMajorVersionText, $"未找到table元素,正在重试第 {retryCount} 次");
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        tableElement = await page.QuerySelectorAsync("table");
                    }
                    else
                    {
                        break;
                    }
                } while (retryCount++ <= maxRetryCount);

                if (retryCount > maxRetryCount && tableElement == null)
                {
                    ShowMessage(webMajorVersionText, "未找到table元素,所有尝试均失败");
                    continue;
                }

                var trElements = await tableElement.QuerySelectorAllAsync("tbody>tr");
                ShowMessage(webMajorVersionText,
                    $"发现版本总数:{trElements.Length},已抓取:{versions.Sum(v => v.Versions.Count)}");
                foreach (var trElement in trElements)
                {
                    var webMinorVersionText = await trElement.GetInnerTextAsync();
                    if (webMinorVersionText.Contains("\n"))
                    {
                        webMinorVersionText = webMinorVersionText.Split("\n")[0];
                    }

                    var downloadElement = await trElement.QuerySelectorAsync("a[href^=\"unityhub://\"");
                    if (downloadElement == null)
                    {
                        ShowMessage($"{webMajorVersionText}-{webMinorVersionText}", "未找到下载按钮");
                        continue;
                    }

                    var href = await downloadElement.GetPropertyAsync("href");
                    if (href == null)
                    {
                        ShowMessage($"{webMajorVersionText}-{webMinorVersionText}", "未找到下载地址");
                        continue;
                    }

                    var hrefString = href.ToString();
                    if (string.IsNullOrEmpty(hrefString)) continue;

                    var releaseDate = await trElement.QuerySelectorAsync("td:nth-child(2) > div > span");
                    var releaseDateText = await releaseDate.GetInnerTextAsync();

                    var match = Regex.Match(hrefString, "unityhub://(?<version>.*)/(?<hash>.*)",
                        RegexOptions.Compiled,
                        TimeSpan.FromSeconds(3));
                    if (match.Success)
                    {
                        vr.WebMajorVersionText = webMajorVersionText;
                        var version = match.Groups["version"].Value;
                        var hash = match.Groups["hash"].Value;
                        vr.Versions[version] = new VersionRecordItem
                        {
                            releaseDate = releaseDateText,
                            hash = hash,
                            version = version,

                            linux =
                                $"https://download.unity3d.com/download_unity/{hash}/LinuxEditorInstaller/Unity.tar.xz",
                            mac =
                                $"https://download.unity3d.com/download_unity/{hash}/MacEditorInstaller/Unity-{version}.pkg",
                            macArm64 =
                                $"https://download.unity3d.com/download_unity/{hash}/MacEditorInstallerArm64/Unity-{version}.pkg",
                            win64 =
                                $"https://download.unity3d.com/download_unity/{hash}/Windows64EditorInstaller/UnitySetup64-{version}.exe"
                        };
                        versions.Add(vr);
                        //ShowMessage($"{webMajorVersionText}-{webMinorVersionText}",$"日期:{releaseDateText},hash:{hash},总数:{dict.Keys.Count}");
                    }
                    else
                    {
                        ShowMessage($"{webMajorVersionText}-{webMinorVersionText}", "未能解析版本号和hash值");
                    }
                }
            }

            var json = JsonConvert.SerializeObject(versions, Formatting.Indented);
            await File.WriteAllTextAsync(EditorJSONFile, json);

            await page.CloseAsync();
            await browser.CloseAsync();

            return true;
        }
        catch (Exception ex)
        {
            ShowMessage($"读取页面出错,详情:{ex.Message},堆栈:{ex.StackTrace}");
        }
        finally
        {
            await page?.CloseAsync();
            await browser?.CloseAsync();
        }

        return false;
    }

    public void TryLoadEditorJson()
    {
        if (!File.Exists(EditorJSONFile))
        {
            ShowMessage("请先更新编辑器资源");
            return;
        }

        try
        {
            var json = File.ReadAllText(EditorJSONFile);
            var versionRecords = JsonConvert.DeserializeObject<List<VersionRecord>>(json);
            var editorItems = new List<EditorItem>();
            foreach (var versionRecord in versionRecords)
            {
                foreach (var (version, item) in versionRecord.Versions)
                {
                    var editorItem = new EditorItem()
                    {
                        Version = version,
                        Linux = new List<EditorComponent>(),
                        Mac = new List<EditorComponent>(),
                        MacArm64 = new List<EditorComponent>(),
                        Win64 = new List<EditorComponent>(),
                        ReleaseDate = item.releaseDate,
                        Hash = item.hash
                    };
                    editorItem.Linux.Add(new EditorComponent()
                    {
                        Name = "UnityEditor",
                        DownloadUrl = item.linux
                    });
                    editorItem.Mac.Add(new EditorComponent()
                    {
                        Name = "UnityEditor",
                        DownloadUrl = item.mac,
                    });
                    editorItem.MacArm64.Add(new EditorComponent()
                    {
                        Name = "UnityEditor",
                        DownloadUrl = item.macArm64,
                    });
                    editorItem.Win64.Add(new EditorComponent()
                    {
                        Name = "UnityEditor",
                        DownloadUrl = item.win64,
                    });

                    //如果没有hash则尝试从url中提取填充
                    if (string.IsNullOrEmpty(editorItem.Hash))
                    {
                        var url = editorItem.Win64.First().DownloadUrl;
                        editorItem.Hash = url.Replace("https://download.unity3d.com/download_unity/", "")
                            .Substring(0, 12);
                    }

                    editorItem.FillWin64EditorComponent();

                    editorItem.FillMacOsEditorComponent();
                    editorItem.FillMacOsArm64EditorComponent();
                    editorItem.FillLinuxEditorComponent();

                    editorItems.Add(editorItem);
                }
            }

            gridView.OptionsBehavior.ReadOnly = true;
            //gridView.OptionsBehavior.Editable = false;

            gridView.OptionsSelection.MultiSelect = true;
            gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;

            gridControl.DataSource = editorItems;
            gridView.Columns[nameof(EditorItem.VersionMajor)].Group();
            //gridView.Columns[nameof(EditorItem.VersionMinor)].Group();

            Text += $"(资源更新时间:{new FileInfo(EditorJSONFile).LastWriteTime:yyyy-MM-dd HH:mm:ss})";
        }
        catch (Exception e)
        {
            ShowMessage(e.Message);
        }
    }

    private async void btnDownloadEditorJson_Click(object sender, EventArgs e)
    {
        // DownloadEditorJsonAsync(_ =>
        // {
        //     TryLoadEditorJson();
        //     ShowMessage("编辑器资源下载成功并完成加载");
        // });

        btnDownloadEditorJson.Enabled = false;
        try
        {
            if (await GenerateEditorJsonFileAsync())
            {
                TryLoadEditorJson();
                ShowMessage("编辑器资源下载成功并完成加载");
            }
            else
            {
                ShowMessage("编辑器资源下载失败");
            }
        }
        finally
        {
            btnDownloadEditorJson.Enabled = true;
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version;
        Text += $" -v{version}";

        TryLoadEditorJson();
    }

    private DownloadService CreateDownloader(int parallelCount)
    {
        var downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 1, // Math.Max(1, Environment.ProcessorCount - 1),
            ParallelDownload = true,
            ParallelCount = parallelCount,
            RequestConfiguration =
            {
                Proxy = new WebProxy()
                {
                    BypassProxyOnLocal = true,
                    Address = new Uri(txtProxy.Text.Trim())
                }
            }
        };

        var downloader = new DownloadService(downloadOpt);
        return downloader;
    }

    private async void btnDownloadEditor_Click(object sender, EventArgs e)
    {
        btnDownloadEditor.Enabled = false;
        lblTotalTime.Text = "";
        pbar.Position = 0;

        if (!int.TryParse(txtDownloadCount.Text, out var num))
        {
            num = 4;
        }

        var view = gridControl.FocusedView as GridView;
        if (view == null) return;

        var rows = view.GetSelectedRows();
        var totalDownloadTaskCount = rows.Length;
        if (totalDownloadTaskCount <= 0)
        {
            ShowMessage("请先选择要下载的项");
            return;
        }

        ShowMessage($"当前选中了 {totalDownloadTaskCount} 项准备下载");

        var total = Stopwatch.StartNew();

        var timer = new Timer();
        timer.Interval = 1000;
        timer.Tick += (s, _) =>
        {
            lblTotalTime.Text = total.ToString();
            pbar.Position =
                (int)rows.Select(r => ((EditorComponent)view.GetRow(r)).DownloadProgress).Sum() /
                (rows.Length);


            if (Volatile.Read(ref totalDownloadTaskCount) <= 0)
            {
                total.Stop();
                timer.Stop();
            }

            view.RefreshData();
        };
        timer.Start();


        foreach (var i in rows)
        {
            var editorComponent = view.GetRow(i) as EditorComponent;
            if (editorComponent == null) return;

            var directory = Path.Combine(txtSaveDirectory.Text, editorComponent.Version);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var path = Path.Combine(directory, editorComponent.FileName);

            var sw = Stopwatch.StartNew();
            ShowMessage($"开始从 {editorComponent.DownloadUrl} 下载到 {path}");

            var downloader = CreateDownloader(num);
            downloader.DownloadStarted += (s, dpce) =>
            {
                editorComponent.DownloadSize = dpce.TotalBytesToReceive / 1024 / 1024;;
            };
            downloader.DownloadProgressChanged += (s, dpce) =>
            {
                editorComponent.DownloadProgress = dpce.ProgressPercentage;
                editorComponent.DownloadElapsed = sw.Elapsed;
                //editorComponent.DownloadSize = dpce.ReceivedBytesSize / 1024 / 1024;

                if (dpce.ProgressPercentage >= 100)
                {
                    editorComponent.DownloadCompleted = true;
                    editorComponent.DownloadProgress = 100;
                }
            };
            downloader.DownloadFileCompleted += (s, ace) =>
            {
                sw.Stop();
                editorComponent.DownloadCompleted = ace.Error == null;

                if (editorComponent.DownloadCompleted)
                {
                    editorComponent.DownloadProgress = 100;
                }

                Interlocked.Decrement(ref totalDownloadTaskCount);
            };

            _ = downloader.DownloadFileTaskAsync(editorComponent.DownloadUrl, path);
        }

        Invoke(() => { btnDownloadEditor.Enabled = true; });
    }

    private void btnOpenDirectory_Click(object sender, EventArgs e)
    {
        Process.Start("explorer", txtSaveDirectory.Text.Trim());
    }

    private static void Patch()
    {
        ReplaceLicense();
        PatchDll();

        static void PatchDll()
        {
            var newCert = """
                          -----BEGIN CERTIFICATE-----
                          MIIE7zCCA9egAwIBAgIUUCAbT5WTG+L4uTES4NbaOwvc4L8wDQYJKoZIhvcNAQEFBQAwgfMxCzAJ
                          BgNVBAYTAkRLMRMwEQYDVQQIDApDb3BlbmhhZ2VuMRMwEQYDVQQHDApDb3BlbmhhZ2VuMR8wHQYD
                          VQQKDBZVbml0eSBUZWNobm9sb2dpZXMgQXBzMUkwRwYDVQQLDEAuLi4uLi4uLi4uLi4uLi4uLi4u
                          Li4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uMQswCQYDVQQDDAJJ
                          VDEgMB4GCSqGSIb3DQEJARYRYWRtaW5AdW5pdHkzZC5jb20xHzAdBgNVBCkMFi4uLi4uLi4uLi4u
                          Li4uLi4uLi4uLi4wHhcNMjMwNDI2MTIxMDQzWhcNMjQwNDI1MTIxMDQzWjCB8zELMAkGA1UEBhMC
                          REsxEzARBgNVBAgMCkNvcGVuaGFnZW4xEzARBgNVBAcMCkNvcGVuaGFnZW4xHzAdBgNVBAoMFlVu
                          aXR5IFRlY2hub2xvZ2llcyBBcHMxSTBHBgNVBAsMQC4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4u
                          Li4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4uLi4xCzAJBgNVBAMMAklUMSAwHgYJ
                          KoZIhvcNAQkBFhFhZG1pbkB1bml0eTNkLmNvbTEfMB0GA1UEKQwWLi4uLi4uLi4uLi4uLi4uLi4u
                          Li4uLjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAL3t/3ossioWbzMe6SK13Zn7rskP
                          kLtNJjlNUbFRxz7IzJokoG387NSXOlOAn2qEHIRpDNsxDTH20fkLzYBwe0Tr5a2u9l+DOWpRnt7J
                          zMJaTBWiWsrLnZht5ePRj7Vn7c25qm7Pdq9iuYr0zKmqEW3+eNH7a6PHgQyJRLkk/zuE0drczBkn
                          REazOmACir9o1gjU/U36FYN+v3r4sELHDKmJ5J+QrxfmxFsXzXuZc8wTrE8pxWXIIWLZRjic/zKx
                          VQifYUQ9wUhFJuTdByuHKhi37uG8jUNyfPe1PCffQhLRb2pbCw5kjKfkJwJexhd+MBsODFboC3oh
                          ryD0Vwq3678CAwEAAaN5MHcwDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggr
                          BgEFBQcDBDAJBgNVHRMEAjAAMBwGA1UdEQQVMBOBEWFkbWluQHVuaXR5M2QuY29tMB0GA1UdDgQW
                          BBQ61G5XMwod4trNVUBXzlbFSdA0yzANBgkqhkiG9w0BAQUFAAOCAQEAImvfvAvV5Yqq2gjF16Sb
                          y9MGIQiYjyVBIDoHxwXneHCAaxR6IKS+5elwUruJ/D+BK6vK09Ju9DzqLL6yHo/ZqfycfkcAGcxH
                          /RTOnuz/ZZ2pLHyz0sjvoONQLHNKmYTvMnLvQLiTj71VYyMUQW6EfaFttIDzzW48cUduExH+UIiQ
                          9OeZmSEsW+NeGB4KejdggVTqJMrPf85kA/8PBxlYlxn6icqDu40T4l1Uc/qDvj8d598MbOD1PBLc
                          nbAR2VgljJN55v48Fmmwg1g2cIjWQt4jTuJWL67O65S1tKKmlsnEKT+37f2B8l2ijm7/fwA9qI2i
                          dAUsITaJDHJ+FC6LGg==
                          -----END CERTIFICATE-----
                          """;
            var oriCert = """
                          -----BEGIN CERTIFICATE-----
                          MIIE7zCCA9egAwIBAgIRAJZpZsDepakv5CaXJit+yx4wDQYJKoZIhvcNAQEFBQAwVDELMAkGA1UE
                          BhMCQkUxGTAXBgNVBAoTEEdsb2JhbFNpZ24gbnYtc2ExKjAoBgNVBAMTIUdsb2JhbFNpZ24gUGVy
                          c29uYWxTaWduIDIgQ0EgLSBHMjAeFw0xMjA4MDcxMzA3MzdaFw0xMzA4MDgxMzA3MzdaMIGHMQsw
                          CQYDVQQGEwJESzETMBEGA1UECBMKQ29wZW5oYWdlbjETMBEGA1UEBxMKQ29wZW5oYWdlbjEfMB0G
                          A1UEChMWVW5pdHkgVGVjaG5vbG9naWVzIEFwczELMAkGA1UEAxMCSVQxIDAeBgkqhkiG9w0BCQEW
                          EWFkbWluQHVuaXR5M2QuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA18DUjObN
                          s/rPbTGfVbQkt0FVOEhBb3FF90ZzkLs5dApGvtuKTUmo94Xoha5rkXBWFTnRGRXSANuqAljfHFiJ
                          3QtXB4l9SzrwGrvswWHh3hZh/AIhbwBanDWT02NEd92hOPOoCkMzHxGcJWT+dKYgSTgTmDx28tsG
                          urvgkdETqO8Ueo/Y0hIXRTQMtJ0wih6U6WQ1RxY+qTo6ImrAz/CjhtpfkyZ+yj8iZbW5uCJ8/bjO
                          MmTpO/awDcrkooFxd16/hMuCuhkq4Iejuk8/9i48DyqtA7Q3utQJ2FA97NONuWdOz7lms/MeHHNG
                          Izhhe+vyAbZuSrtr8gQF3Rw5Edu1bwIDAQABo4IBhjCCAYIwDgYDVR0PAQH/BAQDAgWgMEwGA1Ud
                          IARFMEMwQQYJKwYBBAGgMgEoMDQwMgYIKwYBBQUHAgEWJmh0dHBzOi8vd3d3Lmdsb2JhbHNpZ24u
                          Y29tL3JlcG9zaXRvcnkvMBwGA1UdEQQVMBOBEWFkbWluQHVuaXR5M2QuY29tMAkGA1UdEwQCMAAw
                          HQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMEMEMGA1UdHwQ8MDowOKA2oDSGMmh0dHA6Ly9j
                          cmwuZ2xvYmFsc2lnbi5jb20vZ3MvZ3NwZXJzb25hbHNpZ24yZzIuY3JsMFUGCCsGAQUFBwEBBEkw
                          RzBFBggrBgEFBQcwAoY5aHR0cDovL3NlY3VyZS5nbG9iYWxzaWduLmNvbS9jYWNlcnQvZ3NwZXJz
                          b25hbHNpZ24yZzIuY3J0MB0GA1UdDgQWBBS77TdJmqER3qBFAT+yU4zBB8a1xTAfBgNVHSMEGDAW
                          gBQ/FdJtfC/nMZ5DCgaolGwsO8XuZTANBgkqhkiG9w0BAQUFAAOCAQEAC//kW1Pu07FhYo0Wmju5
                          ZCDaolsieAQpnDeJC1P76dfQxnGGibscK1xRk3lhiFx6cDoxnOyCiKt/CdWWtAMO6bnO4UmFEtO3
                          UljKg4VmayvGMhW5dup3M7FRn/CDM6UJl3dHJ3PmclbDZEQ0ctiXxBwIEPFy1Y1X9b3SwznX3pWJ
                          /UsQ270DtKuVz3kUqSpZEhBo8Gb1m+FoGsnGQb+8vnfEGgD9/bxURhTUeteQ1N+CGyfTCd0QVqKx
                          zPO43SpWwQ50SDtQT0bEZeA+UOdqSH04W4XCkcmx+1zZ8GtHihaefyxDceZOKKPq4Gi+02JbwWuX
                          JXFP96+m73xQXG96dg==
                          -----END CERTIFICATE-----
                          """;
            var installedVersions = Directory.GetDirectories(@"C:\Program Files", "Unity 202*",
                SearchOption.TopDirectoryOnly);
            foreach (var installedVersion in installedVersions)
            {
                var version = Path.GetFileName(installedVersion).Replace("Unity ", "");
                Console.WriteLine($@"正在处理 {version}");
                var targetDll = Path.Combine(installedVersion, "Editor", "Data", "Resources", "Licensing", "Client",
                    "Unity.Licensing.EntitlementResolver.dll");
                if (!File.Exists(targetDll)) continue;

                var fileData = File.ReadAllBytes(targetDll);
                var fingerprintData = Encoding.ASCII.GetBytes(oriCert);
                var end = GetPositionAfterMatch(fileData, fingerprintData);
                var begin = end - fingerprintData.Length;
                if (begin < 0 || begin > fileData.Length - fingerprintData.Length)
                {
                    Console.WriteLine(@"未能在文件中找到特征码，跳过文件Patch");
                }
                else
                {
                    try
                    {
                        var keydata = Encoding.ASCII.GetBytes(newCert);
                        if (keydata.Length == fingerprintData.Length)
                        {
                            File.Copy(targetDll, targetDll + ".bak", true);
                            using var file = File.Open(targetDll, FileMode.Open, FileAccess.ReadWrite);
                            file.Position = begin;
                            file.Write(keydata, 0, keydata.Length);
                            Console.WriteLine($@"文件 {targetDll} Patch 已完成");
                        }
                        else Console.WriteLine(@"文件Patch无效，Cert长度不一致");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($@"Dll Patch出现异常，详情：{ex.Message}");
                    }
                }
            }
        }

        static void ReplaceLicense()
        {
            try
            {
                var licenseDir = @"C:\ProgramData\Unity";
                if (!Directory.Exists(licenseDir))
                {
                    Directory.CreateDirectory(licenseDir);
                }

                var licenseFile = Path.Combine(licenseDir, "Unity_lic.ulf");
                if (File.Exists(licenseFile))
                {
                    File.Copy(licenseFile, licenseFile + ".bak", true);
                }

                File.WriteAllText(licenseFile, Resources.Unity_lic);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"License Patch出现异常，详情：{ex.Message}");
            }
        }
    }

    static int GetPositionAfterMatch(byte[] data, byte[] pattern)
    {
        for (int i = 0; i < (data.Length - pattern.Length); i++)
        {
            bool match = true;
            for (int k = 0; k < pattern.Length; k++)
            {
                if (data[i + k] != pattern[k])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                return i + pattern.Length;
            }
        }

        return -1;
    }

    private void pbar_DoubleClick(object sender, EventArgs e)
    {
        if ((ModifierKeys & Keys.Control) != 0)
        {
            Patch();
            ShowMessage("completed");
        }
    }

    private void cbxHub_SelectedValueChanged(object sender, EventArgs e)
    {
        var downloadUri = "";
        switch (cbxHub.Text.Trim())
        {
            case "Windows":
                downloadUri = "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe";
                break;
            case "Mac":
                //downloadUri = "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.dmg";
                break;
            case "Linux":
                //downloadUri = "https://unity3d.com/get-unity/download?ref=personal";
                break;
            default:
                break;
        }

        if (string.IsNullOrEmpty(downloadUri)) return;

        var fileName = Path.Combine(txtSaveDirectory.Text.Trim(),
            Path.GetFileName(downloadUri) + cbxHub.Text.Trim() + ".tmp");
        if (File.Exists(fileName)) File.Delete(fileName);
        var downloader = CreateDownloader(4);
        downloader.DownloadFileCompleted += (s, e) =>
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(fileName);
            var newName = Path.Combine(txtSaveDirectory.Text.Trim(),
                Path.GetFileNameWithoutExtension(downloadUri) + $"-{versionInfo.FileVersion}.exe");
            File.Move(fileName, newName, true);
            ShowMessage($"Unity Hub {versionInfo.FileVersion}下载完成");
        };
        downloader.DownloadStarted += (s, e) => { ShowMessage($"开始下载 {cbxHub.Text.Trim()} Unity Hub,请稍等"); };
        _ = downloader.DownloadFileTaskAsync(downloadUri, fileName);
    }

    private void btnManualUpdate_Click(object sender, EventArgs e)
    {
        const string jsResourceFileName = "fetch.js";
        if (File.Exists(jsResourceFileName))
        {
            Clipboard.SetText(File.ReadAllText(jsResourceFileName));
            ShowMessage("脚本代码已拷贝至剪贴板");
        }
        else
        {
            ShowMessage("${jsResourceFileName}脚本文件不存在");
        }
    }
}