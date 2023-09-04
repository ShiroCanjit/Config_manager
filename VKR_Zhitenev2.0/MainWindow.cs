using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using MySql.Data.MySqlClient;

namespace VKR_Zhitenev2
{
    public partial class MainWindow : Form
    {
        private static ObjectQuery oq;
        private static ManagementObjectSearcher query;
        private static ManagementObjectCollection queryCollection;
        private ManagementObjectCollection oc, oc1, oc2,oc3,oc4;
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn col_1 = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn col_2 = new DataGridViewComboBoxColumn();
        MySqlConnection constr = new MySqlConnection("server=localhost; port=3306; database=zhitenev_vkr; username=root; password=");
        private MySqlCommand cmd;
        private MySqlDataAdapter adapter;
        public MainWindow()
        {
            InitializeComponent();
            Autorization auth = new Autorization();
            auth.Owner = this;
            auth.ShowDialog();
            this.Hide();
            
            
        }
        public string currentUser;
        public string currentdep;
        public string inv_num ="";

        void SetActiveTab(int Index)
        {
            if (indicator.Visible == false)
                indicator.Show();
            switch (bunifuPages1.SelectedIndex - 1)
            {
                case 0:
                    indicator.Left = equipment_button.Right - equipment_button.Width;
                    break;
                case 1:
                    indicator.Left = replacement_button.Right - replacement_button.Width;
                    break;                 
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectTab(1);
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            bunifuPages1.SelectTab(2);
        }
         

        private void bunifuPages1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveTab(bunifuPages1.SelectedIndex);
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            try
            {
                bunifuPanel2.Dock = DockStyle.None; // Un-dock
                bunifuPanel3.Dock = DockStyle.None; // Un-dock
                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception)
            {

            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            string sql = "SELECT fullname, departments.name, departments.id FROM user_info INNER JOIN departments ON user_info.department_id = departments.id WHERE login = '" + currentUser +"'";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    bunifuTextBox5.Text = row[0].ToString() + Environment.NewLine + row[1].ToString();
                    greet_label.Text = "С возвращением!";
                    appointment_label.Text = row[0].ToString();
                    office_label.Text = row[1].ToString();
                    currentdep = row[2].ToString();
                }
                constr.Close();
                dt.Rows.Clear();
                dt.Columns.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            equipment_button.Text = "ОБОРУДОВАНИЕ";
            this.replacement_button.Text = "ПРОВЕРКА" + Environment.NewLine + "КОНФИГУРАЦИИ";
            indicator.Hide();
            bunifuPictureBox1.Location = new Point((tabPage1.Width - bunifuPictureBox1.Width)/2, 36);
            greet_label.Location = new Point((tabPage1.Width - greet_label.Width)/2, 165);
            appointment_label.Location = new Point((tabPage1.Width - appointment_label.Width)/2, 200);
            office_label.Location = new Point((tabPage1.Width - office_label.Width)/2, 235);
            timer1.Start();

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            if(bunifuDataGridView1.ColumnCount == 0)
            {
                bunifuDropdown2.Hide();
                bunifuDropdown3.Hide();
                DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
                bunifuDataGridView1.Columns.Add(col1);
                bunifuDataGridView1.Columns.Add(col);
                DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
                bunifuDataGridView1.Columns.Add(col2);
                DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
                bunifuDataGridView1.Columns.Add(col3);
                bunifuDataGridView1.Columns.Add(col_1);
                bunifuDataGridView1.Columns.Add(col_2);
                fill();
                bunifuDataGridView1.Columns[0].Name = "Инвентарный номер";
                bunifuDataGridView1.Columns[1].Name = "Тип оборудования";
                bunifuDataGridView1.Columns[2].Name = "Название";
                bunifuDataGridView1.Columns[3].Name = "Стоимость  (руб.)";
                bunifuDataGridView1.Columns[4].Name = "Помещение";
                bunifuDataGridView1.Columns[5].Name = "Статус";
                bunifuDataGridView1.Rows.Clear();
                bunifuDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                bunifuDataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                bunifuDataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                bunifuDataGridView1.AllowUserToResizeColumns = false;
                bunifuDataGridView1.AllowUserToResizeRows = false;
                bunifuDataGridView1.Columns[0].ReadOnly = true;
            }
            retrieve();


        }
        public void retrieve()
        {
            dt.Rows.Clear();
            dt.Columns.Clear();
            bunifuDataGridView1.Rows.Clear();
            string filter = "";
            if(bunifuDropdown1.SelectedItem != null)
                if(bunifuDropdown1.SelectedIndex!=0)
                    filter+=" AND equipment.cabinet_id = '"+bunifuDropdown1.SelectedItem.ToString()+"'";
            if(bunifuDropdown2.SelectedItem != null)
                if(bunifuDropdown2.SelectedIndex != 0)
                filter += " AND equipment.type_id = '" + search_type(bunifuDropdown2.SelectedItem.ToString()) + "'";
            if(bunifuDropdown3.SelectedItem != null)
                if (bunifuDropdown3.SelectedIndex != 0)
                if(bunifuDropdown3.SelectedIndex == 1)
                filter += " AND equipment.status = 1";
            else if (bunifuDropdown3.SelectedIndex == 2) filter += " AND equipment.status = 0";

            string sql = "SELECT inv_number, types.name, equipment.name, equipment.price, cabinet_id, status " +
                "FROM equipment INNER JOIN types ON equipment.type_id = types.id " +
                "INNER JOIN cabinets ON cabinet_id = cabinets.name " +
                "INNER JOIN departments ON cabinets.department_id = departments.id " +
                "INNER JOIN user_info ON user_info.department_id = departments.id WHERE user_info.login = '" + currentUser + "' AND user_info.department_id = cabinets.department_id" + filter;
            
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    if( Convert.ToBoolean(row[5]) == true)
                    bunifuDataGridView1.Rows.Add(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), "Исправно");
                    else bunifuDataGridView1.Rows.Add(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), "Неисправно");

                }
                constr.Close();
                dt.Rows.Clear();
                dt.Columns.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }

            bunifuDataGridView1.ClearSelection();
        }
        public void fill()
        {
            col_1.Items.Clear();
            col_2.Items.Clear();
            col.Items.Clear();
            bunifuDropdown2.Items.Clear();
            bunifuDropdown1.Items.Clear();
            bunifuDropdown2.Items.Add("Любой тип");
            bunifuDropdown1.Items.Add("Любое помещение");
            string sql = "SELECT name FROM types";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    bunifuDropdown2.Items.Add(row[0].ToString());
                    col.Items.Add(row[0].ToString());
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
            sql = "SELECT cabinets.name FROM cabinets " +
                "INNER JOIN departments ON cabinets.department_id = departments.id " +
                "INNER JOIN user_info ON user_info.department_id = departments.id WHERE user_info.login = '" + currentUser + "' AND user_info.department_id = cabinets.department_id";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    bunifuDropdown1.Items.Add(row[0].ToString());
                    col_1.Items.Add(row[0].ToString());
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
            col_2.Items.Add("Исправно");
            col_2.Items.Add("Неисправно");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            current_time_label.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy, HH:mm:ss");
        }

        private void bunifuButton1_Click_1(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            if(bunifuRadioButton2.Checked == true)
            {
                try
                {
                    oc = GetManagementObjectCollection(bunifuTextBox2.Text, bunifuTextBox3.Text, bunifuTextBox1.Text, bunifuTextBox4.Text, "Win32_OperatingSystem");
                    oc1 = GetManagementObjectCollection(bunifuTextBox2.Text, bunifuTextBox3.Text, bunifuTextBox1.Text, bunifuTextBox4.Text, "Win32_ComputerSystem");
                    oc2 = GetManagementObjectCollection(bunifuTextBox2.Text, bunifuTextBox3.Text, bunifuTextBox1.Text, bunifuTextBox4.Text, "Win32_Processor");
                    oc3 = GetManagementObjectCollection(bunifuTextBox2.Text, bunifuTextBox3.Text, bunifuTextBox1.Text, bunifuTextBox4.Text, "Win32_BIOS");
                    oc4 = GetManagementObjectCollection(bunifuTextBox2.Text, bunifuTextBox3.Text, bunifuTextBox1.Text, bunifuTextBox4.Text, "Win32_PhysicalMemory");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения");
                    return;                    
                }
            }
                
            else if(bunifuRadioButton1.Checked == true)
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * From Win32_OperatingSystem");
                oc = query.Get();
                query = new ManagementObjectSearcher("SELECT * From Win32_ComputerSystem");
                oc1 = query.Get();
                query = new ManagementObjectSearcher("SELECT * From Win32_Processor");
                oc2 = query.Get();
                query = new ManagementObjectSearcher("SELECT * From Win32_BIOS");
                oc3 = query.Get();
                query = new ManagementObjectSearcher("SELECT * From Win32_PhysicalMemory");
                oc4 = query.Get();                
            }            
            int nod = 0;
            treeView1.Nodes.Add("Операционная система");
            foreach (ManagementObject mo in oc)
            {             
                treeView1.Nodes[nod].Nodes.Add("Операционная система: " + mo["Caption"]);
                treeView1.Nodes[nod].Nodes.Add("Версия ОС: " + mo["Version"]);
                treeView1.Nodes[nod].Nodes.Add("Производитель : " + mo["Manufacturer"]);
                treeView1.Nodes[nod].Nodes.Add("Директория установки Windows : " + mo["WindowsDirectory"]);
            }
            nod++;
            treeView1.Nodes.Add("Устройство");
            foreach (ManagementObject mo in oc1)
            {
                treeView1.Nodes[nod].Nodes.Add("Производитель : " + mo["Manufacturer"]);
                treeView1.Nodes[nod].Nodes.Add("Имя устройства : " + mo["Name"]);
                treeView1.Nodes[nod].Nodes.Add("Модель : " + mo["Model"]);
            }
            nod++;
            treeView1.Nodes.Add("Процессор");
            foreach (ManagementObject mo in oc2)
            {
                treeView1.Nodes[nod].Nodes.Add("Название : " + mo["Name"]);
                treeView1.Nodes[nod].Nodes.Add("Описание : " + mo["Description"]);
                treeView1.Nodes[nod].Nodes.Add("Тактовая частота : " + mo["CurrentClockSpeed"]);
            }
            nod++;
            treeView1.Nodes.Add("BIOS");
            foreach (ManagementObject mo in oc3)
            {
                treeView1.Nodes[nod].Nodes.Add("Версия BIOS : " + mo["Version"]);
            }
            nod++;
            treeView1.Nodes.Add("Оперативная память");
            long cap = 0;
            string speed ="";
            int numRAM = 0;
            foreach (ManagementObject mo in oc4)
            {
                cap += Convert.ToInt64(mo["Capacity"]);
                speed += Convert.ToString(mo["Speed"]) + "&";

            }            
            for (int i = 0; i < speed.Length; i++)
            {
                if (speed[i] == '&') 
                {
                    numRAM++;
                }
            }
            speed = speed.Remove(speed.Length - 1);
            treeView1.Nodes[nod].Nodes.Add("Количество модулей : " + numRAM);
            treeView1.Nodes[nod].Nodes.Add("Общий объем памяти : " + cap/1073741824 + " Гб" );
            treeView1.Nodes[nod].Nodes.Add("Латентность : " + speed);
            treeView1.ExpandAll();
            CheckConfig(treeView1.Nodes[1].Nodes[1].Text);
            
        }               
        
        private string search_os(string v1, string v2, string v3, string v4)
        {
            string str = "";
            string sql = "SELECT os.id FROM os WHERE os.name=@name AND os.version =@version AND os.manufacturer =@manufacturer AND os.directory = @directory";
            try
            {
                constr.Open();
                MySqlCommand cmd = new MySqlCommand(sql, constr);
                cmd.Parameters.AddWithValue("@name", v1);
                cmd.Parameters.AddWithValue("@version", v2);
                cmd.Parameters.AddWithValue("@manufacturer", v3);
                cmd.Parameters.AddWithValue("@directory", v4);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    sql = "INSERT INTO os (name, version, manufacturer, directory) VALUES(@name, @version, @manufactuter, @directoty);";
                    cmd = new MySqlCommand(sql, constr);
                    cmd.Parameters.AddWithValue("@name", treeView1.Nodes[0].Nodes[0].Text);
                    cmd.Parameters.AddWithValue("@version", treeView1.Nodes[0].Nodes[1].Text);
                    cmd.Parameters.AddWithValue("@manufactuter", treeView1.Nodes[0].Nodes[2].Text);
                    cmd.Parameters.AddWithValue("@directoty", treeView1.Nodes[0].Nodes[3].Text);
                    try
                    {                       
                        cmd.ExecuteNonQuery();
                    }
                    //Отлавливаем возможные ошибки при добавлении новых данных
                    catch (Exception ex)
                    {
                         MessageBox.Show(ex.Message);
                        constr.Close();
                    }
                    constr.Close();
                    dt.Rows.Clear();
                    dt.Columns.Clear();
                    str = search_os(v1, v2, v3, v4);
                }
                else 
                    foreach (DataRow row in dt.Rows)
                        {
                            str = row[0].ToString();
                        }
                constr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            dt.Rows.Clear();
            dt.Columns.Clear();
            return str;
        }
        private string search_comp_sys(string v)
        {
            string str = "";
            string sql = "SELECT id FROM comp_sys WHERE name=@name;";
            try
            {
                constr.Open();
                MySqlCommand cmd = new MySqlCommand(sql, constr);
                cmd.Parameters.AddWithValue("@name", v);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    sql = "INSERT INTO comp_sys (manufacturer, name, model) VALUES(@manufactuter, @name, @model);" ;
                    cmd = new MySqlCommand(sql, constr);
                    cmd.Parameters.AddWithValue("@manufactuter", treeView1.Nodes[1].Nodes[0].Text);
                    cmd.Parameters.AddWithValue("@name", treeView1.Nodes[1].Nodes[1].Text);
                    cmd.Parameters.AddWithValue("@model", treeView1.Nodes[1].Nodes[2].Text);
                    try
                    {

                        cmd.ExecuteNonQuery();

                    }
                    //Отлавливаем возможные ошибки при добавлении новых данных
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        constr.Close();
                    }
                    constr.Close();
                    dt.Rows.Clear();
                    dt.Columns.Clear();
                    str = search_comp_sys(v);
                }
                else
                    foreach (DataRow row in dt.Rows)
                {
                    str = row[0].ToString();
                }
                constr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            dt.Rows.Clear();
            dt.Columns.Clear();
            return str;
        }
        private string search_processor(string v1, string v2, string v3)
        {
            string str = "";
            string sql = "SELECT id FROM processor WHERE name=@name AND descr = @descr AND clockspeed =@clockspeed";
            try
            {
                constr.Open();
                MySqlCommand cmd = new MySqlCommand(sql, constr);
                cmd.Parameters.AddWithValue("@name", v1);
                cmd.Parameters.AddWithValue("@descr", v2);
                cmd.Parameters.AddWithValue("@clockspeed", v3);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    sql = "INSERT INTO processor (name, descr, clockspeed) VALUES(@name, @descr, @cloakspeed);";
                    cmd = new MySqlCommand(sql, constr);
                    cmd.Parameters.AddWithValue("@name", treeView1.Nodes[2].Nodes[0].Text);
                    cmd.Parameters.AddWithValue("@descr", treeView1.Nodes[2].Nodes[1].Text);
                    cmd.Parameters.AddWithValue("@cloakspeed", treeView1.Nodes[2].Nodes[2].Text);
                    try
                    {
                        

                        cmd.ExecuteNonQuery();

                    }
                    //Отлавливаем возможные ошибки при добавлении новых данных
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        constr.Close();
                    }

                    constr.Close();
                    dt.Rows.Clear();
                    dt.Columns.Clear();
                    str = search_processor(v1, v2, v3);
                }
                else
                    foreach (DataRow row in dt.Rows)
                {
                    str = row[0].ToString();
                }
                constr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            dt.Rows.Clear();
            dt.Columns.Clear();
            return str;
        }
        private string search_ram(string v, string v1, string v2)
        {
            string str = "";
            string sql = "SELECT id FROM ram WHERE number=@number AND volume =@volume AND timings = @timings";
            try
            {
                constr.Open();
                MySqlCommand cmd = new MySqlCommand(sql, constr);
                cmd.Parameters.AddWithValue("@number", v);
                cmd.Parameters.AddWithValue("@volume", v1);
                cmd.Parameters.AddWithValue("@timings", v2);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    sql = "INSERT INTO ram (number, volume, timings) VALUES(@number, @volume, @timings)";
                    cmd = new MySqlCommand(sql, constr);
                    cmd.Parameters.AddWithValue("@number", treeView1.Nodes[4].Nodes[0].Text);
                    cmd.Parameters.AddWithValue("@volume", treeView1.Nodes[4].Nodes[1].Text);
                    cmd.Parameters.AddWithValue("@timings", treeView1.Nodes[4].Nodes[2].Text);
                    try
                    {

                        cmd.ExecuteNonQuery();

                    }
                    //Отлавливаем возможные ошибки при добавлении новых данных
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        constr.Close();
                    }

                    constr.Close();
                    dt.Rows.Clear();
                    dt.Columns.Clear();
                    str = search_ram(v, v1, v2);
                }
                else
                    foreach (DataRow row in dt.Rows)
                {
                    str = row[0].ToString();
                }
                constr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            dt.Rows.Clear();
            dt.Columns.Clear();
            return str;
        }
        private void insert_config(string inv_num, string v1, string v2, string v3, string v4, string v5)
        {
            string sql = "INSERT INTO configuration (inventory_number, os_id, comp_sys_id, processor_id, bios_version, ram_id) VALUES(@inv, @os_id, @comp_sys_id, @processor_id, @bios_version, @ram_id)";
            cmd = new MySqlCommand(sql, constr);
            cmd.Parameters.AddWithValue("@inv", inv_num);
            cmd.Parameters.AddWithValue("@os_id", v1);
            cmd.Parameters.AddWithValue("@comp_sys_id", v2);
            cmd.Parameters.AddWithValue("@processor_id", v3);
            cmd.Parameters.AddWithValue("@bios_version", v4);
            cmd.Parameters.AddWithValue("@ram_id", v5);
            try
            {
                constr.Open();
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

        private void CheckConfig(string pc_name)
        {
            string sql = "SELECT comp_sys.id FROM comp_sys WHERE name = @name";
            try
            {
                constr.Open();
                MySqlCommand cmd = new MySqlCommand(sql, constr);
                cmd.Parameters.AddWithValue("@name", pc_name);
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    Eq_add pc_add = new Eq_add();
                    pc_add.Owner = this;
                    pc_add.fill();
                    pc_add.indicator = 1;
                    pc_add.bunifuLabel1.Text = "Добавление компьютера";
                    pc_add.bunifuDropdown1.SelectedItem = "Компьютер";
                    pc_add.bunifuDropdown1.Enabled = false;
                    constr.Close();
                    pc_add.ShowDialog();
                    if (inv_num != "")
                    {                        
                        insert_config(inv_num, search_os(treeView1.Nodes[0].Nodes[0].Text, treeView1.Nodes[0].Nodes[1].Text, treeView1.Nodes[0].Nodes[2].Text, treeView1.Nodes[0].Nodes[3].Text), 
                            search_comp_sys(treeView1.Nodes[1].Nodes[1].Text), 
                            search_processor(treeView1.Nodes[2].Nodes[0].Text, treeView1.Nodes[2].Nodes[1].Text, treeView1.Nodes[2].Nodes[2].Text),
                            treeView1.Nodes[3].Nodes[0].Text,
                            search_ram(treeView1.Nodes[4].Nodes[0].Text,
                            treeView1.Nodes[4].Nodes[1].Text, treeView1.Nodes[4].Nodes[2].Text));

                    }
                }
                else
                {

                    constr.Close();
                    if (MessageBox.Show("Обновить данные о текущем компьютере?", "Найдено совпадение", MessageBoxButtons.OKCancel,
               MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        sql = "SELECT inventory_number FROM configuration INNER JOIN comp_sys ON configuration.comp_sys_id = comp_sys.id WHERE name ='" + pc_name + "'";
                        cmd = new MySqlCommand(sql, constr);
                        try
                        {
                            constr.Open();
                            adapter = new MySqlDataAdapter(cmd);
                            adapter.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                inv_num = row[0].ToString();
                            }
                            constr.Close();                            
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            constr.Close();
                        }
                        dt.Rows.Clear();
                        dt.Columns.Clear();

                        sql = "UPDATE configuration SET os_id='" + search_os(treeView1.Nodes[0].Nodes[0].Text, treeView1.Nodes[0].Nodes[1].Text, treeView1.Nodes[0].Nodes[2].Text, treeView1.Nodes[0].Nodes[3].Text) + "', " +
                           // "comp_sys_id='" + update_comp_sys(pc_name) + "'," +
                            " processor_id ='" + search_processor(treeView1.Nodes[2].Nodes[0].Text, treeView1.Nodes[2].Nodes[1].Text, treeView1.Nodes[2].Nodes[2].Text) + "'," +
                            " bios_version ='" + treeView1.Nodes[3].Nodes[0].Text + "'," +
                            " ram_id ='" + search_ram(treeView1.Nodes[4].Nodes[0].Text, treeView1.Nodes[4].Nodes[1].Text, treeView1.Nodes[4].Nodes[2].Text) + "'" +
                            " WHERE inventory_number='" + inv_num + "'";
                        cmd = new MySqlCommand(sql, constr);
                        try
                        {
                            constr.Open();
                            adapter = new MySqlDataAdapter(cmd);
                            adapter.UpdateCommand = constr.CreateCommand();
                            adapter.UpdateCommand.CommandText = sql;
                            //В случае успешного добавления показываем MessageBox 
                            if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                            {
                               // MessageBox.Show("Запись успешно изменена");
                            }
                            constr.Close();
                            //retrieve();//Вызов функции обновления таблицы
                        }
                        //Отлавливаем возможные ошибки при добавлении новых данных
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            constr.Close();
                        }
                        dt.Rows.Clear();
                        dt.Columns.Clear();
                        update_comp_sys(pc_name);
                    }                 
                    

                }
                constr.Close();
                dt.Rows.Clear();
                dt.Columns.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }

        }

        private string update_comp_sys(string pc_name)
        {
            string str = "";
            string sql = "UPDATE comp_sys SET manufacturer = '" + treeView1.Nodes[1].Nodes[0].Text + "', model = '" + treeView1.Nodes[1].Nodes[2].Text + "' WHERE name = '" + pc_name + "'";
            cmd = new MySqlCommand(sql, constr);
            try
            {

                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.UpdateCommand = constr.CreateCommand();
                adapter.UpdateCommand.CommandText = sql;
                //В случае успешного добавления показываем MessageBox 
                if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                {
                    // MessageBox.Show("Запись успешно изменена");
                }
                constr.Close();
                //retrieve();//Вызов функции обновления таблицы
            }
            //Отлавливаем возможные ошибки при добавлении новых данных
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            dt.Rows.Clear();
            dt.Columns.Clear();
            return str;
        }

        private void bunifuCheckBox1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            if (bunifuCheckBox1.Checked)
            {
                bunifuDropdown2.Show();
                bunifuDropdown3.Show();
            }
            else
            {
                bunifuDropdown3.SelectedIndex = 0;
                bunifuDropdown2.Text = "По типу оборудования";
                bunifuDropdown3.Text = "По статусу";                
                fill();
                retrieve();
                bunifuDropdown2.Hide();
                bunifuDropdown3.Hide();
            }

        }       

        private void bunifuButton2_Click_1(object sender, EventArgs e)
        {

            if (bunifuDataGridView1.SelectedRows != null)
            {
                WebBrowser browser = new WebBrowser();
                browser.url_str = "http://www.google.com/search?q=" + bunifuDataGridView1.SelectedRows[0].Cells[1].Value.ToString() + " " + bunifuDataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                browser.Show();
            }
            else
                MessageBox.Show("Выберите оборудование для замены");
        }
        private static ManagementObjectCollection GetManagementObjectCollection(string pComputerName, string pAccountName, string pAccountDomain, string pAccountPassword, string Statisticpart)
        {
            ManagementScope managementScope = default(ManagementScope);
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Username = pAccountDomain + "\\" + pAccountName;
            connectionOptions.Password = pAccountPassword;
            managementScope = new ManagementScope("\\\\" + pComputerName + "\\root\\cimv2", connectionOptions);
            oq = new System.Management.ObjectQuery("SELECT * FROM " +Statisticpart);
            query = new ManagementObjectSearcher(managementScope, oq);
            queryCollection = query.Get();
            return queryCollection;
        }

        private void bunifuDataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void bunifuDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string id = bunifuDataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            string type = search_type(bunifuDataGridView1.SelectedRows[0].Cells[1].Value.ToString());
            string name = bunifuDataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            string price = bunifuDataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            string cabinet = bunifuDataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            string status = "0";
            if (bunifuDataGridView1.SelectedRows[0].Cells[5].Value.ToString() == "Исправно")
                status = "1";
            update(id, type, name, price, cabinet, status);
        }

        public string search_type(string v)
        {
            string str = "";
            string sql = "SELECT id FROM types WHERE name='" + v + "'";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    str = row[0].ToString();
                }
                constr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
            dt.Rows.Clear();
            dt.Columns.Clear();
            return str;
        }

        private void update(string id, string type, string name, string price, string cabinet, string status)
        {
            string sql = "UPDATE equipment SET type_id='" + type + "', name='" + name + "', price ='"+price+"', cabinet_id ='"+cabinet+"', status ='"+status+"' WHERE inv_number='" + id + "'";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.UpdateCommand = constr.CreateCommand();
                adapter.UpdateCommand.CommandText = sql;
                //В случае успешного добавления показываем MessageBox 
                if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Запись успешно изменена");
                }
                constr.Close();
                retrieve();//Вызов функции обновления таблицы
            }
            //Отлавливаем возможные ошибки при добавлении новых данных
            catch (Exception ex)
            {
                MessageBox.Show("Неверный формат данных");
                constr.Close();
                retrieve();
            }
        }

        
        private void bunifuDataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            String selected = bunifuDataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            bunifuDataGridView1.Rows.Add();
            delete(selected);
        }

        private void delete(string selected)
        {
            string sql = "DELETE FROM equipment WHERE inv_number='" + selected + "'";
            cmd = new MySqlCommand(sql, constr);
            try
            {
                constr.Open();
                adapter = new MySqlDataAdapter(cmd);
                adapter.DeleteCommand = constr.CreateCommand();
                adapter.DeleteCommand.CommandText = sql;
                //В случае успешного добавления показываем MessageBox
                if (MessageBox.Show("Вы уверены что хотите удалить выбранную запись?", "Удаление записи", MessageBoxButtons.OKCancel,
               MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Запись успешно удалена");
                    }
                }
                constr.Close();
                //retrieve();//Вызов функции обновления таблицы
            }
            //Отлавливаем возможные ошибки при добавлении новых данных
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                constr.Close();
            }
        }       

        private void bunifuDataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            bunifuDataGridView1.Rows.RemoveAt(bunifuDataGridView1.Rows.Count - 1);
            
            retrieve();
        }

        private void bunifuDropdown2_SelectedIndexChanged(object sender, EventArgs e)
        {

            retrieve();
        }

        private void bunifuDropdown3_SelectedIndexChanged(object sender, EventArgs e)
        {

            retrieve();
        }

        private void bunifuDropdown1_SelectedIndexChanged(object sender, EventArgs e)
        {

            retrieve();
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            Type_add type = new Type_add();
            type.Owner = this;
            type.Show();
        }

        private void bunifuDataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            treeView2.Nodes.Clear();
            if (bunifuDataGridView1.SelectedRows.Count != 0)
                if (bunifuDataGridView1.SelectedRows[0].Cells[1].Value != null)
            if (bunifuDataGridView1.SelectedRows[0].Cells[1].Value.ToString() == "Системный блок" )
            {
                string sql = "SELECT os.name, os.version, os.manufacturer, os.directory, " +
                    "comp_sys.manufacturer, comp_sys.name, comp_sys.model, " +
                    "processor.name, processor.descr, processor.clockspeed, " +
                    "configuration.bios_version, " +
                    "ram.number, ram.volume, ram.timings " +
                    "FROM os INNER JOIN configuration ON os.id = configuration.os_id " +
                    "INNER JOIN comp_sys ON comp_sys.id = configuration.comp_sys_id " +
                    "INNER JOIN processor ON processor.id = configuration.processor_id " +
                    "INNER JOIN ram ON ram.id = configuration.ram_id" +
                    " WHERE configuration.inventory_number=@number";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, constr);
                    cmd.Parameters.AddWithValue("@number", bunifuDataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt1);
                    treeView2.Nodes.Add("Операционная система");
                    treeView2.Nodes.Add("Устройство");
                    treeView2.Nodes.Add("Процессор");
                    treeView2.Nodes.Add("BIOS");
                    treeView2.Nodes.Add("Операционная память");
                    foreach (DataRow row in dt1.Rows)
                    {
                        treeView2.Nodes[0].Nodes.Add(row[0].ToString());
                        treeView2.Nodes[0].Nodes.Add(row[1].ToString());
                        treeView2.Nodes[0].Nodes.Add(row[2].ToString());
                        treeView2.Nodes[0].Nodes.Add(row[3].ToString());
                        treeView2.Nodes[1].Nodes.Add(row[4].ToString());
                        treeView2.Nodes[1].Nodes.Add(row[5].ToString());
                        treeView2.Nodes[1].Nodes.Add(row[6].ToString());
                        treeView2.Nodes[2].Nodes.Add(row[7].ToString());
                        treeView2.Nodes[2].Nodes.Add(row[8].ToString());
                        treeView2.Nodes[2].Nodes.Add(row[9].ToString());
                        treeView2.Nodes[3].Nodes.Add(row[10].ToString());
                        treeView2.Nodes[4].Nodes.Add(row[11].ToString());
                        treeView2.Nodes[4].Nodes.Add(row[12].ToString());
                        treeView2.Nodes[4].Nodes.Add(row[13].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    constr.Close();
                }
                treeView2.ExpandAll();
            }
            else treeView2.Nodes.Clear();
            dt1.Rows.Clear();
            dt1.Columns.Clear();
        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            cabinet_add c_add = new cabinet_add();
            c_add.Owner = this;
            c_add.currentuser = currentUser;
            c_add.currentdepartment = currentdep;
            c_add.Show();
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            Eq_add eq = new Eq_add(); 
            eq.Owner = this;
            eq.Show();
        }

        private void bunifuRadioButton2_CheckedChanged2(object sender, Bunifu.UI.WinForms.BunifuRadioButton.CheckedChangedEventArgs e)
        {
            if (bunifuRadioButton2.Checked == true)
            {
                bunifuLabel5.Show();
                bunifuLabel6.Show();
                bunifuLabel7.Show();
                bunifuLabel8.Show();
                bunifuTextBox1.Show();
                bunifuTextBox2.Show();
                bunifuTextBox3.Show();
                bunifuTextBox4.Show();
            }
            else Hider();
        }
        private void Hider()
        {
            bunifuLabel5.Hide();
            bunifuLabel6.Hide();
            bunifuLabel7.Hide();
            bunifuLabel8.Hide();
            bunifuTextBox1.Hide();
            bunifuTextBox2.Hide();
            bunifuTextBox3.Hide();
            bunifuTextBox4.Hide();
        }
        private void tabPage3_Enter(object sender, EventArgs e)
        {
            Hider();
            
        }
    }
}
