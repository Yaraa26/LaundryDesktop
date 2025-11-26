using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Linq;

namespace DesktopLaundry
{
    public class FormTambahPesananFromMenu : Form
    {
        private DatabaseHelper dbHelper;
        private ComboBox cmbPelanggan;
        private ComboBox cmbLayanan;
        private TextBox txtBerat;
        private TextBox txtCatatan;
        private Label lblTotal;
        private Label lblTglAmbil;

        public FormTambahPesananFromMenu()
        {
            dbHelper = new DatabaseHelper();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Tambah Pesanan Baru - A&Y SUDS Laundry";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateFormContent();
            LoadDataComboBox();
            CalculateTotalAndDate();
        }

        private void CreateFormContent()
        {
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Tambah Pesanan Baru (Menu Pesanan)";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 144, 255);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(300, 30);
            this.Controls.Add(lblTitle);

            int yPos = 70;

            // Pelanggan
            Label lblPelanggan = new Label();
            lblPelanggan.Text = "Pelanggan:";
            lblPelanggan.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblPelanggan.Location = new Point(20, yPos);
            lblPelanggan.Size = new Size(100, 20);
            this.Controls.Add(lblPelanggan);

            cmbPelanggan = new ComboBox();
            cmbPelanggan.Location = new Point(130, yPos);
            cmbPelanggan.Size = new Size(300, 25);
            cmbPelanggan.Font = new Font("Segoe UI", 10);
            cmbPelanggan.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbPelanggan);

            yPos += 45;

            // Layanan
            Label lblLayanan = new Label();
            lblLayanan.Text = "Layanan:";
            lblLayanan.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblLayanan.Location = new Point(20, yPos);
            lblLayanan.Size = new Size(100, 20);
            this.Controls.Add(lblLayanan);

            cmbLayanan = new ComboBox();
            cmbLayanan.Location = new Point(130, yPos);
            cmbLayanan.Size = new Size(300, 25);
            cmbLayanan.Font = new Font("Segoe UI", 10);
            cmbLayanan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLayanan.SelectedIndexChanged += (s, e) => CalculateTotalAndDate();
            this.Controls.Add(cmbLayanan);

            yPos += 45;

            // Berat
            Label lblBerat = new Label();
            lblBerat.Text = "Berat (kg):";
            lblBerat.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblBerat.Location = new Point(20, yPos);
            lblBerat.Size = new Size(100, 20);
            this.Controls.Add(lblBerat);

            txtBerat = new TextBox();
            txtBerat.Location = new Point(130, yPos);
            txtBerat.Size = new Size(100, 25);
            txtBerat.Font = new Font("Segoe UI", 10);
            txtBerat.Text = "1";
            txtBerat.TextChanged += (s, e) => CalculateTotalAndDate();
            this.Controls.Add(txtBerat);

            yPos += 45;

            // Tanggal Ambil
            Label lblTglAmbilTitle = new Label();
            lblTglAmbilTitle.Text = "Estimasi Selesai:";
            lblTglAmbilTitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblTglAmbilTitle.Location = new Point(20, yPos);
            lblTglAmbilTitle.Size = new Size(100, 20);
            this.Controls.Add(lblTglAmbilTitle);

            lblTglAmbil = new Label();
            lblTglAmbil.Text = DateTime.Now.AddDays(2).ToString("dd MMM yyyy");
            lblTglAmbil.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTglAmbil.ForeColor = Color.FromArgb(30, 144, 255);
            lblTglAmbil.Location = new Point(130, yPos);
            lblTglAmbil.Size = new Size(200, 20);
            this.Controls.Add(lblTglAmbil);

            yPos += 45;

            // Total
            Label lblTotalTitle = new Label();
            lblTotalTitle.Text = "Total:";
            lblTotalTitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblTotalTitle.Location = new Point(20, yPos);
            lblTotalTitle.Size = new Size(100, 20);
            this.Controls.Add(lblTotalTitle);

            lblTotal = new Label();
            lblTotal.Text = "Rp 7.000";
            lblTotal.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(46, 204, 113);
            lblTotal.Location = new Point(130, yPos);
            lblTotal.Size = new Size(200, 25);
            this.Controls.Add(lblTotal);

            yPos += 45;

            // Catatan
            Label lblCatatan = new Label();
            lblCatatan.Text = "Catatan:";
            lblCatatan.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblCatatan.Location = new Point(20, yPos);
            lblCatatan.Size = new Size(100, 20);
            this.Controls.Add(lblCatatan);

            txtCatatan = new TextBox();
            txtCatatan.Location = new Point(130, yPos);
            txtCatatan.Size = new Size(300, 80);
            txtCatatan.Font = new Font("Segoe UI", 10);
            txtCatatan.Multiline = true;
            txtCatatan.ScrollBars = ScrollBars.Vertical;
            this.Controls.Add(txtCatatan);

            yPos += 100;

            // Tombol
            Button btnSimpan = new Button();
            btnSimpan.Text = "💾 Simpan Pesanan";
            btnSimpan.BackColor = Color.FromArgb(46, 204, 113);
            btnSimpan.ForeColor = Color.White;
            btnSimpan.FlatStyle = FlatStyle.Flat;
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Size = new Size(150, 40);
            btnSimpan.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSimpan.Location = new Point(130, yPos);
            btnSimpan.Click += (s, e) => SimpanPesanan();
            this.Controls.Add(btnSimpan);

            Button btnBatal = new Button();
            btnBatal.Text = "❌ Batal";
            btnBatal.BackColor = Color.FromArgb(192, 57, 43);
            btnBatal.ForeColor = Color.White;
            btnBatal.FlatStyle = FlatStyle.Flat;
            btnBatal.FlatAppearance.BorderSize = 0;
            btnBatal.Size = new Size(100, 40);
            btnBatal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnBatal.Location = new Point(290, yPos);
            btnBatal.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnBatal);
        }

        private void LoadDataComboBox()
        {
            try
            {
                // Load pelanggan
                DataTable dtPelanggan = dbHelper.GetDataPelanggan();
                if (dtPelanggan.Rows.Count > 0)
                {
                    cmbPelanggan.DisplayMember = "Nama";
                    cmbPelanggan.ValueMember = "PelangganID";
                    cmbPelanggan.DataSource = dtPelanggan;
                }

                // Load layanan
                DataTable dtLayanan = dbHelper.GetDataLayanan();
                if (dtLayanan.Rows.Count > 0)
                {
                    cmbLayanan.DisplayMember = "NamaLayanan";
                    cmbLayanan.ValueMember = "LayananID";
                    cmbLayanan.DataSource = dtLayanan;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateTotalAndDate()
        {
            try
            {
                if (cmbLayanan.SelectedValue != null && !string.IsNullOrEmpty(txtBerat.Text))
                {
                    DataTable dtLayanan = dbHelper.GetDataLayanan();
                    var selectedLayanan = dtLayanan.AsEnumerable()
                        .FirstOrDefault(row => row.Field<int>("LayananID") == (int)cmbLayanan.SelectedValue);

                    if (selectedLayanan != null)
                    {
                        decimal harga = Convert.ToDecimal(selectedLayanan["Harga"]);
                        int durasi = Convert.ToInt32(selectedLayanan["Durasi"]);

                        if (decimal.TryParse(txtBerat.Text, out decimal berat))
                        {
                            decimal total = harga * berat;
                            lblTotal.Text = $"Rp {total:N0}";

                            DateTime tglAmbil = DateTime.Now.AddDays(durasi);
                            lblTglAmbil.Text = tglAmbil.ToString("dd MMM yyyy");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculate total: {ex.Message}");
            }
        }

        private void SimpanPesanan()
        {
            if (cmbPelanggan.SelectedValue == null)
            {
                MessageBox.Show("Harap pilih pelanggan!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbLayanan.SelectedValue == null)
            {
                MessageBox.Show("Harap pilih layanan!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtBerat.Text) || !decimal.TryParse(txtBerat.Text, out decimal berat) || berat <= 0)
            {
                MessageBox.Show("Berat harus berupa angka yang lebih besar dari 0!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int pelangganID = (int)cmbPelanggan.SelectedValue;
                int layananID = (int)cmbLayanan.SelectedValue;
                string catatan = txtCatatan.Text;

                DataTable dtLayanan = dbHelper.GetDataLayanan();
                var selectedLayanan = dtLayanan.AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("LayananID") == layananID);

                if (selectedLayanan != null)
                {
                    decimal harga = Convert.ToDecimal(selectedLayanan["Harga"]);
                    decimal totalHarga = harga * berat;

                    bool success = dbHelper.TambahTransaksi(pelangganID, totalHarga, "Diterima", catatan);

                    if (success)
                    {
                        MessageBox.Show("Pesanan berhasil ditambahkan!", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menambahkan pesanan!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}