using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku
{
    public class sudokuMoottori
    {
        private int[,] peliruudukko;
        private Random rnd = new Random();

        private Pakka[] rivit;
        private Pakka[] sarakkeet;
        private Pakka[,] testatutNumerot;
        private Pakka[,] pikkuGrid;

        private int rivi, sarake;

        public sudokuMoottori()
        {
            Alusta();
        }

        public void Alusta()
        {
            this.rivit = new Pakka[9];
            this.sarakkeet = new Pakka[9];
            this.testatutNumerot = new Pakka[9, 9];
            this.pikkuGrid = new Pakka[3, 3];
            this.rivi = 0;
            this.sarake = 0;

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    pikkuGrid[y, x] = new Pakka(rnd);
                    pikkuGrid[y, x].Alusta();
                }
            }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    testatutNumerot[y, x] = new Pakka(rnd);
                }
            }

            for (int i = 0; i < 9; i++)
            {
                rivit[i] = new Pakka(rnd);
                rivit[i].Alusta();
                sarakkeet[i] = new Pakka(rnd);
                sarakkeet[i].Alusta();
            }

            peliruudukko = new int[9, 9];
            

            for (int rivi = 0; rivi < 9; rivi++)
            {
                for (int sarake = 0; sarake < 9; sarake++)
                {
                    peliruudukko[rivi, sarake] = 0;
                }
            }
        }

        public void RatkaisunAlustus()
        {

            this.testatutNumerot = new Pakka[9, 9];

            this.rivi = 0;
            this.sarake = 0;

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    testatutNumerot[y, x] = new Pakka(rnd);
                }
            }
        }


        // 2.0
        #region LuoPeliv0.2

        public void LuoPeli()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            do
            {
                if (LuoPeliruudukko() == true)
                {
                    Console.WriteLine("Saatiin tehtyä");
                    break;
                }
                else
                {
                    Console.WriteLine("Ei saatu tehtyä. Yritetään uusiksi..");
                }
            } while (true);

            Console.WriteLine("Generointiin kului " + ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - milliseconds) + "ms");

        }

        public void KayLapiYhdenVaihtoehdonPaikat(int[,] taulukko, bool ekaKierros, int nykyinenRivi, int nykyinenSarake)
        {
            if (nykyinenRivi > 8)
                return;

            int[,] kiinteatNumerot = taulukko;// luetaan laudalta käyttäjän syöttämät numerot, eli nämä on "kiinteitä" joihin ei kosketa

            testatutNumerot = new Pakka[9, 9];

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    testatutNumerot[y, x] = new Pakka(rnd);
                }
            }

            for (int rivi = nykyinenRivi; rivi < 9; rivi++)
            {
                for (int sarake = nykyinenSarake; sarake < 9; sarake++)
                {

                    if (kiinteatNumerot[rivi, sarake] > 0) // mikäli on käyttäjän asettama numero, ei tehdä mitään
                    { continue; }

                    int[] grid = LaskeGrid();

                    int gy = grid[0];
                    int gx = grid[1];

                    Pakka vaihtoehdot = LaskeVaihtoehdot(sarakkeet[sarake], rivit[rivi], pikkuGrid[gy, gx]); // Haetaan mahdolliset vaihtoehdot

                    if (vaihtoehdot != null) // Mikäli meillä on vaihtoehtoja
                    {
                        if (vaihtoehdot.NumeroidenMaara() == 1) // mikäli on vain 1 vaihtoehto
                        {
                            int arvottuNumero = vaihtoehdot.NostaJaPoistaSatunnainen();

                            if (ekaKierros)
                            {
                                kiinteatNumerot[rivi, sarake] = arvottuNumero;
                            }

                            peliruudukko[rivi, sarake] = arvottuNumero; // Lisätään arvottu numero peliruudukkoon
                            rivit[rivi].PoistaNumero(arvottuNumero); // poistetaan arvottu numero kyseisen rivin pelattavissa olevista numeroista
                            sarakkeet[sarake].PoistaNumero(arvottuNumero); // ja sama sarakkeelle
                            pikkuGrid[gy, gx].PoistaNumero(arvottuNumero); // ja vielä 3x3 gridille
                        }
                    }
                }
            }
        }

        public bool Ratkaisualgoritmi(int[,] esisyotetytNumerot)
        {
            // KayLapiYhdenVaihtoehdonPaikat(esisyotetytNumerot, true, 0, 0);
            bool liikuttiinTaaksepain = false;
            bool yritetaanRatkaista = true;

            RatkaisunAlustus();

            do
            {

                if ((OnkoKiinteaNumero(esisyotetytNumerot) && liikuttiinTaaksepain == true))
                {
                    yritetaanRatkaista = SiirryEdelliseenSoluun();

                    if (yritetaanRatkaista == false)
                        return false;

                    if (OnkoKiinteaNumero(esisyotetytNumerot) == false)
                        PoistaNumero();

                    PoistaYritykset();

                    continue;
                }
                    
                else if ((OnkoKiinteaNumero(esisyotetytNumerot) && liikuttiinTaaksepain == false))
                {
                    yritetaanRatkaista = SiirrySeuraavaanSoluun();

                    if (yritetaanRatkaista == false)
                        return false;

                    continue;
                }
                

                bool onnistunutSiirto = YritaSijoittaaNumero();

                if (onnistunutSiirto == true)
                {
                    yritetaanRatkaista = SiirrySeuraavaanSoluun();
                    if (yritetaanRatkaista == false)
                        return true;

                    liikuttiinTaaksepain = false;
                }
                else
                {

                    yritetaanRatkaista = SiirryEdelliseenSoluun();

                    if (yritetaanRatkaista == false) // ollaan päädytty tilanteeseen, jossa kaikki ratkaisut on yritetty, eikä voida liikkua enää taaksepäin
                        return false;

                    if (OnkoKiinteaNumero(esisyotetytNumerot) == false)
                        PoistaNumero();

                    PoistaYritykset();

                    liikuttiinTaaksepain = true;
                }
                    
                
            } while (yritetaanRatkaista);

            return false; // HOX!!
        }

        public bool YritaSijoittaaNumero()
        {
            Pakka vaihtoehdot = OnkoVaihtoehtoja();

            if (vaihtoehdot != null)
            {
                SijoitaArvottuNumero(vaihtoehdot);
                return true;
            }
            else
            {
                return false;
            }

        }

        public void PoistaYritykset()
        {
            int[] seuraavasolu = LaskeSeuraavaSolu();


            for (int y = seuraavasolu[0]; y < 9; y++) // tyhjennetään yritykset
            {
                for (int x = seuraavasolu[1]; x < 9; x++)
                {
                    testatutNumerot[y, x].Tyhjenna();
                }
            }
        }

        public void PoistaNumero()
        {
            int[] gridi = LaskeGrid();
            int gy = gridi[0];
            int gx = gridi[1];

            int edellinenNumero = peliruudukko[this.rivi, this.sarake]; // otetaan vanha numero talteen

            
            peliruudukko[this.rivi, this.sarake] = 0; // poistetaan numero ruudukosta

            // palautetaan numero takaisin riveille ja sarakkeille pelattavissa oleviin numeroihin
            this.rivit[this.rivi].Lisaa(edellinenNumero);
            this.sarakkeet[this.sarake].Lisaa(edellinenNumero);

            pikkuGrid[gy, gx].Lisaa(edellinenNumero);

        }

        public bool SiirryEdelliseenSoluun()
        {

            int edellinenRivi, edellinenSarake;
            if (this.sarake > 0)
            {
                edellinenSarake = this.sarake - 1;
                edellinenRivi = this.rivi;
            }
            else
            {
                edellinenSarake = 8;
                edellinenRivi = rivi - 1;
            }

            this.sarake = edellinenSarake;
            this.rivi = edellinenRivi;

            if (this.rivi < 0)
                return false;
            else
                return true;

        }

        public bool SiirrySeuraavaanSoluun()
        {
            int[] arvot = LaskeSeuraavaSolu();
            this.rivi = arvot[0];
            this.sarake = arvot[1];
            if (this.rivi > 8)
                return false;
            else
                return true;
        }

        private int[] LaskeSeuraavaSolu()
        {

            int[] arvot = new int[2];

            if (this.sarake < 8)
            {
                arvot[1] = this.sarake + 1;
                arvot[0] = this.rivi;
            }
            else
            {
                arvot[1] = 0;
                arvot[0] = this.rivi + 1;
            }
            return arvot;
        }

        public Pakka OnkoVaihtoehtoja()
        {
            int[] grid = LaskeGrid();
            // onko rivit ja sarakkeet väärinpäin!
            Pakka vaihtoehdot = LaskeVaihtoehdot(this.sarakkeet[this.sarake], this.rivit[rivi], this.pikkuGrid[grid[0], grid[1]]); // Haetaan mahdolliset vaihtoehdot
            if (vaihtoehdot == null)
                return null;

            for (int i = 0; i < testatutNumerot[this.rivi, this.sarake].NumeroidenMaara(); i++)
            {
                // poistetaan vaihtoehdoista jo koitetut numerot
                vaihtoehdot.PoistaNumero(testatutNumerot[this.rivi, this.sarake].NostaNumeroPaikasta(i));
            }

            if (vaihtoehdot.NumeroidenMaara() > 0)
                return vaihtoehdot;
            else
                return null;
        }

        public bool OnkoKiinteaNumero(int[,] kiinteatNumerot)
        {
            if (kiinteatNumerot[this.rivi, this.sarake] > 0)
                return true;
            else
                return false;
        }

        

        public int[] LaskeGrid()
        {
            int rivi = this.rivi;
            int sarake = this.sarake;

            int[] arvot = new int[2];
            arvot[0] = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
            arvot[1] = (int)Math.Floor((decimal)(sarake) / 3);
            return arvot;
        }

        public int SijoitaArvottuNumero(Pakka vaihtoehdot)
        {
            int rivi = this.rivi;
            int sarake = this.sarake;

            int arvottuNumero = vaihtoehdot.NostaJaPoistaSatunnainen();
            int[] grid = LaskeGrid();
            int gy = grid[0];
            int gx = grid[1];

            peliruudukko[rivi, sarake] = arvottuNumero; // Lisätään arvottu numero peliruudukkoon
            testatutNumerot[rivi, sarake].Lisaa(arvottuNumero); // Lisätään arvottu numero kyseisen solun testattuihin numeroihin
            rivit[rivi].PoistaNumero(arvottuNumero); // poistetaan arvottu numero kyseisen rivin pelattavissa olevista numeroista
            sarakkeet[sarake].PoistaNumero(arvottuNumero); // ja sama sarakkeelle
            pikkuGrid[gy, gx].PoistaNumero(arvottuNumero); // ja vielä 3x3 gridille
            return arvottuNumero;
        }

        #region Pelin ratkaisualgoritmi
        public bool RatkaisePeli(int[,] taulukko)
        {
            int[,] kiinteatNumerot = taulukko;// luetaan laudalta käyttäjän syöttämät numerot, eli nämä on "kiinteitä" joihin ei kosketa

            testatutNumerot = new Pakka[9, 9];

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    testatutNumerot[y, x] = new Pakka(rnd);
                }
            }

            bool liikuttuTaaksepain = false;
            int montakoAskeltaTaaksepain = 0;

            int riviMax = 0;
            int sarakeMax = 0;

            for (int rivi = 0; rivi < 9; rivi++)
            {
                for (int sarake = 0; sarake < 9; sarake++)
                {
                    if (rivi == 7)
                    {

                    }

                    if (rivi >= riviMax)
                    {
                        riviMax = rivi;
                        sarakeMax = 0;

                        if (sarake > sarakeMax)
                            sarakeMax = sarake;
                    }





                    if (liikuttuTaaksepain) // mikäli ollaan edellisellä kierroksella liikuttu taaksepäin
                    {
                        int edellinenRivi, edellinenSarake;
                        if (sarake > 0)
                        {
                            edellinenSarake = sarake - 1;
                            edellinenRivi = rivi;
                        }
                        else
                        {
                            edellinenSarake = 8;
                            edellinenRivi = rivi - 1;
                        }
                        sarake = edellinenSarake;
                        rivi = edellinenRivi;
                        if (sarake < 0)
                            sarake = 0;
                        if (rivi < 0)
                            rivi = 0;
                    }

                    bool kiinteaNumero;
                    if (kiinteatNumerot[rivi, sarake] > 0)
                        kiinteaNumero = true;
                    else
                        kiinteaNumero = false;

                    if (kiinteatNumerot[rivi, sarake] > 0) // mikäli on käyttäjän asettama numero, ei tehdä mitään
                    {
                        if (liikuttuTaaksepain) // mikäli ollaan edellisellä kierroksella liikuttu taaksepäin
                        {
                            int edellinenRivi, edellinenSarake;
                            if (sarake > 0)
                            {
                                edellinenSarake = sarake - 1;
                                edellinenRivi = rivi;
                            }
                            else
                            {
                                edellinenSarake = 8;
                                edellinenRivi = rivi - 1;
                            }
                            sarake = edellinenSarake;
                            rivi = edellinenRivi;
                        }

                        continue;
                    }

                    int gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
                    int gx = (int)Math.Floor((decimal)(sarake) / 3);

                    Pakka vaihtoehdot = LaskeVaihtoehdot(sarakkeet[sarake], rivit[rivi], pikkuGrid[gy, gx]); // Haetaan mahdolliset vaihtoehdot

                    if (vaihtoehdot != null) // Mikäli meillä on vaihtoehtoja
                    {
                        for (int i = 0; i < testatutNumerot[rivi, sarake].NumeroidenMaara(); i++)
                        {
                            // poistetaan vaihtoehdoista jo koitetut numerot
                            vaihtoehdot.PoistaNumero(testatutNumerot[rivi, sarake].NostaNumeroPaikasta(i));
                        }

                        //Console.WriteLine("Vaihtoehdot: ");
                        //vaihtoehdot.ListaaNumerot();

                        if (vaihtoehdot.NumeroidenMaara() > 0) // mikäli meille jäi vaihtoehtoja enemmän kuin vaihtoehdot - jo yritetyt numerot
                        {
                            int arvottuNumero = vaihtoehdot.NostaJaPoistaSatunnainen();

                            peliruudukko[rivi, sarake] = arvottuNumero; // Lisätään arvottu numero peliruudukkoon
                            testatutNumerot[rivi, sarake].Lisaa(arvottuNumero); // Lisätään arvottu numero kyseisen solun testattuihin numeroihin
                            rivit[rivi].PoistaNumero(arvottuNumero); // poistetaan arvottu numero kyseisen rivin pelattavissa olevista numeroista
                            sarakkeet[sarake].PoistaNumero(arvottuNumero); // ja sama sarakkeelle
                            pikkuGrid[gy, gx].PoistaNumero(arvottuNumero); // ja vielä 3x3 gridille

                            liikuttuTaaksepain = false; // eteenpäin ja täysiä!
                            // Console.WriteLine("Arvottiin rivi: " + rivi + " solu: " + sarake);
                            continue; // ei tarvetta käydä for-lausetta loppuun enää
                        }
                    }

                    if (vaihtoehdot == null || vaihtoehdot.NumeroidenMaara() == 0) // mikäli mahdollisia siirtoja ei ole, joudutaan palaamaan takaisin
                    {
                        bool liikutaanTaaksepain = false; // liikutaanko taaksepäin, oletetaan että ei


                        // poista numero tästä solusta, kun ei ole vaihtoehtoja, eikä käyttäjän syöttämä solu ja ollaan liikuttu taaksepäin

                        int edellinenNumero = peliruudukko[rivi, sarake]; // otetaan vanha numero talteen

                        peliruudukko[rivi, sarake] = 0; // poistetaan numero ruudukosta

                        // palautetaan numero takaisin riveille ja sarakkeille pelattavissa oleviin numeroihin
                        rivit[rivi].Lisaa(edellinenNumero);
                        sarakkeet[sarake].Lisaa(edellinenNumero);

                        gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
                        gx = (int)Math.Floor((decimal)(sarake) / 3);

                        pikkuGrid[gy, gx].Lisaa(edellinenNumero); // palautetaan numero myös 3x3 gridille


                        do
                        {
                            if (rivi == 1 && sarake == 0)
                            {

                            }

                            int edellinenRivi, edellinenSarake;

                            //
                            if (sarake > 0)
                            {
                                edellinenSarake = sarake - 1;
                                edellinenRivi = rivi;
                            }
                            else
                            {
                                edellinenSarake = 8;
                                edellinenRivi = rivi - 1;
                            }

                            // palataan edelliseen soluun
                            rivi = edellinenRivi;
                            sarake = edellinenSarake;
                            liikuttuTaaksepain = false;
                            ///

                            if (rivi < 0)
                            {
                                // ei löydetty ratkaisua ollenkaan näillä numeroilla, palautetaan false
                                return false;
                            }

                            if (kiinteatNumerot[rivi, sarake] > 0) // mikäli on käyttäjän asettama numero, ei tehdä mitään
                                continue;

                            //Console.WriteLine("Ei mahdollisia siirtoja, palataan taaksepäin soluun: rivi: " + rivi + " solu: " + sarake);

                            edellinenNumero = peliruudukko[rivi, sarake]; // otetaan vanha numero talteen

                            peliruudukko[rivi, sarake] = 0; // poistetaan numero ruudukosta

                            // palautetaan numero takaisin riveille ja sarakkeille pelattavissa oleviin numeroihin
                            rivit[rivi].Lisaa(edellinenNumero);
                            sarakkeet[sarake].Lisaa(edellinenNumero);

                            gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
                            gx = (int)Math.Floor((decimal)(sarake) / 3);

                            pikkuGrid[gy, gx].Lisaa(edellinenNumero); // palautetaan numero myös 3x3 gridille


                            Pakka uudetVaihtoehdot = LaskeVaihtoehdot(sarakkeet[sarake], rivit[rivi], pikkuGrid[gy, gx]);

                            for (int i = 0; i < testatutNumerot[rivi, sarake].NumeroidenMaara(); i++)
                            {
                                // poistetaan uusista vaihtoehdoista jo koitetut numerot
                                uudetVaihtoehdot.PoistaNumero(testatutNumerot[rivi, sarake].NostaNumeroPaikasta(i));
                            }

                            // listataan vaihtoehdot debuggausta varten
                            //Console.WriteLine("Vaihtoehdot");
                            //uudetVaihtoehdot.ListaaNumerot();

                            if (uudetVaihtoehdot.NumeroidenMaara() <= 0) // mikäli vaihtoehtoja ei ole, jatketaan edelleen taaksepäin
                            {
                                liikutaanTaaksepain = true; // jep, pakki päälle
                                montakoAskeltaTaaksepain++;
                            }

                            else
                            {
                                liikutaanTaaksepain = false; // vaihtoehtoja löytyy, eli jatketaan eteenpäin
                                //Console.WriteLine("Palattiin yhteensä " + montakoAskeltaTaaksepain + " askelta taaksepäin.");
                                montakoAskeltaTaaksepain = 0;

                                // tyhjennetään testatutNumerot nykyisestä solusta + 1 -> eteenpäin
                                // tämä siksi, että kun ollaan palattu taaksepäin, niin nyt peli on tästä eteenpäin aivan erilainen

                                int seuraavaX;
                                int seuraavaY;

                                if (sarake < 8)
                                {
                                    seuraavaX = sarake + 1;
                                    seuraavaY = rivi;
                                }
                                else
                                {
                                    seuraavaX = 0;
                                    seuraavaY = rivi + 1;
                                }

                                // y = seuraavaY ja x = seuraavaX
                                // HOX!

                                for (int y = seuraavaY; y < 9; y++) // tyhjennetään yritykset
                                {
                                    for (int x = seuraavaX; x < 9; x++)
                                    {
                                        testatutNumerot[y, x].Tyhjenna();
                                    }
                                }

                            }

                        } while (liikutaanTaaksepain); // loopataan taaksepäin niin pitkään, että päästään ratkaisuun

                        // merkitään vielä, että ollaan liikuttu taaksepäin
                        liikuttuTaaksepain = true;

                    }

                    // Console.WriteLine(vaihtoehdot.NumeroidenMaara());
                }
            }


            return false;
        }
        #endregion

        public bool LuoPeliruudukko()
        {

            // Alustetaan aputaulukot

            rivit = new Pakka[9];
            sarakkeet = new Pakka[9];
            testatutNumerot = new Pakka[9, 9];
            pikkuGrid = new Pakka[3, 3];

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    pikkuGrid[y, x] = new Pakka(rnd);
                    pikkuGrid[y, x].Alusta();
                }
            }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    testatutNumerot[y, x] = new Pakka(rnd);
                }
            }

            for (int i = 0; i < 9; i++)
            {
                rivit[i] = new Pakka(rnd);
                rivit[i].Alusta();
                sarakkeet[i] = new Pakka(rnd);
                sarakkeet[i].Alusta();
            }

            // Luodaan varsinainen pelitaulukko
            peliruudukko = new int[9, 9];


            bool liikuttuTaaksepain = false;
            int montakoAskeltaTaaksepain = 0;


            for (int rivi = 0; rivi < 9; rivi++)
            {
                for (int sarake = 0; sarake < 9; sarake++)
                {

                    if (liikuttuTaaksepain) // mikäli ollaan edellisellä kierroksella liikuttu taaksepäin
                    {
                        int edellinenRivi, edellinenSarake;
                        if (sarake > 0)
                        {
                            edellinenSarake = sarake - 1;
                            edellinenRivi = rivi;
                        }
                        else
                        {
                            edellinenSarake = 8;
                            edellinenRivi = rivi - 1;
                        }
                        sarake = edellinenSarake;
                        rivi = edellinenRivi;
                    }

                    int gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
                    int gx = (int)Math.Floor((decimal)(sarake) / 3);

                    Pakka vaihtoehdot = LaskeVaihtoehdot(sarakkeet[sarake], rivit[rivi], pikkuGrid[gy, gx]); // Haetaan mahdolliset vaihtoehdot

                    if (vaihtoehdot != null) // Mikäli meillä on vaihtoehtoja
                    {
                        for (int i = 0; i < testatutNumerot[rivi, sarake].NumeroidenMaara(); i++)
                        {
                            // poistetaan vaihtoehdoista jo koitetut numerot
                            vaihtoehdot.PoistaNumero(testatutNumerot[rivi, sarake].NostaNumeroPaikasta(i));
                        }

                        //Console.WriteLine("Vaihtoehdot: ");
                        //vaihtoehdot.ListaaNumerot();

                        if (vaihtoehdot.NumeroidenMaara() > 0) // mikäli meille jäi vaihtoehtoja enemmän kuin vaihtoehdot - jo yritetyt numerot
                        {
                            int arvottuNumero = vaihtoehdot.NostaJaPoistaSatunnainen();

                            peliruudukko[rivi, sarake] = arvottuNumero; // Lisätään arvottu numero peliruudukkoon
                            testatutNumerot[rivi, sarake].Lisaa(arvottuNumero); // Lisätään arvottu numero kyseisen solun testattuihin numeroihin
                            rivit[rivi].PoistaNumero(arvottuNumero); // poistetaan arvottu numero kyseisen rivin pelattavissa olevista numeroista
                            sarakkeet[sarake].PoistaNumero(arvottuNumero); // ja sama sarakkeelle
                            pikkuGrid[gy, gx].PoistaNumero(arvottuNumero); // ja vielä 3x3 gridille

                            liikuttuTaaksepain = false; // eteenpäin ja täysiä!
                            // Console.WriteLine("Arvottiin rivi: " + rivi + " solu: " + sarake);
                            continue; // ei tarvetta käydä for-lausetta loppuun enää
                        }
                    }

                    if (vaihtoehdot == null || vaihtoehdot.NumeroidenMaara() == 0) // mikäli mahdollisia siirtoja ei ole, joudutaan palaamaan takaisin
                    {
                        bool liikutaanTaaksepain = false; // liikutaanko taaksepäin, oletetaan että ei

                        do
                        {
                            int edellinenRivi, edellinenSarake;

                            if (sarake > 0)
                            {
                                edellinenSarake = sarake - 1;
                                edellinenRivi = rivi;
                            }
                            else
                            {
                                edellinenSarake = 8;
                                edellinenRivi = rivi - 1;
                            }

                            // palataan edelliseen soluun
                            rivi = edellinenRivi;
                            sarake = edellinenSarake;

                            if (rivi < 0)
                            {
                                // ei löydetty ratkaisua ollenkaan näillä numeroilla, palautetaan false
                                return false;
                            }

                            //Console.WriteLine("Ei mahdollisia siirtoja, palataan taaksepäin soluun: rivi: " + rivi + " solu: " + sarake);

                            int edellinenNumero = peliruudukko[rivi, sarake]; // otetaan vanha numero talteen

                            peliruudukko[rivi, sarake] = 0; // poistetaan numero ruudukosta

                            // palautetaan numero takaisin riveille ja sarakkeille pelattavissa oleviin numeroihin
                            rivit[rivi].Lisaa(edellinenNumero);
                            sarakkeet[sarake].Lisaa(edellinenNumero);

                            gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
                            gx = (int)Math.Floor((decimal)(sarake) / 3);

                            pikkuGrid[gy, gx].Lisaa(edellinenNumero); // palautetaan numero myös 3x3 gridille


                            Pakka uudetVaihtoehdot = LaskeVaihtoehdot(sarakkeet[sarake], rivit[rivi], pikkuGrid[gy, gx]);

                            for (int i = 0; i < testatutNumerot[rivi, sarake].NumeroidenMaara(); i++)
                            {
                                // poistetaan uusista vaihtoehdoista jo koitetut numerot
                                uudetVaihtoehdot.PoistaNumero(testatutNumerot[rivi, sarake].NostaNumeroPaikasta(i));
                            }

                            // listataan vaihtoehdot debuggausta varten
                            //Console.WriteLine("Vaihtoehdot");
                            //uudetVaihtoehdot.ListaaNumerot();

                            if (uudetVaihtoehdot.NumeroidenMaara() <= 0) // mikäli vaihtoehtoja ei ole, jatketaan edelleen taaksepäin
                            {
                                liikutaanTaaksepain = true; // jep, pakki päälle
                                montakoAskeltaTaaksepain++;
                            }

                            else
                            {
                                liikutaanTaaksepain = false; // vaihtoehtoja löytyy, eli jatketaan eteenpäin
                                //Console.WriteLine("Palattiin yhteensä " + montakoAskeltaTaaksepain + " askelta taaksepäin.");
                                montakoAskeltaTaaksepain = 0;

                                // tyhjennetään testatutNumerot nykyisestä solusta + 1 -> eteenpäin
                                // tämä siksi, että kun ollaan palattu taaksepäin, niin nyt peli on tästä eteenpäin aivan erilainen

                                int seuraavaX;
                                int seuraavaY;

                                if (sarake < 8)
                                {
                                    seuraavaX = sarake + 1;
                                    seuraavaY = rivi;
                                }
                                else
                                {
                                    seuraavaX = 0;
                                    seuraavaY = rivi + 1;
                                }


                                for (int y = seuraavaY; y < 9; y++) // tyhjennetään yritykset
                                {
                                    for (int x = seuraavaX; x < 9; x++)
                                    {
                                        testatutNumerot[y, x].Tyhjenna();
                                    }
                                }

                            }

                        } while (liikutaanTaaksepain); // loopataan taaksepäin niin pitkään, että päästään ratkaisuun

                        // merkitään vielä, että ollaan liikuttu taaksepäin
                        liikuttuTaaksepain = true;

                    }

                    // Console.WriteLine(vaihtoehdot.NumeroidenMaara());
                }
            }

            return true; // saatiin luotua kenttä
        }

        public Pakka LaskeVaihtoehdot(Pakka sarakePakka, Pakka riviPakka, Pakka pikkugridi)
        {
            Pakka vaihtoehdot = new Pakka(rnd);

            for (int i = 1; i < 10; i++) // tsekataan numerot 1-9 ja verrataan niitä testattavaan sarakkeeseen, riviin ja 3x3 gridiin
            {
                if (sarakePakka.onkoPakassa(i) && riviPakka.onkoPakassa(i) && pikkugridi.onkoPakassa(i))
                {
                    vaihtoehdot.Lisaa(i);
                }
            }

            if (vaihtoehdot.NumeroidenMaara() > 0)
            {
                return vaihtoehdot;
            }

            else
                return null;
        }

        public Pakka LaskeVaihtoehdot(int rivi, int sarake)
        {
            Pakka vaihtoehdot = new Pakka(rnd);
            Pakka sarakePakka = sarakkeet[sarake];
            Pakka riviPakka = rivit[rivi];

            int gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
            int gx = (int)Math.Floor((decimal)(sarake) / 3);

            Pakka pikkugridi = pikkuGrid[gy, gx];

            for (int i = 1; i < 10; i++) // tsekataan numerot 1-9 ja verrataan niitä testattavaan sarakkeeseen, riviin ja 3x3 gridiin
            {
                if (sarakePakka.onkoPakassa(i) && riviPakka.onkoPakassa(i) && pikkugridi.onkoPakassa(i))
                {
                    vaihtoehdot.Lisaa(i);
                }
            }

            if (vaihtoehdot.NumeroidenMaara() > 0)
            {
                return vaihtoehdot;
            }

            else
                return null;
        }

        public void SijoitaNumero(int rivi, int sarake, int numero)
        {
            int gy = (int)(Math.Floor((decimal)(rivi) / 3));
            int gx = (int)Math.Floor((decimal)(sarake) / 3);

            pikkuGrid[gy, gx].PoistaNumero(numero);
            peliruudukko[rivi, sarake] = numero;

            rivit[rivi].PoistaNumero(numero);
            sarakkeet[sarake].PoistaNumero(numero);
        }

        public void PoistaSolu(int rivi, int sarake)
        {
            int edellinenNumero = peliruudukko[rivi, sarake];

            if (edellinenNumero != 0)
            {
                peliruudukko[rivi, sarake] = 0; // poistetaan numero ruudukosta

                // palautetaan numero takaisin riveille ja sarakkeille pelattavissa oleviin numeroihin
                rivit[rivi].Lisaa(edellinenNumero);
                sarakkeet[sarake].Lisaa(edellinenNumero);

                int gy = (int)(Math.Floor((decimal)(rivi) / 3)); // lasketaan missä 3x3 gridissä ollaan
                int gx = (int)Math.Floor((decimal)(sarake) / 3);

                pikkuGrid[gy, gx].Lisaa(edellinenNumero); // palautetaan numero myös 3x3 gridille
            }
        }

        #endregion

        public int haeNumero(int rivi, int sarake)
        {
            return peliruudukko[rivi, sarake];
        }
    }

}
