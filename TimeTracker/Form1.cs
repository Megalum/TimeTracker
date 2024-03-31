using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TimeTracker
{

    public partial class Form1 : Form
    {

        DataBase dataBase = new DataBase();
        int selectedRows;
        bool status;
        string id;

        public Form1()
        {
            InitializeComponent();
        }

        private string getID(string text)
        {
            return comboBox2.Items[0].ToString();
        }

        private bool statusProgress()
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.Equals(DateTime.Today) &&
                    dataGridView1.Rows[i].Cells[5].Value.Equals("Выполняется"))
                {
                    id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    return true;
                }
            }
            return false;
        }

        private void createBox(DateTime date)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.Equals(date) &&
                    !dataGridView1.Rows[i].Cells[5].Value.Equals("Выполнено"))
                {
                    comboBox1.Items.Add(dataGridView1.Rows[i].Cells[4].Value.ToString());
                    comboBox2.Items.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
            }
            if (comboBox1.Items.Count == 0)
            {
                button2.Enabled = false;
            } else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void createColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("Date", "Date");
            dataGridView1.Columns.Add("Time_Start", "Time_Start");
            dataGridView1.Columns.Add("Time_Stop", "Time_Stop");
            dataGridView1.Columns.Add("Task", "Task");
            dataGridView1.Columns.Add("Status_Task", "Status");

        }
        private void createColumns(DataGridView dwg)
        {
            dwg.Columns.Add("Date", "Дата");
            dwg.Columns.Add("Time_Start", "Время начала");
            dwg.Columns.Add("Time_Stop", "Время окончания");
            dwg.Columns.Add("Task", "Планирование");
            dwg.Columns.Add("Status_Task", "Статус");
        }

        private void readSingleRow(DataGridView dwg, IDataRecord record)
        {
            dwg.Rows.Add(record.GetInt32(0), record.GetValue(1), record.GetValue(2), record.GetValue(3), record.GetString(4), record.GetString(5));
        }

        private void readClone(DataGridView firstDwg, DataGridView secondDwg)
        {
            secondDwg.Rows.Clear();

            for (int i = 0; i < firstDwg.Rows.Count - 1; i++)
            {
                secondDwg.Rows.Add(firstDwg.Rows[i].Cells[1].Value.ToString().Remove(10), firstDwg.Rows[i].Cells[2].Value,
                    firstDwg.Rows[i].Cells[3].Value, firstDwg.Rows[i].Cells[4].Value);
            }
        }

        private void refrashDataGrid(DataGridView dwg)
        {
            dwg.Rows.Clear();

            string query = $"SELECT * FROM Tracker";

            SqlCommand command = new SqlCommand(query, dataBase.getConnection());
            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                readSingleRow(dwg, reader);
            }
            reader.Close();
        }

        private void editInfomarion(DateTime date)
        {
            textBox1.Clear();
            textBox1.Text = "Планы на ";
            if (date.Equals(DateTime.Today))
            {
                textBox1.Text += "cегодня:\r\n";
            }
            else
            {
                textBox1.Text += date.Day.ToString() + ":\r\n";
            }

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.Equals(date))
                {
                    for(int j = 2; j < dataGridView1.ColumnCount; j++)
                    {
                        textBox1.Text += dataGridView1.Rows[i].Cells[j].Value.ToString();
                        textBox1.Text += "   ";
                    }
                    textBox1.Text += "\r\n";
                }
            }

        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g;
            string sText;
            int iX;
            float iY;

            SizeF sizeText;
            TabControl ctlTab;

            ctlTab = (TabControl)sender;

            g = e.Graphics;

            sText = ctlTab.TabPages[e.Index].Text;
            sizeText = g.MeasureString(sText, ctlTab.Font);
            iX = e.Bounds.Left + 6;
            iY = e.Bounds.Top + (e.Bounds.Height - sizeText.Height) / 2;
            g.DrawString(sText, ctlTab.Font, Brushes.Black, iX, iY);

            e.Graphics.SetClip(e.Bounds);
            string text = tabControl1.TabPages[e.Index].Text;
            SizeF sz = e.Graphics.MeasureString(text, e.Font);

            bool bSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            using (SolidBrush b = new SolidBrush(bSelected ? SystemColors.Highlight : SystemColors.Control))
                e.Graphics.FillRectangle(b, e.Bounds);

            using (SolidBrush b = new SolidBrush(bSelected ? SystemColors.HighlightText : SystemColors.ControlText))
                e.Graphics.DrawString(text, e.Font, b, e.Bounds.X + 2, e.Bounds.Y + (e.Bounds.Height - sz.Height) / 2);

            if (tabControl1.SelectedIndex == e.Index)
                e.DrawFocusRectangle();

            e.Graphics.ResetClip();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query;
            if (status)
            {
                label8.Visible = true;
                comboBox1.Visible = true;
                status = false;
                button2.Text = "Начать";
                query = $"UPDATE Tracker SET Time_Stop = \'{DateTime.Now.TimeOfDay.ToString().Remove(8)}\', " +
                    $"Status_Task = \'Выполнено\'" +
                    $"WHERE id = \'{id}\'";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.ExecuteReader();
                dataBase.closeConnection();
                refrashDataGrid(dataGridView1);
                createBox(DateTime.Today);
            } else
            {
                label8.Visible = false;
                comboBox1.Visible = false;
                status = true;
                id = comboBox2.Items[comboBox1.SelectedIndex].ToString();
                button2.Text = "Закончить";
                query = $"UPDATE Tracker SET Time_Start = \'{DateTime.Now.TimeOfDay.ToString().Remove(8)}\', " +
                    $"Status_Task = \'Выполняется\'" +
                    $"WHERE id = \'{id}\'";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.ExecuteReader();
                dataBase.closeConnection();
                refrashDataGrid(dataGridView1);                
            }
            editInfomarion(DateTime.Today);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Today;
            createColumns();
            createColumns(dataGridView2);
            refrashDataGrid(dataGridView1);
            readClone(dataGridView1, dataGridView2);
            status = statusProgress();
            if (status)
            {
                comboBox1.Visible = false;
                label8.Visible = false;
                button2.Text = "Закончить";
            } else
            {
                createBox(DateTime.Today);
                button2.Text = "Начать";
            }
            editInfomarion(DateTime.Today);
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dateTimePicker3.Value.TimeOfDay < dateTimePicker2.Value.TimeOfDay)
            {
                MessageBox.Show("Время начала не может быть позднее времени окончания!", "Error");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Заполните поле планирование!", "Error");
            }
            else
            {
                string query = $"INSERT INTO Tracker (Day_Of_Completion, Time_Start, Time_Stop, Task, Status_Task) " +
                    $"VALUES (\'{dateTimePicker1.Value.ToString().Remove(10, 8)}\', " +
                    $"\'{dateTimePicker2.Value.TimeOfDay}\', " +
                    $"\'{dateTimePicker3.Value.TimeOfDay}\', " +
                    $"\'{textBox2.Text}\', " +
                    $"\'Ожидается\')";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.ExecuteReader();
                dataBase.closeConnection();
                refrashDataGrid(dataGridView1);
                createBox(DateTime.Today);

                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Today;
                dateTimePicker2.Value = DateTime.Today;
                dateTimePicker3.Value = DateTime.Today;
                tabControl1.SelectedIndex = 0;
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            editInfomarion(monthCalendar1.SelectionRange.Start);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dateTimePicker4.Value.TimeOfDay < dateTimePicker5.Value.TimeOfDay)
            {
                MessageBox.Show("Время начала не может быть позднее времени окончания!", "Error");
            }
            else if (textBox3.Text == "")
            {
                MessageBox.Show("Заполните поле планирование!", "Error");
            }
            else
            {
                string query = $"UPDATE Tracker SET Time_Start = \'{dateTimePicker5.Value.TimeOfDay}\', " +
                    $"Time_Stop = \'{dateTimePicker4.Value.TimeOfDay}\', " +
                    $"Task = \'{textBox3.Text}\' " +
                    $"WHERE Id = {dataGridView1.Rows[selectedRows].Cells[0].Value}";

                SqlCommand command = new SqlCommand(query, dataBase.getConnection());
                command.ExecuteReader();
                dataBase.closeConnection();
                refrashDataGrid(dataGridView1);
                readClone(dataGridView1, dataGridView2);
                editInfomarion(DateTime.Today);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string query = $"DELETE FROM Tracker " +
                $"WHERE Id = {dataGridView1.Rows[selectedRows].Cells[0].Value}";
            SqlCommand command = new SqlCommand(query, dataBase.getConnection());
            command.ExecuteReader();
            dataBase.closeConnection();
            refrashDataGrid(dataGridView1);
            readClone(dataGridView1, dataGridView2);
            editInfomarion(DateTime.Today);
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            label5.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
            textBox3.Visible = true;
            dateTimePicker4.Visible = true;
            dateTimePicker5.Visible = true;

            selectedRows = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[selectedRows];

                dateTimePicker4.Value = DateTime.Parse(row.Cells[2].Value.ToString());
                dateTimePicker5.Value = DateTime.Parse(row.Cells[1].Value.ToString());
                textBox3.Text = row.Cells[3].Value.ToString();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
