using Terminal.Gui;
using Yomurai;
using Yomurai.Scrapers;

static void UIMain()
{
    Application.Init();
    var top = Application.Top;

    var win = new Window()
    {
        X = 0,
        Y = 1,
        Width = Dim.Fill(),
        Height = Dim.Fill()
    };

    var menu = new MenuBar(new MenuBarItem[]
        {
            new MenuBarItem("_File", new[]
            {
                new MenuItem("Loaded scrapers", "",
                    () =>
                    {
                        MessageBox.Query("Scrapers",
                            string.Join("\n", (from x in Shared.Scrapers select x.ScraperName)), "OK");
                    }),
                new MenuItem("_Quit", "", () =>
                {
                    var result = MessageBox.Query("Warning", "Are you sure to quit?", "Yes", "No");
                    if (result == 0)
                    {
                        //top.Running = false;
                        top.RequestStop();
                    }
                })
            }),
            new MenuBarItem("_Novel", new[]
            {
                new MenuItem("Download from url", "Auto detect scraper", () =>
                {
                    var button_OK = new Button("OK");
                    var button_Cancel = new Button("Cancel");
                    var dialog = new Dialog("Enter url", 50, 6, button_OK, button_Cancel);
                    var textField_Url = new TextField()
                    {
                        X = 0,
                        Y = 0,
                        Width = Dim.Fill()
                    };
                    var pbar = new ProgressBar()
                    {
                        X = 0,
                        Y = 1,
                        Width = Dim.Fill()
                    };

                    button_OK.Clicked += () =>
                    {
                        //pbar.Pulse();
                        var task = new Task(() =>
                        {
                            WebUtils.DownloadNovel(new Url(textField_Url.Text.ToString()));
                            Application.MainLoop.Invoke(() => { MessageBox.Query("Info", "Done", "OK"); });
                        });
                        task.Start();
                        //MessageBox.Query("Info", "Done", "OK");
                        //win.Remove(dialog);
                        //dialog.Dispose();
                    };
                    button_Cancel.Clicked += () =>
                    {
                        win.Remove(dialog);
                        dialog.Dispose();
                    };

                    dialog.Add(textField_Url, pbar);

                    win.Add(dialog);
                })
            })
        }
    );
    top.Add(menu, win);

    Application.Run();
}

//Console.WriteLine(new Uri("https://www.baidu.com/a.html").AbsolutePath);
Utils.Init();
var doc = WebUtils.GetDocumentFromUrl(new Url("https://www.linovelib.com/novel/2507.html"));
var scraper = new Linovelib();
var tocPage = WebUtils.GetDocumentFromUrl(scraper.GetTocPageUrl(doc));
var toc = scraper.GetTableOfContent(tocPage);
Console.ReadLine();