using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sudoku
{
    public static class CSVReader
    {
        public static int[,] LueTiedostosta()
        {
            int[,] taulu = new int[9, 9];

            try
            {
                using (var reader = new StreamReader(@"esimerkkipeli1.csv"))
                {

                    int rivi = 0;

                    while (!reader.EndOfStream)
                    {

                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        for (int i = 0; i < values.Length; i++)
                        {
                            taulu[rivi, i] = int.Parse(values[i]);
                        }

                        rivi++;
                    }
                }

                return taulu;

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
