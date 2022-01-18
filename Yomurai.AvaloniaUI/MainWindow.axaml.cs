using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Yomurai.Legado;

namespace Yomurai.AvaloniaUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private ListBox ScraperListBox = null;
        private ListBox DownloadedListBox = null;
        private TextBox UrlTextBox = null;
        private ProgressBar DownloadPBar = null;

        private void FindControls()
        {
            ScraperListBox = this.FindControl<ListBox>("ScraperListBox");
            DownloadedListBox = this.FindControl<ListBox>("DownloadedListBox");
            UrlTextBox = this.FindControl<TextBox>("UrlTextBox");
            DownloadPBar = this.FindControl<ProgressBar>("DownloadPBar");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            FindControls();

            var legadoNames = from x in BookSource.GetSourceNames() select x + " (Legado)";
            ScraperListBox.Items = (from x in Shared.Scrapers select x.ScraperName).Concat(legadoNames);
            DownloadedListBox.Items = (from x in NovelUtils.GetDownloadedNovelInfos() select x.Title);
        }
        
        private void DownloadNovel_Click(object sender, RoutedEventArgs e)
        {
            var downloader = new NovelDownloader();
            downloader.UpdateProgress += (value, maximum) =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (value == -1 || maximum == -1)
                    {
                        DownloadPBar.IsIndeterminate = true;
                    }
                    else
                    {
                        if (DownloadPBar.IsIndeterminate)
                        {
                            DownloadPBar.IsIndeterminate = false;
                        }

                        if (maximum != DownloadPBar.Maximum)
                        {
                            DownloadPBar.Maximum = maximum;
                        }
                        DownloadPBar.Value = value;
                    }
                });
            };
            new Task(() => downloader.DownloadNovel(new Url(UrlTextBox.Text))).Start();
        }
    }
}