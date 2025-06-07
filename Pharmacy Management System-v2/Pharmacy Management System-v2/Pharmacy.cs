using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Pharmacy_Management_System_v2
{
    partial class Pharmacy : Form
    {
   
        private DataGridView dgvMedicines;
        private DataGridView dgvCart;
        private NumericUpDown nudQuantity;
        private TextBox txtSearch;
        private Label lblTotal;
        private Button btnAddToCart;
        private Button btnSell;
        private Label lblSearch;
        private Label lblQuantity;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private Panel panelControls;
        private Panel panelCart;


        private OleDbConnection connection;
        private DataTable medicinesTable;
        private DataTable cartTable;
        private decimal totalAmount = 0;
        private int invoiceCounter = 1000; // Starting invoice number

        public Pharmacy()
        {
            InitializeCustomComponents();
            InitializeConnection();
            InitializeCartTable();
            SetupDataGridViews();
            this.Load += Pharmacy_Load;
        }

        private void InitializeCustomComponents()
        {
            
            this.Text = "Pharmacy Management System";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            statusStrip.Items.Add(lblStatus);
            this.Controls.Add(statusStrip);


            panelControls = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.LightGray
            };

            panelCart = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            this.Controls.Add(panelCart);
            this.Controls.Add(panelControls);

            // Search controls
            lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            txtSearch = new TextBox
            {
                Location = new Point(80, 17),
                Size = new Size(200, 23)
            };
            txtSearch.TextChanged += txtSearch_TextChanged;

            
            lblQuantity = new Label
            {
                Text = "Quantity:",
                Location = new Point(300, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            nudQuantity = new NumericUpDown
            {
                Location = new Point(370, 17),
                Size = new Size(80, 23),
                Minimum = 1,
                Maximum = 1000
            };

            btnAddToCart = new Button
            {
                Text = "Add to Cart",
                Location = new Point(470, 16),
                Size = new Size(100, 25),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddToCart.Click += btnAddToCart_Click;


            dgvMedicines = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

          
            var cartLabel = new Label
            {
                Text = "Shopping Cart",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

          
            var totalPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.LightGray
            };

            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            btnSell = new Button
            {
                Text = "Complete Sale",
                Dock = DockStyle.Right,
                Size = new Size(120, 30),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 10, 10, 10)
            };
            btnSell.Click += btnSell_Click;

       
            panelControls.Controls.Add(lblSearch);
            panelControls.Controls.Add(txtSearch);
            panelControls.Controls.Add(lblQuantity);
            panelControls.Controls.Add(nudQuantity);
            panelControls.Controls.Add(btnAddToCart);

            var medicinesPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 300,
                Padding = new Padding(10)
            };
            medicinesPanel.Controls.Add(dgvMedicines);

            var cartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            cartPanel.Controls.Add(dgvCart);
            cartPanel.Controls.Add(cartLabel);

            totalPanel.Controls.Add(lblTotal);
            totalPanel.Controls.Add(btnSell);

            panelCart.Controls.Add(cartPanel);
            panelCart.Controls.Add(totalPanel);
            panelCart.Controls.Add(medicinesPanel);
        }

        private void InitializeConnection()
        {
            try
            {
                string[] possiblePaths = {
                    Path.Combine(Application.StartupPath, "PMS.accdb"),
                    Path.Combine(Application.StartupPath, "..\\..\\PMS.accdb"),
                    @"C:\Users\husna\source\repos\Pharmacy Management System-v2\Pharmacy Management System-v2\PMS.accdb"
                };

                string connectionString = null;

                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        connectionString = $@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={path};Persist Security Info=False;";
                        break;
                    }
                }

                if (connectionString == null)
                {
                    MessageBox.Show("Database file not found. Please ensure PMS.accdb exists in the application directory.",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                connection = new OleDbConnection(connectionString);
                lblStatus.Text = "Database connection initialized";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database connection: {ex.Message}",
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void InitializeCartTable()
        {
            cartTable = new DataTable();
            cartTable.Columns.Add("MedicineID", typeof(int));
            cartTable.Columns.Add("Name", typeof(string));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("Price", typeof(decimal));
            cartTable.Columns.Add("Subtotal", typeof(decimal));
            dgvCart.DataSource = cartTable;
        }

        private void SetupDataGridViews()
        {
          
            dgvMedicines.AutoGenerateColumns = false;
            dgvMedicines.Columns.Clear();

         
            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ID",
                HeaderText = "ID",
                Name = "colID",
                Width = 50
            });

            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "NAME",
                HeaderText = "Medicine Name",
                Name = "colName",
                Width = 200
            });

            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "CATEGORY",
                HeaderText = "Category",
                Name = "colCategory",
                Width = 120
            });

            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "QUANTITY",
                HeaderText = "In Stock",
                Name = "colQuantity",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvMedicines.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "PRICE",
                HeaderText = "Price",
                Name = "colPrice",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });


            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Name",
                HeaderText = "Medicine Name",
                Name = "colName",
                ReadOnly = true,
                Width = 200
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Quantity",
                HeaderText = "Quantity",
                Name = "colQuantity",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Price",
                HeaderText = "Unit Price",
                Name = "colPrice",
                ReadOnly = true,
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Subtotal",
                HeaderText = "Subtotal",
                Name = "colSubtotal",
                ReadOnly = true,
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

        
            var deleteButton = new DataGridViewButtonColumn()
            {
                Text = "Remove",
                UseColumnTextForButtonValue = true,
                Name = "colDelete",
                Width = 80,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.IndianRed,
                    ForeColor = Color.White,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
           };
            dgvCart.Columns.Add(deleteButton);

            
            dgvCart.CellValueChanged += dgvCart_CellValueChanged;
            dgvCart.CellContentClick += dgvCart_CellContentClick;
            dgvMedicines.SelectionChanged += dgvMedicines_SelectionChanged;
        }

        private void dgvMedicines_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMedicines.SelectedRows.Count > 0)
            {
                var selectedRow = dgvMedicines.SelectedRows[0];
                int availableQuantity = Convert.ToInt32(selectedRow.Cells["colQuantity"].Value);
                nudQuantity.Maximum = availableQuantity;
                nudQuantity.Value = 1;
            }
        }

        private void Pharmacy_Load(object sender, EventArgs e)
        {
            LoadMedicines();
        }

        private void LoadMedicines()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                lblStatus.Text = "Loading medicines...";

                string query = "SELECT ID, NAME, CATEGORY, QUANTITY, PRICE FROM Medicines WHERE QUANTITY > 0 ORDER BY NAME";
                var command = new OleDbCommand(query, connection);
                var adapter = new OleDbDataAdapter(command);

                medicinesTable = new DataTable();
                adapter.Fill(medicinesTable);

                dgvMedicines.DataSource = medicinesTable;

                lblStatus.Text = $"Loaded {medicinesTable.Rows.Count} medicines";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading medicines";
                MessageBox.Show("Error loading medicines: " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvMedicines.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a medicine from the list.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nudQuantity.Value <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.",
                    "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvMedicines.SelectedRows[0];
            int medicineId = Convert.ToInt32(selectedRow.Cells["colID"].Value);
            string medicineName = selectedRow.Cells["colName"].Value.ToString();
            decimal price = Convert.ToDecimal(selectedRow.Cells["colPrice"].Value);
            int availableQuantity = Convert.ToInt32(selectedRow.Cells["colQuantity"].Value);
            int requestedQuantity = (int)nudQuantity.Value;

            if (requestedQuantity > availableQuantity)
            {
                MessageBox.Show($"Only {availableQuantity} items available in stock.",
                    "Stock Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
            DataRow[] existingRows = cartTable.Select($"MedicineID = {medicineId}");

            if (existingRows.Length > 0)
            {
               
                int currentQuantity = Convert.ToInt32(existingRows[0]["Quantity"]);
                if (currentQuantity + requestedQuantity > availableQuantity)
                {
                    MessageBox.Show($"Cannot add more than available quantity. Already have {currentQuantity} in cart.",
                        "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                existingRows[0]["Quantity"] = currentQuantity + requestedQuantity;
                existingRows[0]["Subtotal"] = (currentQuantity + requestedQuantity) * price;
            }
            else
            {
               
                DataRow newRow = cartTable.NewRow();
                newRow["MedicineID"] = medicineId;
                newRow["Name"] = medicineName;
                newRow["Quantity"] = requestedQuantity;
                newRow["Price"] = price;
                newRow["Subtotal"] = requestedQuantity * price;
                cartTable.Rows.Add(newRow);
            }

            CalculateTotal();
            nudQuantity.Value = 1;
        }

        private void CalculateTotal()
        {
            totalAmount = 0;
            foreach (DataRow row in cartTable.Rows)
            {
                totalAmount += Convert.ToDecimal(row["Subtotal"]);
            }

            lblTotal.Text = $"Total: {totalAmount:C}";
        }

        private void dgvCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvCart.Columns["colQuantity"].Index)
            {
                try
                {
                 
                    int newQuantity = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells["colQuantity"].Value);
                    decimal price = Convert.ToDecimal(dgvCart.Rows[e.RowIndex].Cells["colPrice"].Value);

              
                    int medicineId = Convert.ToInt32(cartTable.Rows[e.RowIndex]["MedicineID"]);
                    DataRow medicineRow = medicinesTable.Select($"ID = {medicineId}")[0];
                    int availableQuantity = Convert.ToInt32(medicineRow["QUANTITY"]);

                    if (newQuantity > availableQuantity)
                    {
                        MessageBox.Show($"Only {availableQuantity} items available in stock.",
                            "Stock Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgvCart.Rows[e.RowIndex].Cells["colQuantity"].Value = availableQuantity;
                        newQuantity = availableQuantity;
                    }

                    dgvCart.Rows[e.RowIndex].Cells["colSubtotal"].Value = newQuantity * price;
                    CalculateTotal();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating quantity: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCart.Columns["colDelete"].Index && e.RowIndex >= 0)
            {
                // Remove item from cart
                cartTable.Rows.RemoveAt(e.RowIndex);
                CalculateTotal();
            }
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty. Add medicines to cart before selling.",
                    "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                connection.Open();
                lblStatus.Text = "Processing sale...";

                GenerateInvoice();

                cartTable.Rows.Clear();
                totalAmount = 0;
                lblTotal.Text = "Total: $0.00";
                LoadMedicines();

                lblStatus.Text = "Sale completed successfully!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error processing sale";
                MessageBox.Show("Error processing sale: " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void GenerateInvoice()
        {
            try
            {
                StringBuilder invoice = new StringBuilder();
                invoice.AppendLine("PHARMACY INVOICE");
                invoice.AppendLine("----------------");
                invoice.AppendLine($"Invoice #: {invoiceCounter++}");
                invoice.AppendLine($"Date: {DateTime.Now:g}");
                invoice.AppendLine();
                invoice.AppendLine("Items Purchased:");
                invoice.AppendLine();

                foreach (DataRow cartRow in cartTable.Rows)
                {
                    int medicineId = Convert.ToInt32(cartRow["MedicineID"]);
                    string medicineName = cartRow["Name"].ToString();
                    int quantitySold = Convert.ToInt32(cartRow["Quantity"]);
                    decimal price = Convert.ToDecimal(cartRow["Price"]);
                    decimal subtotal = Convert.ToDecimal(cartRow["Subtotal"]);

                    
                    invoice.AppendLine($"{medicineName} - {quantitySold} x {price:C} = {subtotal:C}");

                    string updateQuery = "UPDATE Medicines SET QUANTITY = QUANTITY - @Quantity WHERE ID = @MedicineID";
                    var updateCommand = new OleDbCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@Quantity", quantitySold);
                    updateCommand.Parameters.AddWithValue("@MedicineID", medicineId);
                    updateCommand.ExecuteNonQuery();
                }

                invoice.AppendLine();
                invoice.AppendLine($"TOTAL AMOUNT: {totalAmount:C}");
                invoice.AppendLine();
                invoice.AppendLine("Thank you for your purchase!");

               
                RecordSale();

                
                MessageBox.Show(invoice.ToString(), "Invoice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Invoice generated and sale recorded";
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating invoice: " + ex.Message);
            }
        }
        private void RecordSale()
        {
            OleDbTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                string headerQuery = @"INSERT INTO SalesHeader (InvoiceNumber, SaleDate, TotalAmount) 
                             VALUES (?, ?, ?)";

                var headerCmd = new OleDbCommand(headerQuery, connection, transaction);
                headerCmd.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceCounter - 1;
                headerCmd.Parameters.Add("@SaleDate", OleDbType.Date).Value = DateTime.Now;
                headerCmd.Parameters.Add("@TotalAmount", OleDbType.Currency).Value = totalAmount;
                headerCmd.ExecuteNonQuery();

         
                var getIDCmd = new OleDbCommand("SELECT @@IDENTITY", connection, transaction);
                int saleID = Convert.ToInt32(getIDCmd.ExecuteScalar());

                foreach (DataRow cartRow in cartTable.Rows)
                {
                    string detailQuery = @"INSERT INTO SalesDetails 
                                (SaleID, MedicineID, Quantity, UnitPrice, Subtotal)
                                VALUES (?, ?, ?, ?, ?)";

                    var detailCmd = new OleDbCommand(detailQuery, connection, transaction);
                    detailCmd.Parameters.Add("@SaleID", OleDbType.Integer).Value = saleID;
                    detailCmd.Parameters.Add("@MedicineID", OleDbType.Integer).Value = Convert.ToInt32(cartRow["MedicineID"]);
                    detailCmd.Parameters.Add("@Quantity", OleDbType.Integer).Value = Convert.ToInt32(cartRow["Quantity"]);
                    detailCmd.Parameters.Add("@UnitPrice", OleDbType.Currency).Value = Convert.ToDecimal(cartRow["Price"]);
                    detailCmd.Parameters.Add("@Subtotal", OleDbType.Currency).Value = Convert.ToDecimal(cartRow["Subtotal"]);
                    detailCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                lblStatus.Text = "Sale recorded successfully";
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                lblStatus.Text = "Error recording sale";
                MessageBox.Show($"Error recording sale: {ex.Message}\n\n{ex.StackTrace}",
                               "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (medicinesTable != null)
            {
                string searchText = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchText))
                {
                    dgvMedicines.DataSource = medicinesTable;
                }
                else
                {
                    var dv = medicinesTable.DefaultView;
                    dv.RowFilter = $"NAME LIKE '%{searchText}%' OR CATEGORY LIKE '%{searchText}%'";
                    dgvMedicines.DataSource = dv.ToTable();
                }
            }
        }
    }
}