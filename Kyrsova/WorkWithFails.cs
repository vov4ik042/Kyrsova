using System;
using System.Windows.Forms;

namespace Kyrsova
{
    internal class WorkWithFails
    {
        private string FileName { get; set; }
        public WorkWithFails() { }

        public string OpenFileDialogFunction(OpenFileDialog openFileDialog1, bool TxtBool)
        {
            if (TxtBool == true)
                openFileDialog1.Filter = "Txt files (*.txt)|*.txt";
            else
                openFileDialog1.Filter = "Huf files (*.huf)|*.huf";
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.CheckFileExists == true || openFileDialog1.CheckPathExists == true)
                {
                    FileName = openFileDialog1.FileName;
                    return FileName;
                }
            }
            return "0";
        }
    }
}
