using projeArayuz.Data;
using projeArayuz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projeArayuz
{
    public partial class Form3 : Form
    {
        #region Databases
        MusicShopDbContext db = new MusicShopDbContext();
        Manager signUpManager = new Manager();
        int verificationCode = 0;
        Form2 frm2 = new Form2();
        #endregion

        #region Constructors
        public Form3(Manager m, int vCode, Form2 frm)
        {
            InitializeComponent();
            signUpManager = m;
            verificationCode = vCode;
            frm2 = frm;
        }

        public Form3(int vCode, Manager m)
        {
            InitializeComponent();
            signUpManager = m;
            verificationCode = vCode;
            btnSubmitVerification.Text = "Update";
        } 
        #endregion

        private void btnSubmitVerification_Click(object sender, EventArgs e)
        {
            if (btnSubmitVerification.Text == "Submit")
            {
                if (verificationCode.ToString() == txtVerificationCode.Text)
                {
                    db.Managers.Add(signUpManager);
                    db.SaveChanges();
                    MessageBox.Show("Registration Successful");
                    frm2.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Check Your Verification Code Again");
                }
            }
            else if (btnSubmitVerification.Text == "Update")
            {
                if (verificationCode.ToString() == txtVerificationCode.Text)
                {
                    Form2 frm2 = new Form2(signUpManager.ManagerName);
                    frm2.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Check Your Verification Code Again");
                }
            }
        }
    }
}
