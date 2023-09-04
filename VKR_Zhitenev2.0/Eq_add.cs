using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace VKR_Zhitenev2
{
    public partial class Eq_add : Form
    {
        MySqlConnection constr = new MySqlConnection("server=localhost; port=3306; database=zhitenev_vkr; username=root; password=");

        private MySqlCommand cmd;
        public int indicator;
        private MySqlDataAdapter adapter;

        DataTable dt = new DataTable();        
        public Eq_add()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            bunifuButton1.Location = new Point((this.Width - bunifuButton1.Width) / 2, 267);
            fill();

        }

        public void fill()
        {
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
            sql = "SELECT name FROM cabinets";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    bunifuDropdown3.Items.Add(row[0].ToString());
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

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if (bunifuTextBox1.Text!="" && bunifuTextBox2.Text != "" && bunifuTextBox3.Text != "" && bunifuDropdown3.SelectedItem != null)
            {
                if (indicator == 1 || bunifuDropdown1.SelectedItem != null)
                {
                    MainWindow mainWindow = this.Owner as MainWindow;
                    if (mainWindow != null && indicator == 1)
                    {
                        add(bunifuTextBox1.Text, mainWindow.search_type("Системный блок"), bunifuTextBox3.Text, bunifuTextBox2.Text, bunifuDropdown3.SelectedItem.ToString());
                    }

                    else if (mainWindow != null)
                    {
                        add(bunifuTextBox1.Text, mainWindow.search_type(bunifuDropdown1.SelectedItem.ToString()), bunifuTextBox3.Text, bunifuTextBox2.Text, bunifuDropdown3.SelectedItem.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Заполните все поля");
                }

            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }

        }
               

        private void add(string text1, string v1, string text2, string text3, string v2)
        {
            MainWindow mainWindow = this.Owner as MainWindow;
            string sql = "INSERT INTO equipment (inv_number, type_id, name, price, cabinet_id, status) VALUES(@inv, @type, @name, @price, @cabinet, 1)";
            cmd = new MySqlCommand(sql, constr);
            cmd.Parameters.AddWithValue("@inv", text1);
            cmd.Parameters.AddWithValue("@type", v1);
            cmd.Parameters.AddWithValue("@name", text2);
            cmd.Parameters.AddWithValue("@price", text3);
            cmd.Parameters.AddWithValue("@cabinet", v2);
            try
            {
                constr.Open();
                //В случае успешного добавления показываем MessageBox
                if (cmd.ExecuteNonQuery() > 0)
                {
                    clearTxts();
                    MessageBox.Show("Запись успешно добавлена");
                    if (indicator == 1)
                    {
                        mainWindow.inv_num = text1;
                        this.Close();
                    }
                        

                }
                constr.Close();
                  
                if (mainWindow != null && indicator != 1)
                {
                    mainWindow.retrieve();//Вызов функции обновления таблицы
                }
                 
            }
            //Отлавливаем возможные ошибки при добавлении новых данных
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            indicator = 0;
        }

        private void clearTxts()
        {
            bunifuTextBox1.Clear();
            bunifuTextBox2.Clear();
            bunifuTextBox3.Clear();
            bunifuDropdown1.Text = "Выберите тип оборудования";
            bunifuDropdown3.Text = "Выберите кабинет";
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
                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception)
            {

            }
        }
    }
}
