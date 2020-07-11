using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using cwklib2020;

namespace Engine_Gen_Alg_Test
{


    public partial class Form1 : Form
    {
        private static Random rnd = new Random();
        private int Pop_Size;
        public List<Individual> Current_Pop = new List<Individual>();
        public List<Individual> New_Pop = new List<Individual>();
        List<TextBox> Levels;
        List<CheckBox> Devices;
        public int Gen_Num { get; private set; }
        public Individual Best_Run { get; private set; }
        public int Mutation_Rate;
        public float Crossover_Rate;
        public int Tournament_Pressure;
        public int Cross_site;
        public int Elitism;  //Percentage of unchanged in new generation 
       
        private BackgroundWorker New_Gen = new BackgroundWorker();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.backgroundWorker1.WorkerSupportsCancellation = true;

            this.backgroundWorker1.WorkerReportsProgress = true;


        }
        private void Pop_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }


        }

        private void pop_btn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Pop_tbx.Text)) { MessageBox.Show("Enter Population size"); }
            if (String.IsNullOrEmpty(M_tbx.Text)) { MessageBox.Show("Enter Mutation Rate"); }
            if (String.IsNullOrEmpty(C_tbx.Text)) { MessageBox.Show("Enter CrossOver Rate"); }
            if (String.IsNullOrEmpty(T_tbx.Text)) { MessageBox.Show("Enter Tournament size"); }
            if (String.IsNullOrEmpty(Cs_tbx.Text) && !Cs_cbx.Checked) { MessageBox.Show("Choose Crossove Site"); }
            if (String.IsNullOrEmpty(Mg_tbx.Text) || String.IsNullOrEmpty(Cv_tbx.Text) ) { MessageBox.Show("Enter Terminating Criteria"); }
            
            else
            {
                pop_btn.Enabled = false;
                //reset main Parameters for Algorithm 
                Pop_Size = Convert.ToInt32(Pop_tbx.Text);
                Current_Pop = Population.Init_Pop_Of_size(Pop_Size).ToList();
                Gen_Num = 0;                
                this.backgroundWorker1.RunWorkerAsync();
               


            }

        }

       
        

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            int Flag=0; int temp=0;
            for (int i = 0; i <= Convert.ToInt32(Mg_tbx.Text); i++)
            {
                //Sort and find best fitness
                Current_Pop.Sort(new IndividualComparer()); Best_Run = Current_Pop[0];
                temp = Best_Run.Fitness;

                New_Generation();
                // sort again and find best fitness 
                Current_Pop.Sort(new IndividualComparer());
                //Check for Convergence
                if (temp==Current_Pop[0].Fitness) 
                {
                    Flag++;
                    // if Reached Convergence, Stop Algorithm
                    if (Flag == Convert.ToInt32(Cv_tbx.Text)) 
                    { 
                       MessageBox.Show("Algorithm Appears to have Reached Convergence");
                        backgroundWorker1.CancelAsync();
                    }
                
                }
                else if(temp != Current_Pop[0].Fitness) { Flag = 0; }

                // Sleep for 10ms to 
                System.Threading.Thread.Sleep(10);
                // Report the progress now
                this.backgroundWorker1.ReportProgress(i);
                

                // Cancel process if it was flagged to be stopped.
                if (this.backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
            this.textBox1.Text = Gen_Num.ToString();
            this.textBox2.Text = Best_Run.Fitness.ToString();
            Update_Config(Best_Run);

        }

       

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pop_btn.Enabled = true;
            if (e.Error != null)
            {
                // An error occurred
                MessageBox.Show("Error!");
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Algorithm Stopped");



            }
            else
            {
                // The process finished

                MessageBox.Show("Algorithm Has Finished!");
                
                

            }
        }

        private void Stop_btn_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();



        }

        private void M_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }
        }

        private void C_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }
        }

        private void T_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }
        }

        private void Cs_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }
        }

        private void Mg_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }
        }
        public void New_Generation()
        {
            Pop_Size = Convert.ToInt32(Pop_tbx.Text);
            Mutation_Rate = Convert.ToInt32(M_tbx.Text);
            Crossover_Rate = Convert.ToInt32(C_tbx.Text);
            Tournament_Pressure = Convert.ToInt32(T_tbx.Text);
            Elitism = Convert.ToInt32((1 - (Crossover_Rate / 100)) * Pop_Size);
            if (Cs_cbx.Checked) { Cross_site = rnd.Next(0, 15); }//Chooses a random Crossover site            
            else { Cross_site = Convert.ToInt32(Cs_tbx.Text); }


            for (int i = 0; i < Pop_Size; i++)
            {
                if (i < Elitism)
                {
                    // Current_Pop.Sort(new IndividualComparer());
                    New_Pop.Add(Current_Pop[i]);
                }
                if (i >= Elitism && i < Pop_Size)
                {
                    Tuple<Individual, Individual> Parents = Population.Tournament_Selection(Current_Pop, Tournament_Pressure);
                    Tuple<Individual, Individual> Children = Population.Crossover_and_Mutate(Parents.Item1, Parents.Item2, Cross_site, Mutation_Rate);
                    //To ensure no Duplicates Check Existence of Children in the New Generation and if it exists,                   //Generate New Child until it doesnt
                    while (New_Pop.Contains(Children.Item1) || New_Pop.Contains(Children.Item2))
                    { Children = Population.Crossover_and_Mutate(Parents.Item1, Parents.Item2, Cross_site, Mutation_Rate); }
                    if (!New_Pop.Contains(Children.Item1)) { New_Pop.Add(Children.Item1); }
                    if (!New_Pop.Contains(Children.Item2)) { New_Pop.Add(Children.Item2); }


                }
            }


            Current_Pop = New_Pop;
            Gen_Num++;





        }
        private void Update_Config(Individual Best)
        {

            Levels = new List<TextBox> { L0_tbx, L1_tbx, L2_tbx, L3_tbx, L4_tbx, L5_tbx, L6_tbx, L7_tbx, L8_tbx };
            Devices = new List<CheckBox> { D0_cbx, D1_cbx, D2_cbx, D3_cbx, D4_cbx };

            //Update Texbbox according to index and Coressponding Levels
            foreach (var Lvl in Levels.Select((x, i) => new { Value = x, Index = i }))
            {

                for (int i = 0; i < 5; i++)
                {
                    if (Best.Byte_Sequence[Lvl.Index][0] == 1)
                    { Levels[Lvl.Index].Text = "Very low"; }
                    else if (Best.Byte_Sequence[Lvl.Index][1] == 1)
                    { Levels[Lvl.Index].Text = "low"; }
                    else if (Best.Byte_Sequence[Lvl.Index][2] == 1)
                    { Levels[Lvl.Index].Text = "Medium"; }
                    else if (Best.Byte_Sequence[Lvl.Index][3] == 1)
                    { Levels[Lvl.Index].Text = "High"; }
                    else if (Best.Byte_Sequence[Lvl.Index][4] == 1)
                    { Levels[Lvl.Index].Text = "Very High"; }

                }
            }

            //Update Texbbox according to index and Coressponding Levels
            foreach (var Dev in Devices.Select((x, i) => new { Value = x, Index = i }))
            {
                int temp = Dev.Index + 9; // First Device index will be Elemement 9 in Byte Sequence
                if (Best.Byte_Sequence[temp][0] == 1) { Devices[Dev.Index].Checked = true; }

            }

            // Update Timing Level Texbox
            if (Best.Byte_Sequence[14][0] == 1) { T0_tbx.Text = "Low"; }
            if (Best.Byte_Sequence[14][1] == 1) { T0_tbx.Text = "Medium"; }
            if (Best.Byte_Sequence[14][2] == 1) { T0_tbx.Text = "High"; }

        }

        private void Cv_tbx_KeyPress(object sender, KeyPressEventArgs e)
        {

            //This ensures that only positive Numbers are allowed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;

            }
        }

        private void Cs_cbx_CheckedChanged(object sender, EventArgs e)
        {
            //Disable texbox when Random selected 
            if (Cs_cbx.Checked)
            { Cs_tbx.Enabled = false; }
        }
    }


    public partial class Individual
    {
        public string String_Sequence { get; set; }
        public byte[][] Byte_Sequence { get; set; }
        public int Fitness { get; set; }
    }
   
   internal partial class IndividualComparer : IComparer<Individual>
    {
        // To sort individuals by fitest first 
        public int Compare(Individual x, Individual y)
        {

            int result = y.Fitness.CompareTo(x.Fitness); // decending order 

            return result;
        }
    }

}

