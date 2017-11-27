﻿using System;
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

        static string[,] determinist;
        static List<string> multimea_starilor_determinist = new List<string>();
        static int index_global_determinist = 0;
        static List<string> definirea_noii_stari = new List<string>();

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
                    else echivalent[rand_fct_tranzitie_a_echivalent, coloana] = s;
                else if (s != "-") echivalent[rand_fct_tranzitie_a_echivalent, coloana] += "_" + s;
                else echivalent[rand_fct_tranzitie_a_echivalent, coloana] = echivalent[rand_fct_tranzitie_a_echivalent, coloana];
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
                    completare_stari(vreau_index_pentru(stari_in_care_se_consuma_epsilon[0]), rand_tranzitie_A_echivalent, stari_in_care_se_consuma_epsilon[0]);
                }
            }
            else
            {
                foreach (string s in stari_in_care_se_consuma_epsilon)
                    if (vreau_index_pentru(s) != rand_tranzitie_A_echivalent)
                        completare_stari(vreau_index_pentru(s), rand_tranzitie_A_echivalent, s);
            }
            return true;
        }

        // Metoda de cautare a indexului starii curente in matricea starii de tranzitie
        static private int vreau_index_pentru(string stare)
        {
            int rand_al_starii;
            for (rand_al_starii = 0; rand_al_starii < multimea_starilor.Count(); rand_al_starii++)
                if (stare == multimea_starilor[rand_al_starii])
                    return rand_al_starii;
            return -1;
        }

        // Metoda de cautare a indexului starii curente in matricea starii de tranzitie a automatului determinist
        static private int din_determinist_vreau_index_pentru(string stare)
        {
            int rand_al_starii;
            for (rand_al_starii = 0; rand_al_starii < multimea_starilor_determinist.Count(); rand_al_starii++)
                if (stare == multimea_starilor[rand_al_starii])
                    return rand_al_starii;
            return -1;
        }

        // Completarea starilor finale ale atomatului echivalent
        static private void completarea_starilor_finale_la_automatul_echivalent(string stare_curenta,int rand_in_automat_echivalent)
        {
            List<string> stari_descoperite = new List<string>();
            foreach (string p in multimea_starilor_finale)
                if (stare_curenta == p)
                    if (multimea_starilor_finale_ale_automatului_echivalent.Count() == 0) multimea_starilor_finale_ale_automatului_echivalent.Add(multimea_starilor[rand_in_automat_echivalent]);
                    else
                    {
                        foreach (string q in multimea_starilor_finale_ale_automatului_echivalent)
                            if (multimea_starilor[rand_in_automat_echivalent] == q)
                                este_deja_in_lista = true;

                        if (este_deja_in_lista == false)
                        {
                            multimea_starilor_finale_ale_automatului_echivalent.Add(multimea_starilor[rand_in_automat_echivalent]);
                            stari_descoperite.Add(multimea_starilor[rand_in_automat_echivalent]);
                        }
                        este_deja_in_lista = false;
                    }
            if (stari_descoperite.Count != 0)
                foreach (string s in stari_descoperite)
                    multimea_starilor_finale.Add(s);
        }

        // Eliminare a unui rand dintr-o matrice
        static private void alter_tranzition_function(int step, string stare_inutila)
        {
            string[,] new_matrix = new string[multimea_starilor.Count() - step, alfabetul_limbajului.Count() - 1];
            int index_of_useless_state = vreau_index_pentru(stare_inutila);
            int index_row_for_new_matrix = -1;
            int index_col_for_new_matrix = -1;

            for (int i = 0; i < multimea_starilor.Count(); i++)
            {
                if (i != index_of_useless_state)
                {
                    index_row_for_new_matrix++;
                    for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                    {
                        if (j != index_of_epsilon)
                        {
                            index_col_for_new_matrix++; ;
                            new_matrix[index_row_for_new_matrix, index_col_for_new_matrix] = echivalent[i, j];
                        }
                    }
                    index_col_for_new_matrix = -1;
                }
            }
            Array.Clear(echivalent, 0, echivalent.Length);
            echivalent = new_matrix;
        }

        // Eliminare coloana epsilon
        static private void no_epsilon_in_echivalent()
        {
            string[,] new_matrix = new string[multimea_starilor.Count() , alfabetul_limbajului.Count() - 1];
            int index_col_for_new_matrix = -1;

            for (int i = 0; i < multimea_starilor.Count(); i++)
            {
                for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                {
                    if (j != index_of_epsilon)
                    {
                        index_col_for_new_matrix++; ;
                        new_matrix[i, index_col_for_new_matrix] = echivalent[i, j];
                    }
                }
                index_col_for_new_matrix = -1;
            }
            Array.Clear(echivalent, 0, echivalent.Length);
            echivalent = new_matrix;
        }

        // Metoda definire stare noua in determinist
        static private List<string> definire_new_state(string noua_stare,int coloana)
        {
            List<string> starile_din_echivalent_ = parsare_string(noua_stare);
            List<string> suma_stari_din_echivalent = new List<string>();

            if (noua_stare == "-")
                return parsare_string(noua_stare);

            for(int i = 0; i < starile_din_echivalent_.Count(); i++)
            {
                List<string> auxiliar = parsare_string(echivalent[vreau_index_pentru(starile_din_echivalent_[i]), coloana]);

                for(int j = 0; j < auxiliar.Count(); j++)
                {
                    if (auxiliar.Count == 1 && auxiliar[0] != "-") 
                    {
                        if(suma_stari_din_echivalent.Count == 0)
                            suma_stari_din_echivalent.Add(auxiliar[0]);
                        else foreach (string s in suma_stari_din_echivalent)
                            if (s != auxiliar[0])
                                suma_stari_din_echivalent.Add(auxiliar[0]); 
                    }
                    else if(auxiliar.Count != 1)
                    {
                        foreach (string s in auxiliar)
                        {
                            if (suma_stari_din_echivalent.Count == 0)
                                suma_stari_din_echivalent.Add(s);
                            else
                            {
                                int nr_stari = suma_stari_din_echivalent.Count;
                                bool exist = false;
                                for (int l = 0; l < nr_stari; l++) 
                                {
                                    if (s == suma_stari_din_echivalent[l])
                                        exist = true;
                                }
                                if(exist == false)
                                    suma_stari_din_echivalent.Add(s);
                                exist = false;
                            }
                        }
                    }
                }
            }
            return suma_stari_din_echivalent;
        }

        // Metoda recursiva de completare a starilor noi formate
        static private bool new_states_complete(int coloana,string stare_noua,int index_lipire_coloana_stare_noua,string stare_parinte,bool further)
        {
            if (further == true)
            {
                if (stare_noua == "-")
                {
                    if (coloana == 0)
                    {
                        determinist[index_global_determinist, coloana] = "-";
                        index_global_determinist++;
                    }
                    else determinist[index_lipire_coloana_stare_noua, coloana] = "-";
                    return true;
                }
                else
                {
                    List<string> lista = parsare_string(stare_noua);

                    if (lista.Count() == 1)
                    {
                        if (coloana == 0)
                        {
                            determinist[index_global_determinist, coloana] = stare_noua;
                            index_global_determinist++;
                        }
                        else determinist[index_lipire_coloana_stare_noua-1, coloana] = stare_noua;
                    }
                    else
                    {
                        if (coloana == 0)
                        {
                            determinist[index_global_determinist, coloana] = stare_noua;
                            index_global_determinist++;
                        }
                        else determinist[index_lipire_coloana_stare_noua, coloana] = stare_noua;



                        for (int t = 0; t < alfabetul_limbajului.Count; t++)
                        {
                            lista.Clear();

                            lista = definire_new_state(stare_noua, t);
                            bool prezent = false;

                            foreach (string s in multimea_starilor_determinist)
                                if (list_to_string(lista) == s)
                                    prezent = true;
                            bool go_deep = false;
                            if (prezent == false)
                            {
                                multimea_starilor_determinist.Add(list_to_string(lista));
                                go_deep = true;
                            }
                            new_states_complete(t, list_to_string(lista), index_global_determinist, "", go_deep);



                        }

                        return true;
                    }
                }
            }
            return true;
        }

        // Metoda trasnformare list in string
        static private string list_to_string(List<string> lista)
        {
            string stare_noua_="-"; 

            if (lista.Count != 0)
            {
                stare_noua_ = lista[0];
                if (lista.Count > 1) 
                {
                    for (int i = 1; i < lista.Count; i++) 
                    {
                        stare_noua_ += "_";
                        stare_noua_ += lista[i];
                    }
                }
            }

            return stare_noua_;
        }

        // Metoda ce completeza recursiv matricia functie de tranzitie pentru automatul determinist 
        static private bool complete_row_for_determinist(int index_rand_in_echivalent, int coloana, List<string> stari_din_echivalent)
        {
            definirea_noii_stari.Clear();

            // Actualizare multime stari determinist
            // Imi introduce starea din care a plecat automatul cand s-a cerut aceasta metoda in multimea starilor deterministului
            if (din_determinist_vreau_index_pentru(multimea_starilor[index_rand_in_echivalent]) == -1)
            {
                multimea_starilor_determinist.Add(multimea_starilor[index_rand_in_echivalent]);
            }

            // Daca noua stare nu a mai aparut in multimea de stari 
            if (din_determinist_vreau_index_pentru(echivalent[index_rand_in_echivalent, coloana]) == -1)
            {
                if (stari_din_echivalent.Count() == 1 && stari_din_echivalent[0] == "-")
                    goto fara_memorare;
                multimea_starilor_determinist.Add(echivalent[index_rand_in_echivalent, coloana]);
                fara_memorare:;

                // Pentru stare absenta
                if (stari_din_echivalent.Count() == 1 && stari_din_echivalent[0] == "-")
                {
                    if (coloana == 0)
                    {
                        determinist[index_global_determinist, coloana] = "-";
                        index_global_determinist++;
                    }
                    else determinist[index_rand_in_echivalent, coloana] = "-";
                    return true;
                }
                else
                {
                    if (stari_din_echivalent.Count() == 1)
                    {
                        if (coloana == 0)
                        {
                            determinist[index_global_determinist, coloana] = stari_din_echivalent[0];
                            index_global_determinist++;
                        }
                        else determinist[index_rand_in_echivalent, coloana] = stari_din_echivalent[0]; 
                    }
                    else
                    {
                        if (coloana == 0)
                        {
                            determinist[index_global_determinist, coloana] = echivalent[index_rand_in_echivalent, coloana];
                            index_global_determinist++;
                        }
                        else determinist[index_rand_in_echivalent, coloana] = echivalent[index_rand_in_echivalent, coloana];
                        
                        return true;
                    }
                }
            }
            return true;
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
                            if (vreau_index_pentru(s) != row)
                                completare_stari(vreau_index_pentru(s), row, s);

                    }
                }


                // Rescrierea elementelor automatului cu stari finite echivalent
                // Detectarea starilor inutile  
                bool[] prezenta_stari = new bool[multimea_starilor.Count()];
                for (int i = 0; i < multimea_starilor.Count(); i++, Console.WriteLine(""))
                    for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                    {
                        stari_in_care_se_consuma_elementul.Clear();
                        stari_in_care_se_consuma_elementul = parsare_string(echivalent[i,j]);
                        foreach (string s in stari_in_care_se_consuma_elementul)
                            foreach (string p in multimea_starilor)
                                if (s == p) prezenta_stari[vreau_index_pentru(s)] = true;
                    }
                // Se marcheaza prezenta starii initiale
                prezenta_stari[vreau_index_pentru(starea_initila)] = true;

                // Sterge randul corespunzator starilor inutile din matricea echivalent
                int step = 0;
                for (int i = 0; i < multimea_starilor.Count(); i++)
                    if (prezenta_stari[i] == false)
                    {
                        step++;
                        alter_tranzition_function(step,multimea_starilor[i]);
                    }
                if (step == 0)
                    no_epsilon_in_echivalent();

                // Rescrierea multimei starilor automatului echivalent si a functiei de tranzitie prin eliminarea starilor inutile
                for (int i = 0; i < multimea_starilor.Count(); i++)
                    if (prezenta_stari[i] == false)
                        multimea_starilor.RemoveAt(i);

                // Rescrierea alfabetului automatului fara caracterul epsilon
                for (int i = 0; i < alfabetul_limbajului.Count(); i++)
                    if (alfabetul_limbajului[i] == "e")
                        alfabetul_limbajului.RemoveAt(i);

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
            //bool final_stari = false;
            //int max_size_of_states = 2 ^ multimea_starilor.Count();
            //determinist = new string[max_size_of_states, alfabetul_limbajului.Count()];
            //List<string> stari_ale_automatului_determinist = new List<string>(); 
            //if (tranzitie_epsilon == true)
            //{
            //    while(final_stari == false)
            //    {
            //        for (int rand = 0; rand < multimea_starilor.Count(); rand++)
            //        {
            //            for (int coloana = 0; coloana < alfabetul_limbajului.Count(); coloana++)
            //            {
            //                List<string> stari_din_echivalent = new List<string>();

            //                stari_din_echivalent.Clear();
            //                stari_din_echivalent = parsare_string(echivalent[rand, coloana]);

            //                bool present = false;

            //                if (multimea_starilor_determinist.Count > 0) 
            //                foreach (string s in multimea_starilor_determinist)
            //                    if (echivalent[rand, coloana] == s)
            //                        present = true;

            //                if (present == false)
            //                complete_row_for_determinist(rand,coloana, stari_din_echivalent);
            //                present = false;

            //                for (int col = 0; col < alfabetul_limbajului.Count; col++)
            //                {
            //                    if (echivalent[rand, col] == "-")
            //                    { determinist[rand, coloana] = "-"; break; }


            //                    definirea_noii_stari = definire_new_state(echivalent[rand, col], col);
            //                    bool prezent = false;

            //                    foreach (string s in multimea_starilor_determinist)
            //                        if (list_to_string(definirea_noii_stari) == s)
            //                            prezent = true;

            //                    if (prezent == false)
            //                        new_states_complete(col, list_to_string(definirea_noii_stari), index_global_determinist, echivalent[rand, col], true) ;
                                
            //                }

            //                //definire_new_state(index_global_determinist, "q2_q3", coloana);
            //                // se face parcurgerea pe starile din echivalent 
            //                // se baga in matricea determinist 
            //                // se apeleaza metoda prin care se verifica daca starea curenta este simpla sau compusa 
            //                // daca e simpla se intoarce cu simplu si se continua for-ul asta       ^
            //                // daca nu e compusa se baga in matricea determinist si se se revine la |

            //                //apeleaza metoda ce intoarce randul corespunzator randului din echivalent in determinist rand , coloana 
            //            }
            //            if (rand == multimea_starilor.Count() - 1)
            //                final_stari = true;
            //        }
            //    }
            //}
            //else Console.WriteLine("Automatul cu stari finite nu are tranzitii epsilon deci nu se poate afisa automatul echivalent deoarece acesta nu exista .");

            Console.ReadKey();
        }
    }
}
