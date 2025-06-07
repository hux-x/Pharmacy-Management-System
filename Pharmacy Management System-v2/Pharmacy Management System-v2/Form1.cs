using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pharmacy_Management_System_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetupForm();
            LoadComponents();
        }

        private void SetupForm()
        {
            this.Text = "Pharmacy Management System";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void LoadComponents()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(0, 123, 255);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;

            Label titleLabel = new Label();
            titleLabel.Text = "PHARMACY MANAGEMENT SYSTEM";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.AutoSize = false;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(40);

            Label welcomeLabel = new Label();
            welcomeLabel.Text = "Welcome to Pharmacy Management System";
            welcomeLabel.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            welcomeLabel.ForeColor = Color.FromArgb(50, 50, 50);
            welcomeLabel.AutoSize = true;
            welcomeLabel.Location = new Point(250, 30);
            mainPanel.Controls.Add(welcomeLabel);

            Label descLabel = new Label();
            descLabel.Text = "Please select a module to continue:";
            descLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            descLabel.ForeColor = Color.FromArgb(100, 100, 100);
            descLabel.AutoSize = true;
            descLabel.Location = new Point(320, 70);
            mainPanel.Controls.Add(descLabel);

          
            Button pharmacyBtn = CreateModuleButton("Pharmacy Module", "💊", Color.FromArgb(40, 167, 69), new Point(150, 120));
            pharmacyBtn.Click += PharmacyBtn_Click;
            mainPanel.Controls.Add(pharmacyBtn);

            Button inventoryBtn = CreateModuleButton("Inventory Module", "📦", Color.FromArgb(23, 162, 184), new Point(450, 120));
            inventoryBtn.Click += InventoryBtn_Click;
            mainPanel.Controls.Add(inventoryBtn);

            Button salesBtn = CreateModuleButton("Sales Module", "💰", Color.FromArgb(220, 53, 69), new Point(300, 300));
            salesBtn.Click += SalesBtn_Click;
            mainPanel.Controls.Add(salesBtn);

            Label creditsLabel = new Label();
            creditsLabel.Text = "Developed by:\n" +
                              "Hasnain Iqbal (2023-ag-9954)\n" +
                              "Syeda Laiba Nadeem (2023-ag-10014)\n" +
                              "Areesha Ramzan (2023-ag-9929)";
            creditsLabel.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            creditsLabel.ForeColor = Color.FromArgb(80, 80, 80);
            creditsLabel.AutoSize = true;
            creditsLabel.Location = new Point(320, 500);
            creditsLabel.TextAlign = ContentAlignment.MiddleCenter;
            mainPanel.Controls.Add(creditsLabel);

            Panel footerPanel = new Panel();
            footerPanel.BackColor = Color.FromArgb(0, 123, 255);
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 50;

            Label footerLabel = new Label();
            footerLabel.Text = "© 2023 Pharmacy Management System - v2.0";
            footerLabel.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            footerLabel.ForeColor = Color.White;
            footerLabel.AutoSize = false;
            footerLabel.Dock = DockStyle.Fill;
            footerLabel.TextAlign = ContentAlignment.MiddleCenter;

            footerPanel.Controls.Add(footerLabel);

            this.Controls.Add(mainPanel);
            this.Controls.Add(footerPanel);
        }

        private Button CreateModuleButton(string text, string icon, Color color, Point location)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.ForeColor = Color.White;
            btn.BackColor = color;
            btn.Size = new Size(300, 180);
            btn.Location = location;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.TextImageRelation = TextImageRelation.ImageAboveText;
            btn.Padding = new Padding(0, 10, 0, 10);

            Label btnIcon = new Label();
            btnIcon.Text = icon;
            btnIcon.Font = new Font("Segoe UI", 36, FontStyle.Regular);
            btnIcon.AutoSize = true;
            btnIcon.Location = new Point(120, 15);
            btnIcon.TextAlign = ContentAlignment.MiddleCenter;
            btn.Controls.Add(btnIcon);

            return btn;
        }

        private void PharmacyBtn_Click(object sender, EventArgs e)
        {
            Pharmacy pharmacy = new Pharmacy();
            pharmacy.Show();
            
        }

        private void InventoryBtn_Click(object sender, EventArgs e)
        {
            Inventory inventory = new Inventory();
            inventory.Show();
            this.Close();
        }

        private void SalesBtn_Click(object sender, EventArgs e)
        {
            Sales salesForm = new Sales(@"C:\Users\husna\source\repos\Pharmacy Management System-v2\Pharmacy Management System-v2\PMS.accdb");
            salesForm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}