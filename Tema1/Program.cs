using System;
using System.Collections.Generic;
using System.Linq;
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
        static int index_global_determinist = -1;
        static List<string> definirea_noii_stari = new List<string>();

        // Definirea fisierului de intrare
        static StreamReader file;
        static string cale;

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
        
        // Sortare string
        static private string aranjare(string original)
        {
            List<string> descompunere = parsare_string(original);
            List<string> auxiliar = new List<string>();
            List<int> indici = new List<int>(descompunere.Count);

            for(int i = 0; i < descompunere.Count; i++)
            {
                indici.Add(vreau_index_pentru(descompunere[i]));
            }

            indici.Sort();

            for (int i = 0; i < descompunere.Count; i++)
            {
                auxiliar.Add(multimea_starilor[indici[i]]);
            }

            return list_to_string(auxiliar); 
        }

        // Metoda de aranjare a starilor in celula matricii functie de tranzitie a automatului cu stari finite echivalent
        static private void i_want_to_make_echivalent_beautiful()
        {
            for (int i = 0; i < multimea_starilor.Count(); i++)
                for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                    if (echivalent[i, j] != "-")
                        echivalent[i, j] = aranjare(echivalent[i, j]);
        }

        // Metoda de a introduce o noua stare in matricea automatului determinist
        static private bool introduce_stare_in_matrice(int index_in_determinist,string stare,int coloana)
        {
            determinist[index_in_determinist, coloana] = stare;
            return true;
        }

        //  Metoda de verificare daca starea data este o stare noua
        static private bool state_verify(string stare)
        {
            bool present = false;

            if (stare != "-")
            {
                foreach (string s in multimea_starilor_determinist)
                    if (stare == s)
                        present = true;

                if (present == false)
                {
                    multimea_starilor_determinist.Add(stare);
                    index_global_determinist++;
                    for (int coloana = 0; coloana < alfabetul_limbajului.Count(); coloana++)
                    {
                        introduce_stare_in_matrice(index_global_determinist,list_to_string(definire_new_state(stare,coloana)), coloana);
                    }
                }
                else return false;
            }
            else return false;

            return true;
        }

        static void Main(string[] args)
        {
            file = new StreamReader(args[0].ToString());
            cale = args[1].ToString(); 

            File.WriteAllText(cale, string.Empty);

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

            // Afisarea limbajului
            foreach (string s in alfabetul_limbajului) File.AppendAllText(cale,s + " "); File.AppendAllText(cale,"\r\n");

            // Afisarea multimii starilor 
            foreach (string s in multimea_starilor) File.AppendAllText(cale,s + " "); File.AppendAllText(cale,"\r\n");

            // Afisarea multimii starilor finale 
            foreach (string s in multimea_starilor_finale) File.AppendAllText(cale,s + " "); File.AppendAllText(cale,"\r\n");

            // Afisarea starii initiale 
            File.AppendAllText(cale,starea_initila); File.AppendAllText(cale,"\r\n");

            // Afisare functie de tranzitie a automatului cu stari finite echivalent
            for (int i = 0; i < multimea_starilor.Count(); i++, File.AppendAllText(cale, "\r\n"))
                for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                    File.AppendAllText(cale,tranzitie[i, j] + " ");


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
                File.AppendAllText(cale, "\r\n");
                File.AppendAllText(cale, "E");
                File.AppendAllText(cale, "\r\n");
                File.AppendAllText(cale, "\r\n");
            }
            else
            {
                if (automat_nedeterminist == true)
                { File.AppendAllText(cale, "\r\n"); File.AppendAllText(cale, "F"); File.AppendAllText(cale, "\r\n"); File.AppendAllText(cale, "\r\n"); }
                else { File.AppendAllText(cale, "\r\n"); File.AppendAllText(cale, "D"); File.AppendAllText(cale, "\r\n"); File.AppendAllText(cale, "\r\n"); }
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
                for (int i = 0; i < multimea_starilor.Count(); i++)
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
                foreach (string s in alfabetul_limbajului) File.AppendAllText(cale,s + " "); File.AppendAllText(cale,"\r\n");

                // Afisarea multimii starilor 
                foreach (string s in multimea_starilor) File.AppendAllText(cale,s + " "); File.AppendAllText(cale,"\r\n");

                // Afisarea multimii starilor finale 
                multimea_starilor_finale_ale_automatului_echivalent = parsare_string(aranjare(list_to_string(multimea_starilor_finale_ale_automatului_echivalent)));
                foreach (string s in multimea_starilor_finale_ale_automatului_echivalent) File.AppendAllText(cale,s + " "); File.AppendAllText(cale,"\r\n");

                // Afisarea starii initiale 
                File.AppendAllText(cale,starea_initila); File.AppendAllText(cale,"\r\n");

                i_want_to_make_echivalent_beautiful();

                // Afisare functie de tranzitie a automatului cu stari finite echivalent
                for (int i = 0; i < multimea_starilor.Count(); i++,File.AppendAllText(cale, "\r\n"))
                    for (int j = 0; j < alfabetul_limbajului.Count(); j++)
                        File.AppendAllText(cale,echivalent[i, j] + " "); 
                
            }


            // Exercitiu al IV-lea
            bool final_stari = false;
            int max_size_of_states = (int)(Math.Pow((double)2,(double)multimea_starilor.Count())); 

            determinist = new string[max_size_of_states, alfabetul_limbajului.Count()];
            List<string> stari_ale_automatului_determinist = new List<string>();
            if (tranzitie_epsilon == true)
            {
                while (final_stari == false)
                {
                    for (int rand = 0; rand < multimea_starilor.Count(); rand++)
                    {
                        
                        if (multimea_starilor_determinist.Count > 0)
                        {
                            bool present = false;

                            foreach (string s in multimea_starilor_determinist)
                                if (multimea_starilor[rand] == s)
                                    present = true;

                            if (present == false)
                            {
                                multimea_starilor_determinist.Add(multimea_starilor[rand]);
                                index_global_determinist++;
                                for (int coloana = 0; coloana < alfabetul_limbajului.Count(); coloana++)
                                {
                                    introduce_stare_in_matrice(index_global_determinist, echivalent[rand, coloana], coloana);
                                }
                            }
                        }
                        else
                        {
                            multimea_starilor_determinist.Add(multimea_starilor[rand]);
                            index_global_determinist++;
                            for (int coloana = 0; coloana < alfabetul_limbajului.Count(); coloana++)
                            {
                                introduce_stare_in_matrice(index_global_determinist, echivalent[rand, coloana], coloana);
                            }
                        }

                        bool nu_mai_sunt_stari_noi = false;

                        while (nu_mai_sunt_stari_noi == false)
                        {
                            int numar_de_stari = multimea_starilor_determinist.Count();
                            nu_mai_sunt_stari_noi = true;

                            for (int line = 0; line < numar_de_stari; line++)
                            {
                                for (int coloana = 0; coloana < alfabetul_limbajului.Count(); coloana++)
                                {
                                    if(state_verify(determinist[line, coloana]) == true)
                                    {
                                        nu_mai_sunt_stari_noi = false;
                                    }
                                }
                            }
                            
                        }
                        if (rand == multimea_starilor.Count() - 1)
                            final_stari = true;
                    }
                }
            }

        }
    }
}
