using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tema1
{
    class Program
    {
        // Definirea listelor si variabilelor necesare
        static List<string> alfabetul_limbajului = new List<string>();
        static List<string> multimea_starilor = new List<string>();
        static List<string> multimea_starilor_finale = new List<string>();
        
        static string starea_initila = "", linie;
        static string[] words;
        static char delimiterChar = ' ';
        static int row = -1;

        static bool automat_nedeterminist = false;
        static string[,] tranzitie;
        
        static int coloana_epsilon = -1;
        static bool tranzitie_epsilon = false;

        static string[,] echivalent;
        static List<string> stari_in_care_se_consuma_elementul = new List<string>();
        static int index_of_epsilon = 0;
        static List<string> multimea_starilor_finale_ale_automatului_echivalent = new List<string>();
        static bool este_deja_in_lista = false;


        // Definirea fisierului de intrare
        static StreamReader file = new StreamReader(@"C:\Users\Denis\source\repos\Tema1\Tema1\1_in.txt");

        // Metoda de parasare a unui string
        static private List<string> parsare_string(string multime_de_stari)
        {
            List<string> sir_parsat = new List<string>();
            char delimiterChar = '_';
            string[] stari;

            stari = multime_de_stari.Split(delimiterChar);
            foreach (string s in stari) sir_parsat.Add(s);

            return sir_parsat;
        }

        //Metoda copiere rand din matricea tranzitiei automatului intial in rand din matricea tranzitiei automatului echivalent
        static private void copy(int rand_fct_tranzitie,int rand_fct_tranzitie_a_echivalent, int coloana)
        {
            List<string> stari_in_care_ajungi = parsare_string(tranzitie[rand_fct_tranzitie, coloana]);
            foreach (string s in stari_in_care_ajungi)
                if (echivalent[rand_fct_tranzitie_a_echivalent, coloana] == "-")
                    if (s != "-")
                        echivalent[rand_fct_tranzitie_a_echivalent, coloana] = s;
                    else break;
                else if (s != "-") echivalent[rand_fct_tranzitie_a_echivalent, coloana] += "_" + s;
        }

        // Metoda de completare a noilor stari in care ajunge automatul dupa eliminarea tranzitiilor epsilon
        static private bool completare_stari(int rand_tranzitie,int rand_tranzitie_A_echivalent,string stare)
        {
            List<string> stari_in_care_se_consuma_epsilon = new List<string>();
            stari_in_care_se_consuma_epsilon=parsare_string(tranzitie[rand_tranzitie, index_of_epsilon]);
            
            if(stari_in_care_se_consuma_epsilon.Count() == 1)
            {
                if (stari_in_care_se_consuma_epsilon[0] == "-")
                {
                    for (int i = 0; i < alfabetul_limbajului.Count(); i++)
                        if (i != index_of_epsilon) copy(rand_tranzitie, rand_tranzitie_A_echivalent, i);
                }
                else {
                    for (int i = 0; i < alfabetul_limbajului.Count(); i++)
                        if (i != index_of_epsilon) copy(rand_tranzitie, rand_tranzitie_A_echivalent, i);
                    completare_stari(vrea_index_pentru(stari_in_care_se_consuma_epsilon[0]), rand_tranzitie_A_echivalent, stari_in_care_se_consuma_epsilon[0]);
                }
            }
            else
            {
                foreach (string s in stari_in_care_se_consuma_epsilon)
                    completare_stari(vrea_index_pentru(s), rand_tranzitie_A_echivalent, s);
            }
            return true;
        }

        // Metoda de cautare a indexului starii curente in matricea starii de tranzitie
        static private int vrea_index_pentru(string stare)
        {
            int rand_al_starii;
            for (rand_al_starii = 0; rand_al_starii < multimea_starilor.Count(); rand_al_starii++)
                if (stare == multimea_starilor[rand_al_starii])
                    return rand_al_starii;
            return -1;
        }

        // Completarea starilor finale ale atomatului echivalent
        static private void completarea_starilor_finale_la_automatul_echivalent(string stare_curenta,int rand_in_automat_echivalent)
        {
            foreach (string p in multimea_starilor_finale)
                if (stare_curenta == p)
                    if (multimea_starilor_finale_ale_automatului_echivalent.Count() == 0) multimea_starilor_finale_ale_automatului_echivalent.Add(multimea_starilor[rand_in_automat_echivalent]);
                    else
                    {
                        foreach (string q in multimea_starilor_finale_ale_automatului_echivalent)
                            if (multimea_starilor[rand_in_automat_echivalent] == q)
                                este_deja_in_lista = true;

                        if (este_deja_in_lista == false)
                            multimea_starilor_finale_ale_automatului_echivalent.Add(multimea_starilor[rand_in_automat_echivalent]);

                        este_deja_in_lista = false;
                    }
        }

        static void Main(string[] args)
        {
            // Exercitiu I

            // Completarea variabelor locale cu valorile citite din fisier pentru Alfabetul limbajului, Multimea starilor , Multimea starilor finale si Starea finala
            if ((linie = file.ReadLine()) != null)
            {
                words = linie.Split(delimiterChar);
                foreach (string s in words) alfabetul_limbajului.Add(s);
            }
            if ((linie = file.ReadLine()) != null)
            {
                words = linie.Split(delimiterChar);
                foreach (string s in words) multimea_starilor.Add(s);
            }
            if ((linie = file.ReadLine()) != null)
            {
                words = linie.Split(delimiterChar);
                foreach (string s in words) multimea_starilor_finale.Add(s);
            }
            if ((linie = file.ReadLine()) != null) starea_initila = linie;

            // Definirea si completarea functiei de tranzitie si marcarea prezentei nedeterminismului
            tranzitie = new string[multimea_starilor.Count(), alfabetul_limbajului.Count()];
            while (row <= multimea_starilor.Count())
            {
                row++;
                if ((linie = file.ReadLine()) != null)
                {
                    words = linie.Split(delimiterChar);
                    for (int col = 0; col < words.Count(); col++)
                    {
                        tranzitie[row, col] = words[col];
                        if (words[col].IndexOf("_") != -1)
                            automat_nedeterminist = true;
                    }
                }
            }
            

            // Exercitiu al II-lea

            // Verificarea automatului daca are tranzitii epsilon 
            for (int i = 0; i < alfabetul_limbajului.Count(); i++)
            {
                if (alfabetul_limbajului[i] == "e")
                    coloana_epsilon = i;
            }
            if(coloana_epsilon != -1)
            {
                for (int i = 0; i < multimea_starilor.Count(); i++)
                    if (tranzitie[i, coloana_epsilon] != "-")
                        tranzitie_epsilon = true;
            }

            // Identificarea automatului cu stari finite
            if (tranzitie_epsilon == true)
            {
                if (automat_nedeterminist == true)
                    Console.WriteLine("AFE : Automatul cu stari finite este nedeterminist si are tranzitii epsilon .");
                else Console.WriteLine("AFE : Automatul cu stari finite este determinist si are tranzitii epsilon .");
            }
            else
            {
                if (automat_nedeterminist == true)
                    Console.WriteLine("AFN : Automatul cu stari finite este nedeterminist si nu are tranzitii epsilon .");
                else Console.WriteLine("AFD : Automatul cu stari finite este determinist si nu are tranzitii epsilon");
            }
             

            // Exercitiu al III-le
            echivalent = new string[multimea_starilor.Count(), alfabetul_limbajului.Count()];
            if (tranzitie_epsilon == true)
            {
                // Initializarea starilor finale ala automatului cu stari finite echivalent
                foreach (string s in multimea_starilor_finale)
                    multimea_starilor_finale_ale_automatului_echivalent.Add(s);

                // Aflarea coloanei epsilon
                for(int i = 0; i < alfabetul_limbajului.Count(); i++)
                {
                    if (alfabetul_limbajului[i] == "e")
                        index_of_epsilon = i;
                }

                // Crearea functie de tranzitie a automatului cu stari finite echivalent
                for (row = 0; row < multimea_starilor.Count(); row++)
                {
                    for (int col = 0; col < alfabetul_limbajului.Count(); col++)
                    {
                        if (col != index_of_epsilon) echivalent[row, col] = tranzitie[row, col];
                        else echivalent[row, col] = "-";
                    }
                }
                for (row = 0; row < multimea_starilor.Count(); row++)
                {
                    if (tranzitie[row, index_of_epsilon] != "-")
                    {
                        // Salvarea noilor stari finale ale automatului echivalent
                        stari_in_care_se_consuma_elementul = parsare_string(tranzitie[row, index_of_epsilon].ToString());

                        // Completarea starilor finale ale automatului cu stari finite echivalent
                        foreach (string s in stari_in_care_se_consuma_elementul)
                            completarea_starilor_finale_la_automatul_echivalent(s,row);

                        // Completarea matricei functiei de tranzitie a automatului cu stari finite echivalent
                        foreach (string s in stari_in_care_se_consuma_elementul)
                            completare_stari(vrea_index_pentru(s), row, s);
                    }
                }

                // Afisarea automatului cu stari finale echivalent

                // Afisarea limbajului
                foreach (string s in alfabetul_limbajului) Console.Write(s + " "); Console.WriteLine("");

                // Afisarea multimii starilor 
                foreach (string s in multimea_starilor) Console.Write(s + " "); Console.WriteLine("");

                // Afisarea multimii starilor finale 
                foreach (string s in multimea_starilor_finale_ale_automatului_echivalent) Console.Write(s + " "); Console.WriteLine("");

                // Afisarea starii initiale 
                Console.Write(starea_initila); Console.WriteLine("");
                
                // Afisare functie de tranzitie a automatului cu stari finite echivalent
                for (int i = 0; i < multimea_starilor.Count(); i++, Console.WriteLine(""))
                    for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                        Console.Write(echivalent[i, j] + " ");
                
            }
            else Console.WriteLine("Automatul cu stari finite nu are tranzitii epsilon deci nu se poate face automatul echivalent .");

            
            // Exercitiu al IV-lea
             
            if (tranzitie_epsilon == true)
            {
                // afisare automat determinist 
            }
            else Console.WriteLine("Automatul cu stari finite nu are tranzitii epsilon deci nu se poate afisa automatul echivalent deoarece acesta nu exista .");

            Console.ReadKey();
        }
    }
}
