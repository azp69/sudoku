using System;
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
    public partial class FrmNumerovalitsin : Form
    {
        public Button _nappi;
        public sudokuMoottori _moottori;
        public List<Button> napit = new List<Button>();


        public FrmNumerovalitsin()
        {
            InitializeComponent();
        }

        public FrmNumerovalitsin(Button nappi, sudokuMoottori moottori)
        {
            InitializeComponent();
            this._nappi = nappi;
            this._moottori = moottori;

            foreach (Control c in tableLayoutPanel1.Controls) // tehdään napeista lista, niin helpompi käsitellä
            {
                if (c.GetType() == typeof(Button))
                {
                    napit.Add((Button)c);
                }
            }

            
            int numero = int.Parse(this._nappi.Name.Remove(0, 6));
            int rivi = (numero / 9);
            int sarake = (numero - (numero / 9) * 9);

            Pakka vaihtoehdot = this._moottori.LaskeVaihtoehdot(rivi, sarake);

            foreach (Button b in napit)
            {
                if (vaihtoehdot != null)
                {
                    for (int i = 0; i < vaihtoehdot.NumeroidenMaara(); i++)
                    {
                        if (vaihtoehdot.onkoPakassa(int.Parse(b.Text)))
                        {
                            b.Enabled = true;
                        }
                        else
                        {
                            b.Enabled = false;
                        }
                    }
                }
                else
                {
                    b.Enabled = false;
                }
            }


        }

        private void valitseNumero(object sender, EventArgs e)
        {
            Button valittunumero = (Button)sender;
            int solu = int.Parse(this._nappi.Name.Remove(0, 6));

            int numero = int.Parse(valittunumero.Text);

            int rivi = (solu / 9);
            int sarake = (solu - (solu / 9) * 9);

            this._moottori.SijoitaNumero(rivi, sarake, numero);

            this._nappi.Text = valittunumero.Text;
            this.Close();
        }
    }
}
