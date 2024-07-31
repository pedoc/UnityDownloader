using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using PuppeteerSharp;

// ReSharper disable AsyncApostle.ConfigureAwaitHighlighting

#pragma warning disable SYSLIB0014

namespace UnityDownloader
{
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
                memTxt.Text += msg + Environment.NewLine;
            }
        }

        const string EditorJSONFile = "editor.json";

        private void DownloadEditorJsonAsync(Action<AsyncCompletedEventArgs> downloadCompletedHandler)
        {
            var url = txtEditorJson.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                ShowMessage("编辑器资源不能为空");
                return;
            }

            DownloadFileAsync(url, EditorJSONFile, args => { ShowMessage($"编辑器资源下载进度：{args.ProgressPercentage}%"); },
                downloadCompletedHandler);
        }

        private async Task<bool> GenerateEditorJsonFileAsync()
        {
            const string url = "https://unity3d.com/get-unity/download/archive";
            // var html = DownloadContent(url);
            // if (string.IsNullOrEmpty(html))
            // {
            //     ShowMessage("拉取u3d版本列表失败," + url);
            //     return false;
            // }

            var browserFetcher = new BrowserFetcher()
            {
                WebProxy = new WebProxy(txtProxy.Text.Trim())
            };
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true,
                    IgnoreHTTPSErrors = true,
                    Args = new[]
                    {
                        "--proxy-server=\"" + txtProxy.Text.Trim() + "\""
                    }
                });
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync(url, new NavigationOptions()
            {
                WaitUntil = [WaitUntilNavigation.DOMContentLoaded]
            });
            var elements = await page.QuerySelectorAllAsync(
                "body > div.flex.min-h-screen.flex-col > main > div.relative.flex.flex-wrap.justify-center.gap-2.p-2 >button ");
            var dict = new Dictionary<string, object>();
            foreach (var elementHandle in elements)
            {
                await elementHandle.ClickAsync();

                var downloadElements = await page.QuerySelectorAllAsync("a[href^=\"unityhub://\"]");
                foreach (var downloadElement in downloadElements)
                {
                    var href = await downloadElement.GetPropertyAsync("href");
                    if (href == null) continue;
                    var hrefString = href.ToString();
                    if (string.IsNullOrEmpty(hrefString)) continue;
                    var match = Regex.Match(hrefString, "unityhub://(?<version>.*)/(?<hash>.*)", RegexOptions.Compiled,
                        TimeSpan.FromSeconds(3));
                    if (match.Success)
                    {
                        var version = match.Groups["version"].Value;
                        var hash = match.Groups["hash"].Value;

                        dict[version] = new
                        {
                            linux =
                                $"https://download.unity3d.com/download_unity/{hash}/LinuxEditorInstaller/Unity.tar.xz",
                            mac =
                                $"https://download.unity3d.com/download_unity/{hash}/MacEditorInstaller/Unity-{version}.pkg",
                            macArm64 =
                                $"https://download.unity3d.com/download_unity/{hash}/MacEditorInstallerArm64/Unity-{version}.pkg",
                            win64 =
                                $"https://download.unity3d.com/download_unity/{hash}/Windows64EditorInstaller/UnitySetup64-{version}.exe"
                        };
                    }
                }
            }

            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            await File.WriteAllTextAsync(EditorJSONFile, json);

            await page.CloseAsync();
            await browser.CloseAsync();

            return true;
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
                var editor = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
                var editorItems = new List<EditorItem>();
                foreach (var (version, items) in editor)
                {
                    var editorItem = new EditorItem()
                    {
                        Version = version,
                        Linux = new List<EditorComponent>(),
                        Mac = new List<EditorComponent>(),
                        MacArm64 = new List<EditorComponent>(),
                        Win64 = new List<EditorComponent>(),
                    };
                    foreach (var (plat, editorUrl) in items)
                    {
                        switch (plat.ToLower())
                        {
                            case "linux":
                                editorItem.Linux.Add(new EditorComponent()
                                {
                                    Name = "UnityEditor",
                                    DownloadUrl = editorUrl,
                                });
                                break;
                            case "mac":
                                editorItem.Mac.Add(new EditorComponent()
                                {
                                    Name = "UnityEditor",
                                    DownloadUrl = editorUrl,
                                });
                                break;
                            case "macarm64":
                                editorItem.MacArm64.Add(new EditorComponent()
                                {
                                    Name = "UnityEditor",
                                    DownloadUrl = editorUrl,
                                });
                                break;
                            case "win64":
                                editorItem.Win64.Add(new EditorComponent()
                                {
                                    Name = "UnityEditor",
                                    DownloadUrl = editorUrl,
                                });
                                break;
                            default: continue;
                        }
                    }

                    var url = editorItem.Win64.First().DownloadUrl;
                    editorItem.Hash = url.Replace("https://download.unity3d.com/download_unity/", "").Substring(0, 12);
                    editorItem.FillWin64EditorComponent();

                    editorItems.Add(editorItem);
                }

                gridView.OptionsBehavior.ReadOnly = true;
                gridView.OptionsBehavior.Editable = false;

                gridView.OptionsSelection.MultiSelect = true;
                gridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;

                gridControl.DataSource = editorItems;
                gridView.Columns[nameof(EditorItem.VersionMajor)].Group();
                //gridView.Columns[nameof(EditorItem.VersionMinor)].Group();
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }

        private string DownloadContent(
            string url)
        {
            try
            {
                using WebClient wb = new WebClient();
                if (!string.IsNullOrEmpty(txtProxy.Text))
                {
                    wb.Proxy = new WebProxy(txtProxy.Text);
                }

                return wb.DownloadString(new Uri(url));
            }
            catch (Exception e)
            {
                ShowMessage($"下载 {url} 时出错,原因:{e.Message}");
                return string.Empty;
            }
        }

        private void DownloadFileAsync(
            string url,
            string path,
            Action<DownloadProgressChangedEventArgs> progressHandler,
            Action<AsyncCompletedEventArgs> downloadCompletedHandler)
        {
            try
            {
                using WebClient wb = new WebClient();
                if (!string.IsNullOrEmpty(txtProxy.Text))
                {
                    wb.Proxy = new WebProxy(txtProxy.Text);
                }

                wb.DownloadProgressChanged += (sender, args) =>
                {
                    //ShowMessage($"下载进度：{args.ProgressPercentage}%");
                    progressHandler?.Invoke(args);
                };
                wb.DownloadFileCompleted += (sender, args) =>
                {
                    //ShowMessage("下载完成");
                    downloadCompletedHandler?.Invoke(args);
                };

                wb.DownloadFileAsync(new Uri(url), path);
            }
            catch (Exception e)
            {
                ShowMessage($"下载 {url} 时出错,原因:{e.Message}");
            }
        }


        private async void btnDownloadEditorJson_Click(object sender, EventArgs e)
        {
            // DownloadEditorJsonAsync(_ =>
            // {
            //     TryLoadEditorJson();
            //     ShowMessage("编辑器资源下载成功并完成加载");
            // });

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

        private void MainForm_Load(object sender, EventArgs e)
        {
            TryLoadEditorJson();
        }

        private async void btnDownloadEditor_Click(object sender, EventArgs e)
        {
            btnDownloadEditor.Enabled = false;

            if (!int.TryParse(txtDownloadCount.Text, out var num))
                num = 4;

            var view = gridControl.FocusedView as GridView;
            if (view == null) return;

            var rows = view.GetSelectedRows();
            if (rows.Length <= 0)
            {
                ShowMessage("请先选择要下载的项");
                return;
            }

            ShowMessage($"当前选中了 {rows.Length} 项准备下载");
            Parallel.ForEachAsync(rows, new ParallelOptions()
            {
                MaxDegreeOfParallelism = num
            }, async (i, ct) =>
            {
                var editorComponent = view.GetRow(i) as EditorComponent;
                if (editorComponent == null) return;

                var directory = Path.Combine(txtSaveDirectory.Text, editorComponent.Version);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var path = Path.Combine(directory, editorComponent.FileName);
                var mre = new ManualResetEvent(false);
                var sw = Stopwatch.StartNew();
                ShowMessage($"开始从 {editorComponent.DownloadUrl} 下载到 {path}");
                DownloadFileAsync(editorComponent.DownloadUrl, path, dpce =>
                {
                    editorComponent.DownloadProgress = $"{dpce.ProgressPercentage}%,{sw.Elapsed}";
                    view.RefreshRow(i);
                }, ace =>
                {
                    sw.Stop();
                    //editorComponent.DownloadProgress = $"下载完成,{sw.Elapsed}";
                    mre.Set();
                });
                mre.WaitOne();
            }).ContinueWith((t) => { Invoke(() => { btnDownloadEditor.Enabled = true; }); });
        }
    }
}