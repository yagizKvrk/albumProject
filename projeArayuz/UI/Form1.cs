using Guna.UI2.Designer;
using NAudio.Wave;
using Newtonsoft.Json;
using projeArayuz.Data;
using projeArayuz.Migrations;
using projeArayuz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace projeArayuz
{
    public partial class Form1 : Form
    {
        #region DatabasesAndLists
        MusicShopDbContext db = new MusicShopDbContext();
        List<Datum> tracksOnTheAlbum = new List<Datum>();
        List<Album> clickedAlbums = new List<Album>();
        List<Contributor> clickedArtists = new List<Contributor>();
        string Username = "";
        Manager sessionManager = new Manager();
        static Random random = new Random();
        int rand_code1 = random.Next(10000, 99999);
        #endregion

        #region InitializingAndClosing
        public Form1(string username)
        {
            InitializeComponent();
            Username = username;
            sessionManager = db.Managers.Where(x => x.ManagerName == Username || x.MailAddress == username).FirstOrDefault();
            dgvTracks.Visible = false;
            pnlAlbumInfo.Visible = false;
            pnlManagerPanel.Visible = false;
            tbcShopInfo.Visible = false;
            pnlManagerShopInfos.Visible = false;
            if (tgsStock.Checked)
            {
                lblStock.Text = "In Stock";
            }
            else
            {
                lblStock.Text = "Out of Stock";
            }

            if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour < 12)
            {
                lblWelcome2.Text = "Good Morning " + sessionManager.ManagerName;
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                lblWelcome2.Text = "Good Afternoon " + sessionManager.ManagerName;
            }
            else if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour < 22)
            {
                lblWelcome2.Text = "Good Evening " + sessionManager.ManagerName;
            }
            else if (DateTime.Now.Hour >= 22 && DateTime.Now.Hour < 5)
            {
                lblWelcome2.Text = "Good Night " + sessionManager.ManagerName;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //string time = DateTime.Now.GetHashCode().ToString();//DateTime.Now.ToString("yyyy-MM-dd");
            //string json = JsonConvert.SerializeObject(db.Albums);
            //File.WriteAllText(time, json);

            db.Database.ExecuteSqlCommand("Truncate TABLE [Data]");
            db.Database.ExecuteSqlCommand("DELETE FROM Artists\r\nDBCC CHECKIDENT ('Artists', RESEED, 0)");
            db.Database.ExecuteSqlCommand("DELETE FROM Albums\r\nDBCC CHECKIDENT ('Albums', RESEED, 0)");
            db.SaveChanges();
        }
        private void cbClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region HomePageEvents
        private void btnHomePage_Click(object sender, EventArgs e)
        {
            ManagerPageReset();
            lstAlbums.Visible = true;
            pboHomePage.Visible = true;
            pnlAlbumInfo.Visible = false;
            pnlSearch.Visible = true;
            dgvTracks.Visible = false;
            tbcShopInfo.Visible = false;
            pnlManagerPanel.Visible = false;
            pnlManagerShopInfos.Visible = false;
        }
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            lstAlbums.Items.Clear();
            List<string> url = new List<string>();
            List<string> album1 = new List<string>();
            List<string> albumID = new List<string>();

            int sayi1 = db.Tracks.Count();
            string artist = txtArtist.Text.Trim();
            string track = txtTrack.Text.Trim();
            string album = txtAlbum.Text.Trim();

            await FindTrack(artist, track, album);

            var includedTable = db.Tracks.Where(x => x.DatumID > sayi1).Include(x => x.artist).Include(x => x.album).ToList();

            string picture = "";
            foreach (var item in includedTable)
            {
                if (!url.Contains(item.album.cover_medium))
                {
                    string x = $"{item.album.title}\r\n{item.artist.name}";
                    url.Add(item.album.cover_medium);
                    album1.Add(x);
                    albumID.Add(item.album.id.ToString());
                    if (picture == "")
                    {
                        picture = item.artist.picture_medium;
                        cpboArtist.Load(picture);
                    }
                }
            }
            ImageList img = new ImageList();
            img.ImageSize = new Size(150, 150);
            img.ColorDepth = ColorDepth.Depth32Bit;
            for (int i = 0; i < url.Count; i++)
            {
                WebClient wc = new WebClient();
                byte[] imageByte = wc.DownloadData(url[i]);
                MemoryStream stream = new MemoryStream(imageByte);
                Image im = Image.FromStream(stream);
                img.Images.Add(im);
                lstAlbums.Items.Add(albumID[i], album1[i], i);
            }
            lstAlbums.LargeImageList = img;
        }
        private async void lstAlbum_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tracksOnTheAlbum.Clear();
            lstAlbums.Focus();
            string d = lstAlbums.FocusedItem.Name;
            await TrackInfos(d);
            await AlbumInfos(d);
            var focusedItem = lstAlbums.FocusedItem;
            if (focusedItem.Bounds.Contains(e.Location))
            {
                lstAlbums.Visible = false;
                pboHomePage.Visible = false;
                pnlAlbumInfo.Visible = true;
                pnlSearch.Visible = false;
                dgvTracks.Visible = true;
                btnHomePage.Checked = false;
                dgvTracks.Rows.Clear();
                foreach (var item in tracksOnTheAlbum)
                {
                    dgvTracks.Rows.Add(item.DatumID, item.title, item.duration);

                }
                foreach (var item in clickedAlbums)
                {
                    pboAlbumCover.Load(item.cover_medium);
                    lblAlbumTitle.Text = item.title;
                    lblAlbumReleaseDate.Text = item.release_date;
                }
                foreach (var item in clickedArtists)
                {
                    lblArtistName.Text = item.name;
                }
            }
        }
        private void dgvTracks_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int sarki = (int)dgvTracks.SelectedRows[0].Cells[0].Value;
            Datum a = tracksOnTheAlbum.Where(x => x.DatumID == sarki).FirstOrDefault();
            var url = a.preview;
            using (var mf = new MediaFoundationReader(url))
            using (var wo = new WasapiOut())
            {
                wo.Init(mf);
                wo.Play();
                while (wo.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }
        private void btnAddAlbum_Click(object sender, EventArgs e)
        {
            chkManualAdd.Checked = false;
            FillTheLists();
            tbcShopInfo.Visible = true;
            pnlAlbumInfo.Visible = false;
            pnlManagerPanel.Visible = true;
            foreach (var item in clickedAlbums)
            {
                pboPosterOnManagerPanel.Load(item.cover_medium);
                txtPoster.Text = item.cover_medium;
                txtAlbumTitle.Text = item.title;
                txtReleaseDate.Text = item.release_date;

                txtPoster.ReadOnly = true;
                txtAlbumTitle.ReadOnly = true;
                txtReleaseDate.ReadOnly = true;
            }
            foreach (var item in clickedArtists)
            {
                txtArtistManager.Text = item.name;
                txtArtistManager.ReadOnly = true;
            }

        }
        private void updateUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendVerification1(sessionManager.MailAddress);

            Form3 frm3 = new Form3(rand_code1, sessionManager);
            frm3.Show();
            this.Close();
        }
        private void deleteUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            db.Managers.Remove(sessionManager);
            db.SaveChanges();
            Form2 frm = new Form2();
            frm.Show();
            this.Close();
        }
        #endregion

        #region ManagerPageEvents
        private void btnShopManagement_Click(object sender, EventArgs e)
        {
            FillTheLists();
            pnlManagerPanel.Visible = true;
            tbcShopInfo.Visible = true;
            chkManualAdd.Checked = true;
            pboHomePage.Visible = false;
            pnlManagerShopInfos.Visible = true;
        }
        private void btnSellAlbum_Click(object sender, EventArgs e)
        {
            pnlManagerShopInfos.Visible = true;
            if (btnSellAlbum.Text == "Sell Album")
            {
                if (string.IsNullOrEmpty(txtAlbumTitle.Text) || string.IsNullOrEmpty(txtArtistManager.Text) || string.IsNullOrEmpty(txtReleaseDate.Text) || nudPrice.Value == 0)
                {
                    MessageBox.Show("Missing Information Entry");
                }
                else
                {
                    if (db.ShopInfos.Where(x => x.AlbumTitle == txtAlbumTitle.Text).FirstOrDefault() == null)
                    {
                        ShopInfo album = new ShopInfo()
                        {
                            AlbumTitle = txtAlbumTitle.Text,
                            AlbumArtist = txtArtistManager.Text,
                            ReleaseDate = txtReleaseDate.Text,
                            Price = nudPrice.Value,
                            Discount = nudDiscount.Value,
                            StockStatus = tgsStock.Checked,
                            Poster = txtPoster.Text,
                            ManagerId = sessionManager.ManagerId,
                            CreatedBy = sessionManager.ManagerId,
                            CreatedDate = DateTime.Now
                        };
                        db.ShopInfos.Add(album);
                        db.SaveChanges();
                        ManagerPageReset();
                        FillTheLists();
                        tbcShopInfo.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("This album is already on sale");
                    }
                }
            }
            else if (btnSellAlbum.Text == "Update Album")
            {
                int albumid = (int)dgvList1.SelectedRows[0].Cells[0].Value;
                ShopInfo updateAlbum = db.ShopInfos.Find(albumid);
                if (updateAlbum != null)
                {
                    updateAlbum.AlbumTitle = txtAlbumTitle.Text;
                    updateAlbum.AlbumArtist = txtArtistManager.Text;
                    updateAlbum.ReleaseDate = txtReleaseDate.Text;
                    updateAlbum.Price = nudPrice.Value;
                    updateAlbum.Discount = nudDiscount.Value;
                    updateAlbum.StockStatus = tgsStock.Checked;
                    updateAlbum.Poster = txtPoster.Text;
                    updateAlbum.IsModified = true;
                    updateAlbum.ModifiedBy = sessionManager.ManagerId;
                    updateAlbum.ModifiedDate = DateTime.Now;
                }
                db.SaveChanges();
                ManagerPageReset();
                FillTheLists();
            }
        }
        private void btnCheckPoster_Click(object sender, EventArgs e)
        {
            if (txtPoster.Text != null)
            {
                try
                {
                    pboPosterOnManagerPanel.Load(txtPoster.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Poster link not found!");
                }
            }
            else
            {
                MessageBox.Show("Poster link not found!");
            }
        }
        private void dgvList1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int albumId1 = (int)dgvList1.SelectedRows[0].Cells[0].Value;
            dgvListDoubleClick(albumId1);
        }
        private void dgvList2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int albumId2 = (int)dgvList2.SelectedRows[0].Cells[0].Value;
            dgvListDoubleClick(albumId2);
        }
        private void dgvList3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int albumId3 = (int)dgvList3.SelectedRows[0].Cells[0].Value;
            dgvListDoubleClick(albumId3);
        }
        private void dgvList4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int albumId4 = (int)dgvList4.SelectedRows[0].Cells[0].Value;
            dgvListDoubleClick(albumId4);
        }
        private void dgvList5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int albumId5 = (int)dgvList5.SelectedRows[0].Cells[0].Value;
            dgvListDoubleClick(albumId5);

        }
        private void tgsStock_CheckedChanged(object sender, EventArgs e)
        {
            if (tgsStock.Checked)
            {
                lblStock.Text = "In Stock";
            }
            else
            {
                lblStock.Text = "Out of Stock";
            }
        }
        private void chkManualAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkManualAdd.Checked)
            {
                txtPoster.ReadOnly = false;
                txtAlbumTitle.ReadOnly = false;
                txtReleaseDate.ReadOnly = false;
                txtArtistManager.ReadOnly = false;
            }
            else
            {
                txtPoster.ReadOnly = true;
                txtAlbumTitle.ReadOnly = true;
                txtReleaseDate.ReadOnly = true;
                txtArtistManager.ReadOnly = true;
            }
        }
        private void deleteAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int album = (int)dgvList1.SelectedRows[0].Cells[0].Value;
            ShopInfo deleteAlbum = db.ShopInfos.Find(album);
            db.ShopInfos.Remove(deleteAlbum);
            db.SaveChanges();
            FillTheLists();
        }
        private void dgvList1_SelectionChanged(object sender, EventArgs e)
        {
            CreatedByModifiedBy(dgvList1);
        }
        private void dgvList2_SelectionChanged(object sender, EventArgs e)
        {
            CreatedByModifiedBy(dgvList2);
        }
        private void dgvList3_SelectionChanged(object sender, EventArgs e)
        {
            CreatedByModifiedBy(dgvList3);
        }
        private void dgvList4_SelectionChanged(object sender, EventArgs e)
        {
            CreatedByModifiedBy(dgvList4);
        }
        private void dgvList5_SelectionChanged(object sender, EventArgs e)
        {
            CreatedByModifiedBy(dgvList5);
        }
        #endregion

        #region ApiMethods
        private async Task FindTrack(string artistName, string trackName, string albumName)
        {
            string apiUrl = "https://api.deezer.com/search?q=artist:%22" + artistName + "%22track:%22" + trackName + "%22album:%22" + albumName + "%22";
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(apiUrl);
            string queryFindTrack = await result.Content.ReadAsStringAsync();
            File.WriteAllText("queryFindTrack.json", queryFindTrack);
            var json = File.ReadAllText("queryFindTrack.json");
            Rootobject Root = JsonConvert.DeserializeObject<Rootobject>(json);
            foreach (var item in Root.data)
            {
                Datum t = new Datum()
                {
                    id = item.id,
                    title = item.title,
                    duration = item.duration,
                    preview = item.preview,
                    type = item.type
                };
                Artist art = db.Artists.FirstOrDefault(x => x.name == item.artist.name);
                if (art != null)
                {
                    t.artist = art;
                }
                else
                {
                    t.artist = new Artist()
                    {
                        id = item.artist.id,
                        name = item.artist.name,
                        picture_medium = item.artist.picture_medium,
                        type = item.artist.type
                    };
                }
                Album alb = db.Albums.FirstOrDefault(x => x.title == item.album.title);
                if (alb != null)
                {
                    t.album = alb;
                }
                else
                {
                    t.album = new Album()
                    {
                        id = item.album.id,
                        title = item.album.title,
                        cover_medium = item.album.cover_medium,
                        tracklist = item.album.tracklist,
                        type = item.album.type
                    };
                }
                db.Tracks.Add(t);
                await db.SaveChangesAsync();
            }
        }
        private async Task TrackInfos(string albumid)
        {
            string apiUrl = "https://api.deezer.com/album/" + albumid + "/tracks";
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(apiUrl);
            string queryTrackInfo = await result.Content.ReadAsStringAsync();
            File.WriteAllText("queryTrackInfo.json", queryTrackInfo);
            var json = File.ReadAllText("queryTrackInfo.json");
            Rootobject Root = JsonConvert.DeserializeObject<Rootobject>(json);
            int trackId = 0;
            foreach (var item in Root.data)
            {
                trackId++;
                string durationFormat = (Convert.ToInt32(item.duration) / 60).ToString() + ":" + ((Convert.ToInt32(item.duration) % 60) > 10 ? (Convert.ToInt32(item.duration) % 60).ToString() : ("0" + (Convert.ToInt32(item.duration) % 60).ToString()));
                Datum t = new Datum()
                {
                    DatumID = trackId,
                    id = item.id,
                    title = item.title,
                    duration = durationFormat,
                    preview = item.preview,
                    type = item.type
                };
                tracksOnTheAlbum.Add(t);
            }
        }
        private async Task AlbumInfos(string albumID)
        {
            string apiUrl = "https://api.deezer.com/album/" + albumID;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(apiUrl);
            string queryAlbumInfo = await result.Content.ReadAsStringAsync();
            File.WriteAllText("queryAlbumInfo.json", queryAlbumInfo);
            var json = File.ReadAllText("queryAlbumInfo.json");
            Rootobject Root = JsonConvert.DeserializeObject<Rootobject>(json);
            Album clickedAlbum = new Album()
            {
                cover_medium = Root.cover_medium,
                duration = Root.duration,
                release_date = Root.release_date,
                title = Root.title
            };
            foreach (var item in Root.contributors)
            {
                Contributor art = new Contributor()
                {
                    name = item.name
                };
                clickedArtists.Add(art);
            }
            clickedAlbums.Add(clickedAlbum);
        }
        #endregion

        #region Methods
        private void FillTheLists()
        {
            dgvList1.Rows.Clear();
            dgvList2.Rows.Clear();
            dgvList3.Rows.Clear();
            dgvList4.Rows.Clear();
            dgvList5.Rows.Clear();
            var List1 = db.ShopInfos.ToList();
            foreach (var item in List1)
            {
                dgvList1.Rows.Add(item.id, item.AlbumTitle, item.AlbumArtist, item.ReleaseDate, item.Price, item.Discount, item.StockStatus);
            }

            var List2 = db.ShopInfos.Where(x => x.StockStatus == false)
                .Select(x => new
                {
                    x.id,
                    x.AlbumTitle,
                    x.AlbumArtist,
                    x.StockStatus
                }).ToList();
            foreach (var item in List2)
            {
                dgvList2.Rows.Add(item.id, item.AlbumTitle, item.AlbumArtist, item.StockStatus);
            }

            var List3 = db.ShopInfos.Where(x => x.StockStatus == true)
                .Select(x => new
                {
                    x.id,
                    x.AlbumTitle,
                    x.AlbumArtist,
                    x.StockStatus
                }).ToList();
            foreach (var item in List3)
            {
                dgvList3.Rows.Add(item.id, item.AlbumTitle, item.AlbumArtist, item.StockStatus);
            }
            var List4 = db.ShopInfos.OrderByDescending(x => x.id)
                .Select(x => new
                {
                    x.id,
                    x.AlbumTitle,
                    x.AlbumArtist
                }).Take(10).ToList();
            foreach (var item in List4)
            {
                dgvList4.Rows.Add(item.id, item.AlbumTitle, item.AlbumArtist);
            }

            var List5 = db.ShopInfos.Where(x => x.Discount > 0).OrderByDescending(x => x.Discount)
                .Select(x => new
                {
                    x.id,
                    x.AlbumTitle,
                    x.AlbumArtist,
                    x.Price,
                    x.Discount
                }).ToList();
            foreach (var item in List5)
            {
                dgvList5.Rows.Add(item.id, item.AlbumTitle, item.AlbumArtist, item.Price, item.Discount);
            }
        }
        public void ManagerPageReset()
        {
            btnSellAlbum.Text = "Sell Album";
            txtAlbumTitle.Clear();
            txtArtistManager.Clear();
            txtReleaseDate.Clear();
            nudPrice.Value = 0;
            nudDiscount.Value = 0;
            tgsStock.Checked = false;
            txtPoster.Clear();
            pboPosterOnManagerPanel.Image = null;
        }
        private void dgvListDoubleClick(int albumId)
        {
            btnSellAlbum.Text = "Update Album";
            ShopInfo updateAlbum = db.ShopInfos.Find(albumId);
            txtAlbumTitle.Text = updateAlbum.AlbumTitle;
            txtArtistManager.Text = updateAlbum.AlbumArtist;
            txtReleaseDate.Text = updateAlbum.ReleaseDate;
            nudPrice.Value = updateAlbum.Price;
            nudDiscount.Value = updateAlbum.Discount;
            tgsStock.Checked = updateAlbum.StockStatus;
            txtPoster.Text = updateAlbum.Poster;
            pboPosterOnManagerPanel.Load(txtPoster.Text);
        }
        private void SendVerification1(string mailAddress)
        {
            SmtpClient client = new SmtpClient();
            MailMessage message = new MailMessage();

            client.Credentials = new NetworkCredential("g3musicshop@outlook.com", "aaaAA!+1");

            client.Port = 587; //Simple mail transfer protocol

            client.Host = "smtp-mail.outlook.com";
            client.EnableSsl = true;

            message.To.Add(mailAddress);
            message.From = new MailAddress("g3musicshop@outlook.com", "G3MusicShop");
            message.Subject = "G3MusicShop Verification";
            message.Body = "Your verification code = " + rand_code1;
            client.Send(message);
        }
        private void CreatedByModifiedBy(DataGridView listName)
        {
            try
            {
                int album = (int)listName.SelectedRows[0].Cells[0].Value;
                ShopInfo createInfo = db.ShopInfos.Find(album);
                Manager m = db.Managers.Find(createInfo.ManagerId);
                lblCreatedBy.Text = m.ManagerName;
                lblCreatedDate.Text = createInfo.CreatedDate.ToString();
                lblModifiedBy.Text = "-";
                lblModifiedDate.Text = "-";
                if (createInfo.IsModified == true)
                {
                    Manager m2 = db.Managers.Find(createInfo.ModifiedBy);
                    lblModifiedBy.Text = m2.ManagerName;
                    lblModifiedDate.Text = createInfo.ModifiedDate.ToString();
                }
            }
            catch (Exception)
            {
                lblCreatedBy.Text = "-";
                lblCreatedDate.Text = "-";
                lblModifiedBy.Text = "-";
                lblModifiedDate.Text = "-";
                return;
            }
        }
        #endregion
    }
}

