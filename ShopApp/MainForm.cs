using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ShopApp
{
    public partial class MainForm : Form
    {
        private int userId;
        private DataGridView dgv;
        private TextBox txtSearch;
        private Button btnSearch, btnCart;
        private Label lblCount;

        public MainForm(int uid)
        {
            userId = uid;
            CreateControls();
            LoadProducts("");
            UpdateCartCount();
        }

        private void CreateControls()
        {
            dgv = new DataGridView() { Location = new Point(12, 50), Size = new Size(600, 300) };
            txtSearch = new TextBox() { Location = new Point(12, 12), Size = new Size(150, 20) };
            btnSearch = new Button() { Location = new Point(180, 10), Size = new Size(75, 23), Text = "Найти" };

            Button btnAdd = new Button() { Location = new Point(280, 10), Size = new Size(120, 23), Text = "Добавить в корзину" };
            btnAdd.Click += (s, e) => AddToCart();

            btnCart = new Button() { Location = new Point(500, 10), Size = new Size(110, 23), Text = "Корзина (0)" };
            lblCount = new Label() { Location = new Point(12, 360), AutoSize = true, Text = "0 / 0" };

            btnSearch.Click += (s, e) => LoadProducts(txtSearch.Text);
            btnCart.Click += (s, e) => { new CartForm(userId).ShowDialog(); UpdateCartCount(); };

            Controls.Add(dgv);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(btnAdd);
            Controls.Add(btnCart);
            Controls.Add(lblCount);

            Text = "Магазин";
            Size = new Size(650, 450);
        }

        private void LoadProducts(string search)
        {
            string sql = "SELECT Id, Name, Description, Price, Discount FROM Products WHERE Name LIKE @s OR Description LIKE @s";
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
                lblCount.Text = dt.Rows.Count + " / " + dt.Rows.Count;
            }
        }

        private void UpdateCartCount()
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cart WHERE UserId=@uid", conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                int count = (int)cmd.ExecuteScalar();
                btnCart.Text = $"Корзина ({count})";
            }
        }

        private void AddToCart()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            int productId = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);

            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Cart (UserId, ProductId) VALUES (@uid, @pid)", conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.ExecuteNonQuery();
            }

            UpdateCartCount();
            MessageBox.Show("Товар добавлен в корзину");
        }
    }
}