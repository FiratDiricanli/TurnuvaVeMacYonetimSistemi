using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Turnuva_ve_Maç_Yönetim_Sistemi
{
    public class Takim
    {
        [DisplayName("Takım")]
        public string TakimAdi { get; set; }

        [DisplayName("OM")]
        public int Oynanan { get; set; }

        [DisplayName("G")]
        public int Galibiyet { get; set; }

        [DisplayName("B")]
        public int Beraberlik { get; set; }

        [DisplayName("M")]
        public int Maglubiyet { get; set; }

        [DisplayName("AG")]
        public int AtilanGol { get; set; }

        [DisplayName("YG")]
        public int YenilenGol { get; set; }

        [DisplayName("A")]
        public int Averaj { get { return AtilanGol - YenilenGol; } }

        [DisplayName("P")]
        public int Puan { get; set; }
    }

    public class MacGecmisi
    {
        public string Ozet { get; set; }
    }

    public partial class Form1 : Form
    {
        List<Takim> takimlar = new List<Takim>();
        List<MacGecmisi> maclar = new List<MacGecmisi>();

        TabControl sekmeler;
        TabPage sekmePuan, sekmeYeniMac, sekmeGecmis, sekmeTakimEkle;

        DataGridView dgvPuan;
        ListBox lstGecmis;
        ComboBox cmbEvSahibi, cmbDeplasman;
        TextBox txtEvGol, txtDepGol, txtYeniTakim;
        Button btnMaciBitir, btnTakimEkle;

        public Form1()
        {
            this.Text = "Süper Lig Güncel Puan Durumu - 2300005412 Fırat Diricanlı";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            VerileriHazirla();
            KapsamliArayuzCiz();
        }

        private void VerileriHazirla()
        {
            string[] takimIsimleri = {
                "Galatasaray", "Fenerbahçe", "Trabzonspor", "Beşiktaş", "Başakşehir",
                "Göztepe", "Samsunspor", "Çaykur Rizespor", "Konyaspor", "Gaziantep FK",
                "Kasımpaşa", "Sivasspor", "Alanyaspor", "Antalyaspor", "Adana Demirspor",
                "Kayserispor", "Hatayspor", "Bodrum FK", "Eyüpspor"
            };

            foreach (string isim in takimIsimleri)
            {
                takimlar.Add(new Takim { TakimAdi = isim, Oynanan = 0, Galibiyet = 0, Beraberlik = 0, Maglubiyet = 0, AtilanGol = 0, YenilenGol = 0, Puan = 0 });
            }
        }

        private void KapsamliArayuzCiz()
        {
            sekmeler = new TabControl();
            sekmeler.Dock = DockStyle.Fill;
            sekmeler.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            sekmePuan = new TabPage("🏆 Puan Durumu");
            sekmeYeniMac = new TabPage("⚽ Yeni Maç Oynat");
            sekmeGecmis = new TabPage("📜 Maç Geçmişi");
            sekmeTakimEkle = new TabPage("➕ Takım Ekle");

            dgvPuan = new DataGridView();
            dgvPuan.Dock = DockStyle.Fill;
            dgvPuan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPuan.AllowUserToAddRows = false;
            dgvPuan.ReadOnly = true;
            dgvPuan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPuan.RowHeadersVisible = false;
            dgvPuan.BackgroundColor = Color.White;

            dgvPuan.RowTemplate.Height = 35;
            dgvPuan.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvPuan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvPuan.ColumnHeadersHeight = 40;

            sekmePuan.Controls.Add(dgvPuan);

            sekmeYeniMac.BackColor = Color.WhiteSmoke;

            Label lblEv = new Label() { Text = "Ev Sahibi:", Location = new Point(150, 100), AutoSize = true, Font = new Font("Segoe UI", 12) };
            cmbEvSahibi = new ComboBox() { Location = new Point(270, 95), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };

            Label lblEvGol = new Label() { Text = "Gol:", Location = new Point(500, 100), AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtEvGol = new TextBox() { Location = new Point(550, 95), Width = 70, Font = new Font("Segoe UI", 12) };

            Label lblDep = new Label() { Text = "Deplasman:", Location = new Point(150, 170), AutoSize = true, Font = new Font("Segoe UI", 12) };
            cmbDeplasman = new ComboBox() { Location = new Point(270, 165), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };

            Label lblDepGol = new Label() { Text = "Gol:", Location = new Point(500, 170), AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtDepGol = new TextBox() { Location = new Point(550, 165), Width = 70, Font = new Font("Segoe UI", 12) };

            btnMaciBitir = new Button() { Text = "MAÇI TAMAMLA VE KAYDET", Location = new Point(270, 240), Size = new Size(350, 60), Font = new Font("Segoe UI", 14, FontStyle.Bold), BackColor = Color.Teal, ForeColor = Color.White, Cursor = Cursors.Hand };
            btnMaciBitir.Click += BtnMaciBitir_Click;

            ComboboxlariDoldur();

            sekmeYeniMac.Controls.Add(lblEv); sekmeYeniMac.Controls.Add(cmbEvSahibi);
            sekmeYeniMac.Controls.Add(lblEvGol); sekmeYeniMac.Controls.Add(txtEvGol);
            sekmeYeniMac.Controls.Add(lblDep); sekmeYeniMac.Controls.Add(cmbDeplasman);
            sekmeYeniMac.Controls.Add(lblDepGol); sekmeYeniMac.Controls.Add(txtDepGol);
            sekmeYeniMac.Controls.Add(btnMaciBitir);

            lstGecmis = new ListBox();
            lstGecmis.Dock = DockStyle.Fill;
            lstGecmis.Font = new Font("Consolas", 14);
            sekmeGecmis.Controls.Add(lstGecmis);

            sekmeTakimEkle.BackColor = Color.WhiteSmoke;

            Label lblYeniTakim = new Label() { Text = "Takım Adı:", Location = new Point(200, 100), AutoSize = true, Font = new Font("Segoe UI", 14) };
            txtYeniTakim = new TextBox() { Location = new Point(320, 95), Width = 300, Font = new Font("Segoe UI", 14) };

            btnTakimEkle = new Button() { Text = "LİGE YENİ TAKIM EKLE", Location = new Point(320, 160), Size = new Size(300, 50), Font = new Font("Segoe UI", 14, FontStyle.Bold), BackColor = Color.DarkOrange, ForeColor = Color.White, Cursor = Cursors.Hand };
            btnTakimEkle.Click += BtnTakimEkle_Click;

            sekmeTakimEkle.Controls.Add(lblYeniTakim);
            sekmeTakimEkle.Controls.Add(txtYeniTakim);
            sekmeTakimEkle.Controls.Add(btnTakimEkle);

            sekmeler.TabPages.Add(sekmePuan);
            sekmeler.TabPages.Add(sekmeYeniMac);
            sekmeler.TabPages.Add(sekmeTakimEkle);
            sekmeler.TabPages.Add(sekmeGecmis);
            this.Controls.Add(sekmeler);

            TabloyuGuncelle();
        }

        private void ComboboxlariDoldur()
        {
            cmbEvSahibi.Items.Clear();
            cmbDeplasman.Items.Clear();
            var siraliTakimlar = takimlar.OrderBy(t => t.TakimAdi).ToList();
            foreach (var t in siraliTakimlar)
            {
                cmbEvSahibi.Items.Add(t.TakimAdi);
                cmbDeplasman.Items.Add(t.TakimAdi);
            }
        }

        private void BtnTakimEkle_Click(object sender, EventArgs e)
        {
            string yeniAd = txtYeniTakim.Text.Trim();

            if (string.IsNullOrEmpty(yeniAd))
            {
                MessageBox.Show("Takım adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (takimlar.Any(t => t.TakimAdi.Equals(yeniAd, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Bu takım zaten ligde var!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            takimlar.Add(new Takim { TakimAdi = yeniAd, Oynanan = 0, Galibiyet = 0, Beraberlik = 0, Maglubiyet = 0, AtilanGol = 0, YenilenGol = 0, Puan = 0 });

            TabloyuGuncelle();
            ComboboxlariDoldur();

            MessageBox.Show($"{yeniAd} başarıyla lige eklendi!", "Sistem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtYeniTakim.Clear();
        }

        private void BtnMaciBitir_Click(object sender, EventArgs e)
        {
            if (cmbEvSahibi.SelectedItem == null || cmbDeplasman.SelectedItem == null)
            {
                MessageBox.Show("Lütfen her iki takımı da seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbEvSahibi.SelectedItem.ToString() == cmbDeplasman.SelectedItem.ToString())
            {
                MessageBox.Show("Bir takım kendisiyle maç yapamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int evGol = Convert.ToInt32(txtEvGol.Text);
                int depGol = Convert.ToInt32(txtDepGol.Text);

                string evAdi = cmbEvSahibi.SelectedItem.ToString();
                string depAdi = cmbDeplasman.SelectedItem.ToString();

                Takim evSahibi = takimlar.First(t => t.TakimAdi == evAdi);
                Takim deplasman = takimlar.First(t => t.TakimAdi == depAdi);

                evSahibi.Oynanan++; deplasman.Oynanan++;
                evSahibi.AtilanGol += evGol; evSahibi.YenilenGol += depGol;
                deplasman.AtilanGol += depGol; deplasman.YenilenGol += evGol;

                if (evGol > depGol)
                {
                    evSahibi.Puan += 3;
                    evSahibi.Galibiyet++;
                    deplasman.Maglubiyet++;
                }
                else if (depGol > evGol)
                {
                    deplasman.Puan += 3;
                    deplasman.Galibiyet++;
                    evSahibi.Maglubiyet++;
                }
                else
                {
                    evSahibi.Puan += 1; deplasman.Puan += 1;
                    evSahibi.Beraberlik++;
                    deplasman.Beraberlik++;
                }

                maclar.Add(new MacGecmisi { Ozet = $"{evAdi} {evGol} - {depGol} {depAdi}" });
                lstGecmis.Items.Add($"[{DateTime.Now.ToShortTimeString()}] {evAdi} {evGol} - {depGol} {depAdi}");

                TabloyuGuncelle();
                MessageBox.Show("Maç başarıyla kaydedildi ve istatistikler güncellendi!", "Sistem", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtEvGol.Clear(); txtDepGol.Clear();
                cmbEvSahibi.SelectedIndex = -1; cmbDeplasman.SelectedIndex = -1;
            }
            catch
            {
                MessageBox.Show("Lütfen gol sayılarına sadece rakam giriniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TabloyuGuncelle()
        {
            dgvPuan.DataSource = null;
            dgvPuan.DataSource = takimlar.OrderByDescending(t => t.Puan).ThenByDescending(t => t.Averaj).ThenByDescending(t => t.AtilanGol).ToList();
        }
    }
}