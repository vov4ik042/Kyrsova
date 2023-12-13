using System;
using System.Windows.Forms;

namespace Kyrsova
{
    public partial class MainWindow1 : Form
    {
        private string FileName { get; set; }
        private byte SelectedIndexChanged { get; set; }
        public MainWindow1()
        {
            InitializeComponent();
            EnableAndVisible(false, true, comboBox1, label1, button1, button2);
        }

        private void EnableAndVisible(bool enable, bool visible, params Control[] control)
        {
            for (int i = 0; i < control.Length; i++)
            {
                control[i].Enabled = enable;
                control[i].Visible = visible;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool radioButtonBool;
            if (comboBox1.SelectedIndex >= 0)
            {
                if (radioButton1.Checked == true)
                    radioButtonBool = true;
                else
                    radioButtonBool = false;
                var workFile = new WorkWithFails();
                FileName = workFile.OpenFileDialogFunction(openFileDialog1, radioButtonBool);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && FileName != "0" && !string.IsNullOrEmpty(FileName))
            {
                var compression = new Compression();
                if (radioButton1.Checked == true)
                {
                    compression.CompressionFile(FileName, $"{FileName}.huf");
                }
                else
                {
                    compression.DeCompression(FileName, $"{FileName}.txt");
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            EnableAndVisible(true, true, comboBox1, label1, button1, button2);
            comboBox1.Items.Clear();
            comboBox1.Text = string.Empty;
            comboBox1.Items.Add("Txt");
            comboBox1.SelectedIndex = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            EnableAndVisible(false, true, comboBox1, label1);
            EnableAndVisible(true, true, button1, button2);
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Huf");
            comboBox1.SelectedIndex = 0;
        }
    }
}
