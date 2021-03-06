﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sudoku
{
    public partial class FrmPelilauta : Form
    {
        sudokuMoottori Pelimoottori;
        public List<Button> napit = new List<Button>();

        public FrmPelilauta()
        {
            InitializeComponent();
            Pelimoottori = new sudokuMoottori();

            foreach (Control c in tableLayoutPanel1.Controls) // tehdään napeista lista, niin helpompi käsitellä
            {
                if (c.GetType() == typeof(Button))
                {
                    napit.Add((Button)c);
                }
            }
        }



        private void valitseNumero(object sender, EventArgs e)
        {
            Button nappi = (Button)sender;

            int sarakeNro = tableLayoutPanel1.GetColumn(nappi);
            int riviNro = tableLayoutPanel1.GetRow(nappi);

            for (int i = 0; i < 9; i++)
            {
                if (i > 2 && i < 6)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tableLayoutPanel1.GetControlFromPosition(k, i).BackColor = SystemColors.ControlLightLight;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        tableLayoutPanel1.GetControlFromPosition(k+3, i).BackColor = Color.Gainsboro;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        tableLayoutPanel1.GetControlFromPosition(k+6, i).BackColor = SystemColors.ControlLightLight;
                    }
                }
                else
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Control c = tableLayoutPanel1.GetControlFromPosition(k, i);
                        c.BackColor = Color.Gainsboro;
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        tableLayoutPanel1.GetControlFromPosition(k+3, i).BackColor = SystemColors.ControlLightLight;
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        tableLayoutPanel1.GetControlFromPosition(k+6, i).BackColor = Color.Gainsboro;
                    }
                }
            }

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                tableLayoutPanel1.GetControlFromPosition(sarakeNro, i).BackColor = Color.Silver;
            }
            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                tableLayoutPanel1.GetControlFromPosition(i, riviNro).BackColor = Color.Silver;
            }



            if (nappi.Text == "") // mikäli nappi on tyhjä, eli ei ole numeroa, niin avataan numerovalitsin
            {
                FrmNumerovalitsin valitse = new FrmNumerovalitsin(nappi, Pelimoottori);
                valitse.Show(); // avataan numerovalitsin
            }
            else // tyhjennetään valinta
            {
                int numero = int.Parse(nappi.Name.Remove(0, 6));
                int rivi = (numero / 9);
                int sarake = (numero - (numero / 9) * 9);
                Pelimoottori.PoistaSolu(rivi, sarake);
                nappi.Text = "";
            }
        }

        private void FrmPelilauta_Load(object sender, EventArgs e)
        {

        }

        private void BtnUusiPeli_Click(object sender, EventArgs e)
        {
            Pelimoottori.LuoPeli();

            int numero = 0;

            for (int rivi = 0; rivi < 9; rivi++) // haetaan nappeihin arvot (numerot) pelimoottorista
            {
                for (int sarake = 0; sarake < 9; sarake++)
                {
                    napit[numero++].Text = Pelimoottori.haeNumero(rivi, sarake).ToString();
                }
            }
        }
    }
}
