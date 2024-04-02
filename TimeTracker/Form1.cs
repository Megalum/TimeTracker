using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TimeTracker
{
    public partial class Form1 : Form
    {
        private DataBase dataBase = new DataBase();
        private string id;
        private int selectedRows;
        private bool status;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (status)
            {
                label8.Visible = true;
                comboBox1.Visible = true;
                status = false;
                button2.Text = "Начать";
                await dataBase.EndStartlementTaskAsync(id, "Выполнено", null, DateTime.Now.TimeOfDay.ToString().Remove(8));
                await refrashDataGridAsync(dataGridView1);
                createBox(DateTime.Today);
            }
            else
            {
                label8.Visible = false;
                comboBox1.Visible = false;
                status = true;
                id = comboBox2.Items[comboBox1.SelectedIndex].ToString();
                button2.Text = "Закончить";
                await dataBase.EndStartlementTaskAsync(id, "Выполняется", DateTime.Now.TimeOfDay.ToString().Remove(8), null);
                await refrashDataGridAsync(dataGridView1);
            }
            editInfomarion(DateTime.Today);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private async void button5_Click(object sender, EventArgs e)
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
                await dataBase.InsertElementAsync(dateTimePicker1.Value.ToString().Remove(10, 8), textBox2.Text, dateTimePicker2.Value.TimeOfDay, dateTimePicker3.Value.TimeOfDay, "Ожидается");
                await refrashDataGridAsync(dataGridView1);
                createBox(DateTime.Today);

                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Today;
                dateTimePicker2.Value = DateTime.Today;
                dateTimePicker3.Value = DateTime.Today;
                tabControl1.SelectedIndex = 0;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private async void button7_Click(object sender, EventArgs e)
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
                await dataBase.UpdateElementAsync(dataGridView1.Rows[selectedRows].Cells[0].Value.ToString(), textBox3.Text, dateTimePicker5.Value.TimeOfDay, dateTimePicker4.Value.TimeOfDay);
                await refrashDataGridAsync(dataGridView1);
                readClone(dataGridView1, dataGridView2);
                editInfomarion(DateTime.Today);
            }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await dataBase.DeleteElemenyByIdAsync(dataGridView1.Rows[selectedRows].Cells[0].Value.ToString());
            await refrashDataGridAsync(dataGridView1);
            readClone(dataGridView1, dataGridView2);
            editInfomarion(DateTime.Today);
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
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void createColumns()
        {
            dataGridView1.Columns.Add("id", "№");
            dataGridView1.Columns.Add("Date", "Дата");
            dataGridView1.Columns.Add("Time_Start", "Время начала");
            dataGridView1.Columns.Add("Time_Stop", "Время окончания");
            dataGridView1.Columns.Add("Task", "Задача");
            dataGridView1.Columns.Add("Status_Task", "Статус");
        }

        private void createColumns(DataGridView dwg)
        {
            dwg.Columns.Add("Date", "Дата");
            dwg.Columns.Add("Time_Start", "Время начала");
            dwg.Columns.Add("Time_Stop", "Время окончания");
            dwg.Columns.Add("Task", "Задача");
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

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
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
                textBox1.Text += date.ToString("d MMMM") + ":\r\n";
            }

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.Equals(date))
                {
                    for (int j = 2; j < dataGridView1.ColumnCount; j++)
                    {
                        textBox1.Text += dataGridView1.Rows[i].Cells[j].Value.ToString();
                        textBox1.Text += "   ";
                    }
                    textBox1.Text += "\r\n";
                }
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            button9.Enabled = false;
            button10.Enabled = true;
            button11.Enabled = true;
            statistics(7);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
            button9.Enabled = true;
            button11.Enabled = true;
            statistics(31);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            button11.Enabled = false;
            button10.Enabled = true;
            button9.Enabled = true;
            statistics(365);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            
            dateTimePicker1.MinDate = DateTime.Today;

            createColumns();
            createColumns(dataGridView2);
            await refrashDataGridAsync(dataGridView1);
            await autoChangesStatusAsync();
            //DateTime start = DateTime.Parse(dataGridView1.Rows[0].Cells[2].Value.ToString());
            //DateTime stop = DateTime.Parse(dataGridView1.Rows[0].Cells[3].Value.ToString());
            //TimeSpan timeGep = stop - start;
            //string gep = "16:30:32";
            //int o = (int.Parse(gep.Remove(2)) * 60) + (int.Parse(gep.Remove(0, 3).Remove(2)));
            //MessageBox.Show(o.ToString());
            readClone(dataGridView1, dataGridView2);
            status = statusProgress("Выполняется");
            if (status)
            {
                comboBox1.Visible = false;
                label8.Visible = false;
                button2.Text = "Закончить";
            }
            else
            {
                button2.Text = "Начать";
                if (statusProgress("Ожидается"))
                {
                    comboBox1.Visible = true;
                    label8.Visible = true;
                    createBox(DateTime.Today);                   
                }
                else
                {
                    button2.Enabled = false;
                }
            }
            editInfomarion(DateTime.Today);
            statistics(7);
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            editInfomarion(monthCalendar1.SelectionRange.Start);
        }

        private void readClone(DataGridView firstDwg, DataGridView secondDwg)
        {
            secondDwg.Rows.Clear();

            for (int i = 0; i < firstDwg.Rows.Count - 1; i++)
            {
                DateTime dt = DateTime.Parse(firstDwg.Rows[i].Cells[1].Value.ToString());
                if (dt >= DateTime.Today)
                {
                    secondDwg.Rows.Add(firstDwg.Rows[i].Cells[1].Value.ToString().Remove(10), firstDwg.Rows[i].Cells[2].Value,
                        firstDwg.Rows[i].Cells[3].Value, firstDwg.Rows[i].Cells[4].Value);
                }
            }
        }

        private void readSingleRow(DataGridView dwg, IDataRecord record)
        {
            monthCalendar1.AddBoldedDate(DateTime.Parse(record.GetValue(1).ToString()));
            dwg.Rows.Add(record.GetInt32(0), record.GetValue(1), record.GetValue(2), record.GetValue(3), record.GetString(4), record.GetString(5));           
        }

        private async Task refrashDataGridAsync(DataGridView dwg)
        {
            dwg.Rows.Clear();

            SqlDataReader reader = dataBase.GetElementsList();

            while (await reader.ReadAsync())
            {
                readSingleRow(dwg, reader);
            }
            reader.Close();
            dataBase.closeConnection();
        }

        private async Task autoChangesStatusAsync()
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                DateTime dt = DateTime.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());

                if (dt >= DateTime.Today) { break; }
                if (dataGridView1.Rows[i].Cells[5].Value.Equals("Ожидается"))
                {
                    await dataBase.ChangesStatus(dataGridView1.Rows[i].Cells[0].Value.ToString());
                } else if (dataGridView1.Rows[i].Cells[5].Value.Equals("Выполняется"))
                {
                    await dataBase.EndChangesStatus(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
            }
            await refrashDataGridAsync(dataGridView1);
        }

        private bool statusProgress(string status)
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.Equals(DateTime.Today) &&
                    dataGridView1.Rows[i].Cells[5].Value.Equals(status))
                {
                    id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    return true;
                }
            }
            return false;
        }

        private void statistics(int value)
        {
            chart1.Series[0].Points.Clear();
            TimeSpan[] graph = timeSpean(value);
            DateTime day = dayStartStatistix(value);

            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd-MM-yyyy";
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "HH:mm:ss";
            chart1.Series[0].XValueType = ChartValueType.DateTime;
            chart1.ChartAreas[0].AxisX.Minimum = day.ToOADate();
            chart1.ChartAreas[0].AxisX.Maximum = DateTime.Today.ToOADate();
            chart1.ChartAreas[0].AxisY.Minimum = DateTime.Today.ToOADate();

            for (int i = 0; i < value; i++)
            {
                if (graph[i] > TimeSpan.Parse(DateTime.Today.TimeOfDay.ToString()))
                {
                    chart1.Series[0].Points.AddXY(day, DateTime.Parse(graph[i].ToString()));
                }
                day = day.AddDays(1);
            }
            

        }

        private TimeSpan[] timeSpean(int count)
        {
            TimeSpan[] output = new TimeSpan[count];
            DateTime day = dayStartStatistix(count);
            TimeSpan timeng = TimeSpan.Parse("00:00:00");

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                {
                    if (DateTime.Parse(dataGridView1.Rows[j].Cells[1].Value.ToString()) > day)
                    {
                        break;
                    }
                    else if (DateTime.Parse(dataGridView1.Rows[j].Cells[1].Value.ToString()) == day &&
                        dataGridView1.Rows[j].Cells[5].Value.Equals("Выполнено"))
                    {
                        DateTime start = DateTime.Parse(dataGridView1.Rows[j].Cells[2].Value.ToString());
                        DateTime stop = DateTime.Parse(dataGridView1.Rows[j].Cells[3].Value.ToString());
                        TimeSpan timeGep = stop - start + timeng;
                        timeng = timeGep;
                    }
                }
                day = day.AddDays(1);
                output[i] = timeng;
                timeng = TimeSpan.Parse("00:00:00");
            }

            return output;
        }

        private DateTime dayStartStatistix(int count)
        {
            string dateGep;
            DateTime day;
            if (count == 7)
            {
                dateGep = "07.01." + DateTime.Today.Year.ToString();
                day = new DateTime(2024, 1, 1).AddDays(Double.Parse(
                DateTime.Today.Subtract(DateTime.Parse(dateGep)).Days.ToString()));
            }
            else if (count == 31)
            {
                dateGep = "01.02." + DateTime.Today.Year.ToString();
                day = new DateTime(2024, 1, 1).AddDays(Double.Parse(
                DateTime.Today.Subtract(DateTime.Parse(dateGep)).Days.ToString()));
            }
            else
            {
                dateGep = DateTime.Today.ToString("dd.MM") + "." + (DateTime.Today.Year - 1).ToString();
                day = DateTime.Parse(dateGep);
            }
            return day;
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
        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }


    }
}