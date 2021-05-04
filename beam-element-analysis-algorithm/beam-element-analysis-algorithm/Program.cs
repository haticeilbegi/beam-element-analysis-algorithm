using System;
using System.IO;

namespace beam_element_analysis_algorithm
{
    class Program
    {
        // FINITE ELEMENT METHOD - BEAM ELEMENT ALAYSIS ALGORITHM

        static void Main(string[] args)
        {
            //TOPOLOJI MATRISININ BOYUTLARI
            var topology = new int[3, 6]
            {
                { 1,   2,   3,   4,   5,   6 },
                { 4,   5,   6,   7,   8,   9 },
                { 7,   8,   9,   10,  11,  12 }
            };

            var KGSize = 12; // TOPOLOJI MATRISININ BOYUTU (12x12)

            // ELEMAN SAYISI
            int eleman = 3;

            int z = 1200;

            // A, L, I, E Değerleri (sırasıyla)
            var variables = new decimal[eleman][];
            variables[0] = new decimal[] { 6300, 4050, 4252500, 200000 };
            variables[1] = new decimal[] { 6300, 6750, 4252500, 117000 };
            variables[2] = new decimal[] { 6300, 2700, 4252500, 68000 };

            var matrises = new decimal[eleman, 6, 6]; // STIFFNESS MATRIS BOYUTU

            for (int i = 0; i < eleman; i++)
            {
                decimal A = 0, L = 0, I = 0, E = 0; // yeni değişkeni buraya gir
                A = variables[i][0];
                L = variables[i][1];
                I = variables[i][2];
                E = variables[i][3]; // Değişken sayısı artınca [i][3] 3'ü arttır, 

                var formuleArray = new decimal[6, 6]
                {
                    { A * (L * L),    0,            0,                  A * (L * L),    0,             0               },
                    { 0,              12 * I,       6 * L * I,          0,              -12 * I,       6 * L * I       },
                    { 0,              6 * L * I,    4 * (L * L) * I,    0,              -6 * L * I,    2 * (L * L) * I },
                    { A * (L * L),    0,            0,                  A * (L * L),    0,             0               },
                    { 0,              -12 * I,      -6 * L * I,         0,              12 * I,        -6 * L * I      },
                    { 0,              6 * L * I,    2 * (L * L) * I,    0,              0,             4 * (L * L) * I }
                };

                for (int k = 0; k < 6; k++)
                    for (int l = 0; l < 6; l++)
                        matrises[i, k, l] = E / (L * L * L) * formuleArray[k, l]; // Matrisin başındaki kat sayısı iptal edeceksen EL^3'ü sil
            }

            var KG = new decimal[KGSize, KGSize];
            for (int k = 0; k < 3; k++)
            {
                for (int l = 0; l < 6; l++)
                {
                    for (int m = 0; m < 6; m++)
                    {
                        KG[(topology[k, l] - 1), (topology[k, m] - 1)] =
                            KG[(topology[k, l] - 1), (topology[k, m] - 1)] +
                            matrises[k, l, m];
                    }
                }
            }

            for (int j = 0; j < KGSize; j++)
            {
                for (int k = 0; k < KGSize; k++)
                {
                    Console.Write(KG[j, k].ToString("0.000") + "\t");
                }
                Console.WriteLine();
            }

            Console.Write("\n\nSonuç matrisini kaydetmek istiyor musunuz (y/n): ");
            var save = Console.ReadLine();
            if (save.ToLower() == "y")
            {
                var fileName = @"D:\matris";
                using (StreamWriter file = new StreamWriter($"{fileName}.xls"))
                {
                    for (int i = 0; i < KGSize; i++)
                    {
                        for (int j = 0; j < KGSize; j++)
                        {
                            file.Write($"{KG[i, j].ToString("0.000")}\t");
                        }
                        file.Write(Environment.NewLine);
                    }
                }

                Console.WriteLine($"\n\nKG matrisi \"{fileName}.xls\" kaydedildi.\n\n");
            }

            Console.WriteLine("Kapatmak için bir tuşa bas");

            Console.ReadKey();
        }
    }
}
