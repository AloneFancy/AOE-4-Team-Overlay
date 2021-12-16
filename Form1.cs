using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WindowsFormsApp6.Properties;
//using MongoDB.Driver;


namespace WindowsFormsApp6
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        static string get_request(string url)
        {
            string html = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            
            
            /*if (request.ContentLength < 30)
                return null;*/
            request.AutomaticDecompression = DecompressionMethods.GZip;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                }
            }catch (WebException ex)
            {
                return String.Empty;
            }
            return html;
        }

        class players{
            private string name;
            private int civ,elo;
            public players(string name,int civ,int elo)
            {
                this.civ = civ; 
                this.name = name;   
                this.elo = elo;
            }
            public string Name 
            {
                get { return name; }
                set { name = value; }
            }
            public int Civ
            {
                get { return civ; }
                set { civ = value; }    
            }
            public int Elo
            {
                get { return elo; }
                set { elo = value; }
            }
        }
        Bitmap  getCivFlag(int civ)
        {
            switch (civ)
            {
                    case 0: return Resources.Abbasid_Dynasty;
                    case 1: return Resources.Chinese;
                    case 2: return Resources.Delhi_Sultanate;
                    case 3: return Resources.English;
                    case 4: return Resources.French;
                    case 5: return Resources.Holy_Roman;
                    case 6: return Resources.Mongols;
                    case 7: return Resources.Rus;
                default:break;
            }
            return Resources.Chinese;
        }
        private void button1_Click(object sender, EventArgs e)
        {            
            String url = "https://aoeiv.net/api/player/matches?game=aoe4" + "&profile_id=" + textBox1.Text + "&count=1";
            Console.WriteLine(url);
            String url_guide = "https://aoeiv.net/api/strings?game=aoe4&language=en";
            dynamic guide  = JObject.Parse(get_request(url_guide));
            dynamic data;
            try
            {
                data = JArray.Parse(get_request(url));
            }
            catch (Exception ex)
            {
                return;
            }
            if (data.Count == 0 || data == null)
                return;
            
                var guide_map = guide["map_type"];
                String query = "$.[?(@.id==" + data[0]["map_type"] + ")]";
                var maps = guide_map.SelectToken(query);

                String result = "Map: " + maps["string"] + "\n";

                String test = data[0]["num_players"];
                int team_players = Int32.Parse(test);
                List<players> team1 = new List<players>();
                List<players> team2 = new List<players>();

                for (int i = 0; i < team_players; i++)
                {
                    String temp = data[0]["players"][i]["civ"];
                    int civ = Int32.Parse(temp);
                    String eloz = data[0]["players"][i]["rating"];
                    int elo = Int32.Parse(eloz);
                    String name = data[0]["players"][i]["name"];
                    players temp1 = new players(name, civ, elo);
                    if (data[0]["players"][i]["team"] == 1)
                        team1.Add(temp1);
                    else
                        team2.Add(temp1);
                }
                //MessageBox.Show(result);
                Point tablePosition = new Point();
                tablePosition.X = textBox2.Location.X;
                tablePosition.Y = 100;
                createTableLayoutPanel(tablePosition, team1, team2);
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }       
        private void createTableLayoutPanel(Point XY,List<players>team1,List<players>team2)
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            panel.Location = new System.Drawing.Point(XY.X,XY.Y);            
            int resize_height = 20 * (team1.Count) + 60;
            panel.Size = new System.Drawing.Size(600, resize_height);
            Controls.Add(panel);
            panel.ColumnCount = 6;
            panel.RowCount = 1;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 2F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.5F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.5F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 2F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            panel.Controls.Add(new Label() {Text = "ELO"}, 0, 0);
            panel.Controls.Add(new Label() {Text = "CIV" }, 1, 0);
            panel.Controls.Add(new Label() { Text = "Team 1" }, 2, 0);
            panel.Controls.Add(new Label() { Text = "Team 2" }, 3, 0);            
            panel.Controls.Add(new Label() { Text = "CIV" }, 4, 0);
            panel.Controls.Add(new Label() { Text = "ELO" }, 5, 0);
                        
            for (int i = 0; i < team1.Count; i++)
            {
                panel.RowCount = panel.RowCount + 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));                
                panel.Controls.Add(new Label() { Text = team1[i].Elo.ToString() }, 0, panel.RowCount - 1);
                panel.Controls.Add(new PictureBox() { Image = getCivFlag(team1[i].Civ), SizeMode = PictureBoxSizeMode.StretchImage}, 1, panel.RowCount - 1);
                panel.Controls.Add(new Label() { Text = team1[i].Name}, 2, panel.RowCount - 1);
                panel.Controls.Add(new Label() { Text = team2[i].Name  },3, panel.RowCount - 1);
                panel.Controls.Add(new PictureBox() { Image = getCivFlag(team2[i].Civ), SizeMode = PictureBoxSizeMode.StretchImage }, 4, panel.RowCount - 1);
                panel.Controls.Add(new Label() { Text = team2[i].Elo.ToString() }, 5, panel.RowCount - 1);
            }            
        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {
            
           
        }
    }
}
