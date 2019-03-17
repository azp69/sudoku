using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku
{
    public class Pakka
    {
        private List<int> _pakka;
        private Random rnd;


        public Pakka(Random randomi)
        {
            //rnd = new Random();
            this.rnd = randomi;
            this._pakka = new List<int>();
        }

        public void Alusta()
        {
            this._pakka = new List<int>();

            for (int i = 1; i < 10; i++)
            {
                Lisaa(i);
            }
        }

        public int Nosta()
        {
            return _pakka.ElementAt(_pakka.Count);
        }

        public void ListaaNumerot()
        {
            foreach (int n in this._pakka)
            {
                Console.Write(n + " ");
            }
            Console.WriteLine();
        }

        public int NostaNumeroPaikasta(int indeksi)
        {
            return this._pakka.ElementAt(indeksi);
        }

        public int NostaJaPoista()
        {
            int n = _pakka.ElementAt(_pakka.Count -1);
            _pakka.RemoveAt(_pakka.Count -1);
            return n;
        }

        public int NostaJaPoistaSatunnainen()
        {
            if (_pakka.Count > 0)
            {
                int rand = this.rnd.Next(0, _pakka.Count);
                int n = _pakka.ElementAt(rand);
                _pakka.RemoveAt(rand);
                return n;
            }
            else
                return 0;
        }

        public void Lisaa(int numero)
        {
            _pakka.Add(numero);
        }

        public void PoistaNumero(int numero)
        {
            for (int i = 0; i < _pakka.Count; i++)
            {
                if (_pakka.ElementAt(i) == numero)
                    _pakka.RemoveAt(i);
            }
        }

        public bool onkoPakassa(int numero)
        {
            foreach (int n in _pakka)
            {
                if (n == numero)
                    return true;
            }

            return false;
        }

        public int NumeroidenMaara()
        {
            return this._pakka.Count();
        }

        public void Tyhjenna()
        {
            this._pakka.Clear();
        }
    }
}
