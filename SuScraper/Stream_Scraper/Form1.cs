using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using Bunifu.Framework.UI;
using System.Diagnostics;
using HtmlAgilityPack;
using WordPressSharp;
using WordPressSharp.Models;
using Renci.SshNet;
using MySql.Data.MySqlClient;

namespace Stream_Scraper
{
    public partial class Form1 : Form
    {
        List<Bunifu.Framework.UI.BunifuFlatButton> mainButtons = new List<Bunifu.Framework.UI.BunifuFlatButton>();
        List<BunifuCheckbox> ListChecks = new List<BunifuCheckbox>();
        private void hideAll()
        {
            foreach (BunifuCheckbox item in ListChecks)
                item.Checked = false;
        }
        
        public Form1()
        {
            InitializeComponent();
            mainButtons.Add(btnDatabase);
            mainButtons.Add(btnScrape);
            mainButtons.Add(btnSearch);
            mainButtons.Add(btnUi);
            btnDatabase.Click += refreshBtn;
            btnScrape.Click += refreshBtn;
            btnSearch.Click += refreshBtn;
            btnUi.Click += refreshBtn;
            btnDatabase.Click += movePanel;
            btnScrape.Click += movePanel;
            btnSearch.Click += movePanel;
            btnUi.Click += movePanel;
            refreshBtn(btnScrape, new EventArgs());

            //-----------
            ListChecks.Add(CheckBoxMovies);
            ListChecks.Add(TVShowsCheckBox);
            ListChecks.Add(AnimeCheckBox);

            //---------------------
            BotTypes.SelectedIndex = 0;

            Console.WriteLine(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss"));
           

        }
        private void refreshBtn(object sender, EventArgs e)
        {
            foreach(Bunifu.Framework.UI.BunifuFlatButton btn in mainButtons)
            {
                if (btn.Equals((Bunifu.Framework.UI.BunifuFlatButton)sender))
                { btn.Normalcolor = Color.FromArgb(0,0,0);
                  btn.OnHovercolor = Color.FromArgb(0, 0, 0);

                }
                else
                {
                    btn.Normalcolor = Color.FromArgb(0, 122, 204);
                    btn.OnHovercolor = Color.FromArgb(0, 180, 200);
                }
            }
        }
        private void movePanel(object sender, EventArgs e)
        {

            panelSlide.Location = new Point(panelSlide.Location.X, ((Bunifu.Framework.UI.BunifuFlatButton)sender).Location.Y);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        public StopWatch Exit = new StopWatch();
        private  void bunifuFlatButton4_Click(object sender, EventArgs e)
        {
            StartScrapingButton.Enabled = false;
            StopScrapingButton.Enabled = true;
            ScrapingProgress.Visible = true;
            ScrapingProgress.animated = true;
            Exit.Stop = false;


            //Task.Run(() => _0123MoviesAPI.getMovies(StartScrapingButton, StopScrapingButton, ScrapingProgress, Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://www9.0123movies.com/list/movies.html"));
            //Task.Run(() => SolarAPI.getMovies(StartScrapingButton, StopScrapingButton, ScrapingProgress,Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://www3.solarmovie.one/movie/"));
            //NotCompleted//Task.Run(() => _0123MoviesAPI.getTVShows(StartScrapingButton, StopScrapingButton, ScrapingProgress, Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://www9.0123movies.com/list/tv-series.html"));
            //Task.Run(() => ryuanime.getAnimes(Preview,StartScrapingButton, StopScrapingButton, ScrapingProgress,Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://www2.gogoanime.io/anime-list.html"));


            Task.Run(() => libertyvf.getAllMovies(UpdateCheck,Preview, StartScrapingButton, StopScrapingButton, ScrapingProgress, Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://libertyvf.co/films/","movies_fr"));
            //Task.Run(() => libertyvf.getAllTV(UpdateCheck, Preview, StartScrapingButton, StopScrapingButton, ScrapingProgress, Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://libertyvf.co/series/", "tv_fr","seasons_fr", "episodes_fr"));
            //Task.Run(() => frenchStream.getAllMovies(UpdateCheck,Preview, StartScrapingButton, StopScrapingButton, ScrapingProgress, Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://french-film.co/film-streaming/", "movies_fr"));
            //Task.Run(() => frenchStream.getAllTV(UpdateCheck, Preview, StartScrapingButton, StopScrapingButton, ScrapingProgress, Exit, ScrapeDataGrid, TotalPagesScrapeLabel, ScrapedLabel, "https://french-serie.co/serie-streaming/serie-en-vf-streaming", "tv_fr","seasons_fr", "episodes_fr"));
            




        }

        private void bunifuFlatButton5_Click(object sender, EventArgs e)
        {
            StopScrapingButton.Enabled = false;
            StartScrapingButton.Enabled = true;
            ScrapingProgress.Visible = false;
            ScrapingProgress.animated = false;
            Exit.Stop = true;
        }
        public class StopWatch{
            public bool Stop { get; set; }
            public StopWatch()
            {
                Stop = false;
            }
        }

        private void CheckBoxMovies_Click(object sender, EventArgs e)
        {
            hideAll();
            ((BunifuCheckbox)sender).Checked = true;
        }
        private static string GetInstanceName()
        {
            string returnvalue = "not found";
            //Checks bandwidth usage for CUPC.exe..Change it with your application Name
            string applicationName = "CUPC";
            PerformanceCounterCategory[] Array = PerformanceCounterCategory.GetCategories();
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i].CategoryName.Contains(".NET CLR Networking"))
                    foreach (var item in Array[i].GetInstanceNames())
                    {
                        if (item.ToLower().Contains(applicationName.ToString().ToLower()))
                            returnvalue = item;

                    }

            }
            return returnvalue;
        }
        private void BandwidthMonitor_Tick(object sender, EventArgs e)
        {
            var bytesSentPerformanceCounter = new PerformanceCounter();
            bytesSentPerformanceCounter.CategoryName = ".NET CLR Networking";
            bytesSentPerformanceCounter.CounterName = "Bytes Sent";
            bytesSentPerformanceCounter.InstanceName = GetInstanceName();
            bytesSentPerformanceCounter.ReadOnly = true;

            var bytesReceivedPerformanceCounter = new PerformanceCounter();
            bytesReceivedPerformanceCounter.CategoryName = ".NET CLR Networking";
            bytesReceivedPerformanceCounter.CounterName = "Bytes Received";
            bytesReceivedPerformanceCounter.InstanceName = GetInstanceName();
            bytesReceivedPerformanceCounter.ReadOnly = true;

            //UpBandwidth.Text = ((int)(bytesSentPerformanceCounter.RawValue / 1024)).ToString();
            // DownBandwidth.Text = ((int)(bytesReceivedPerformanceCounter.RawValue / 1024)).ToString();
            
        }

        private void ScrapeDataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            TotalGridLabel.Text = (Int64.Parse(TotalGridLabel.Text) + 1).ToString();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (((Bunifu.Framework.UI.BunifuFlatButton)sender).Tag.ToString())
            {
                case "Bot":
                    BotPanel.BringToFront();
                    break;
                case "Scrape":
                    ScrapePanel.BringToFront();
                    break;
            }
        }

        private void StartBotBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(URLtemplate.Text))
            {
                MessageBox.Show("Empty Url");
                return;
            }
            if (string.IsNullOrEmpty(USRtemplate.Text))
            {
                MessageBox.Show("Empty Username");
                return;
            }
            if (string.IsNullOrEmpty(PWDtemplate.Text))
            {
                MessageBox.Show("Empty Password");
                return;
            }
            WordPressClient client = AutoPoster.WordpressCL(URLtemplate.Text, USRtemplate.Text, PWDtemplate.Text);
            if (client == null)
            {
                MessageBox.Show("Error connecting");
                return;
            }
            if(BotTypes.SelectedIndex == 0)
            {
                MessageBox.Show("No type was selected");
                return;
            }
            if (!Information.EstablishExternalConnection())
                return;
            if(BotTypes.SelectedIndex == 1)
                // movies bot
                Task.Run(() => AutoPoster.BotMovies(client,"movies","movies_fr"));
            else
                // tvshows bot
                Task.Run(() => AutoPoster.BotEpisodes(client, "episodes", "episodes_fr", "seasons_fr", "tv_fr"));
        }
    }
}
