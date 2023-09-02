using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

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
                ShowMessage("�༭����Դ����Ϊ��");
                return;
            }

            DownloadFileAsync(url, EditorJSONFile, args =>
            {
                ShowMessage($"�༭����Դ���ؽ��ȣ�{args.ProgressPercentage}%");
            }, downloadCompletedHandler);
        }

        public void TryLoadEditorJson()
        {
            if (!File.Exists(EditorJSONFile))
            {
                ShowMessage("���ȸ��±༭����Դ");
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
                    //ShowMessage($"���ؽ��ȣ�{args.ProgressPercentage}%");
                    progressHandler?.Invoke(args);
                };
                wb.DownloadFileCompleted += (sender, args) =>
                {
                    //ShowMessage("�������");
                    downloadCompletedHandler?.Invoke(args);
                };

                wb.DownloadFileAsync(new Uri(url), path);
            }
            catch (Exception e)
            {
                ShowMessage($"���� {url} ʱ����,ԭ��:{e.Message}");
            }
        }

        private void DownloadFile(
            string url,
            string path)
        {
            using WebClient wb = new WebClient();
            if (!string.IsNullOrEmpty(txtProxy.Text))
            {
                wb.Proxy = new WebProxy(txtProxy.Text);
            }
            wb.DownloadFile(new Uri(url), path);
        }

        private void btnDownloadEditorJson_Click(object sender, EventArgs e)
        {
            DownloadEditorJsonAsync(_ =>
            {
                TryLoadEditorJson();
                ShowMessage("�༭����Դ���سɹ�����ɼ���");
            });
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
                ShowMessage("����ѡ��Ҫ���ص���");
                return;
            }
            ShowMessage($"��ǰѡ���� {rows.Length} ��׼������");
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
                ShowMessage($"��ʼ�� {editorComponent.DownloadUrl} ���ص� {path}");
                DownloadFileAsync(editorComponent.DownloadUrl, path, dpce =>
                {
                    editorComponent.DownloadProgress = $"{dpce.ProgressPercentage}%,{sw.Elapsed}";
                    view.RefreshRow(i);

                }, ace =>
                {
                    sw.Stop();
                    editorComponent.DownloadProgress = $"�������,{sw.Elapsed}";
                    mre.Set();
                });
                mre.WaitOne();
            }).ContinueWith((t) =>
            {
                Invoke(() =>
                {
                    btnDownloadEditor.Enabled = true;
                });
            });
        }
    }
}