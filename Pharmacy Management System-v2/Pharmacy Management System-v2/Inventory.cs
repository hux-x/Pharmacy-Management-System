using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace Pharmacy_Management_System_v2
{
    public partial class Inventory : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=C:\Users\husna\source\repos\Pharmacy Management System-v2\Pharmacy Management System-v2\PMS.accdb;";
        private DataGridView dataGridView1;
        private TextBox searchBox;

        public Inventory()
        {
            InitializeComponent();
            SetupForm();
            LoadData();
        }

        private void SetupForm()
        {
            this.Text = "Pharmacy Inventory Management";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 245, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(20);
            this.Controls.Add(mainPanel);

            Label headerLabel = new Label();
            headerLabel.Text = "MEDICINE INVENTORY";
            headerLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            headerLabel.ForeColor = Color.FromArgb(0, 123, 255);
            headerLabel.AutoSize = true;
            headerLabel.Location = new Point(20, 20);
            mainPanel.Controls.Add(headerLabel);

  
            Panel searchPanel = new Panel();
            searchPanel.BackColor = Color.White;
            searchPanel.BorderStyle = BorderStyle.FixedSingle;
            searchPanel.Size = new Size(940, 60);
            searchPanel.Location = new Point(20, 70);
            mainPanel.Controls.Add(searchPanel);

     
            searchBox = new TextBox();
            searchBox.Font = new Font("Segoe UI", 10);
            searchBox.Size = new Size(300, 30);
            searchBox.Location = new Point(10, 15);
            searchBox.TextChanged += SearchBox_TextChanged;
            searchPanel.Controls.Add(searchBox);

            searchBox.GotFocus += (s, e) => {
                if (searchBox.Text == "Search medicines...") searchBox.Text = "";
            };
            searchBox.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(searchBox.Text)) searchBox.Text = "Search medicines...";
            };
            searchBox.Text = "Search medicines...";

            Button searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            searchButton.BackColor = Color.FromArgb(0, 123, 255);
            searchButton.ForeColor = Color.White;
            searchButton.FlatStyle = FlatStyle.Flat;
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Size = new Size(100, 30);
            searchButton.Location = new Point(320, 15);
            searchButton.Cursor = Cursors.Hand;
            searchButton.Click += SearchButton_Click;
            searchPanel.Controls.Add(searchButton);

  
            dataGridView1 = new DataGridView();
            dataGridView1.Location = new Point(20, 150);
            dataGridView1.Size = new Size(940, 400);
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 123, 255);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            mainPanel.Controls.Add(dataGridView1);

            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(940, 60);
            buttonPanel.Location = new Point(20, 570);
            mainPanel.Controls.Add(buttonPanel);

          
            Button addButton = CreateActionButton("Add New", Color.FromArgb(40, 167, 69), new Point(0, 0));
            addButton.Click += AddButton_Click;
            buttonPanel.Controls.Add(addButton);

            Button editButton = CreateActionButton("Edit", Color.FromArgb(255, 193, 7), new Point(150, 0));
            editButton.Click += EditButton_Click;
            buttonPanel.Controls.Add(editButton);

            Button deleteButton = CreateActionButton("Delete", Color.FromArgb(220, 53, 69), new Point(300, 0));
            deleteButton.Click += DeleteButton_Click;
            buttonPanel.Controls.Add(deleteButton);

            Button refreshButton = CreateActionButton("Refresh", Color.FromArgb(108, 117, 125), new Point(450, 0));
            refreshButton.Click += RefreshButton_Click;
            buttonPanel.Controls.Add(refreshButton);

            Button backButton = CreateActionButton("Back to Main", Color.FromArgb(23, 162, 184), new Point(750, 0));
            backButton.Click += BackButton_Click;
            buttonPanel.Controls.Add(backButton);
        }

        private Button CreateActionButton(string text, Color color, Point location)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.ForeColor = Color.White;
            button.BackColor = color;
            button.Size = new Size(140, 40);
            button.Location = location;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            return button;
        }

        private void LoadData()
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand("SELECT * FROM Medicines", connection);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    SetupDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["ID"].Visible = false;
                dataGridView1.Columns["NAME"].HeaderText = "Medicine Name";
                dataGridView1.Columns["CATEGORY"].HeaderText = "Category";
                dataGridView1.Columns["QUANTITY"].HeaderText = "Quantity";
                dataGridView1.Columns["PRICE"].HeaderText = "Price";
                dataGridView1.Columns["EXPIRYDATE"].HeaderText = "Expiry Date";
                dataGridView1.Columns["QUANTITY_PILL_PER_LEAF"].HeaderText = "Pills/Leaf";
                dataGridView1.Columns["NUMBER_OF_LEAFS_PER_BOX"].HeaderText = "Leafs/Box";

                dataGridView1.Columns["PRICE"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["EXPIRYDATE"].DefaultCellStyle.Format = "dd-MMM-yyyy";
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text != "Search medicines..." && !string.IsNullOrEmpty(searchBox.Text))
            {
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        OleDbCommand command = new OleDbCommand(
                            "SELECT * FROM Medicines WHERE NAME LIKE @SearchText OR CATEGORY LIKE @SearchText",
                            connection);
                        command.Parameters.AddWithValue("@SearchText", "%" + searchBox.Text + "%");

                        OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error searching: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (string.IsNullOrEmpty(searchBox.Text))
            {
                LoadData();
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchBox_TextChanged(searchBox, e);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddEditMedicineForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (OleDbConnection connection = new OleDbConnection(connectionString))
                        {
                            connection.Open();
                            OleDbCommand command = new OleDbCommand(
                                "INSERT INTO Medicines (NAME, CATEGORY, QUANTITY, PRICE, EXPIRYDATE, QUANTITY_PILL_PER_LEAF, NUMBER_OF_LEAFS_PER_BOX) " +
                                "VALUES (@Name, @Category, @Quantity, @Price, @ExpiryDate, @PillsPerLeaf, @LeafsPerBox)",
                                connection);

                            command.Parameters.AddWithValue("@Name", addForm.MedicineName);
                            command.Parameters.AddWithValue("@Category", addForm.Category);
                            command.Parameters.AddWithValue("@Quantity", addForm.Quantity);
                            command.Parameters.AddWithValue("@Price", addForm.Price);
                            command.Parameters.AddWithValue("@ExpiryDate", addForm.ExpiryDate);
                            command.Parameters.AddWithValue("@PillsPerLeaf", addForm.PillsPerLeaf);
                            command.Parameters.AddWithValue("@LeafsPerBox", addForm.LeafsPerBox);

                            command.ExecuteNonQuery();
                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding medicine: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                int id = Convert.ToInt32(row.Cells["ID"].Value);

                using (var editForm = new AddEditMedicineForm(
                    id,
                    row.Cells["NAME"].Value.ToString(),
                    row.Cells["CATEGORY"].Value.ToString(),
                    Convert.ToInt32(row.Cells["QUANTITY"].Value),
                    Convert.ToDecimal(row.Cells["PRICE"].Value),
                    Convert.ToDateTime(row.Cells["EXPIRYDATE"].Value),
                    Convert.ToInt32(row.Cells["QUANTITY_PILL_PER_LEAF"].Value),
                    Convert.ToInt32(row.Cells["NUMBER_OF_LEAFS_PER_BOX"].Value)))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (OleDbConnection connection = new OleDbConnection(connectionString))
                            {
                                connection.Open();
                                OleDbCommand command = new OleDbCommand(
                                    "UPDATE Medicines SET " +
                                    "NAME = @Name, " +
                                    "CATEGORY = @Category, " +
                                    "QUANTITY = @Quantity, " +
                                    "PRICE = @Price, " +
                                    "EXPIRYDATE = @ExpiryDate, " +
                                    "QUANTITY_PILL_PER_LEAF = @PillsPerLeaf, " +
                                    "NUMBER_OF_LEAFS_PER_BOX = @LeafsPerBox " +
                                    "WHERE ID = @ID",
                                    connection);

                                command.Parameters.AddWithValue("@Name", editForm.MedicineName);
                                command.Parameters.AddWithValue("@Category", editForm.Category);
                                command.Parameters.AddWithValue("@Quantity", editForm.Quantity);
                                command.Parameters.AddWithValue("@Price", editForm.Price);
                                command.Parameters.AddWithValue("@ExpiryDate", editForm.ExpiryDate);
                                command.Parameters.AddWithValue("@PillsPerLeaf", editForm.PillsPerLeaf);
                                command.Parameters.AddWithValue("@LeafsPerBox", editForm.LeafsPerBox);
                                command.Parameters.AddWithValue("@ID", id);

                                command.ExecuteNonQuery();
                                LoadData();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating medicine: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a medicine to edit.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this medicine?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                    try
                    {
                        using (OleDbConnection connection = new OleDbConnection(connectionString))
                        {
                            connection.Open();
                            OleDbCommand command = new OleDbCommand("DELETE FROM Medicines WHERE ID=@ID", connection);
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting record: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a medicine to delete.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }
    }

    public class AddEditMedicineForm : Form
    {
        public string MedicineName { get; private set; }
        public string Category { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public int PillsPerLeaf { get; private set; }
        public int LeafsPerBox { get; private set; }

        private TextBox nameTextBox;
        private TextBox categoryTextBox;
        private NumericUpDown quantityNumeric;
        private TextBox priceTextBox;
        private DateTimePicker expiryPicker;
        private NumericUpDown pillsNumeric;
        private NumericUpDown leafsNumeric;

        public AddEditMedicineForm() : this(0, "", "", 0, 0, DateTime.Now.AddYears(1), 0, 0) { }

        public AddEditMedicineForm(int id, string name, string category, int quantity, decimal price,
            DateTime expiryDate, int pillsPerLeaf, int leafsPerBox)
        {
            this.Text = id == 0 ? "Add New Medicine" : "Edit Medicine";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(20);
            this.Controls.Add(mainPanel);

      
            Label nameLabel = new Label()
            {
                Text = "Medicine Name:",
                AutoSize = true,
                Location = new Point(20, 20)
            };
            nameTextBox = new TextBox()
            {
                Size = new Size(400, 30),
                Location = new Point(20, 50)
            };
            mainPanel.Controls.Add(nameLabel);
            mainPanel.Controls.Add(nameTextBox);

            Label categoryLabel = new Label()
            {
                Text = "Category:",
                AutoSize = true,
                Location = new Point(20, 90)
            };
            categoryTextBox = new TextBox()
            {
                Size = new Size(400, 30),
                Location = new Point(20, 120)
            };
            mainPanel.Controls.Add(categoryLabel);
            mainPanel.Controls.Add(categoryTextBox);


            Label quantityLabel = new Label()
            {
                Text = "Quantity:",
                AutoSize = true,
                Location = new Point(20, 160)
            };
            quantityNumeric = new NumericUpDown()
            {
                Size = new Size(150, 30),
                Location = new Point(20, 190),
                Minimum = 0,
                Maximum = 10000
            };
            mainPanel.Controls.Add(quantityLabel);
            mainPanel.Controls.Add(quantityNumeric);

            Label priceLabel = new Label()
            {
                Text = "Price:",
                AutoSize = true,
                Location = new Point(200, 160)
            };
            priceTextBox = new TextBox()
            {
                Size = new Size(150, 30),
                Location = new Point(200, 190)
            };
            mainPanel.Controls.Add(priceLabel);
            mainPanel.Controls.Add(priceTextBox);

            Label expiryLabel = new Label()
            {
                Text = "Expiry Date:",
                AutoSize = true,
                Location = new Point(20, 230)
            };
            expiryPicker = new DateTimePicker()
            {
                Size = new Size(150, 30),
                Location = new Point(20, 260),
                Format = DateTimePickerFormat.Short
            };
            mainPanel.Controls.Add(expiryLabel);
            mainPanel.Controls.Add(expiryPicker);

            Label pillsLabel = new Label()
            {
                Text = "Pills per leaf:",
                AutoSize = true,
                Location = new Point(200, 230)
            };
            pillsNumeric = new NumericUpDown()
            {
                Size = new Size(150, 30),
                Location = new Point(200, 260),
                Minimum = 0,
                Maximum = 1000
            };
            mainPanel.Controls.Add(pillsLabel);
            mainPanel.Controls.Add(pillsNumeric);


            Label leafsLabel = new Label()
            {
                Text = "Leafs per box:",
                AutoSize = true,
                Location = new Point(20, 300)
            };
            leafsNumeric = new NumericUpDown()
            {
                Size = new Size(150, 30),
                Location = new Point(20, 330),
                Minimum = 0,
                Maximum = 1000
            };
            mainPanel.Controls.Add(leafsLabel);
            mainPanel.Controls.Add(leafsNumeric);


            Button saveButton = new Button()
            {
                Text = "Save",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Size = new Size(100, 40),
                Location = new Point(150, 380),
                DialogResult = DialogResult.OK
            };
            saveButton.Click += (s, e) => {
                if (ValidateInputs())
                {
                    MedicineName = nameTextBox.Text;
                    Category = categoryTextBox.Text;
                    Quantity = (int)quantityNumeric.Value;
                    Price = decimal.Parse(priceTextBox.Text);
                    ExpiryDate = expiryPicker.Value;
                    PillsPerLeaf = (int)pillsNumeric.Value;
                    LeafsPerBox = (int)leafsNumeric.Value;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            };

            Button cancelButton = new Button()
            {
                Text = "Cancel",
                Size = new Size(100, 40),
                Location = new Point(270, 380),
                DialogResult = DialogResult.Cancel
            };

            mainPanel.Controls.Add(saveButton);
            mainPanel.Controls.Add(cancelButton);

     
            if (id != 0)
            {
                nameTextBox.Text = name;
                categoryTextBox.Text = category;
                quantityNumeric.Value = quantity;
                priceTextBox.Text = price.ToString("0.00");
                expiryPicker.Value = expiryDate;
                pillsNumeric.Value = pillsPerLeaf;
                leafsNumeric.Value = leafsPerBox;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter medicine name", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(priceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (expiryPicker.Value < DateTime.Today)
            {
                MessageBox.Show("Expiry date cannot be in the past", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}