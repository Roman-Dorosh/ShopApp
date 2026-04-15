using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ShopApp
{
    public partial class CartForm : Form
    {
        private int userId;
        private DataGridView dgv;
        private Label lblTotal;

        public CartForm(int uid)
        {
            userId = uid;
            CreateControls();
            LoadCart();
        }

        private void CreateControls()
        {
            dgv = new DataGridView() { Location = new Point(12, 12), Size = new Size(500, 300), AllowUserToAddRows = false };
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            lblTotal = new Label() { Location = new Point(12, 330), AutoSize = true, Text = "Общая сумма: 0" };

            Button btnRemove = new Button() { Location = new Point(420, 330), Size = new Size(100, 23), Text = "Удалить" };
            btnRemove.Click += (s, e) => RemoveFromCart();

            Button btnOrder = new Button() { Location = new Point(420, 360), Size = new Size(100, 23), Text = "Оформить" };
            btnOrder.Click += (s, e) => Checkout();

            Controls.Add(dgv);
            Controls.Add(lblTotal);
            Controls.Add(btnRemove);
            Controls.Add(btnOrder);

            Text = "Корзина";
            Size = new Size(550, 440);
        }

        private void LoadCart()
        {
            string sql = @"SELECT p.Id AS ProductId, p.Name, p.Description, p.Price - p.Price * p.Discount / 100 AS FinalPrice 
                           FROM Cart c JOIN Products p ON c.ProductId = p.Id WHERE c.UserId = @uid";
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;

                decimal total = 0;
                foreach (DataRow row in dt.Rows) total += Convert.ToDecimal(row["FinalPrice"]);
                lblTotal.Text = "Общая сумма: " + total.ToString("0.00");
            }
        }

        private void RemoveFromCart()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            int productId = Convert.ToInt32(dgv.CurrentRow.Cells["ProductId"].Value);

            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Cart WHERE UserId=@uid AND ProductId=@pid", conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.ExecuteNonQuery();
            }
            LoadCart();
        }

        private void Checkout()
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Cart WHERE UserId=@uid", conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Заказ оформлен! Спасибо за покупку.");
            LoadCart();
        }
    }
}