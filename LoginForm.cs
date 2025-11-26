using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopLaundry
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ApplyStyles();
        }

        private void ApplyStyles()
        {
            // Background gradient biru langit
            this.BackColor = Color.FromArgb(240, 248, 255);

            // Style panel login
            panelLogin.BackColor = Color.White;
            panelLogin.BorderStyle = BorderStyle.FixedSingle;

            // Judul aplikasi
            lblAppTitle.ForeColor = Color.FromArgb(30, 144, 255);
            lblAppTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);

            lblSubtitle.ForeColor = Color.FromArgb(100, 149, 237);
            lblSubtitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Role selection
            lblRole.ForeColor = Color.FromArgb(70, 130, 180);
            lblRole.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Style radio buttons
            rbOwner.ForeColor = Color.FromArgb(70, 130, 180);
            rbAdmin.ForeColor = Color.FromArgb(70, 130, 180);
            rbKasir.ForeColor = Color.FromArgb(70, 130, 180);

            // Style textboxes
            txtUsername.BackColor = Color.White;
            txtPassword.BackColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            // Style login button
            btnLogin.BackColor = Color.FromArgb(30, 144, 255);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string role = GetSelectedRole();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username dan Password harus diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthenticateUser(username, password, role))
            {
                // Notifikasi sukses
                MessageBox.Show($"Login berhasil! Selamat datang {username}", "Sukses Login",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Langsung buka dashboard kasir
                DashboardKasir dashboard = new DashboardKasir(username, role);
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username atau Password salah!", "Login Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSelectedRole()
        {
            if (rbOwner.Checked) return "Owner";
            if (rbAdmin.Checked) return "Admin";
            if (rbKasir.Checked) return "Kasir";
            return "Kasir"; // Default
        }

        private bool AuthenticateUser(string username, string password, string role)
        {
            DatabaseHelper db = new DatabaseHelper();
            return db.ValidateLogin(username, password, role);
        }

        private void rbRole_CheckedChanged(object sender, EventArgs e)
        {
            // Auto-fill credentials saat ganti role
            if (rbOwner.Checked)
            {
                txtUsername.Text = "owner";
                txtPassword.Text = "owner123";
            }
            else if (rbAdmin.Checked)
            {
                txtUsername.Text = "admin";
                txtPassword.Text = "admin123";
            }
            else if (rbKasir.Checked)
            {
                txtUsername.Text = "kasir";
                txtPassword.Text = "kasir123";
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }
    }
}