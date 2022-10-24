using projeArayuz.Data;
using projeArayuz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace projeArayuz
{
    public partial class Form2 : Form
    {
        #region Constructors
        //Normal Constructor
        public Form2()
        {
            InitializeComponent();
            timer1.Start();
            txtOneUsePassword.Visible = false;
            prgPassword.Value = 0;
            if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour < 12)
            {
                lblWelcome.Text = "Good Morning";
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                lblWelcome.Text = "Good Afternoon";
            }
            else if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour < 22)
            {
                lblWelcome.Text = "Good Evening";
            }
            else if (DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 5)
            {
                lblWelcome.Text = "Good Night";
            }
            pnlLogin.Visible = true;
            pnlRemindPassword.Visible = false;
            pnlSignUp.Visible = false;
            pnlUpdate.Visible = false;
        }

        //Update Page Constructor
        public Form2(string updateManagerComing)
        {
            InitializeComponent();
            updateManager2 = db.Managers.Where(x => x.ManagerName == updateManagerComing).FirstOrDefault();
            pnlLoadingScreen.Visible = false;
            pnlLogin.Visible = false;
            pnlRemindPassword.Visible = false;
            pnlSignUp.Visible = false;
            pnlUpdate.Visible = true;
            txtUsernameUpdate.ReadOnly = false;
            txtUsernameUpdate.Text = updateManager2.ManagerName;
        }
        #endregion

        #region PasswordDecryptionAndRegex
        Regex r = new Regex(@"^(?=(.*[a-z]){3,})(?=(.*[A-Z]){2,})(?=(.*[!:+*]){2,})(?!.*\s).{8,20}?");
        Regex r2 = new Regex(@"^(?=(.*[a-z]){3,})");
        Regex r3 = new Regex(@"^(?=(.*[A-Z]){2,})");
        Regex r4 = new Regex(@"^(?=(.*[!:+*]){2,})");
        Regex r5 = new Regex(@".{8,20}");
        Regex mailReg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        static Random random = new Random();

        int rand_code; // = random.Next(10000, 99999);

        public static String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }
        #endregion

        #region Databases
        MusicShopDbContext db = new MusicShopDbContext();
        Manager updateManager2 = new Manager();
        #endregion

        #region LoadingPageEvents
        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => "http://www.baidu.com",
                    _ =>
                        """http://www.gstatic.com/generate_204""",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            prgLoading.Increment(4);
            if (prgLoading.Value == 100 && CheckForInternetConnection() == true)
            {
                timer1.Stop();
                pnlLoadingScreen.Visible = false;
            }
            else if (prgLoading.Value == 100 && CheckForInternetConnection() == false)
            {
                timer1.Stop();
                MessageBox.Show("Please! Check your internet connection.");
            }
        }
        #endregion

        #region LoginPageEvents
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Manager loginManager = db.Managers.Where(x => x.ManagerName == txtUsername1.Text || x.MailAddress == txtUsername1.Text).FirstOrDefault();

            if (loginManager != null)
            {
                if (loginManager.ManagerPassword == sha256_hash(txtPassword1.Text))
                {
                    Form1 frm = new Form1(txtUsername1.Text);
                    this.Hide();
                    frm.Show();
                }
                else
                {
                    MessageBox.Show("Username or Password is wrong");
                }
            }
            else
            {
                MessageBox.Show("Username or Password is wrong");
            }
        }
        private void btnSignUp1_Click(object sender, EventArgs e)
        {
            pnlSignUp.Visible = true;
            pnlLogin.Visible = false;
        }
        private void linklblForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlRemindPassword.Visible = true;
            pnlLogin.Visible = false;
        }
        #endregion

        #region SignUpPageEvents
        private void btnSignUp2_Click(object sender, EventArgs e)
        {
            Manager checkManager = db.Managers.Where(x => x.ManagerName == txtUsername2.Text).FirstOrDefault();
            if (checkManager == null)
            {
                if (txtPassword2.Text == txtConfirmPassword.Text && r.IsMatch(txtPassword2.Text) && mailReg.IsMatch(txtMailSignUp.Text))
                {
                    Manager newManager = new Manager()
                    {
                        ManagerName = txtUsername2.Text,
                        MailAddress = txtMailSignUp.Text,
                        ManagerPassword = sha256_hash(txtPassword2.Text)
                    };
                    SendVerification(txtMailSignUp.Text);
                    Form3 frm = new Form3(newManager, rand_code, this);
                    frm.Show();
                    this.Hide();

                    pnlSignUp.Visible = false;
                    pnlLogin.Visible = true;
                }
                else
                {
                    MessageBox.Show("Registration failed");
                }
            }
            else
            {
                MessageBox.Show("Username is already used");
            }

        }
        private void txtPassword2_TextChanged(object sender, EventArgs e)
        {
            List<bool> c = new List<bool>();

            bool c2 = r2.IsMatch(txtPassword2.Text);

            bool c3 = r3.IsMatch(txtPassword2.Text);

            bool c4 = r4.IsMatch(txtPassword2.Text);

            bool c5 = r5.IsMatch(txtPassword2.Text);

            c.Add(c2);
            c.Add(c3);
            c.Add(c4);
            c.Add(c5);

            if (c.Where(x => x == true).Count() == 0)
            {
                prgPassword.Value = 0;
            }
            else if (c.Where(x => x == true).Count() == 1)
            {
                prgPassword.Value = 25;
            }
            else if (c.Where(x => x == true).Count() == 2)
            {
                prgPassword.Value = 50;
            }
            else if (c.Where(x => x == true).Count() == 3)
            {
                prgPassword.Value = 75;
            }
            else if (c.Where(x => x == true).Count() == 4)
            {
                prgPassword.Value = 100;
            }
        }
        #endregion

        #region RemindPageEvents
        private void btnRemindPassword_Click(object sender, EventArgs e)
        {
            Manager remindManager = db.Managers.Where(x => x.ManagerName == txtUsername3.Text || x.MailAddress == txtUsername3.Text).FirstOrDefault();
            if (btnRemindPassword.Text == "Remind Password")
            {
                if (remindManager != null)
                {
                    MessageBox.Show("Check your mail address for one use password");
                    SendVerification(remindManager.MailAddress);
                    txtUsername3.ReadOnly = true;
                    txtOneUsePassword.ReadOnly = false;
                    txtOneUsePassword.Visible = true;
                    btnRemindPassword.Text = "Login";
                }
            }
            else if (btnRemindPassword.Text == "Login")
            {
                if (txtOneUsePassword.Text == rand_code.ToString())
                {
                    pnlRemindPassword.Visible = false;
                    pnlUpdate.Visible = true;
                    txtUsernameUpdate.Text = remindManager.ManagerName;
                    txtUsernameUpdate.ReadOnly = true;
                    pnlRemindPassword.Visible = false;
                    pnlUpdate.Visible = true;
                    btnRemindPassword.Text = "Remind Password";
                }
                else
                {
                    MessageBox.Show("Check your mail address for one use password");
                    txtOneUsePassword.Clear();
                }
            }
        }
        #endregion

        #region UpdatePageEvents
        private void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            Manager updateManager = db.Managers.Where(x => x.ManagerName == txtUsernameUpdate.Text).FirstOrDefault();
            if (updateManager != null)
            {
                if (r.IsMatch(txtPasswordUpdate.Text) && txtPasswordUpdate.Text == txtConfirmPasswordUpdate.Text)
                {
                    updateManager.ManagerName = txtUsernameUpdate.Text;
                    updateManager.ManagerPassword = sha256_hash(txtPasswordUpdate.Text);
                    txtMailUpdate.Text = updateManager.MailAddress;
                    txtMailUpdate.ReadOnly = true;
                    db.SaveChanges();
                    Form1 frm = new Form1(txtUsernameUpdate.Text);
                    this.Hide();
                    frm.Show();
                }
                else
                {
                    MessageBox.Show("Password change failed");
                }
            }
            else
            {
                if (r.IsMatch(txtPasswordUpdate.Text) && txtPasswordUpdate.Text == txtConfirmPasswordUpdate.Text)
                {
                    updateManager2.ManagerName = txtUsernameUpdate.Text;
                    updateManager2.ManagerPassword = sha256_hash(txtPasswordUpdate.Text);
                    txtMailUpdate.Text = updateManager2.MailAddress;
                    txtMailUpdate.ReadOnly = true;
                    db.SaveChanges();
                    Form1 frm = new Form1(txtUsernameUpdate.Text);
                    this.Hide();
                    frm.Show();
                }
                else
                {
                    MessageBox.Show("Password change failed");
                }
            }
        }
        private void txtPasswordUpdate_TextChanged(object sender, EventArgs e)
        {
            List<bool> c = new List<bool>();

            bool c2 = r2.IsMatch(txtPasswordUpdate.Text);

            bool c3 = r3.IsMatch(txtPasswordUpdate.Text);

            bool c4 = r4.IsMatch(txtPasswordUpdate.Text);

            bool c5 = r5.IsMatch(txtPasswordUpdate.Text);

            c.Add(c2);
            c.Add(c3);
            c.Add(c4);
            c.Add(c5);

            if (c.Where(x => x == true).Count() == 0)
            {
                prgPassword2.Value = 0;
            }
            else if (c.Where(x => x == true).Count() == 1)
            {
                prgPassword2.Value = 25;
            }
            else if (c.Where(x => x == true).Count() == 2)
            {
                prgPassword2.Value = 50;
            }
            else if (c.Where(x => x == true).Count() == 3)
            {
                prgPassword2.Value = 75;
            }
            else if (c.Where(x => x == true).Count() == 4)
            {
                prgPassword2.Value = 100;
            }
        }
        #endregion

        #region EmailVerificationMethod
        private void SendVerification(string mailAddress)
        {
            rand_code = random.Next(10000, 99999);
            SmtpClient client = new SmtpClient();
            MailMessage message = new MailMessage();

            client.Credentials = new NetworkCredential("g3musicshop@outlook.com", "aaaAA!+1");

            client.Port = 587; //Simple mail transfer protocol

            client.Host = "smtp-mail.outlook.com";
            client.EnableSsl = true;

            message.To.Add(mailAddress);
            message.From = new MailAddress("g3musicshop@outlook.com", "G3MusicShop");
            message.Subject = "G3MusicShop Verification";
            message.Body = "Your verification code = " + rand_code;
            client.Send(message);
        } 
        #endregion
    }
}
