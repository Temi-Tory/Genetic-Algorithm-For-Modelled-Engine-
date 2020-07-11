using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cwklib2020;



namespace Engine_Gen_Alg_Test
{
    public static class Population
    {

        private static Random rnd = new Random();


        //To generate new individual Run sequence
        internal static Individual Random_Sequence()
        {
            FitFunc newtest = new FitFunc();
            Individual I = new Individual();
            string Indiv_s;
            byte[][] Indiv_b = new byte[15][];

            for (int i = 0; i < 9; i++)// elements 0-8  reperesent parameter levels
            {
                bool[] L = new bool[5];
                int lvl = rnd.Next(0, 5); // generate random parameter level 
                L[lvl] = true; // set level to next a parameter and convert to byte array
                byte[] L_b = Array.ConvertAll(L, b => b ? (byte)1 : (byte)0);
                Indiv_b[i] = L_b;
            }
            for (int i = 9; i < 14; i++) // elements 9-13 reperent device presence
            {
                int D_presence = rnd.Next(0, 2); // Randomly decide if device during run    
                bool[] D = new bool[1];
                if (D_presence == 1) { D[0] = true; }
                else { D[0] = false; }
                byte[] D_b = Array.ConvertAll(D, b => b ? (byte)1 : (byte)0);
                Indiv_b[(i)] = D_b;
            }

            bool[] T = new bool[3];
            int T_Level = rnd.Next(0, 3);       //Randomly choose a Timer level 
            T[T_Level] = true;
            byte[] T_b = Array.ConvertAll(T, b => b ? (byte)1 : (byte)0);
            Indiv_b[14] = T_b;  //  Element 14 Represents Timer Lels

            Indiv_s = ByteSeq_To_String(Indiv_b);

            //Set Properties Of new Individual
            I.Byte_Sequence = Indiv_b;
            I.String_Sequence = Indiv_s;
            I.Fitness = newtest.evalFunc(Indiv_s);
            
            return I;
        }

        // Concantates the elements in the byte[][] array to a single string
        internal static string ByteSeq_To_String (byte[][] B)
        {
            
            byte[] tempc = B.SelectMany(a => a).ToArray();
            string[] arr = ((IEnumerable)tempc).Cast<byte>()
                                  .Select(x => x.ToString())
                                  .ToArray();
            string Result = string.Join("", arr);
            return Result;
        }

        public static HashSet<Individual> Init_Pop_Of_size(int s) //where s will be the Population 
        {
            //Using a hash set  to ensure No duplicate Runs as well as A sorted list based on Fitnes
            HashSet<Individual> Population = new HashSet<Individual>();
            for (int i = 0; i < s; i++)
            {
                Individual Indiv = Random_Sequence();
                //This while loop specifies what to do when there is a duplicate entry 
                //ie If Value already exists in Population list, keep  generating new random indiviudal until it doesnt
                while (Population.Contains(Indiv)) { Indiv = Random_Sequence(); }
                Population.Add(Indiv);

            }

            return Population;

        }

        // Tuple Return Value to select two parents after tournament 
        public static Tuple<Individual, Individual> Tournament_Selection(List<Individual> Population, int k) //where s will be the Population 
        {
            List<Individual> Pop = new List<Individual>();

            int N = Population.Count();
            for (int i = 0; i < k; i++)
            {
                //Iterate through Tournament size to Fill Tournament List 
                Pop.Add(Population[rnd.Next(1, N)]);
            }

            Pop.Sort(new IndividualComparer());
            Individual best = Pop[0];
            Individual second = Pop[1];

            return new Tuple<Individual, Individual>(best, second);
        }

        public static Tuple<Individual, Individual> Crossover_and_Mutate(Individual Parent1, Individual Parent2, int Site, int mutationrate)
        {
            Individual child1 = new Individual();
            Individual child2 = new Individual();
            FitFunc newtest = new FitFunc();

            // Initialise new Jagged Array for Children 
            child1.Byte_Sequence = new byte[15][];
            child2.Byte_Sequence = new byte[15][];
            //First Crossover 
            for (int i = 0; i < Parent1.Byte_Sequence.Length; i++)
            {
                if (i < Site)
                {
                    child1.Byte_Sequence[i] = Parent1.Byte_Sequence[i];
                    child2.Byte_Sequence[i] = Parent2.Byte_Sequence[i];
                }
                else
                {
                    child1.Byte_Sequence[i] = Parent2.Byte_Sequence[i];
                    child2.Byte_Sequence[i] = Parent1.Byte_Sequence[i];
                }
            }

            //Then Perform muation function (mutation will happen depending on mutation rate)
            Mutate(child1, mutationrate);
            Mutate(child2, mutationrate);

            //Updates Child's String_Sequence and fitness property
            child1.String_Sequence = ByteSeq_To_String(child1.Byte_Sequence);
            child1.Fitness = newtest.evalFunc(child1.String_Sequence);
            child2.String_Sequence = ByteSeq_To_String(child2.Byte_Sequence);
            child2.Fitness= newtest.evalFunc(child2.String_Sequence);

            //Returns two new children
            return new Tuple<Individual, Individual>(child1, child2);
        }

        internal static Individual Mutate(Individual child, int rate)
        {           
            
            for (int i = 0; i < child.Byte_Sequence.Length; i++)
            {
                if (rnd.Next(0, 100) < rate)
                {
                    if (i < 9 || i == 14)
                    { //Mutate by scrambling order to change levels(or timing)
                        byte[] temp = child.Byte_Sequence[i].OrderBy(b => rnd.Next()).ToArray();
                        child.Byte_Sequence[i] = temp;
                    }

                    if (i > 8 && i < 14)
                    {//Mutate by fliiping state fo the presence of the device.                        
                        if (child.Byte_Sequence[i][0] == 1) 
                        { 
                            child.Byte_Sequence[i][0] = 0; 
                        }
                        else { 
                            child.Byte_Sequence[i][0] = 1; 
                        }

                    }
                }
            }
            return child;
        }


    }
}

