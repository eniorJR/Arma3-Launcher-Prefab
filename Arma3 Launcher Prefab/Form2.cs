using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Arma3_Launcher_Prefab
{
    public partial class Form2 : Form
    {
        public string newfoldera3; 

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           

            
            using (var carpeta = new System.Windows.Forms.FolderBrowserDialog())
            {

                System.Windows.Forms.DialogResult result = carpeta.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    newfoldera3 = carpeta.SelectedPath;
                    this.Close();
                    
                    using (Form1 form1 = new Form1())
                        form1.Show();
                }

            }
        }
    }
}
