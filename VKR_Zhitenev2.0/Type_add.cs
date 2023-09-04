using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKR_Zhitenev2
{
    public partial class Type_add : Form
    {
        public Type_add()
        {
            InitializeComponent();
        }
        MySqlConnection constr = new MySqlConnection("server=localhost; port=3306; database=zhitenev_vkr; username=root; password=");

        private MySqlCommand cmd;

        private MySqlDataAdapter adapter;

        DataTable dt = new DataTable();
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            if (mainWindow != null && bunifuTextBox3.Text != "")
                add(bunifuTextBox3.Text);
            else MessageBox.Show("Заполните все поля");
        }

        private void add(string text)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            string sql = "SELECT * FROM types WHERE name = '"+ text +"'";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    constr.Close();
                    MessageBox.Show("Тип уже существует");
                    dt.Rows.Clear();
                    dt.Columns.Clear();
                }
                else
                {
                    sql = "INSERT INTO types (name) VALUES(@name)";
                    cmd = new MySqlCommand(sql, constr);
                    cmd.Parameters.AddWithValue("@name", text);
                    try
                    {
                         //В случае успешного добавления показываем MessageBox
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                           MessageBox.Show("Запись успешно добавлена");
                        }
                        constr.Close();                       

                    }
                    //Отлавливаем возможные ошибки при добавлении новых данных
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        constr.Close();
                    }
                }
                if (mainWindow != null)
                {
                    mainWindow.Activate();
                    mainWindow.fill();//Вызов функции обновления таблицы
                    fill();
                }
                dt.Rows.Clear();
                dt.Columns.Clear();
                clearTxts();
            }
            //Отлавливаем возможные ошибки при заполнении
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            
        }

        private void clearTxts()
        {
            bunifuTextBox3.Clear();
        }

        private void Type_add_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            bunifuLabel1.Text = "Добавление типов";
            bunifuButton1.Location = new Point((this.Width - bunifuButton1.Width) / 2, 150);
            fill();
        }

        private void fill()
        {
            bunifuDropdown1.Items.Clear();
            string sql = "SELECT name FROM types";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    bunifuDropdown1.Items.Add(row[0].ToString());
                }
                constr.Close();
                dt.Rows.Clear();
                dt.Columns.Clear();
            }
            //Отлавливаем возможные ошибки при заполнении
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            try
            {
                bunifuPanel1.Dock = DockStyle.None; // Un-dock
                bunifuPanel2.Dock = DockStyle.None; // Un-dock
                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception)
            {

            }
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuDropdown1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bunifuPanel2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuTextBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
