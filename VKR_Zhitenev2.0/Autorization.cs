using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VKR_Zhitenev2
{
    public partial class Autorization : Form
    {
          
        public Autorization()
        {
            InitializeComponent();
        }

        private void Autorization_Load(object sender, EventArgs e)
        {

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            bunifuPictureBox1.Location = new Point((this.Width - bunifuPictureBox1.Width) / 2, 62);
            bunifuTextBox1.Location = new Point((this.Width - bunifuTextBox1.Width) / 2, 172);
            bunifuTextBox2.Location = new Point((this.Width - bunifuTextBox2.Width) / 2, 238);
            bunifuButton1.Location = new Point((this.Width - bunifuButton1.Width) / 2, 304);
            bunifuButton1.Text = "Авторизация";
        }        

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
            
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {

            try
            {
                bunifuPanel1.Dock = DockStyle.None; // Un-dock
                this.WindowState = FormWindowState.Minimized;                
            }
            catch (Exception)
            {
                
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string loginUs = bunifuTextBox1.Text;
            string loginpass = bunifuTextBox2.Text;
            MySqlConnection constr =new MySqlConnection("server=localhost; port=3306; database=zhitenev_vkr; username=root; password=");
            constr.Open();
            var table = new DataTable();
            string sql = "SELECT * FROM user_info WHERE login=@PNAME AND password=@PWD";
            MySqlCommand cmd = new MySqlCommand(sql, constr);
            cmd.Parameters.AddWithValue("@PNAME", loginUs);
            cmd.Parameters.AddWithValue("@PWD", loginpass);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                constr.Close();
                this.Hide();
                MainWindow projectWindow = this.Owner as MainWindow;
                projectWindow.currentUser = loginUs;
                projectWindow.Activate();
            }
            else
            {
                constr.Close();
                MessageBox.Show("Ошибка авторизации");
                bunifuTextBox1.Clear();
                bunifuTextBox2.Clear();
            }

        }
    }
}
