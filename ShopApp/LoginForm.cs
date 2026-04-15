using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ShopApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            CreateTextBoxesAndButtons();
        }

        private TextBox txtUser, txtPass;
        private Button btnLogin, btnRegister;

        private void CreateTextBoxesAndButtons()
        {
            txtUser = new TextBox() { Location = new System.Drawing.Point(50, 30), Size = new System.Drawing.Size(150, 20) };
            txtPass = new TextBox() { Location = new System.Drawing.Point(50, 60), Size = new System.Drawing.Size(150, 20), UseSystemPasswordChar = true };
            btnLogin = new Button() { Location = new System.Drawing.Point(50, 100), Size = new System.Drawing.Size(70, 23), Text = "Вход" };
            btnRegister = new Button() { Location = new System.Drawing.Point(130, 100), Size = new System.Drawing.Size(70, 23), Text = "Регистрация" };

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            Controls.Add(txtUser);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);

            Text = "Авторизация";
            Size = new System.Drawing.Size(280, 180);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
                cmd.Parameters.AddWithValue("@u", txtUser.Text);
                cmd.Parameters.AddWithValue("@p", txtPass.Text);
                object res = cmd.ExecuteScalar();
                if (res != null)
                {
                    MainForm main = new MainForm((int)res);
                    main.Show();
                    this.Hide();
                }
                else MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDB;Integrated Security=True"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)", conn);
                cmd.Parameters.AddWithValue("@u", txtUser.Text);
                cmd.Parameters.AddWithValue("@p", txtPass.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Регистрация успешна!");
            }
        }
    }
}