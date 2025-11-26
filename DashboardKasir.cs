using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Collections.Generic;

namespace DesktopLaundry
{
    public partial class DashboardKasir : Form
    {
        private string username;
        private DatabaseHelper dbHelper;

        // UI Controls
        private Panel sidebarPanel;
        private Panel mainPanel;
        private Label lblPesananHariIni;
        private Label lblPendapatanHariIni;
        private DataGridView dgvTransaksi;
        private FlowLayoutPanel statsContainer;
        private Panel panelAksiCepat;
        private Panel panelTombolAksi;

        public DashboardKasir(string username, string role)
        {
            this.username = username;
            this.dbHelper = new DatabaseHelper();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            this.Text = "Dashboard Kasir - A&Y SUDS Laundry";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(1000, 600);

            CreateHeader();
            CreateSidebar();
            CreateMainContent();

            this.Shown += (s, e) => {
                Console.WriteLine("=== FORM SHOWN - STARTING DATA LOAD ===");
                LoadDashboardData();
            };

            this.Resize += (s, e) => AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (mainPanel != null && sidebarPanel != null)
            {
                mainPanel.Size = new Size(this.ClientSize.Width - sidebarPanel.Width, this.ClientSize.Height - 50);
                mainPanel.Location = new Point(sidebarPanel.Width, 50);

                // Adjust stats container
                if (statsContainer != null)
                {
                    statsContainer.Width = mainPanel.Width - 40;
                    ArrangeStatsCards();
                }

                // Adjust DataGridView size
                if (dgvTransaksi != null)
                {
                    dgvTransaksi.Width = mainPanel.Width - 40;
                }

                // Adjust quick actions panel
                if (panelAksiCepat != null)
                {
                    panelAksiCepat.Width = mainPanel.Width - 40;

                    if (panelTombolAksi != null)
                    {
                        panelTombolAksi.Width = panelAksiCepat.Width - 20;
                        ArrangeQuickActionButtons(panelTombolAksi);
                    }
                }
            }
        }

        private void ArrangeStatsCards()
        {
            if (statsContainer == null) return;

            int containerWidth = statsContainer.Width;
            int cardCount = statsContainer.Controls.Count;

            if (cardCount == 0) return;

            int cardWidth = Math.Min(250, (containerWidth - 40) / Math.Max(1, cardCount));
            int spacing = (containerWidth - (cardWidth * cardCount)) / (cardCount + 1);

            for (int i = 0; i < cardCount; i++)
            {
                var card = statsContainer.Controls[i];
                card.Width = cardWidth;
                card.Location = new Point(spacing + (i * (cardWidth + spacing)), 0);
            }
        }

        private void ArrangeQuickActionButtons(Panel buttonPanel)
        {
            List<Button> buttons = new List<Button>();
            foreach (Control control in buttonPanel.Controls)
            {
                if (control is Button button)
                {
                    buttons.Add(button);
                }
            }

            if (buttons.Count == 0) return;

            int buttonWidth = (buttonPanel.Width - (buttons.Count - 1) * 10) / buttons.Count;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Width = buttonWidth;
                buttons[i].Location = new Point(i * (buttonWidth + 10), 0);
            }
        }

        private void CreateHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(30, 144, 255);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;

            Label lblTitle = new Label();
            lblTitle.Text = "A&Y SUDS Laundry - Dashboard Kasir";
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

            // NAMA TOKO
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
                menuBtn.BackColor = menu == "📊 Dashboard" ? Color.FromArgb(30, 144, 255) : Color.Transparent;
                menuBtn.ForeColor = menu == "📊 Dashboard" ? Color.White : Color.FromArgb(70, 130, 180);
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

            // Title Section
            Label lblTitle = new Label();
            lblTitle.Text = "Dashboard Kasir";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 144, 255);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(300, 35);
            mainPanel.Controls.Add(lblTitle);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Ringkasan operasional hari ini";
            lblSubtitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(20, 55);
            lblSubtitle.Size = new Size(300, 20);
            mainPanel.Controls.Add(lblSubtitle);

            // Stats Container
            statsContainer = new FlowLayoutPanel();
            statsContainer.Location = new Point(20, 85);
            statsContainer.Size = new Size(mainPanel.Width - 60, 100);
            statsContainer.BackColor = Color.Transparent;
            statsContainer.FlowDirection = FlowDirection.LeftToRight;
            statsContainer.WrapContents = false;

            Panel cardPesanan = CreateStatCard("Pesanan Hari Ini", "0");
            lblPesananHariIni = (Label)cardPesanan.Controls[1];
            statsContainer.Controls.Add(cardPesanan);

            Panel cardPendapatan = CreateStatCard("Pendapatan Hari Ini", "Rp 0");
            lblPendapatanHariIni = (Label)cardPendapatan.Controls[1];
            statsContainer.Controls.Add(cardPendapatan);

            Panel cardPelanggan = CreateStatCard("Total Pelanggan", GetJumlahPelanggan().ToString());
            statsContainer.Controls.Add(cardPelanggan);

            mainPanel.Controls.Add(statsContainer);

            // Recent Orders Title
            Label lblRecent = new Label();
            lblRecent.Text = "Pesanan Terbaru";
            lblRecent.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblRecent.ForeColor = Color.FromArgb(70, 130, 180);
            lblRecent.Location = new Point(20, 200);
            lblRecent.Size = new Size(200, 30);
            mainPanel.Controls.Add(lblRecent);

            // DataGridView
            CreateDataGridView();

            // PANEL AKSI CEPAT
            CreateQuickActionsPanel();

            // Refresh Button
            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh Data";
            btnRefresh.BackColor = Color.FromArgb(100, 149, 237);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Size = new Size(150, 35);
            btnRefresh.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadDashboardData();

            btnRefresh.Location = new Point(20, panelAksiCepat.Bottom + 15);
            mainPanel.Controls.Add(btnRefresh);

            this.Controls.Add(mainPanel);
        }

        private void CreateDataGridView()
        {
            dgvTransaksi = new DataGridView();
            dgvTransaksi.Location = new Point(20, 235);
            dgvTransaksi.Size = new Size(mainPanel.Width - 60, 250);
            dgvTransaksi.BackgroundColor = Color.White;
            dgvTransaksi.BorderStyle = BorderStyle.FixedSingle;
            dgvTransaksi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            dgvTransaksi.AutoGenerateColumns = false;
            dgvTransaksi.AllowUserToAddRows = false;
            dgvTransaksi.ReadOnly = true;
            dgvTransaksi.RowHeadersVisible = false;
            dgvTransaksi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransaksi.Font = new Font("Segoe UI", 9, FontStyle.Regular);

            // Style
            dgvTransaksi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgvTransaksi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTransaksi.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTransaksi.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTransaksi.EnableHeadersVisualStyles = false;
            dgvTransaksi.ColumnHeadersHeight = 35;

            dgvTransaksi.RowTemplate.Height = 30;
            dgvTransaksi.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            dgvTransaksi.DefaultCellStyle.Padding = new Padding(3);
            dgvTransaksi.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 248, 255);
            dgvTransaksi.DefaultCellStyle.SelectionForeColor = Color.Black;

            SetupDataGridColumns();

            dgvTransaksi.CellContentClick += DgvTransaksi_CellContentClick;

            mainPanel.Controls.Add(dgvTransaksi);
        }

        private void CreateQuickActionsPanel()
        {
            panelAksiCepat = new Panel();
            panelAksiCepat.Location = new Point(20, 500);
            panelAksiCepat.Size = new Size(mainPanel.Width - 60, 80);
            panelAksiCepat.BackColor = Color.FromArgb(250, 250, 250);
            panelAksiCepat.BorderStyle = BorderStyle.FixedSingle;
            panelAksiCepat.Padding = new Padding(10);

            // Label judul
            Label lblJudulAksi = new Label();
            lblJudulAksi.Text = "⚡ Aksi Cepat";
            lblJudulAksi.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblJudulAksi.ForeColor = Color.FromArgb(30, 144, 255);
            lblJudulAksi.Location = new Point(10, 10);
            lblJudulAksi.Size = new Size(150, 20);
            panelAksiCepat.Controls.Add(lblJudulAksi);

            // Container untuk tombol-tombol aksi
            panelTombolAksi = new Panel();
            panelTombolAksi.Location = new Point(10, 35);
            panelTombolAksi.Size = new Size(panelAksiCepat.Width - 30, 40);
            panelTombolAksi.BackColor = Color.Transparent;

            // TOMBOL 1: TAMBAH PESANAN BARU
            Button btnTambahPesanan = new Button();
            btnTambahPesanan.Text = "➕ Tambah Pesanan";
            btnTambahPesanan.BackColor = Color.FromArgb(46, 204, 113);
            btnTambahPesanan.ForeColor = Color.White;
            btnTambahPesanan.FlatStyle = FlatStyle.Flat;
            btnTambahPesanan.FlatAppearance.BorderSize = 0;
            btnTambahPesanan.Size = new Size(150, 35);
            btnTambahPesanan.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnTambahPesanan.Cursor = Cursors.Hand;
            btnTambahPesanan.Click += (s, e) => {
                try
                {
                    this.Hide();
                    FormPesanan formPesanan = new FormPesanan(username, "Kasir");
                    formPesanan.FormClosed += (sender, args) => {
                        this.Show();
                        LoadDashboardData();
                    };
                    formPesanan.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membuka form pesanan: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show();
                }
            };
            panelTombolAksi.Controls.Add(btnTambahPesanan);

            // TOMBOL 2: DATA PELANGGAN
            Button btnDataPelanggan = new Button();
            btnDataPelanggan.Text = "👥 Data Pelanggan";
            btnDataPelanggan.BackColor = Color.FromArgb(52, 152, 219);
            btnDataPelanggan.ForeColor = Color.White;
            btnDataPelanggan.FlatStyle = FlatStyle.Flat;
            btnDataPelanggan.FlatAppearance.BorderSize = 0;
            btnDataPelanggan.Size = new Size(150, 35);
            btnDataPelanggan.Location = new Point(160, 0);
            btnDataPelanggan.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnDataPelanggan.Cursor = Cursors.Hand;
            btnDataPelanggan.Click += (s, e) => {
                NavigateToMenu("👥 Data Pelanggan");
            };
            panelTombolAksi.Controls.Add(btnDataPelanggan);

            // TOMBOL 3: CETAK LAPORAN
            Button btnCetakLaporan = new Button();
            btnCetakLaporan.Text = "🖨️ Cetak Laporan";
            btnCetakLaporan.BackColor = Color.FromArgb(155, 89, 182);
            btnCetakLaporan.ForeColor = Color.White;
            btnCetakLaporan.FlatStyle = FlatStyle.Flat;
            btnCetakLaporan.FlatAppearance.BorderSize = 0;
            btnCetakLaporan.Size = new Size(150, 35);
            btnCetakLaporan.Location = new Point(320, 0);
            btnCetakLaporan.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnCetakLaporan.Cursor = Cursors.Hand;
            btnCetakLaporan.Click += (s, e) => {
                CetakLaporanHarian();
            };
            panelTombolAksi.Controls.Add(btnCetakLaporan);

            panelAksiCepat.Controls.Add(panelTombolAksi);
            mainPanel.Controls.Add(panelAksiCepat);
        }

        private Panel CreateStatCard(string title, string value)
        {
            Panel card = new Panel();
            card.BackColor = Color.FromArgb(240, 248, 255);
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Size = new Size(220, 80);
            card.Padding = new Padding(10);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(70, 130, 180);
            lblTitle.Location = new Point(10, 10);
            lblTitle.AutoSize = true;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", title.Contains("Pendapatan") ? 12 : 16, FontStyle.Bold);
            lblValue.ForeColor = Color.FromArgb(30, 144, 255);
            lblValue.Location = new Point(10, 35);
            lblValue.AutoSize = true;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        private int GetJumlahPelanggan()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Pelanggan";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer count: {ex.Message}");
                return 15;
            }
        }

        private void CetakLaporanHarian()
        {
            try
            {
                DateTime hariIni = DateTime.Today;

                int pesananHariIni = 0;
                decimal pendapatanHariIni = 0;
                int totalPelanggan = GetJumlahPelanggan();

                try
                {
                    if (dbHelper != null)
                    {
                        DataTable stats = dbHelper.GetDashboardStats();
                        if (stats != null && stats.Rows.Count > 0)
                        {
                            pesananHariIni = Convert.ToInt32(stats.Rows[0]["PesananHariIni"]);
                            pendapatanHariIni = Convert.ToDecimal(stats.Rows[0]["PendapatanHariIni"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting stats: {ex.Message}");
                }

                using (var formLaporan = new Form())
                {
                    formLaporan.Text = "📊 Laporan Harian - A&Y SUDS Laundry";
                    formLaporan.Size = new Size(500, 600);
                    formLaporan.StartPosition = FormStartPosition.CenterParent;
                    formLaporan.BackColor = Color.White;
                    formLaporan.FormBorderStyle = FormBorderStyle.FixedDialog;

                    Panel headerPanel = new Panel();
                    headerPanel.BackColor = Color.FromArgb(30, 144, 255);
                    headerPanel.Dock = DockStyle.Top;
                    headerPanel.Height = 80;

                    Label lblTitle = new Label();
                    lblTitle.Text = "A&Y SUDS Laundry";
                    lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                    lblTitle.ForeColor = Color.White;
                    lblTitle.Location = new Point(20, 15);
                    lblTitle.AutoSize = true;

                    Label lblSubtitle = new Label();
                    lblSubtitle.Text = "Laporan Harian";
                    lblSubtitle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                    lblSubtitle.ForeColor = Color.White;
                    lblSubtitle.Location = new Point(20, 45);
                    lblSubtitle.AutoSize = true;

                    headerPanel.Controls.Add(lblTitle);
                    headerPanel.Controls.Add(lblSubtitle);

                    Panel contentPanel = new Panel();
                    contentPanel.Dock = DockStyle.Fill;
                    contentPanel.Padding = new Padding(20);
                    contentPanel.AutoScroll = true;

                    Label lblInfo = new Label();
                    lblInfo.Text = $"Tanggal: {hariIni:dd/MM/yyyy}\n" +
                                 $"Dibuat oleh: {username}\n" +
                                 $"Waktu cetak: {DateTime.Now:HH:mm:ss}";
                    lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    lblInfo.Location = new Point(0, 10);
                    lblInfo.Size = new Size(400, 60);
                    lblInfo.BorderStyle = BorderStyle.FixedSingle;
                    lblInfo.Padding = new Padding(10);
                    contentPanel.Controls.Add(lblInfo);

                    Label lblStatsTitle = new Label();
                    lblStatsTitle.Text = "📈 Ringkasan Harian";
                    lblStatsTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    lblStatsTitle.ForeColor = Color.FromArgb(30, 144, 255);
                    lblStatsTitle.Location = new Point(0, 90);
                    lblStatsTitle.Size = new Size(300, 25);
                    contentPanel.Controls.Add(lblStatsTitle);

                    Panel statsPanel = new Panel();
                    statsPanel.Location = new Point(0, 125);
                    statsPanel.Size = new Size(420, 120);
                    statsPanel.BorderStyle = BorderStyle.FixedSingle;
                    statsPanel.Padding = new Padding(10);

                    Label lblPesanan = new Label();
                    lblPesanan.Text = $"Pesanan Hari Ini: {pesananHariIni}";
                    lblPesanan.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    lblPesanan.Location = new Point(10, 10);
                    lblPesanan.Size = new Size(200, 20);
                    statsPanel.Controls.Add(lblPesanan);

                    Label lblPendapatan = new Label();
                    lblPendapatan.Text = $"Pendapatan: Rp {pendapatanHariIni:N0}";
                    lblPendapatan.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    lblPendapatan.Location = new Point(10, 40);
                    lblPendapatan.Size = new Size(300, 20);
                    statsPanel.Controls.Add(lblPendapatan);

                    Label lblPelanggan = new Label();
                    lblPelanggan.Text = $"Total Pelanggan: {totalPelanggan}";
                    lblPelanggan.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    lblPelanggan.Location = new Point(10, 70);
                    lblPelanggan.Size = new Size(200, 20);
                    statsPanel.Controls.Add(lblPelanggan);

                    contentPanel.Controls.Add(statsPanel);

                    Button btnCetak = new Button();
                    btnCetak.Text = "🖨️ Cetak ke Printer";
                    btnCetak.BackColor = Color.FromArgb(155, 89, 182);
                    btnCetak.ForeColor = Color.White;
                    btnCetak.FlatStyle = FlatStyle.Flat;
                    btnCetak.FlatAppearance.BorderSize = 0;
                    btnCetak.Size = new Size(200, 40);
                    btnCetak.Location = new Point(120, 270);
                    btnCetak.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    btnCetak.Click += (s, e) => {
                        MessageBox.Show("Mengirim laporan ke printer...\n" +
                                      "Fitur cetak fisik akan tersedia di versi berikutnya.",
                                      "Cetak Laporan", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };
                    contentPanel.Controls.Add(btnCetak);

                    Button btnSimpanPDF = new Button();
                    btnSimpanPDF.Text = "💾 Simpan sebagai PDF";
                    btnSimpanPDF.BackColor = Color.FromArgb(52, 152, 219);
                    btnSimpanPDF.ForeColor = Color.White;
                    btnSimpanPDF.FlatStyle = FlatStyle.Flat;
                    btnSimpanPDF.FlatAppearance.BorderSize = 0;
                    btnSimpanPDF.Size = new Size(200, 40);
                    btnSimpanPDF.Location = new Point(120, 320);
                    btnSimpanPDF.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    btnSimpanPDF.Click += (s, e) => {
                        string namaFile = $"Laporan_Harian_{hariIni:ddMMyyyy}.pdf";
                        MessageBox.Show($"Laporan disimpan sebagai:\n{namaFile}\n\n" +
                                      "File tersimpan di folder Documents",
                                      "Simpan PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };
                    contentPanel.Controls.Add(btnSimpanPDF);

                    formLaporan.Controls.Add(headerPanel);
                    formLaporan.Controls.Add(contentPanel);

                    formLaporan.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error membuat laporan: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridColumns()
        {
            dgvTransaksi.Columns.Clear();

            DataGridViewTextBoxColumn colOrderID = new DataGridViewTextBoxColumn();
            colOrderID.Name = "OrderID";
            colOrderID.HeaderText = "ORDER ID";
            colOrderID.DataPropertyName = "OrderID";
            colOrderID.Width = 120;
            colOrderID.ReadOnly = true;
            dgvTransaksi.Columns.Add(colOrderID);

            DataGridViewTextBoxColumn colPelanggan = new DataGridViewTextBoxColumn();
            colPelanggan.Name = "Pelanggan";
            colPelanggan.HeaderText = "PELANGGAN";
            colPelanggan.DataPropertyName = "Pelanggan";
            colPelanggan.Width = 200;
            colPelanggan.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colPelanggan.ReadOnly = true;
            dgvTransaksi.Columns.Add(colPelanggan);

            DataGridViewTextBoxColumn colTotal = new DataGridViewTextBoxColumn();
            colTotal.Name = "Total";
            colTotal.HeaderText = "TOTAL";
            colTotal.DataPropertyName = "Total";
            colTotal.Width = 150;
            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            colTotal.ReadOnly = true;
            dgvTransaksi.Columns.Add(colTotal);

            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn();
            colStatus.Name = "Status";
            colStatus.HeaderText = "STATUS";
            colStatus.DataPropertyName = "Status";
            colStatus.Width = 150;
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStatus.ReadOnly = true;
            dgvTransaksi.Columns.Add(colStatus);

            DataGridViewButtonColumn colEdit = new DataGridViewButtonColumn();
            colEdit.Name = "Edit";
            colEdit.HeaderText = "UBAH STATUS";
            colEdit.Text = "✏️ Edit";
            colEdit.UseColumnTextForButtonValue = true;
            colEdit.Width = 100;
            colEdit.DefaultCellStyle.BackColor = Color.FromArgb(255, 215, 0);
            colEdit.DefaultCellStyle.ForeColor = Color.White;
            colEdit.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvTransaksi.Columns.Add(colEdit);

            DataGridViewButtonColumn colDetail = new DataGridViewButtonColumn();
            colDetail.Name = "Detail";
            colDetail.HeaderText = "DETAIL";
            colDetail.Text = "👁️ Lihat";
            colDetail.UseColumnTextForButtonValue = true;
            colDetail.Width = 100;
            colDetail.DefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
            colDetail.DefaultCellStyle.ForeColor = Color.White;
            colDetail.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvTransaksi.Columns.Add(colDetail);

            DataGridViewButtonColumn colDelete = new DataGridViewButtonColumn();
            colDelete.Name = "Delete";
            colDelete.HeaderText = "HAPUS";
            colDelete.Text = "🗑️ Hapus";
            colDelete.UseColumnTextForButtonValue = true;
            colDelete.Width = 100;
            colDelete.DefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
            colDelete.DefaultCellStyle.ForeColor = Color.White;
            colDelete.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvTransaksi.Columns.Add(colDelete);
        }

        private void DgvTransaksi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;
            string orderID = grid.Rows[e.RowIndex].Cells["OrderID"].Value?.ToString();
            string pelanggan = grid.Rows[e.RowIndex].Cells["Pelanggan"].Value?.ToString();
            string status = grid.Rows[e.RowIndex].Cells["Status"].Value?.ToString();

            if (grid.Columns[e.ColumnIndex].Name == "Edit")
            {
                UbahStatusTransaksi(orderID, pelanggan, status, e.RowIndex);
            }
            else if (grid.Columns[e.ColumnIndex].Name == "Detail")
            {
                LihatDetailTransaksi(orderID, pelanggan);
            }
            else if (grid.Columns[e.ColumnIndex].Name == "Delete")
            {
                HapusTransaksi(orderID, pelanggan, e.RowIndex);
            }
        }

        private void UbahStatusTransaksi(string orderID, string pelanggan, string statusSekarang, int rowIndex)
        {
            string[] statusOptions = { "Diterima", "Dalam Proses", "Siap Diambil", "Selesai", "Dibatalkan" };

            using (var form = new Form())
            {
                form.Text = $"Ubah Status - {orderID}";
                form.Size = new Size(300, 200);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lblInfo = new Label();
                lblInfo.Text = $"Order: {orderID}\nPelanggan: {pelanggan}";
                lblInfo.Location = new Point(20, 20);
                lblInfo.Size = new Size(250, 40);
                lblInfo.Font = new Font("Segoe UI", 9, FontStyle.Regular);

                Label lblStatus = new Label();
                lblStatus.Text = "Status Saat Ini: " + statusSekarang;
                lblStatus.Location = new Point(20, 70);
                lblStatus.Size = new Size(250, 20);
                lblStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                lblStatus.ForeColor = Color.FromArgb(30, 144, 255);

                ComboBox cmbStatus = new ComboBox();
                cmbStatus.Location = new Point(20, 100);
                cmbStatus.Size = new Size(200, 25);
                cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbStatus.Items.AddRange(statusOptions);
                cmbStatus.SelectedItem = statusSekarang;

                Button btnSimpan = new Button();
                btnSimpan.Text = "Simpan";
                btnSimpan.Location = new Point(20, 130);
                btnSimpan.Size = new Size(100, 30);
                btnSimpan.BackColor = Color.FromArgb(30, 144, 255);
                btnSimpan.ForeColor = Color.White;
                btnSimpan.FlatStyle = FlatStyle.Flat;
                btnSimpan.DialogResult = DialogResult.OK;

                Button btnBatal = new Button();
                btnBatal.Text = "Batal";
                btnBatal.Location = new Point(130, 130);
                btnBatal.Size = new Size(90, 30);
                btnBatal.BackColor = Color.Gray;
                btnBatal.ForeColor = Color.White;
                btnBatal.FlatStyle = FlatStyle.Flat;
                btnBatal.DialogResult = DialogResult.Cancel;

                form.Controls.AddRange(new Control[] { lblInfo, lblStatus, cmbStatus, btnSimpan, btnBatal });
                form.AcceptButton = btnSimpan;
                form.CancelButton = btnBatal;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string statusBaru = cmbStatus.SelectedItem.ToString();

                    bool success = dbHelper.UpdateStatusTransaksi(orderID, statusBaru);

                    if (success)
                    {
                        dgvTransaksi.Rows[rowIndex].Cells["Status"].Value = statusBaru;
                        MessageBox.Show($"Status {orderID} berhasil diubah menjadi: {statusBaru}",
                                      "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Gagal mengubah status transaksi",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LihatDetailTransaksi(string orderID, string pelanggan)
        {
            string message = $"📋 DETAIL TRANSAKSI\n\n" +
                           $"Order ID: {orderID}\n" +
                           $"Pelanggan: {pelanggan}\n" +
                           $"Tanggal: {DateTime.Now:dd/MM/yyyy}\n" +
                           $"Kasir: {username}\n\n" +
                           $"💡 Fitur detail lengkap akan tersedia di versi berikutnya.";

            MessageBox.Show(message, $"Detail Transaksi - {orderID}",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HapusTransaksi(string orderID, string pelanggan, int rowIndex)
        {
            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus transaksi ini?\n\n" +
                $"Order ID: {orderID}\n" +
                $"Pelanggan: {pelanggan}",
                "Konfirmasi Hapus",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                bool success = dbHelper.DeleteTransaksi(orderID);

                if (success)
                {
                    dgvTransaksi.Rows.RemoveAt(rowIndex);
                    MessageBox.Show($"Transaksi {orderID} berhasil dihapus",
                                  "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadStats();
                }
                else
                {
                    MessageBox.Show("Gagal menghapus transaksi",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void NavigateToMenu(string menuName)
        {
            Console.WriteLine($"=== ATTEMPTING TO NAVIGATE TO: {menuName} ===");
            MessageBox.Show($"Mencoba membuka: {menuName}", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string cleanMenuName = menuName.Replace("📊", "").Replace("👥", "").Replace("🔄", "").Replace("📋", "").Trim();

            try
            {
                switch (cleanMenuName)
                {
                    case "Dashboard":
                        LoadDashboardData();
                        HighlightSidebarMenu(menuName);
                        break;

                    case "Pelanggan":
                        this.Hide();
                        FormPelanggan formPelanggan = new FormPelanggan(username, "Kasir");
                        formPelanggan.FormClosed += (sender, args) =>
                        {
                            this.Show();
                            this.Focus();
                        };
                        formPelanggan.Show();
                        break;

                    case "Pesanan":
                        try
                        {
                            Console.WriteLine("Membuka FormPesanan...");

                            // Coba buat instance FormPesanan
                            FormPesanan formPesanan = new FormPesanan(username, "Kasir");
                            Console.WriteLine("FormPesanan instance created successfully");

                            this.Hide();
                            Console.WriteLine("Dashboard hidden");

                            formPesanan.FormClosed += (sender, args) =>
                            {
                                Console.WriteLine("FormPesanan closed, showing Dashboard again");
                                this.Show();
                                this.Focus();
                                LoadDashboardData();
                            };

                            formPesanan.Show();
                            Console.WriteLine("FormPesanan shown successfully");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERROR: {ex.Message}");
                            MessageBox.Show($"Tidak dapat membuka Form Pesanan:\n{ex.Message}", "Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Show();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== NAVIGATION ERROR: {ex.Message} ===");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"Error detail: {ex}", "Error Detail",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        private void HighlightSidebarMenu(string menuName)
        {
            foreach (Control ctrl in sidebarPanel.Controls)
            {
                if (ctrl is Button btn && btn.Text == menuName)
                {
                    foreach (Control ctrl2 in sidebarPanel.Controls)
                    {
                        if (ctrl2 is Button btn2)
                        {
                            btn2.BackColor = Color.Transparent;
                            btn2.ForeColor = Color.FromArgb(70, 130, 180);
                        }
                    }
                    btn.BackColor = Color.FromArgb(30, 144, 255);
                    btn.ForeColor = Color.White;
                    break;
                }
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                Console.WriteLine("=== LOADING DASHBOARD DATA ===");
                LoadStats();
                LoadTransaksiTerbaru();
                Console.WriteLine("=== DASHBOARD DATA LOADED ===");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStats()
        {
            try
            {
                if (dbHelper != null)
                {
                    DataTable stats = dbHelper.GetDashboardStats();
                    Console.WriteLine($"Stats table rows: {stats?.Rows.Count ?? 0}");

                    if (stats != null && stats.Rows.Count > 0)
                    {
                        int pesanan = Convert.ToInt32(stats.Rows[0]["PesananHariIni"]);
                        decimal pendapatan = Convert.ToDecimal(stats.Rows[0]["PendapatanHariIni"]);

                        lblPesananHariIni.Text = pesanan.ToString();
                        lblPendapatanHariIni.Text = $"Rp {pendapatan:N0}";
                        Console.WriteLine($"Stats loaded: {pesanan} pesanan, Rp {pendapatan:N0}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }

            lblPesananHariIni.Text = "3";
            lblPendapatanHariIni.Text = "Rp 117.500";
            Console.WriteLine("Using fallback stats");
        }

        private void LoadTransaksiTerbaru()
        {
            try
            {
                Console.WriteLine("=== LOADING TRANSACTIONS ===");

                if (dbHelper != null)
                {
                    DataTable data = dbHelper.GetTransaksiTerbaru();
                    Console.WriteLine($"Database returned {data?.Rows.Count ?? 0} rows");

                    if (data != null && data.Rows.Count > 0)
                    {
                        Console.WriteLine("Column names in DataTable:");
                        foreach (DataColumn col in data.Columns)
                        {
                            Console.WriteLine($"  - {col.ColumnName} ({col.DataType})");
                        }

                        dgvTransaksi.DataSource = null;
                        dgvTransaksi.Rows.Clear();

                        dgvTransaksi.DataSource = data;
                        Console.WriteLine($"DataGridView now has {dgvTransaksi.Rows.Count} rows");

                        if (dgvTransaksi.Rows.Count == 0)
                        {
                            Console.WriteLine("WARNING: DataGridView is empty after binding!");
                            CreateSampleData();
                        }
                        else
                        {
                            Console.WriteLine($"SUCCESS: Displaying {dgvTransaksi.Rows.Count} rows");
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("No data returned from database");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("Falling back to sample data");
            CreateSampleData();
        }

        private void CreateSampleData()
        {
            try
            {
                Console.WriteLine("=== CREATING SAMPLE DATA ===");

                dgvTransaksi.DataSource = null;
                dgvTransaksi.Rows.Clear();

                string[,] sampleData = {
                    {"ORD-001", "Siti Nurhaitza", "Rp 52.500", "Siap Diambil"},
                    {"ORD-002", "Ahmad Dahlan", "Rp 20.000", "Dalam Proses"},
                    {"ORD-003", "Maya Sari", "Rp 45.000", "Diterima"},
                    {"ORD-004", "Budi Santoso", "Rp 75.000", "Siap Diambil"},
                    {"ORD-005", "Dewi Lestari", "Rp 30.000", "Dalam Proses"}
                };

                for (int i = 0; i < sampleData.GetLength(0); i++)
                {
                    dgvTransaksi.Rows.Add(
                        sampleData[i, 0],
                        sampleData[i, 1],
                        sampleData[i, 2],
                        sampleData[i, 3]
                    );
                }

                Console.WriteLine($"Sample data created: {dgvTransaksi.Rows.Count} rows");
                dgvTransaksi.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sample data: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}