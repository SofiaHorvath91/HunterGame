using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace HunterGame
{
    public partial class HunterGame : Form
    {
        bool roundOneOn;
        Button button;
        Label label;
        Random generator = new Random();
        List<string> weaponPictureSources = new List<string>();
        List<string> monsterPictureSources = new List<string>();
        List<PictureBox> weaponsPicBoxesList = new List<PictureBox>();
        List<Label> weaponLabelsList = new List<Label>();
        List<Label> monstersLabelsListToDrag = new List<Label>();
        List<Panel> monstersWeaponsPanelsToFill = new List<Panel>();
        List<Label> roundOneResultLabels = new List<Label>();
        List<WeaponMonsters> weaponsMonsters = new List<WeaponMonsters>();
        List<WeaponMonsters> weaponsMonstersUser = new List<WeaponMonsters>();
        List<Control> roundTwoControls = new List<Control>();
        List<string> monsters = new List<string>();
        List<string> weapons = new List<string>();
        List<string> randomMonstersRoundTwo = new List<string>();
        List<Tuple<string, string>> monsterKillingDescriptions = new List<Tuple<string, string>>();
        List<Tuple<string, string[]>> roundTwoSolutions = new List<Tuple<string, string[]>>();
        int userPointsFirstRound = 0;
        Button startRoundTwoButton;
        PictureBox RoundTwoPicBox;
        ProgressBar roundTwoProgressBar;
        Timer progressBarTimer;
        int countRoundTwo = 0;
        int totalRoundsOfRoundTwo = 12;
        int lifePointsRoundTwo = 6;
        int userResultRoundTwo = 0;
        Label lifePointsLabel;
        Label countRoundsLabel;
        Label killingDescriptionLabel;
        string goodWeapon;
        Timer timerRoundTwo;
        Timer timerShowSolutionRoundTwo;
        double roundOneFinalResult;
        double roundTwoFinalResult;
        double finalResult;
        Label resultComment;
        System.Media.SoundPlayer soundPlayer;

        public HunterGame()
        {
            InitializeComponent();
        }

        void ReadInWMs(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string text = reader.ReadToEnd();
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] splitLine = line.Split(':').ToArray();


                if (splitLine[1].Contains("/"))
                {
                    string[] splitSecondPart = splitLine[1].Split('/').ToArray();
                    List<string> primarymonsters = splitSecondPart[0].Split(' ').ToList();
                    List<string> secondarymonsters = splitSecondPart[1].Split(' ').ToList();

                    WeaponMonsters newWM = new WeaponMonsters(splitLine[0], primarymonsters, secondarymonsters);
                    weaponsMonsters.Add(newWM);
                }
                else
                {
                    WeaponMonsters newWM = new WeaponMonsters(splitLine[0], splitLine[1].Split(' ').ToList(), null);
                    weaponsMonsters.Add(newWM);
                }
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            StartGame();
        }

        void StartGame()
        {
            ClearForm();
            ClearRoundTwoResults();

            userPointsFirstRound = 0;

            this.BackgroundImage = Image.FromFile("../../Pictures/startbackground.png");
            BackgroundImageLayout = ImageLayout.Stretch;

            Controls.Add(new Button()
            {
                Name = "StartButton",
                Left = 520,
                Top = 640,
                Width = 260,
                Height = 60,
                Font = new Font("Chiller", 28, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "JOIN THE HUNT!",
            });
            button = (Button)Controls.Find("StartButton", false)[0];
            button.Click += startButton_Click;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            button.Dispose();
            RoundOneInstructions();
        }

        void RoundOneInstructions()
        {
            this.BackgroundImage = Image.FromFile("../../Pictures/firstroundinstructions.jpg");
            BackgroundImageLayout = ImageLayout.Stretch;

            Controls.Add(new Button()
            {
                Name = "RoundOneButton",
                Left = 520,
                Top = 650,
                Width = 260,
                Height = 60,
                Font = new Font("Chiller", 28, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "START HUNTING!",
            });
            button = (Button)Controls.Find("RoundOneButton", false)[0];
            button.Click += roundOneButton_Click;
        }

        private void roundOneButton_Click(object sender, EventArgs e)
        {
            button.Dispose();
            StartRoundOne();
        }

        void ClearForm()
        {
            weaponPictureSources.Clear();
            monsterPictureSources.Clear();
            weaponsMonstersUser.Clear();
            weaponsPicBoxesList.Clear();
            weaponLabelsList.Clear();
            weaponsMonsters.Clear();
            monstersLabelsListToDrag.Clear();
            monstersWeaponsPanelsToFill.Clear();
            monsters.Clear();
            weapons.Clear();
            monsterKillingDescriptions.Clear();
            roundOneResultLabels.Clear();
            roundTwoControls.Clear();
            roundTwoSolutions.Clear();
            randomMonstersRoundTwo.Clear();
        }

        void GetMonstersWeaponsRefs()
        {
            weaponPictureSources = Directory.GetFiles("../../Pictures/Weapons").ToList();

            ReadInWMs("weaponsmonsters.txt");

            foreach (WeaponMonsters wp in weaponsMonsters)
            {
                if(!weapons.Contains(wp.Weapon))
                {
                    weapons.Add(wp.Weapon);
                }

                foreach (string pm in wp.PrimaryMonsters)
                {
                    if (!monsters.Contains(pm))
                    {
                        monsters.Add(pm);
                    }
                }
            }
        }

        void StartRoundOne()
        {
            ClearForm();

            roundOneOn = true;
            userPointsFirstRound = 0;

            GetMonstersWeaponsRefs();
            monsterPictureSources = Directory.GetFiles("../../Pictures/Monsters").ToList();

            this.BackgroundImage = Image.FromFile("../../Pictures/firstroundbackground.png");
            BackgroundImageLayout = ImageLayout.Stretch;

            GenerateWeaponPicBoxesLabels();
            GenerateMonsterLabelsToDrag();
            GenerateMonsterPanelsToFill();

            foreach (Label l in monstersLabelsListToDrag)
            {
                l.Enabled = true;
                l.MouseDoubleClick += new MouseEventHandler(monsterLabels_DoubleClick);
                l.MouseClick += new MouseEventHandler(monsterLabels_RightClick);
                l.MouseMove += new MouseEventHandler(monsterToDrag_MouseMove);
                l.MouseHover += new EventHandler(monsterToDrag_MouseHover);
                l.MouseLeave += new EventHandler(monsterToDrag_MouseLeave);
            }
            foreach (Panel tb in monstersWeaponsPanelsToFill)
            {
                tb.AllowDrop = true;
                tb.DragDrop += new DragEventHandler(monsterToFill_DragDrop);
                tb.DragOver += new DragEventHandler(monsterToFill_DragOver);
            }

            Controls.Add(new Button()
            {
                Name = "HuntingDoneButton",
                Left = 10,
                Top = 15,
                Width = 150,
                Height = 40,
                Font = new Font("Chiller", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "HUNTING DONE",
            });
            button = (Button)Controls.Find("HuntingDoneButton", false)[0];
            button.Click += huntingDoneButton_Click;

            Controls.Add(new Label()
            {
                Name = "Round1TitleLabel",
                Left = 210,
                Top = 10,
                Width = 1010,
                Height = 50,
                Font = new Font("Chiller", 36, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "ROUND 1 : FIND THE WEAPON AGAINST THE MONSTER!",
            });
            label = (Label)Controls.Find("Round1TitleLabel", false)[0];
        }

        void GenerateWeaponPicBoxesLabels()
        {
            for (int i = 0; i < weaponPictureSources.Count; i++)
            {
                Controls.Add(new PictureBox()
                {
                    Name = weaponsMonsters[i].Weapon + "/Weapon",
                    Left = 12 + i * 105,
                    Top = 120,
                    Width = 100,
                    Height = 100,
                    BackColor = Color.Black,
                });
            }

            foreach (PictureBox pb in this.Controls.OfType<PictureBox>())
            {
                if (pb.Name.Contains("/Weapon"))
                {
                    weaponsPicBoxesList.Add(pb);
                }
            }

            for (int i = 0; i < weaponPictureSources.Count; i++)
            {
                weaponsPicBoxesList[i].Image = Image.FromFile(weaponPictureSources[i]);
                weaponsPicBoxesList[i].ImageLocation = weaponPictureSources[i];

                Size imageSize = weaponsPicBoxesList[i].Image.Size;
                Size fitSize = weaponsPicBoxesList[i].ClientSize;
                weaponsPicBoxesList[i].SizeMode = imageSize.Width > fitSize.Width || imageSize.Height > fitSize.Height ?
                                                  PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
            }

            for (int i = 0; i < weaponPictureSources.Count; i++)
            {
                Controls.Add(new Label()
                {
                    Name = weaponsMonsters[i].Weapon + "_Weapon",
                    Left = 12 + i * 105,
                    Top = 65,
                    Width = 100,
                    Height = 60,
                    Font = new Font("Impact", 12, FontStyle.Regular),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    Text = weaponsMonsters[i].Weapon,
                    TextAlign = ContentAlignment.MiddleCenter,
                });
            }
            foreach (Label l in this.Controls.OfType<Label>())
            {
                if (l.Name.Contains("_Weapon"))
                {
                    weaponLabelsList.Add(l);
                }
            }

        }

        List<string> RandomizeList(List<string> list)
        {
            return list.OrderBy(item => generator.Next()).ToList();
        }

        void GenerateMonsterLabelsToDrag()
        {
            List<string> newMonstersList = RandomizeList(monsters);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Controls.Add(new Label()
                    {
                        Left = 110 + i * 120,
                        Top = 575 + j * 42,
                        Width = 100,
                        Height = 27,
                        Font = new Font("Impact", 12, FontStyle.Regular),
                        ForeColor = Color.Red,
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleCenter,
                    });
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Controls.Add(new Label()
                {
                    Left = 175 + i * 120,
                    Top = 695,
                    Width = 100,
                    Height = 27,
                    Font = new Font("Impact", 12, FontStyle.Regular),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleCenter,
                });
            }
            foreach (Label l in this.Controls.OfType<Label>())
            {
                if (l.Height < 50)
                {
                    monstersLabelsListToDrag.Add(l);
                }
            }
            for (int i = 0; i < newMonstersList.Count; i++)
            {
                monstersLabelsListToDrag[i].Text = newMonstersList[i].ToUpper();
                monstersLabelsListToDrag[i].Name = newMonstersList[i].ToString();
            }
        }

        void GenerateMonsterPanelsToFill()
        {
            for (int i = 0; i < weaponPictureSources.Count; i++)
            {
                Controls.Add(new Panel()
                {
                    Name = weaponsPicBoxesList[i].Name.Split('/')[0] + "_Panel",
                    Left = 12 + i * 105,
                    Top = 230,
                    Size = new System.Drawing.Size(100, 330),
                    Font = new Font("Impact", 12, FontStyle.Regular),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    BorderStyle = BorderStyle.FixedSingle,
                });
            }
            foreach (Panel p in this.Controls.OfType<Panel>())
            {
                if (p.Name.Contains("_Panel"))
                {
                    monstersWeaponsPanelsToFill.Add(p);
                }
            }
        }

        void monsterToDrag_MouseHover(object sender, System.EventArgs e)
        {
            Label l = (Label)sender;
            l.BorderStyle = BorderStyle.FixedSingle;
        }

        private void monsterToDrag_MouseLeave(object sender, System.EventArgs e)
        {
            Label l = (Label)sender;
            l.BorderStyle = BorderStyle.None;
        }

        private void monsterToDrag_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DoDragDrop((sender as Control), DragDropEffects.Move);
            }
        }

        void monsterToFill_DragDrop(object sender, DragEventArgs e)
        {
            Panel p = (Panel)sender;
            Control c = e.Data.GetData(e.Data.GetFormats()[0]) as Control;
            if (c != null)
            {
                if (p.Controls.OfType<Label>().ToList().Count == 0)
                {
                    c.Location = new Point(0, 0);
                    //c.Location = p.PointToClient(new Point(e.X, e.Y));
                    p.Controls.Add(c);
                    SoundsDropLabels(p);
                    c.BringToFront();
                }
                else
                {
                    int i = p.Controls.OfType<Label>().ToList().Count;
                    c.Location = new Point(0, i * 27);
                    p.Controls.Add(c);
                    SoundsDropLabels(p);
                    c.BringToFront();
                }
            }

            foreach (Panel panel in this.Controls.OfType<Panel>())
            {
                List<Label> labels = panel.Controls.OfType<Label>().ToList();
                for (int i = 0; i < labels.Count; i++)
                {
                    if(labels[i].Location != new Point(0, i * 27))
                    {
                        labels[i].Location = new Point(0, i * 27);
                    }
                }
            }
        }

        void SoundsDropLabels(Panel p)
        {
            soundPlayer = new System.Media.SoundPlayer();

            if(p.Name.Contains("blade"))
            {
                soundPlayer.SoundLocation = "throwknife.wav";
                soundPlayer.Play();
            }
            else if(p.Name.Contains("trocution"))
            {
                soundPlayer.SoundLocation = "fizzle.wav";
                soundPlayer.Play();
            }
            else if(p.Name.Contains("bloo"))
            {
                soundPlayer.SoundLocation = "bubbling.wav";
                soundPlayer.Play();
            }
            else if (p.Name.Contains("ecapitatio"))
            {
                soundPlayer.SoundLocation = "decapitation.wav";
                soundPlayer.Play();
            }
            else if (p.Name.Contains("ire"))
            {
                soundPlayer.SoundLocation = "torch.wav";
                soundPlayer.Play();
            }
            else
            {
                soundPlayer.SoundLocation = "dropsword.wav";
                soundPlayer.Play();
            }
        }
        
        private void monsterToFill_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void monsterLabels_RightClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Label monster = (Label)sender;
                foreach (string s in monsterPictureSources)
                {
                    if (s.Contains(monster.Name.ToLower() + "."))
                    {
                        MonsterDescription newM = new MonsterDescription(monster.Name.ToLower());
                        newM.ShowDialog(this);
                    }
                }
            }
        }

        private void huntingDoneButton_Click(object sender, EventArgs e)
        {
            int count = monstersWeaponsPanelsToFill.Sum(x => x.Controls.OfType<Label>().ToList().Count);

            if (count == 35)
            {
                roundOneOn = false;
                button.Dispose();
                foreach (Panel p in monstersWeaponsPanelsToFill)
                {
                    p.AllowDrop = false;
                    List<string> panelLabelNames = new List<string>();
                    foreach (Label label in p.Controls.OfType<Label>().ToList())
                    {
                        panelLabelNames.Add(label.Name);
                    }
                    WeaponMonsters newWM = new WeaponMonsters(p.Name.Split('_')[0], panelLabelNames, null);
                    weaponsMonstersUser.Add(newWM);
                }
                ReadInWeaponMsgBoxes("monsterdescriptions.txt");
                ValidateUserChoices();
                AddMissingMonsters();
                ShowRoundOneResult();
            }
            else
            {
                MessageBox.Show("There are still some monsters to grab!");
            }
        }

        void ValidateUserChoices()
        {
            foreach(WeaponMonsters wm in weaponsMonstersUser)
            {
                WeaponMonsters goodWM = weaponsMonsters.Where(x => x.Weapon == wm.Weapon).ToList().FirstOrDefault();
                Panel goodPanel = monstersWeaponsPanelsToFill.Where(x => x.Name.Contains(wm.Weapon + "_Panel")).ToList().FirstOrDefault();

                foreach(string m in wm.PrimaryMonsters)
                {
                    if(goodWM.PrimaryMonsters.Contains(m))
                    {
                        foreach(Label l in goodPanel.Controls.OfType<Label>().ToList())
                        {
                            if (l.Name.ToLower() == m.ToLower())
                            {
                                l.ForeColor = Color.Green;
                                userPointsFirstRound++;
                            }
                        }
                    }
                    else
                    {
                        if (goodWM.SecondaryMonsters != null)
                        {
                            if (goodWM.SecondaryMonsters.Contains(m))
                            {
                                foreach (Label l in goodPanel.Controls.OfType<Label>().ToList())
                                {
                                    if (l.Name.ToLower() == m.ToLower())
                                    {
                                        l.ForeColor = Color.Green;
                                        userPointsFirstRound++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void AddMissingMonsters()
        {
            foreach (Panel p in monstersWeaponsPanelsToFill)
            {
                List<string> userMonsters = p.Controls.OfType<Label>().ToList().Select(x=>x.Name).ToList();
                WeaponMonsters goodMonsters = weaponsMonsters.Where(x => x.Weapon.ToLower() == p.Name.Split('_')[0].ToLower()).ToList().FirstOrDefault();

                List<string> missingMonsters = new List<string>();
                foreach(string m in goodMonsters.PrimaryMonsters)
                {
                    if(! userMonsters.Contains(m))
                    {
                        missingMonsters.Add(m);
                    }
                }
                if(goodMonsters.SecondaryMonsters != null)
                {
                    foreach(string m in goodMonsters.SecondaryMonsters)
                    {
                        if (!userMonsters.Contains(m))
                        {
                            missingMonsters.Add(m);
                        }
                    }
                }

                for (int j = 0; j < missingMonsters.Count; j++)
                {
                    int i = p.Controls.OfType<Label>().ToList().Count;
                    p.Controls.Add((new Label()
                    {
                        Name = missingMonsters[j],
                        Location = new Point(0, i * 27),
                        Width = 100,
                        Height = 27,
                        Font = new Font("Impact", 12, FontStyle.Regular),
                        ForeColor = Color.Blue,
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = missingMonsters[j].ToUpper(),
                    }));
                }
            }
        }

        void ReadInWeaponMsgBoxes(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string text = reader.ReadToEnd();
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] splitLine = line.Split(':').ToArray();

                Tuple<string, string> newTuple = new Tuple<string, string>(splitLine[0], splitLine[1]);
                monsterKillingDescriptions.Add(newTuple);
            }
        }

        private void monsterLabels_DoubleClick(object sender, MouseEventArgs e)
        {
            if(roundOneOn == true)
            {
                return;
            }
            else
            {
                Label l = (Label)sender;
                foreach(Tuple<string,string> pair in monsterKillingDescriptions)
                {
                    if(l.Name.ToLower() == pair.Item1.ToLower())
                    {
                        MessageBox.Show(pair.Item2);
                    }
                }
            }
        }

        void ShowRoundOneResult()
        {
            Controls.Add(new Button()
            {
                Name = "PlayAgainRoundOne",
                Left = 7,
                Top = 15,
                Width = 200,
                Height = 40,
                Font = new Font("Chiller", 15, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "PLAY AGAIN ROUND 1",
            });
            button = (Button)Controls.Find("PlayAgainRoundOne", false)[0];
            button.Click += playAgainRoundOneDoneButton_Click;

            Controls.Add(new Label()
            {
                Name = "ResultLabel1",
                Left = 415,
                Top = 567,
                Width = 470,
                Height = 30,
                Font = new Font("Chiller", 20, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "YOUR RESULT IS : " + userPointsFirstRound.ToString() + " / 35 POINTS\n",
            });
            Controls.Add(new Label()
            {
                Name = "ResultLabel2",
                Left = 0,
                Top = 594,
                Width = 1285,
                Height = 60,
                Font = new Font("Chiller", 13, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "GREEN YOUR = GOOD MATCHES / RED = BAD MATCHES / BLUE = MONSTERS STILL POSSIBLE TO KILL BY THE WEAPON." +
                       "\nALSO BY DOUBLE-CLICKING ON THE MONSTER'S NAME, YOU CAN SEE THE EXACT WAY OF KILLING IT.",
            });
            Controls.Add(new Label()
            {
                Name = "ResultLabel3",
                Left = 0,
                Top = 650,
                Width = 1285,
                Height = 37,
                Font = new Font("Chiller", 17, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "STUDY CAREFUL THE BEASTS AND THE WEAPONS - IN NEXT ROUND, YOU HAVE TO FIGHT! IF YOU FEEL PREPARED, JUST PUSH THE BUTTON!",
            });

            foreach(Label l in this.Controls.OfType<Label>())
            {
                if(l.Name.Contains("Result"))
                {
                    roundOneResultLabels.Add(l);
                }
            }

            Controls.Add(new Button()
            {
                Name = "StartRoundTwo",
                Left = 555,
                Top = 686,
                Width = 180,
                Height = 38,
                Font = new Font("Chiller", 20, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "START ROUND 2",
            });
            startRoundTwoButton = (Button)Controls.Find("StartRoundTwo", false)[0];
            startRoundTwoButton.Click += startRoundTwoButton_Click;
        }

        void ClearFormRoundOneEnd()
        {
            foreach (Control c in this.Controls)
            {
                c.Dispose();
            }
            foreach (PictureBox p in weaponsPicBoxesList)
            {
                p.Dispose();
            }
            foreach (Panel p in monstersWeaponsPanelsToFill)
            {
                p.Dispose();

                foreach (Label l in p.Controls.OfType<Label>())
                {
                    l.Dispose();
                }
            }
            foreach (Label l in roundOneResultLabels)
            {
                l.Dispose();
            }
            foreach(Label l in weaponLabelsList)
            {
                l.Dispose();
            }
            startRoundTwoButton.Dispose();
            button.Dispose();
        }

        private void playAgainRoundOneDoneButton_Click(object sender, EventArgs e)
        {
            ClearFormRoundOneEnd();
            StartRoundOne();
        }

        private void startRoundTwoButton_Click(object sender, EventArgs e)
        {
            ClearRoundTwoResults();
            ClearFormRoundOneEnd();
            ClearForm();
            GetMonstersWeaponsRefs();
            monsterPictureSources = Directory.GetFiles("../../Pictures/MonstersRound2").ToList();

            RoundTwoInstructions();
        }

        void RoundTwoInstructions()
        {
            this.BackgroundImage = Image.FromFile("../../Pictures/secondroundinstructions.jpg");
            BackgroundImageLayout = ImageLayout.Stretch;

            Controls.Add(new Button()
            {
                Name = "RoundTwoButton",
                Left = 520,
                Top = 650,
                Width = 260,
                Height = 60,
                Font = new Font("Chiller", 28, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "START HUNTING!",
            });
            button = (Button)Controls.Find("RoundTwoButton", false)[0];
            button.Click += roundTwoButton_Click;
        }

        private void roundTwoButton_Click(object sender, EventArgs e)
        {
            button.Dispose();
            StartRoundTwo();
        }

        void StartRoundTwo()
        {
            this.BackgroundImage = Image.FromFile("../../Pictures/secondroundbackground.jpg");
            BackgroundImageLayout = ImageLayout.Stretch;
            
            ReadInWeaponMsgBoxes("monsterdescriptions.txt");

            countRoundTwo = 0;
            lifePointsRoundTwo = 6;
            userResultRoundTwo = 0;

            AddRoundTwoControls();

            randomMonstersRoundTwo = RandomizeList(monsterPictureSources);

            FillMonsterPicBoxWeaponLabels();

            soundPlayer = new System.Media.SoundPlayer();
            soundPlayer.SoundLocation = "creepybackground.wav";
            soundPlayer.Play();
        }

        void AddRoundTwoControls()
        {
            Controls.Add(new Label()
            {
                Name = "RoundsCountTitleLabel",
                Left = 10,
                Top = 10,
                Width = 150,
                Height = 80,
                Font = new Font("Chiller", 20, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "ROUND",
            });

            Controls.Add(new Label()
            {
                Name = "RoundsCountLabel",
                Left = 10,
                Top = 85,
                Width = 150,
                Height = 50,
                Font = new Font("Chiller", 26, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
            });
            countRoundsLabel = (Label)Controls.Find("RoundsCountLabel", false)[0];

            Controls.Add(new Label()
            {
                Name = "LifePointsTitleLabel",
                Left = 10,
                Top = 130,
                Width = 150,
                Height = 80,
                Font = new Font("Chiller", 20, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "LIFEPOINTS",
            });

            Controls.Add(new Label()
            {
                Name = "LifePointsLabel",
                Left = 10,
                Top = 205,
                Width = 150,
                Height = 50,
                Font = new Font("Chiller", 26, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
            });
            lifePointsLabel = (Label)Controls.Find("LifePointsLabel", false)[0];

            Controls.Add(new Label()
            {
                Name = "Round2TitleLabel",
                Left = 210,
                Top = 10,
                Width = 1010,
                Height = 50,
                Font = new Font("Chiller", 36, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "ROUND 2 : USE THE WEAPON AGAINST THE MONSTER!",
            });

            Controls.Add(new Label()
            {
                Name = "RandomMonsterLabel",
                Left = 350,
                Top = 70,
                Width = 600,
                Height = 90,
                Font = new Font("Chiller", 36, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.None,
            });
            label = (Label)Controls.Find("RandomMonsterLabel", false)[0];

            Controls.Add(new ProgressBar()
            {
                Name = "ProgressBarRoundTwo",
                Left = 350,
                Top = 170,
                Width = 600,
                Height = 10,
                ForeColor = Color.Red,
                BackColor = Color.Red,
            });
            roundTwoProgressBar = (ProgressBar)Controls.Find("ProgressBarRoundTwo", false)[0];

            Controls.Add(new PictureBox()
            {
                Name = "RandomMonster",
                Left = 350,
                Top = 185,
                Width = 600,
                Height = 350,
                BackColor = Color.Transparent,
            });
            RoundTwoPicBox = (PictureBox)Controls.Find("RandomMonster", false)[0];

            for (int i = 0; i < 3; i++)
            {
                Controls.Add(new Label()
                {
                    Left = 300 + i * 250,
                    Top = 575,
                    Width = 200,
                    Height = 50,
                    Font = new Font("Chiller", 16, FontStyle.Bold),
                    ForeColor = Color.Red,
                    BackColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.Fixed3D,
                });
            }

            Controls.Add(new Label()
            {
                Name = "MonsterKillingDescription",
                Left = 100,
                Top = 645,
                Width = 1100,
                Height = 90,
                Font = new Font("Chiller", 24, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.None,
            });
            killingDescriptionLabel = (Label)Controls.Find("MonsterKillingDescription", false)[0];

            foreach (Control c in this.Controls)
            {
                roundTwoControls.Add(c);

                if (c.GetType() == typeof(Label))
                {
                    if (c.Top == 575 && c.Width == 200)
                    {
                        weaponLabelsList.Add((Label)c);
                    }
                }
            }
        }
        
        void FillMonsterPicBoxWeaponLabels()
        {
            foreach (Label l in weaponLabelsList)
            {
                l.BackColor = Color.Black;
                l.ForeColor = Color.Red;
                l.Text = "";
                l.Click += weaponLabelChoice_Click;
            }

            countRoundTwo++;
            RoundTwoPicBox.Image = Image.FromFile(randomMonstersRoundTwo[countRoundTwo]);
            RoundTwoPicBox.ImageLocation = randomMonstersRoundTwo[countRoundTwo];
            Size imageSize = RoundTwoPicBox.Image.Size;
            Size fitSize = RoundTwoPicBox.ClientSize;
            RoundTwoPicBox.SizeMode = imageSize.Width > fitSize.Width || imageSize.Height > fitSize.Height ?
                                      PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;

            int indexLabel = generator.Next(0, weaponLabelsList.Count);
            string weaponToExclude = "";

            foreach(WeaponMonsters wm in weaponsMonsters)
            {
                if(wm.PrimaryMonsters.Any(x=> RoundTwoPicBox.ImageLocation.ToString().ToLower().Contains(x.ToLower())))
                {
                    foreach(string m in wm.PrimaryMonsters)
                    {
                        if(RoundTwoPicBox.ImageLocation.ToString().ToLower().Contains(m.ToLower()))
                        {
                            label.Text = m.ToUpper();
                        }
                    }

                    weaponLabelsList[indexLabel].Text = wm.Weapon.ToUpper();
                    goodWeapon = weaponLabelsList[indexLabel].Text.ToLower();
                }

                if (wm.SecondaryMonsters != null)
                {
                    if (wm.SecondaryMonsters.Any(x => RoundTwoPicBox.ImageLocation.ToString().ToLower().Contains(x.ToLower())))
                    {
                        weaponToExclude = wm.Weapon.ToLower();
                    }
                    else
                    {
                        weaponToExclude = "";
                    }
                }
            }

            List<string> weaponsForRandom = new List<string>();

            if(weaponToExclude != "")
            {
                weaponsForRandom = weapons.Where(x => x.ToLower() != weaponToExclude
                                   && x.ToLower() != weaponLabelsList[indexLabel].Text.ToLower()).ToList();
            }
            else
            {
                weaponsForRandom = weapons.Where(x => x.ToLower() != weaponLabelsList[indexLabel].Text.ToLower()).ToList();
            }

            List<int> indexes = new List<int>();
            foreach(Label l in weaponLabelsList)
            {
                if(l.Text == "")
                {
                    int index = generator.Next(0, weaponsForRandom.Count);
                    if(! indexes.Contains(index))
                    {
                        indexes.Add(index);
                        l.Text = weaponsForRandom[index].ToUpper();
                    }
                    else
                    {
                        index = generator.Next(0, weaponsForRandom.Count);
                        l.Text = weaponsForRandom[index].ToUpper();
                    }
                }
            }

            lifePointsLabel.Text = lifePointsRoundTwo.ToString() + " / 6";
            countRoundsLabel.Text = countRoundTwo.ToString() + " / 12";

            timerRoundTwo = new Timer();
            timerRoundTwo.Start();
            timerRoundTwo.Enabled = true;

            if (countRoundTwo < totalRoundsOfRoundTwo / 3)
            {
                timerRoundTwo.Interval = 3500;
            }
            else if (countRoundTwo < ((totalRoundsOfRoundTwo / 3) * 2) && countRoundTwo >= totalRoundsOfRoundTwo / 3)
            {
                timerRoundTwo.Interval = 3000;
            }
            else
            {
                timerRoundTwo.Interval = 2500;
            }
            timerRoundTwo.Tick += timerRoundTwo_Tick;

            ProgressBar();
        }

        void ProgressBar()
        {
            roundTwoProgressBar.Value = 0;
            progressBarTimer = new Timer();
            progressBarTimer.Enabled = true;
            progressBarTimer.Start();
            progressBarTimer.Interval = 1;
            roundTwoProgressBar.Maximum = timerRoundTwo.Interval/20;
            progressBarTimer.Tick += new EventHandler(progressBarTimer_Tick);
        }

        void progressBarTimer_Tick(object sender, EventArgs e)
        {
            if (roundTwoProgressBar.Value < roundTwoProgressBar.Maximum)
            {
                roundTwoProgressBar.Value++;
            }
            else
            {
                progressBarTimer.Stop();
                progressBarTimer.Enabled = false;
                progressBarTimer.Dispose();
            }
        }

        void ColorWeaponLabels(string w)
        {
            foreach(Label l in weaponLabelsList)
            {
                if(l.Text.ToLower() == w)
                {
                    l.BackColor = Color.Green;
                    l.ForeColor = Color.Black;
                }
                else
                {
                    l.BackColor = Color.Red;
                    l.ForeColor = Color.Black;
                }
            }
        }
        
        void ShowMonsterKillingDescription(string monsterimagelocation)
        {
            foreach(Tuple<string, string> mw in monsterKillingDescriptions)
            {
                if(monsterimagelocation.ToLower().Contains(mw.Item1.ToLower()))
                {
                    killingDescriptionLabel.Text = mw.Item2;
                }
            }
        }

        private void weaponLabelChoice_Click(object sender, EventArgs e)
        {
            foreach (Label label in weaponLabelsList)
            {
                label.Click -= weaponLabelChoice_Click;
            }
            
            Label l = (Label)sender;

            progressBarTimer.Stop();
            progressBarTimer.Enabled = false;
            progressBarTimer.Dispose();

            timerRoundTwo.Stop();
            timerRoundTwo.Enabled = false;
            timerRoundTwo.Dispose();
            
            if(countRoundTwo <= totalRoundsOfRoundTwo && lifePointsRoundTwo >= 0)
            {
                ColorWeaponLabels(goodWeapon);
                ShowMonsterKillingDescription(RoundTwoPicBox.ImageLocation.ToString());

                string[] solutions = new string[3];
                solutions[0] = l.Text;
                solutions[1] = goodWeapon;
                solutions[2] = killingDescriptionLabel.Text;
                Tuple<string, string[]> newSolution = new Tuple<string, string[]>(label.Text, solutions);
                roundTwoSolutions.Add(newSolution);

                if (l.Text.ToLower() != goodWeapon)
                {
                    lifePointsRoundTwo--;
                }

                timerShowSolutionRoundTwo = new Timer();
                timerShowSolutionRoundTwo.Start();
                timerShowSolutionRoundTwo.Enabled = true;
                timerShowSolutionRoundTwo.Interval = 2500;
                timerShowSolutionRoundTwo.Tick += timerShowSolutionRoundTwo_Tick;
            }
        }

        void timerRoundTwo_Tick(object sender, EventArgs e)
        {
            foreach (Label label in weaponLabelsList)
            {
                label.Click -= weaponLabelChoice_Click;
            }

            progressBarTimer.Stop();
            progressBarTimer.Enabled = false;
            progressBarTimer.Dispose();
            
            timerRoundTwo.Stop();
            timerRoundTwo.Enabled = false;
            timerRoundTwo.Dispose();
            
            if (countRoundTwo <= totalRoundsOfRoundTwo && lifePointsRoundTwo >= 0)
            {
                ColorWeaponLabels(goodWeapon);
                ShowMonsterKillingDescription(RoundTwoPicBox.ImageLocation.ToString());
                lifePointsRoundTwo--;

                string[] solutions = new string[3];
                solutions[0] = "N/A";
                solutions[1] = goodWeapon;
                solutions[2] = killingDescriptionLabel.Text;
                Tuple<string, string[]> newSolution = new Tuple<string, string[]>(label.Text, solutions);
                roundTwoSolutions.Add(newSolution);

                timerShowSolutionRoundTwo = new Timer();
                timerShowSolutionRoundTwo.Start();
                timerShowSolutionRoundTwo.Enabled = true;
                timerShowSolutionRoundTwo.Interval = 2500;
                timerShowSolutionRoundTwo.Tick += timerShowSolutionRoundTwo_Tick;
            }
        }

        void timerShowSolutionRoundTwo_Tick(object sender, EventArgs e)
        {
            timerShowSolutionRoundTwo.Stop();
            timerShowSolutionRoundTwo.Enabled = false;
            timerShowSolutionRoundTwo.Dispose();

            killingDescriptionLabel.Text = "";
            
            if (countRoundTwo < totalRoundsOfRoundTwo && lifePointsRoundTwo >= 0)
            {
                FillMonsterPicBoxWeaponLabels();
            }
            else if(countRoundTwo == totalRoundsOfRoundTwo || lifePointsRoundTwo < 0)
            {
                RoundTwoEnd();
            }
        }

        void RoundTwoEnd()
        {
            MessageBox.Show("HUNTING OVER");
            
            ShowRoundTwoResult();
        }

        void ShowRoundTwoResult()
        {
            foreach (Control c in roundTwoControls)
            {
                if (c.Name != "Round2TitleLabel")
                {
                    c.Dispose();
                }
            }

            AddRoundTwoResultControls();
            soundPlayer.Stop();
        }

        void AddRoundTwoResultControls()
        {
            Controls.Add(new Button()
            {
                Name = "PlayAgainRoundTwo",
                Left = 10,
                Top = 15,
                Width = 200,
                Height = 40,
                Font = new Font("Chiller", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "PLAY AGAIN ROUND 2",
            });
            startRoundTwoButton = (Button)Controls.Find("PlayAgainRoundTwo", false)[0];
            startRoundTwoButton.Click += startRoundTwoButton_Click;

            Controls.Add(new Button()
            {
                Name = "FinalResults",
                Left = 955,
                Top = 500,
                Width = 200,
                Height = 40,
                Font = new Font("Chiller", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "FINAL RESULT",
            });
            button = (Button)Controls.Find("FinalResults", false)[0];
            button.Click += finalResultButton_Click;

            Controls.Add(new Label()
            {
                Text = "DOUBLE-CLICK ON THE MONSTER'S NAME TO SEE THE EXACT WAY OF KILLING IT!"
                       + "\n\nIF YOU ARE NOT SATISFIED WITH YOUR RESULT, YOU CAN PLAY AGAIN ROUND 2."
                       + "\n\nIF YOU FEEL FINISHED, CLICK ON THE 'FINAL RESULT' BUTTON TO SEE YOUR TOTAL RESULTS OF ROUND 1 AND 2.",
                Left = 850,
                Top = 180,
                Width = 400,
                Height = 300,
                Font = new Font("Chiller", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.None,
            });

            Controls.Add(new Label()
            {
                Text = "MONSTER                   GOOD SOLUTION            YOUR CHOICE",
                Left = 250,
                Top = 50,
                Width = 1200,
                Height = 50,
                Font = new Font("Chiller", 15, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                BorderStyle = BorderStyle.None,
            });

            for (int i = 0; i < roundTwoSolutions.Count; i++)
            {
                Controls.Add(new Label()
                {
                    Name = roundTwoSolutions[i].Item1,
                    Text = roundTwoSolutions[i].Item1,
                    Left = 250,
                    Top = 80 + i * 53,
                    Width = 200,
                    Height = 70,
                    Font = new Font("Impact", 14, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BorderStyle = BorderStyle.None,
                });
                Controls.Add(new Label()
                {
                    Name = roundTwoSolutions[i].Item2[1].ToUpper() + "_" + i.ToString(),
                    Text = roundTwoSolutions[i].Item2[1].ToUpper(),
                    Left = 460,
                    Top = 80 + i * 53,
                    Width = 200,
                    Height = 70,
                    Font = new Font("Impact", 14, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BorderStyle = BorderStyle.None,
                });
                Controls.Add(new Label()
                {
                    Name = roundTwoSolutions[i].Item2[0] + "_" + i.ToString(),
                    Text = roundTwoSolutions[i].Item2[0],
                    Left = 659,
                    Top = 80 + i * 53,
                    Width = 200,
                    Height = 70,
                    Font = new Font("Impact", 14, FontStyle.Regular),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BorderStyle = BorderStyle.None,
                });
            }

            List<Label> userSolutions = new List<Label>();
            foreach(Control c in this.Controls)
            {
                if(c.Left == 659)
                {
                    userSolutions.Add((Label)c);
                }
                else
                {
                    roundTwoControls.Add(c);
                    if (c.Left == 250 && c.ForeColor == Color.White)
                    {
                        c.MouseDoubleClick += new MouseEventHandler(monsterLabels_DoubleClick);
                    }
                }
            }
            List<string> controlnames = roundTwoControls.Select(x => x.Name).ToList();
            foreach(Label l in userSolutions)
            {
                if(controlnames.Contains(l.Name))
                {
                    l.ForeColor = Color.Green;
                }
                roundTwoControls.Add(l);
            }

            userResultRoundTwo = totalRoundsOfRoundTwo - (6 - lifePointsRoundTwo) - (totalRoundsOfRoundTwo - userSolutions.Count);
            Controls.Add(new Label()
            {
                Name = "RoundTwoResult",
                Text = "YOUR RESULT : " + userResultRoundTwo.ToString() + " / 12",
                Left = 850,
                Top = 100,
                Width = 400,
                Height = 90,
                Font = new Font("Chiller", 28, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.None,
            });
            label = (Label)Controls.Find("RoundTwoResult", false)[0];
        }

        void ClearRoundTwoResults()
        {
            foreach (Control c in roundTwoControls)
            {
                c.Dispose();

            }
            foreach(Label l in roundTwoControls.OfType<Label>())
            {
                l.Dispose();
            }
            foreach (Control l in this.Controls)
            {
                l.Dispose();
            }
            foreach(Label l in this.Controls.OfType<Label>())
            {
                l.Dispose();
            }
        }
        
        private void finalResultButton_Click(object sender, EventArgs e)
        {
            ClearRoundTwoResults();

            roundOneFinalResult = ((double)userPointsFirstRound / (double)35) * 100;
            roundTwoFinalResult = ((double)userResultRoundTwo / (double)12) * 100;
            finalResult = (roundOneFinalResult + roundTwoFinalResult) / 2;

            if(finalResult >= 80)
            {
                this.BackgroundImage = Image.FromFile("../../Pictures/resultbackground2.png");
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                this.BackgroundImage = Image.FromFile("../../Pictures/resultbackground.png");
                BackgroundImageLayout = ImageLayout.Stretch;
            }

            AddFinalResultControls();
        }

        void AddFinalResultControls()
        {
            Controls.Add(new Button()
            {
                Name = "ExitButton",
                Left = 1120,
                Top = 15,
                Width = 150,
                Height = 40,
                Font = new Font("Chiller", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "EXIT HUNT",
            });
            Button exitbutton = (Button)Controls.Find("ExitButton", false)[0];
            exitbutton.Click += exitButton_Click;

            Controls.Add(new Button()
            {
                Name = "StartAgainGame",
                Left = 10,
                Top = 15,
                Width = 150,
                Height = 40,
                Font = new Font("Chiller", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "HUNT AGAIN!",
            });
            button = (Button)Controls.Find("StartAgainGame", false)[0];
            button.Click += playAgainButton_Click;

            Controls.Add(new Label()
            {
                Name = "FinalResultTitleLabel",
                Left = 145,
                Top = 10,
                Width = 1010,
                Height = 50,
                Font = new Font("Chiller", 36, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "FINAL RESULTS",
            });

            Controls.Add(new Label()
            {
                Name = "FinalResults",
                Left = 410,
                Top = 190,
                Width = 700,
                Height = 100,
                Font = new Font("Chiller", 36, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "ROUND 1 : " + userPointsFirstRound.ToString() + " / TOTAL 35 = " + String.Format("{0:0.##}", roundOneFinalResult) + " %"
                       + "\nROUND 2 : " + userResultRoundTwo.ToString() + " / TOTAL 12 = " + String.Format("{0:0.##}", roundTwoFinalResult) + " %",
            });
            Controls.Add(new Label()
            {
                Name = "FinalResult",
                Left = 410,
                Top = 295,
                Width = 700,
                Height = 50,
                Font = new Font("Chiller", 36, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "FINAL RESULT = " + String.Format("{0:0.##}", finalResult) + " %",
            });
            Controls.Add(new Label()
            {
                Name = "ResultComment",
                Left = 10,
                Top = 390,
                Width = 1200,
                Height = 100,
                Font = new Font("Chiller", 24, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
            });
            resultComment = (Label)Controls.Find("ResultComment", false)[0];

            CommentToResult();

            Controls.Add(new Label()
            {
                Name = "AdditionalComment",
                Left = 0,
                Top = 510,
                Width = 1220,
                Height = 50,
                Font = new Font("Chiller", 22, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Right-click on the monster labels to check them once again, start a new round or exit and come back later!",
            });

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Controls.Add(new Label()
                    {
                        Left = 205 + i * 120,
                        Top = 565 + j * 32,
                        Width = 100,
                        Height = 25,
                        Font = new Font("Impact", 10, FontStyle.Regular),
                        ForeColor = Color.Red,
                        BackColor = Color.Black,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BorderStyle = BorderStyle.Fixed3D,
                    });
                }
            }

            List<Label> finalLabels = new List<Label>();
            foreach (Control c in this.Controls)
            {
                roundTwoControls.Add(c);
                if(c.Height == 25 && c.Width == 100)
                {
                    finalLabels.Add((Label)c);
                }
            }
            for(int i = 0; i < monsters.Count; i++)
            {
                finalLabels[i].Text = monsters[i].ToUpper();
                finalLabels[i].Name = monsters[i].ToLower();
            }
            foreach(Label l in finalLabels)
            {
                l.MouseClick += monsterLabels_RightClick;
            }
        }

        void CommentToResult()
        {
            if (finalResult <= 20)
            {
                resultComment.Text =
                "HUNTER IN TRAINING - YOU STARTED THE JOURNEY!\nYou have a lot to learn until you can face a monster, so keep on learning, you will get better!";
            }
            else if (finalResult > 20 && finalResult <= 40)
            {
                resultComment.Text =
                "HUNTER IN TRAINING - NOT A BEGINNER ALREADY!\nYou started mastering hunting, keep on - but now if a monster comes around, rather run so you can fight later!";
            }
            else if (finalResult > 40 && finalResult <= 60)
            {
                resultComment.Text =
                "HUNTER IN TRAINING - NOT BAD AT ALL!\nBeing on halfway, still take extra care with the supernatural - running is no shame when it comes to survival!";
            }
            else if (finalResult > 60 && finalResult <= 80)
            {
                resultComment.Text =
                "HUNTER IN TRANING - YOU ARE SO CLOSE!\nStill have some tricks to learn, but you indeed have the chance to survive when crossing a monster.";
            }
            else if (finalResult > 80 && finalResult < 100)
            {
                resultComment.Text =
                "CONGRATULATIONS, YOU ARE A HUNTER!\nWe all make mistakes, but you definitely don't loose your head when it comes to kicking the ass of some ugly beasts!";
            }
            else if (finalResult == 100)
            {
                resultComment.Text =
                "WOW, LOOK AT YOU - YOU ARE A SUPER-HUNTER!\nYou compare to the Winchesters - you are the one the monsters are afraid of in the dark and check under the bed!";
            }
        }

        private void playAgainButton_Click(object sender, EventArgs e)
        {
            foreach(Control c in roundTwoControls)
            {
                c.Dispose();
            }

            StartGame();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
