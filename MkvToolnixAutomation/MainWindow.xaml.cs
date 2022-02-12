using CliWrap;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using MessageBox = HandyControl.Controls.MessageBox;

namespace MkvToolnixAutomation
{
    public partial class MainWindow
    {
        const string MKVMerge_X64_PATH = @"C:\Program Files\MKVToolNix\mkvmerge.exe";
        const string MKVMerge_X86_PATH = @"C:\Program Files (x86)\MKVToolNix\mkvmerge.exe";
        string JSON_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "JSONOptions");

        Dictionary<int, string> mkvDic = new Dictionary<int, string>();
        Dictionary<int, string> subDic = new Dictionary<int, string>();
        Dictionary<int, string> subDic2 = new Dictionary<int, string>();
        Dictionary<int, string> subDic3 = new Dictionary<int, string>();

        public MainWindow()
        {
            InitializeComponent();

            CheckMkvMergeExist(GetMKVMergeLocation());

            if (Directory.Exists(JSON_PATH))
            {
                Directory.Delete(JSON_PATH, true);
            }
            Directory.CreateDirectory(JSON_PATH);
        }

        public const string FileNameRegex = @"(?:(?:720|480|1080)[pi]?|S[0123456789][0123456789]E|19[0123456789][0123456789]|20[0123456789][0123456789]|S[0123456789]E|20(?:1\d|2[01])|\/|\.)";
        public string RemoveSpecialWords(string stringToClean)
        {
            Regex wordFilter = new Regex(FileNameRegex, RegexOptions.IgnoreCase);
            var cleaned = Regex.Replace(stringToClean, @"(\[[^\]]*\])|(\([^\)]*\))", ""); // remove between () and []

            cleaned = wordFilter.Replace(cleaned, " ").Trim();
            cleaned = Regex.Replace(cleaned, "[ ]{2,}", " "); // remove space [More than 2 space] and replace with one space

            cleaned = Regex.Match(cleaned, @"\d+").Value;

            return cleaned.Trim();
        }

        #region Check MKVMerge Location
        private void CheckMkvMergeExist(string location)
        {
            if (!string.IsNullOrEmpty(location))
            {
                brdMkvLocation.Style = ResourceHelper.GetResource<Style>("BorderTipSuccess");
                txtMkvToolnixLocation.Text = location;
                btnSelectMKVToolnixFolder.IsEnabled = false;
            }
            else
            {
                brdMkvLocation.Style = ResourceHelper.GetResource<Style>("BorderTipDanger");
                txtMkvToolnixLocation.Text = "MKVMerge.exe is not found, please Install or Select Installation Folder";
            }
        }
        private string GetMKVMergeLocation()
        {
            if (File.Exists(MKVMerge_X64_PATH))
            {
                return MKVMerge_X64_PATH;
            }
            else if (File.Exists(MKVMerge_X86_PATH))
            {
                return MKVMerge_X86_PATH;
            }
            else
            {
                return null;
            }
        }
        private string GetMKVToolnixLocation(string folder)
        {
            var file = Path.Combine(folder, "mkvmerge.exe");
            if (File.Exists(file))
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        private void btnSelectMKVToolnixFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select MKVToolnix Installation Folder",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CheckMkvMergeExist(GetMKVToolnixLocation(dialog.SelectedPath));
            }
        }
        #endregion

        #region Select Folders
        private void btnLoadOptionFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "MkvToolnix Option files (*.json)|*.json";
            if (fileDialog.ShowDialog() == true)
            {
                txtMkvToolnixOptionFile.Text = fileDialog.FileName;
            }
        }

        private void btnAnimeFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select Anime Folder",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtAnimeFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnSubtitlesFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select Subtitles Folder",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSubtitlesFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnSubtitlesFolderSecond_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select Subtitles Folder",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSubtitlesFolderSecond.Text = dialog.SelectedPath;
            }
        }

        private void btnSubtitlesFolderThird_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select Subtitles Folder",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSubtitlesFolderThird.Text = dialog.SelectedPath;
            }
        }
        #endregion

        #region Get Anime and Subtitles
        private void GetSubtitles()
        {
            if (string.IsNullOrEmpty(txtSubtitlesFolder.Text))
            {
                MessageBox.Error("Subtitles folder is not set, please select subtitle folder");
                return;
            }
            var subs = Directory.GetFiles(txtSubtitlesFolder.Text);
            if (subs != null)
            {
                foreach (var item in subs)
                {
                    var info = new FileInfo(item).Name;
                    var key = Convert.ToInt32(RemoveSpecialWords(info));
                    subDic.Add(key, info);
                }
            }

            if (!string.IsNullOrEmpty(txtSubtitlesFolderSecond.Text))
            {
                var subs2 = Directory.GetFiles(txtSubtitlesFolderSecond.Text);
                if (subs2 != null)
                {
                    foreach (var item in subs2)
                    {
                        var info = new FileInfo(item).Name;
                        var key = Convert.ToInt32(RemoveSpecialWords(info));
                        subDic2.Add(key, info);
                    }
                }
            }

            if (!string.IsNullOrEmpty(txtSubtitlesFolderThird.Text))
            {
                var subs3 = Directory.GetFiles(txtSubtitlesFolderThird.Text);
                if (subs3 != null)
                {
                    foreach (var item in subs3)
                    {
                        var info = new FileInfo(item).Name;
                        var key = Convert.ToInt32(RemoveSpecialWords(info));
                        subDic3.Add(key, info);
                    }
                }
            }
        }

        private void GetAnimeFiles()
        {
            if (string.IsNullOrEmpty(txtAnimeFolder.Text))
            {
                MessageBox.Error("Anime folder is not set, please select Anime folder");
                return;
            }
            var mkvs = Directory.GetFiles(txtAnimeFolder.Text);
            foreach (var item in mkvs)
            {
                var info = new FileInfo(item).Name;
                var key = Convert.ToInt32(RemoveSpecialWords(info));
                mkvDic.Add(key, info);
            }
        }

        private void btnGetAnimeSubList_Click(object sender, RoutedEventArgs e)
        {
            GetSubtitles();
            GetAnimeFiles();

            txtStatus.Text = $"Anime: {mkvDic.Count} files loaded\nSubtitle: {subDic.Count} files loaded";
            btnGenerateJson.IsEnabled = true;
        }
        #endregion

        private void btnGenerateJson_Click(object sender, RoutedEventArgs e)
        {
            GenerateJson();
        }

        private async void GenerateJson()
        {
            try
            {
                prgTotal.Maximum = mkvDic.Count;
                int current = 0;
                foreach (var item in mkvDic)
                {
                    current++;
                    prgTotal.Value = current;
                    var template = await File.ReadAllTextAsync(txtMkvToolnixOptionFile.Text);
                    template = template.Replace("{XOUTNUMBERX}", item.Key.ToString("000"));
                    template = template.Replace("{XFILENAMEX}", item.Value);
                    var subName = subDic.Where(x => x.Key == item.Key).FirstOrDefault().Value;
                    var subName2 = subDic2.Where(x => x.Key == item.Key).FirstOrDefault().Value;
                    var subName3 = subDic3.Where(x => x.Key == item.Key).FirstOrDefault().Value;

                    template = template.Replace("{XSUBFILENAMEX}", subName);
                    template = template.Replace("{XSUBFILENAME2X}", subName2);
                    template = template.Replace("{XSUBFILENAME3X}", subName3);

                    await File.WriteAllTextAsync(@$"{JSON_PATH}\{item.Key}.json", template);
                }
                MessageBox.Success("All MkvToolnix option files Generated Successfully");
                btnMergeMkv.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message);
            }
        }
        private async void btnMergeMkv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsons = Directory.GetFiles(JSON_PATH);
                prgMerge.Maximum = jsons.Length;
                int current = 0;
                foreach (var json in jsons)
                {
                    current++;
                    prgMerge.Value = current;
                    var item = new FileInfo(json);
                    var result = await Cli.Wrap(txtMkvToolnixLocation.Text)
                        .WithArguments(@$"@{item.FullName}")
                        .ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message);
            }
        }

        
    }
}
