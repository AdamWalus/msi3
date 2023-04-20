using System;
using System.Linq;

namespace KeyboardOptimization
{
    class Program
    {
        static Random random = new Random();
        static int Populacja = 100;



        static double CrossoverRate = 0.8;
        static double MutationRate = 0.01;
        static int Generacje = 35;

        static int Keys1 = 26;
        static string[] Keys = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };




        static double[] CzestKlawiszy = new double[]
        {
            0.08167, 0.01492, 0.02782, 0.04253, 0.12702, 0.02228, 0.02015, 0.06094, 0.06966, 0.00153, 0.00772, 0.04025, 0.02406, 0.06749, 0.07507, 0.01929, 0.00095, 0.05987, 0.06327, 0.09056, 0.02758, 0.00978, 0.02360, 0.00150, 0.01974, 0.00074
        };

        static double[][] OdstepOdKlawiszy;

        static void Main(string[] args)
        {
           



            OdstepOdKlawiszy = ObliczOdstepOdKlawiszy();

            int[][] population = InitializePopulation();

            for (int generation = 0; generation < Generacje; generation++)
            {
                double[] fitness = ObliczFitness(population);
                int[][] newPopulation = new int[Populacja][];

                newPopulation = Enumerable.Range(0, Populacja)
                            .Select(_ =>
                            {
                                int[] parent1 = Selection(population, fitness);
                                int[] parent2 = Selection(population, fitness);

                                int[] offspring;

                                if (random.NextDouble() < CrossoverRate)
                                {
                                    offspring = OrderCrossover(parent1, parent2);
                                }
                                else
                                {
                                    offspring = parent1.ToArray();
                                }

                                Mutacja(offspring);
                                return offspring;
                            })
                            .ToArray();


                int bestIndex = Array.IndexOf(fitness, fitness.Min());
                int[] bestIndividual = population[bestIndex];
                Console.WriteLine($"Generacja: {generation + 1}, Ustawienie: {string.Concat(bestIndividual.Select(x => Keys[x]))}, Calkowity koszt: {fitness[bestIndex]}");

                population = newPopulation;
            }
            
        }

        static int[][] InitializePopulation()
        {
            return Enumerable.Range(0, Populacja)
                .Select(_ => Enumerable.Range(0, Keys1)
                    .OrderBy(x => random.Next())
                    .ToArray())
                .ToArray();
        }


        static double[] ObliczFitness(int[][] population)
        {
            return population.Select(individual => ObliczKoszt(individual)).ToArray();
        }

        static int[] Selection(int[][] population, double[] fitness)
        {
            int index1 = random.Next(Populacja);
            int index2 = random.Next(Populacja);

            int[] test1 = population[index1];
            int[] test2 = population[index2];

            return fitness[index1] <= fitness[index2] ? test1 : test2;
        }


        static int[] OrderCrossover(int[] parent1, int[] parent2)
        {
            int[] offspring = new int[Keys1];

            // wybierz losowo dwa punkty krzyżowania
            int crossoverPoint1 = random.Next(1, Keys1 - 2);
            int crossoverPoint2 = random.Next(crossoverPoint1, Keys1 - 1);

            // skopiuj elementy z pierwszego rodzica między punktami krzyżowania
            for (int i = crossoverPoint1; i < crossoverPoint2; i++)
            {
                offspring[i] = parent1[i];
            }

            // skopiuj pozostałe elementy z drugiego rodzica, zachowując kolejność występowania
            int index = 0;
            foreach (int gene in parent2)
            {
                if (!offspring.Contains(gene))
                {
                    if (index == crossoverPoint1)
                    {
                        index = crossoverPoint2;
                    }

                    offspring[index++] = gene;
                }
            }

            return offspring;
        }







        static void Mutacja(int[] individual)
        {
            int mutationPoint1 = random.Next(Keys1);
            int mutationPoint2 = random.Next(Keys1);

            (individual[mutationPoint1], individual[mutationPoint2]) = (individual[mutationPoint2], individual[mutationPoint1]);
        }

        static double ObliczKoszt(int[] layout)
        {
            double Koszt = 0;

            for (int i = 0; i < Keys1; i++)
            {
                for (int j = 0; j < Keys1; j++)
                {
                    Koszt += CzestKlawiszy[i] * CzestKlawiszy[j] * OdstepOdKlawiszy[layout[i]][layout[j]];
                }
            }

            return Koszt;
        }
        static double[][] ObliczOdstepOdKlawiszy()
        {
            string[] QWERTY = new string[]
            {
        "1234567890",
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM"
            };

            int[] keyCoordsX = new int[Keys1];  
            int[] keyCoordsY = new int[Keys1];

            for (int i = 0; i < QWERTY.Length; i++)
            {
                for (int j = 0; j < QWERTY[i].Length; j++)
                {
                    int index = QWERTY[i][j] - 'A';

                    if (index >= 0 && index < Keys1)
                    {
                        keyCoordsX[index] = j;
                        keyCoordsY[index] = i;
                    }
                }
            }

            double[][] distances = Enumerable.Range(0, Keys1)
            .Select(i => Enumerable.Range(0, Keys1)
            .Select(j => Math.Sqrt(Math.Pow(keyCoordsX[i] - keyCoordsX[j], 2) + Math.Pow(keyCoordsY[i] - keyCoordsY[j], 2)))
            .ToArray())
            .ToArray();

            return distances;
        }

    }
}