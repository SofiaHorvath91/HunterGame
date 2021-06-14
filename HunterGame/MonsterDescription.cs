using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HunterGame
{
    public partial class MonsterDescription : Form
    {
        string source;
        List<string> monsterPictureSources;

        public MonsterDescription(string s)
        {
            InitializeComponent();
            this.source = s;

            monsterPictureSources = Directory.GetFiles("../../Pictures/Monsters").ToList();

            foreach(string line in monsterPictureSources)
            {
                if(line.Contains(s+"."))
                {
                    this.BackgroundImage = Image.FromFile(line);
                    BackgroundImageLayout = ImageLayout.Stretch;
                }
            }

        }
    }
}
