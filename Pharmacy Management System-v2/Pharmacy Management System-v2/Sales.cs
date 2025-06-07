using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace Pharmacy_Management_System_v2
{
    public partial class Sales : Form
    {
        
        private string connectionString;
        private DataTable salesHeaderTable;
        private DataTable salesDetailsTable;


        private DataGridView dgvSales;
        private DataGridView dgvSalesDetails;
        private Label lblTitle;
        private Label lblSalesDetails;
        private Label lblStartDate;
        private Label lblEndDate;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnFilter;
        private Button btnRefresh;
        private Button btnPrintInvoice;
        private Label lblRecentSales;
        private NumericUpDown nudRecentSales;

        public Sales(string databasePath)
        {
            InitializeCustomComponents();
            connectionString = $@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={databasePath};Persist Security Info=False;";
            LoadRecentSales(100);
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Sales Management";
            this.ClientSize = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9);

            
            lblTitle = new Label();
            lblTitle.Text = "Sales";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            lblRecentSales = new Label();
            lblRecentSales.Text = "Load recent sales:";
            lblRecentSales.Location = new Point(20, 60);
            lblRecentSales.AutoSize = true;
            this.Controls.Add(lblRecentSales);

            nudRecentSales = new NumericUpDown();
            nudRecentSales.Minimum = 1;
            nudRecentSales.Maximum = 1000;
            nudRecentSales.Value = 100;
            nudRecentSales.Location = new Point(120, 55);
            nudRecentSales.Width = 60;
            this.Controls.Add(nudRecentSales);

            lblStartDate = new Label();
            lblStartDate.Text = "Start Date:";
            lblStartDate.Location = new Point(200, 60);
            lblStartDate.AutoSize = true;
            this.Controls.Add(lblStartDate);

            dtpStartDate = new DateTimePicker();
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Location = new Point(270, 55);
            dtpStartDate.Width = 100;
            this.Controls.Add(dtpStartDate);

            lblEndDate = new Label();
            lblEndDate.Text = "End Date:";
            lblEndDate.Location = new Point(390, 60);
            lblEndDate.AutoSize = true;
            this.Controls.Add(lblEndDate);

            dtpEndDate = new DateTimePicker();
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Location = new Point(460, 55);
            dtpEndDate.Width = 100;
            this.Controls.Add(dtpEndDate);

            btnFilter = new Button();
            btnFilter.Text = "Filter by Date";
            btnFilter.Location = new Point(580, 55);
            btnFilter.Width = 100;
            btnFilter.Click += btnFilter_Click;
            this.Controls.Add(btnFilter);

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh Recent";
            btnRefresh.Location = new Point(690, 55);
            btnRefresh.Width = 100;
            btnRefresh.Click += btnRefresh_Click;
            this.Controls.Add(btnRefresh);

            
            dgvSales = new DataGridView();
            dgvSales.Location = new Point(20, 90);
            dgvSales.Size = new Size(860, 250);
            dgvSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSales.MultiSelect = false;
            dgvSales.ReadOnly = true;
            dgvSales.AllowUserToAddRows = false;
            dgvSales.AllowUserToDeleteRows = false;
            dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSales.SelectionChanged += dgvSales_SelectionChanged;
            this.Controls.Add(dgvSales);

     
            lblSalesDetails = new Label();
            lblSalesDetails.Text = "Sales Details:";
            lblSalesDetails.Location = new Point(20, 350);
            lblSalesDetails.AutoSize = true;
            this.Controls.Add(lblSalesDetails);

          
            btnPrintInvoice = new Button();
            btnPrintInvoice.Text = "Print Invoice";
            btnPrintInvoice.Location = new Point(750, 345);
            btnPrintInvoice.Width = 120;
            btnPrintInvoice.Click += btnPrintInvoice_Click;
            this.Controls.Add(btnPrintInvoice);

            dgvSalesDetails = new DataGridView();
            dgvSalesDetails.Location = new Point(20, 380);
            dgvSalesDetails.Size = new Size(860, 240);
            dgvSalesDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSalesDetails.MultiSelect = false;
            dgvSalesDetails.ReadOnly = true;
            dgvSalesDetails.AllowUserToAddRows = false;
            dgvSalesDetails.AllowUserToDeleteRows = false;
            dgvSalesDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(dgvSalesDetails);

            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Today;
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            LoadRecentSales((int)nudRecentSales.Value);
        }

        private void LoadRecentSales(int recordCount)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string headerQuery = $"SELECT TOP {recordCount} SaleID, InvoiceNumber, SaleDate, TotalAmount FROM SalesHeader ORDER BY SaleDate DESC, SaleID DESC";
                    OleDbDataAdapter headerAdapter = new OleDbDataAdapter(headerQuery, connection);
                    salesHeaderTable = new DataTable();
                    headerAdapter.Fill(salesHeaderTable);

                    
                    if (salesHeaderTable.Rows.Count > 0)
                    {
                        string saleIDs = string.Join(",", salesHeaderTable.AsEnumerable().Select(row => row["SaleID"].ToString()));
                        string detailsQuery = $@"SELECT sd.DetailID, sd.SaleID, m.NAME AS MedicineName, 
                                               sd.Quantity, sd.UnitPrice, sd.Subtotal 
                                               FROM SalesDetails sd 
                                               INNER JOIN Medicines m ON sd.MedicineID = m.ID 
                                               WHERE sd.SaleID IN ({saleIDs})";

                        OleDbDataAdapter detailsAdapter = new OleDbDataAdapter(detailsQuery, connection);
                        salesDetailsTable = new DataTable();
                        detailsAdapter.Fill(salesDetailsTable);
                    }
                    else
                    {
                        salesDetailsTable = new DataTable();
                    }

                    dgvSales.DataSource = salesHeaderTable;

                 
                    if (dgvSales.Columns.Contains("SaleDate"))
                    {
                        dgvSales.Columns["SaleDate"].DefaultCellStyle.Format = "g";
                    }
                    if (dgvSales.Columns.Contains("TotalAmount"))
                    {
                        dgvSales.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
                    }

                    if (dgvSales.Rows.Count > 0)
                    {
                        dgvSales.Rows[0].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recent sales data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSales_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count > 0)
            {
                int saleID = Convert.ToInt32(dgvSales.SelectedRows[0].Cells["SaleID"].Value);
                LoadSalesDetails(saleID);
            }
        }

        private void LoadSalesDetails(int saleID)
        {
            try
            {
                DataView detailsView = new DataView(salesDetailsTable);
                detailsView.RowFilter = $"SaleID = {saleID}";
                dgvSalesDetails.DataSource = detailsView;

                
                if (dgvSalesDetails.Columns.Contains("UnitPrice"))
                {
                    dgvSalesDetails.Columns["UnitPrice"].DefaultCellStyle.Format = "C2";
                }
                if (dgvSalesDetails.Columns.Contains("Subtotal"))
                {
                    dgvSalesDetails.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
                }

               
                if (dgvSalesDetails.Columns.Contains("DetailID"))
                {
                    dgvSalesDetails.Columns["DetailID"].Visible = false;
                }
                if (dgvSalesDetails.Columns.Contains("SaleID"))
                {
                    dgvSalesDetails.Columns["SaleID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string headerQuery = "SELECT SaleID, InvoiceNumber, SaleDate, TotalAmount FROM SalesHeader " +
                                       "WHERE SaleDate BETWEEN @StartDate AND @EndDate " +
                                       "ORDER BY SaleDate DESC, SaleID DESC";

                    OleDbDataAdapter headerAdapter = new OleDbDataAdapter(headerQuery, connection);
                    headerAdapter.SelectCommand.Parameters.AddWithValue("@StartDate", startDate);
                    headerAdapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);

                    salesHeaderTable = new DataTable();
                    headerAdapter.Fill(salesHeaderTable);

                    if (salesHeaderTable.Rows.Count > 0)
                    {
                        string saleIDs = string.Join(",", salesHeaderTable.AsEnumerable().Select(row => row["SaleID"].ToString()));
                        string detailsQuery = $@"SELECT sd.DetailID, sd.SaleID, m.NAME AS MedicineName, 
                                               sd.Quantity, sd.UnitPrice, sd.Subtotal 
                                               FROM SalesDetails sd 
                                               INNER JOIN Medicines m ON sd.MedicineID = m.ID 
                                               WHERE sd.SaleID IN ({saleIDs})";

                        OleDbDataAdapter detailsAdapter = new OleDbDataAdapter(detailsQuery, connection);
                        salesDetailsTable = new DataTable();
                        detailsAdapter.Fill(salesDetailsTable);
                    }
                    else
                    {
                        salesDetailsTable = new DataTable();
                    }

                    dgvSales.DataSource = salesHeaderTable;

                    if (dgvSales.Rows.Count > 0)
                    {
                        dgvSales.Rows[0].Selected = true;
                    }
                    else
                    {
                        dgvSalesDetails.DataSource = null;
                        
                        MessageBox.Show("No sales found for the selected date range.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filter: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecentSales((int)nudRecentSales.Value);
        }

        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count > 0)
            {
                int saleID = Convert.ToInt32(dgvSales.SelectedRows[0].Cells["SaleID"].Value);
                string invoiceNumber = dgvSales.SelectedRows[0].Cells["InvoiceNumber"].Value.ToString();
                string saleDate = Convert.ToDateTime(dgvSales.SelectedRows[0].Cells["SaleDate"].Value).ToString("g");
                decimal totalAmount = Convert.ToDecimal(dgvSales.SelectedRows[0].Cells["TotalAmount"].Value);

                string invoicePreview = $"INVOICE #{invoiceNumber}\n" +
                                       $"Date: {saleDate}\n" +
                                       $"Sale ID: {saleID}\n\n" +
                                       "Items:\n";

                foreach (DataGridViewRow row in dgvSalesDetails.Rows)
                {
                    if (row.IsNewRow) continue;
                    invoicePreview += $"- {row.Cells["MedicineName"].Value} x {row.Cells["Quantity"].Value} @ {Convert.ToDecimal(row.Cells["UnitPrice"].Value):C2} = {Convert.ToDecimal(row.Cells["Subtotal"].Value):C2}\n";
                }

                invoicePreview += $"\nTOTAL AMOUNT: {totalAmount:C2}";

                MessageBox.Show(invoicePreview, "Invoice Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a sale to print its invoice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}