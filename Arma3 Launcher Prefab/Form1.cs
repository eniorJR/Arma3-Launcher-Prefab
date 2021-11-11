using System;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;

namespace Arma3_Launcher_Prefab
{
    public partial class Form1 : Form
    {
        public string arma3folder = @"C:\Program Files (x86)\Steam\steam.exe";
        string carpeta_workshop = @"C:\Program Files (x86)\Steam\steamapps\common\Arma 3\!Workshop\";
        public Form1()
        {
            InitializeComponent();
            startoptions();



        }


        private void startoptions()
        {
            checkfolder(); // Chekeo de carpeta de arma3
            Loadprefabs();
            panel6.Hide(); // Ocultamos panel de ajustes




        }
        private void Loadprefabs()
        {
            string name;
            //Hace un refresh
            comboBox1.Items.Clear();
            comboBox3.Items.Clear();
            string datadirectory = @"data.xml";
            if (!(File.Exists(datadirectory))) { XmlWriter.Create("data.xml"); }
            using (XmlReader reader = XmlReader.Create("data.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                        {
                        //chekea
                        switch (reader.Name.ToString())
                        {
                            case "Name":
                                name = reader.ReadString();
                                
                                comboBox1.Items.Add(name);
                                comboBox3.Items.Add(name);
                                break;
                        }
                    }
                }
            }


        }


        private void Editprefabs()
        {
            using (XmlReader reader = XmlReader.Create("data.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        try
                        {
                            string selection = "none";
                            //return only when you have START tag  
                            if (reader.Name.ToString() == comboBox3.SelectedItem.ToString())
                            {
                                selection = reader.ReadString();
                                textBox3.Text = selection;
                                textBox2.Text = comboBox3.SelectedItem.ToString();
                                var ip = reader.ReadToFollowing("ip-" + comboBox3.SelectedItem.ToString());
                                if (ip.ToString() != "False") { textBox4.Text = reader.ReadElementContentAsString(); } else { textBox4.Text = "0.0.0.0"; }

                            }
                        }
                        catch ( Exception e ){
                                error_process(null, e);


                            }

                    }
                    Console.WriteLine("");
                }
            }
        }



        private void checkfolder()
        {
            if (File.Exists(arma3folder))
            {
                textBox1.Text = arma3folder;
            }
            else{textBox1.Text = "Seleccionar ruta"; arma3folder = "Seleccionar ruta"; }
            
            if (Directory.Exists(carpeta_workshop))
            {
                textBox5.Text = carpeta_workshop;
                modlist();
            }
            else { textBox1.Text = "Seleccionar ruta"; carpeta_workshop = "Seleccionar ruta"; }


        }


        void modlist()
        {
            var mods = Directory.GetDirectories(carpeta_workshop);
            foreach (string carpetas in mods)
            {
                string nombresinruta = carpetas.Replace(carpeta_workshop,"");
                lista_mods.Items.Add(nombresinruta);
            }



        }
        void error_process(string errorTexto, Exception error)
        {
            error1panel.Visible = false;
            System.Threading.Thread.Sleep(100);
            SystemSounds.Hand.Play();
            error1panel.Visible = true;
            if (error != null)
            {
                if (error.Message == "Un nombre no puede empezar con el carácter ';', valor hexadecimal 0x3B.") { alerttext.Text = "¡Algo ha salido mal! (Valor no válido)."; 
                } else if(error.Message == "El carácter ' ', con valor hexadecimal 0x20, no puede incluirse en un nombre.")
                { alerttext.Text = "¡Algo ha salido mal! (Elimina los espacios).";}else { alerttext.Text = error.Message; }
            }
            else if (errorTexto != null)
            { alerttext.Text = errorTexto; }
            else
            {MessageBox.Show("¡Algo ha salido mal!", "Error");}

            }

        private void botonesedicion (string boton)
        {
            try {
                if (boton == "crear")
                {
                    if (textBox2.Text != "" && textBox3.Text != "")
                    {
                        XDocument doc = XDocument.Load("data.xml");
                        XElement root = new XElement("Preset");
                        root.Add(new XElement("Name", textBox2.Text));
                        root.Add(new XElement(textBox2.Text, textBox3.Text));
                        root.Add(new XElement("ip-"+textBox2.Text, textBox4.Text));
                        doc.Element("Presets").Add(root);
                        doc.Save("data.xml");
                        Loadprefabs();
                    }
                    else { error_process("Necesitas rellenar todos los campos.", null); }
                }
                else if (boton == "añadir mod")
                {
                    var modsselecionados = lista_mods.CheckedItems;
                    foreach (string seleccionado in modsselecionados)
                    {
                        textBox3.Text += (carpeta_workshop + seleccionado+";");

                    }
                }
                }
            catch (Exception e)
            {error_process(null,e);}
        }




        void ejecutarjuego(string preset)
        {
            string mods = null;
            string ip_server = null;

            using (XmlReader reader = XmlReader.Create("data.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string selection = "none";
                        if (reader.Name.ToString() == preset)
                        {
                            selection = reader.ReadString();
                            mods = selection;
                            var ip = reader.ReadToFollowing("ip-" + comboBox3.SelectedItem.ToString());
                            if (ip.ToString() != "False" && reader.ReadElementContentAsString() != "0.0.0.0") { ip_server = reader.ReadElementContentAsString(); }
                        }


                    }
                }
            }
            if (arma3folder != "Seleccionar ruta")
            {
                if (mods != null)
                {
                    try
                    {
                        if (ip_server != null) { Process.Start(arma3folder, "-applaunch 107410 -noLauncher -useBE -nosplash -world=empty -skipIntro " + '"' + "-mod=" + mods + '"'+ "-connect="+ip_server); } else {Process.Start(arma3folder, "-applaunch 107410 -noLauncher -useBE -nosplash -world=empty -skipIntro " + '"' + "-mod=" + mods + '"'); }
                        //Para testeos --> MessageBox.Show(arma3folder + "-applaunch 107410 -noLauncher -useBE -nosplash -world=empty -skipIntro "+ "\"" +"-mod=" + mods + "\"");
                        
                    }
                    catch (Exception error)
                    {
                        error_process(null, error);

                    }
                }
            }

        }











        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        { 

        }
        private void button5_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex != -1) { ejecutarjuego(comboBox1.SelectedItem.ToString()); } else { error_process("No has seleccionado ningun preset", null); }
             
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            panel6.Show();
            panel4.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (var carpeta = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = carpeta.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    arma3folder = carpeta.SelectedPath;
                    textBox1.Text = (arma3folder+@"\steam.exe");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel6.Hide();
            panel4.Show();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Editprefabs();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            botonesedicion("crear");
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void alerttext_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            using (var carpeta = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = carpeta.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    carpeta_workshop = carpeta.SelectedPath;
                    textBox5.Text = carpeta_workshop;
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

            botonesedicion("añadir mod");
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
