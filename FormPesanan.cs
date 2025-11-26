using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace DesktopLaundry
{
    public partial class FormPesanan : Form
    {
        private string username;
        private DatabaseHelper dbHelper;
        private Panel sidebarPanel;
        private Panel mainPanel;
        private DataGridView dgvPesanan;

        public FormPesanan(string username, string role)
        {
            Console.WriteLine($"=== FORMPESANAN CONSTRUCTOR CALLED ===");
            Console.WriteLine($"Username: {username}, Role: {role}");

            this.username = username;
            this.dbHelper = new DatabaseHelper();

            // Panggil InitializeComponent dulu baru InitializeForm
            InitializeComponent();
            InitializeForm();

            Console.WriteLine("=== FORMPESANAN INITIALIZED ===");
        }


        private void InitializeForm()
        {
            this.Text = "Manajemen Pesanan - A&Y SUDS Laundry";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(1000, 600);

            CreateHeader();
            CreateSidebar();
            CreateMainContent();

            this.Resize += (s, e) => AdjustLayout();
            this.Shown += (s, e) => LoadDataPesanan();
        }

        private void AdjustLayout()
        {
            if (mainPanel != null && sidebarPanel != null)
            {
                mainPanel.Size = new Size(this.ClientSize.Width - sidebarPanel.Width, this.ClientSize.Height - 50);
                mainPanel.Location = new Point(sidebarPanel.Width, 50);

                if (dgvPesanan != null)
                {
                    dgvPesanan.Width = mainPanel.Width - 60;
                    dgvPesanan.Height = mainPanel.Height - 200;
                }
            }
        }

        private void CreateHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(30, 144, 255);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;

            Label lblTitle = new Label();
            lblTitle.Text = "A&Y SUDS Laundry - Manajemen Pesanan";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 12);
            lblTitle.AutoSize = true;

            Label lblUserInfo = new Label();
            lblUserInfo.Text = $"Kasir: {username}";
            lblUserInfo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblUserInfo.ForeColor = Color.White;
            lblUserInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserInfo.AutoSize = true;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblUserInfo);

            headerPanel.Layout += (s, e) => {
                lblUserInfo.Location = new Point(headerPanel.Width - lblUserInfo.Width - 20, 15);
            };

            this.Controls.Add(headerPanel);
        }

        private void CreateSidebar()
        {
            sidebarPanel = new Panel();
            sidebarPanel.BackColor = Color.FromArgb(240, 248, 255);
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Width = 220;
            sidebarPanel.BorderStyle = BorderStyle.FixedSingle;

            Label lblStoreName = new Label();
            lblStoreName.Text = "A&Y SUDS\nLaundry";
            lblStoreName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStoreName.ForeColor = Color.FromArgb(30, 144, 255);
            lblStoreName.Location = new Point(10, 15);
            lblStoreName.Size = new Size(200, 50);
            lblStoreName.TextAlign = ContentAlignment.MiddleCenter;
            lblStoreName.BackColor = Color.FromArgb(220, 240, 255);
            lblStoreName.BorderStyle = BorderStyle.FixedSingle;
            sidebarPanel.Controls.Add(lblStoreName);

            string[] menuItems = {
                "📊 Dashboard",
                "👥 Pelanggan",
                "📋 Pesanan"
            };

            int yPos = 80;
            foreach (string menu in menuItems)
            {
                Button menuBtn = new Button();
                menuBtn.Text = menu;
                menuBtn.BackColor = menu == "📋 Pesanan" ? Color.FromArgb(30, 144, 255) : Color.Transparent;
                menuBtn.ForeColor = menu == "📋 Pesanan" ? Color.White : Color.FromArgb(70, 130, 180);
                menuBtn.FlatStyle = FlatStyle.Flat;
                menuBtn.FlatAppearance.BorderSize = 0;
                menuBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 240, 255);
                menuBtn.TextAlign = ContentAlignment.MiddleLeft;
                menuBtn.Size = new Size(200, 45);
                menuBtn.Location = new Point(10, yPos);
                menuBtn.Cursor = Cursors.Hand;
                menuBtn.Font = new Font("Segoe UI", 10, FontStyle.Regular);

                menuBtn.Click += (s, e) => {
                    foreach (Control ctrl in sidebarPanel.Controls)
                    {
                        if (ctrl is Button btn)
                        {
                            btn.BackColor = Color.Transparent;
                            btn.ForeColor = Color.FromArgb(70, 130, 180);
                        }
                    }
                    menuBtn.BackColor = Color.FromArgb(30, 144, 255);
                    menuBtn.ForeColor = Color.White;
                    NavigateToMenu(menu);
                };

                sidebarPanel.Controls.Add(menuBtn);
                yPos += 55;
            }

            Button btnLogout = new Button();
            btnLogout.Text = "🚪 Logout";
            btnLogout.BackColor = Color.FromArgb(192, 57, 43);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Size = new Size(200, 45);
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.Click += (s, e) => {
                if (MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            };
            sidebarPanel.Controls.Add(btnLogout);

            sidebarPanel.Layout += (s, e) => {
                btnLogout.Location = new Point(10, sidebarPanel.Height - 80);
            };

            this.Controls.Add(sidebarPanel);
        }

        private void CreateMainContent()
        {
            mainPanel = new Panel();
            mainPanel.BackColor = Color.White;
            mainPanel.Location = new Point(sidebarPanel.Width, 50);
            mainPanel.Size = new Size(this.ClientSize.Width - sidebarPanel.Width, this.ClientSize.Height - 50);
            mainPanel.AutoScroll = false;
            mainPanel.Padding = new Padding(20);

            // Header dengan tombol refresh dan tambah
            Panel headerPanel = new Panel();
            headerPanel.Location = new Point(20, 20);
            headerPanel.Size = new Size(mainPanel.Width - 40, 40);
            headerPanel.BackColor = Color.Transparent;

            Label lblTitle = new Label();
            lblTitle.Text = "Manajemen Pesanan";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 144, 255);
            lblTitle.Location = new Point(0, 0);
            lblTitle.Size = new Size(300, 35);
            headerPanel.Controls.Add(lblTitle);

            // Tombol refresh
            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh Data";
            btnRefresh.BackColor = Color.FromArgb(100, 149, 237);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Size = new Size(120, 30);
            btnRefresh.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += (s, e) => LoadDataPesanan();
            btnRefresh.Location = new Point(headerPanel.Width - 250, 5);
            headerPanel.Controls.Add(btnRefresh);

            // Tombol statistik
            Button btnStatistik = new Button();
            btnStatistik.Text = "📊 Statistik";
            btnStatistik.BackColor = Color.FromArgb(155, 89, 182);
            btnStatistik.ForeColor = Color.White;
            btnStatistik.FlatStyle = FlatStyle.Flat;
            btnStatistik.FlatAppearance.BorderSize = 0;
            btnStatistik.Size = new Size(120, 30);
            btnStatistik.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnStatistik.Cursor = Cursors.Hand;
            btnStatistik.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStatistik.Click += (s, e) => TampilkanStatistikPesanan();
            btnStatistik.Location = new Point(headerPanel.Width - 120, 5);
            headerPanel.Controls.Add(btnStatistik);

            mainPanel.Controls.Add(headerPanel);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Kelola data pesanan laundry";
            lblSubtitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(20, 65);
            lblSubtitle.Size = new Size(300, 20);
            mainPanel.Controls.Add(lblSubtitle);

            // Panel info statistik cepat
            Panel panelStatistikCepat = new Panel();
            panelStatistikCepat.Location = new Point(20, 95);
            panelStatistikCepat.Size = new Size(mainPanel.Width - 60, 60);
            panelStatistikCepat.BackColor = Color.FromArgb(240, 248, 255);
            panelStatistikCepat.BorderStyle = BorderStyle.FixedSingle;
            panelStatistikCepat.Padding = new Padding(10);

            // Hitung statistik
            int totalPesanan = 0;
            int pesananSelesai = 0;
            try
            {
                DataTable stats = dbHelper.GetStatistikPesanan();
                if (stats.Rows.Count > 0)
                {
                    totalPesanan = Convert.ToInt32(stats.Rows[0]["TotalPesanan"]);
                    pesananSelesai = Convert.ToInt32(stats.Rows[0]["PesananSelesai"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }

            Label lblInfoCepat = new Label();
            lblInfoCepat.Text = $"📦 Total {totalPesanan} Pesanan | ✅ {pesananSelesai} Selesai";
            lblInfoCepat.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfoCepat.ForeColor = Color.FromArgb(30, 144, 255);
            lblInfoCepat.Location = new Point(10, 15);
            lblInfoCepat.Size = new Size(400, 20);
            panelStatistikCepat.Controls.Add(lblInfoCepat);

            mainPanel.Controls.Add(panelStatistikCepat);

            // Tombol Tambah Pesanan - PERBAIKAN: Gunakan nama form yang berbeda
            Button btnTambah = new Button();
            btnTambah.Text = "➕ Tambah Pesanan";
            btnTambah.BackColor = Color.FromArgb(46, 204, 113);
            btnTambah.ForeColor = Color.White;
            btnTambah.FlatStyle = FlatStyle.Flat;
            btnTambah.FlatAppearance.BorderSize = 0;
            btnTambah.Size = new Size(180, 35);
            btnTambah.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnTambah.Cursor = Cursors.Hand;
            btnTambah.Location = new Point(20, 170);
            btnTambah.Click += (s, e) => {
                // PERBAIKAN: Gunakan FormTambahPesananFromMenu
                FormTambahPesananFromMenu formTambah = new FormTambahPesananFromMenu();
                if (formTambah.ShowDialog() == DialogResult.OK)
                {
                    LoadDataPesanan();
                }
            };
            mainPanel.Controls.Add(btnTambah);

            // DataGridView
            CreateDataGridView();

            this.Controls.Add(mainPanel);
        }

        private void CreateDataGridView()
        {
            dgvPesanan = new DataGridView();
            dgvPesanan.Location = new Point(20, 220);
            dgvPesanan.Size = new Size(mainPanel.Width - 60, mainPanel.Height - 280);
            dgvPesanan.BackgroundColor = Color.White;
            dgvPesanan.BorderStyle = BorderStyle.None;
            dgvPesanan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPesanan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgvPesanan.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPesanan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvPesanan.EnableHeadersVisualStyles = false;
            dgvPesanan.RowHeadersVisible = false;
            dgvPesanan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPesanan.ReadOnly = true;

            SetupDataGridColumns();
            mainPanel.Controls.Add(dgvPesanan);
        }

        private void SetupDataGridColumns()
        {
            dgvPesanan.Columns.Clear();

            // Sesuai dengan screenshot yang diberikan
            DataGridViewTextBoxColumn colOrderID = new DataGridViewTextBoxColumn();
            colOrderID.Name = "OrderID";
            colOrderID.HeaderText = "ORDER ID";
            colOrderID.DataPropertyName = "OrderID";
            colOrderID.Width = 100;
            dgvPesanan.Columns.Add(colOrderID);

            DataGridViewTextBoxColumn colPelanggan = new DataGridViewTextBoxColumn();
            colPelanggan.Name = "Pelanggan";
            colPelanggan.HeaderText = "PELANGGAN";
            colPelanggan.DataPropertyName = "Pelanggan";
            colPelanggan.Width = 150;
            dgvPesanan.Columns.Add(colPelanggan);

            DataGridViewTextBoxColumn colTelepon = new DataGridViewTextBoxColumn();
            colTelepon.Name = "Telepon";
            colTelepon.HeaderText = "TELEPON";
            colTelepon.DataPropertyName = "Telepon";
            colTelepon.Width = 120;
            dgvPesanan.Columns.Add(colTelepon);

            DataGridViewTextBoxColumn colLayanan = new DataGridViewTextBoxColumn();
            colLayanan.Name = "Layanan";
            colLayanan.HeaderText = "LAYANAN";
            colLayanan.DataPropertyName = "Layanan";
            colLayanan.Width = 200;
            dgvPesanan.Columns.Add(colLayanan);

            DataGridViewTextBoxColumn colBerat = new DataGridViewTextBoxColumn();
            colBerat.Name = "Berat";
            colBerat.HeaderText = "BERAT";
            colBerat.DataPropertyName = "Berat";
            colBerat.Width = 80;
            colBerat.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPesanan.Columns.Add(colBerat);

            DataGridViewTextBoxColumn colTotal = new DataGridViewTextBoxColumn();
            colTotal.Name = "Total";
            colTotal.HeaderText = "TOTAL";
            colTotal.DataPropertyName = "Total";
            colTotal.Width = 120;
            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPesanan.Columns.Add(colTotal);

            DataGridViewTextBoxColumn colTglAmbil = new DataGridViewTextBoxColumn();
            colTglAmbil.Name = "TglAmbil";
            colTglAmbil.HeaderText = "TGL AMBIL";
            colTglAmbil.DataPropertyName = "TglAmbil";
            colTglAmbil.Width = 100;
            colTglAmbil.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPesanan.Columns.Add(colTglAmbil);

            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn();
            colStatus.Name = "Status";
            colStatus.HeaderText = "STATUS";
            colStatus.DataPropertyName = "Status";
            colStatus.Width = 120;
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPesanan.Columns.Add(colStatus);

            // Tombol aksi
            DataGridViewButtonColumn colAksi = new DataGridViewButtonColumn();
            colAksi.Name = "Aksi";
            colAksi.HeaderText = "AKSI";
            colAksi.Text = "Edit";
            colAksi.UseColumnTextForButtonValue = true;
            colAksi.Width = 80;
            colAksi.DefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            colAksi.DefaultCellStyle.ForeColor = Color.White;
            colAksi.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            dgvPesanan.Columns.Add(colAksi);
        }

        private void TampilkanStatistikPesanan()
        {
            try
            {
                DataTable stats = dbHelper.GetStatistikPesanan();
                if (stats.Rows.Count > 0)
                {
                    DataRow row = stats.Rows[0];
                    int totalPesanan = Convert.ToInt32(row["TotalPesanan"]);
                    int pesananSelesai = Convert.ToInt32(row["PesananSelesai"]);
                    int pesananProses = Convert.ToInt32(row["PesananProses"]);
                    int pesananDiterima = Convert.ToInt32(row["PesananDiterima"]);
                    decimal pendapatanHariIni = Convert.ToDecimal(row["PendapatanHariIni"]);

                    string message = $"📊 STATISTIK PESANAN\n\n" +
                                   $"📦 Total Pesanan: {totalPesanan}\n" +
                                   $"✅ Selesai: {pesananSelesai}\n" +
                                   $"🔄 Dalam Proses: {pesananProses}\n" +
                                   $"📥 Diterima: {pesananDiterima}\n" +
                                   $"💰 Pendapatan Hari Ini: Rp {pendapatanHariIni:N0}\n\n" +
                                   $"💡 Statistik update real-time.";

                    MessageBox.Show(message, "Statistik Pesanan",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menampilkan statistik: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NavigateToMenu(string menuName)
        {
            string cleanMenuName = menuName.Replace("📊", "").Replace("👥", "").Replace("📋", "").Trim();

            switch (cleanMenuName)
            {
                case "Dashboard":
                    this.Hide();
                    DashboardKasir dashboard = new DashboardKasir(username, "Kasir");
                    dashboard.Show();
                    break;
                case "Pelanggan":
                    this.Hide();
                    FormPelanggan formPelanggan = new FormPelanggan(username, "Kasir");
                    formPelanggan.Show();
                    break;
                default:
                    // Untuk menu Pesanan, tetap di form ini
                    break;
            }
        }

        private void LoadDataPesanan()
        {
            try
            {
                DataTable data = dbHelper.GetAllPesanan();
                dgvPesanan.DataSource = data;

                // Update info statistik cepat
                UpdateInfoStatistikCepat();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateSampleData();
            }
        }

        private void UpdateInfoStatistikCepat()
        {
            foreach (Control control in mainPanel.Controls)
            {
                if (control is Panel panel && panel.Controls.Count > 0)
                {
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is Label lbl && lbl.Text.Contains("Total"))
                        {
                            try
                            {
                                DataTable stats = dbHelper.GetStatistikPesanan();
                                if (stats.Rows.Count > 0)
                                {
                                    int totalPesanan = Convert.ToInt32(stats.Rows[0]["TotalPesanan"]);
                                    int pesananSelesai = Convert.ToInt32(stats.Rows[0]["PesananSelesai"]);
                                    lbl.Text = $"📦 Total {totalPesanan} Pesanan | ✅ {pesananSelesai} Selesai";
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error update statistik: {ex.Message}");
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void CreateSampleData()
        {
            DataTable table = new DataTable();
            table.Columns.Add("OrderID", typeof(string));
            table.Columns.Add("Pelanggan", typeof(string));
            table.Columns.Add("Telepon", typeof(string));
            table.Columns.Add("Layanan", typeof(string));
            table.Columns.Add("Berat", typeof(string));
            table.Columns.Add("Total", typeof(string));
            table.Columns.Add("TglAmbil", typeof(string));
            table.Columns.Add("Status", typeof(string));

            // Data sesuai screenshot
            table.Rows.Add("ORD-001", "Siti Nurhaliza", "08123456789", "Cuci Kering, Setrika", "3.5 kg", "Rp 52.500,00", "15 Jan 2025", "Siap Diambil");
            table.Rows.Add("ORD-002", "Ahmad Dahlan", "08567891234", "Cuci Lipat", "2 kg", "Rp 20.000,00", "16 Jan 2025", "Dalam Proses");
            table.Rows.Add("ORD-003", "Maya Sari", "08765432109", "Dry Clean", "1.5 kg", "Rp 45.000,00", "17 Jan 2025", "Diterima");

            dgvPesanan.DataSource = table;
        }
    }
}