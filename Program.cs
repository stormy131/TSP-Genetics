using System;
using Entities;
using Tools;
using GA_Solver.Constants;

namespace GA_Solver{
    internal class Program{
        private static List<Chromosome> current_generation = new List<Chromosome>();
        private static Random rnd = new Random();

        private static Chromosome generate_chromosome(){
            List<int> seq = Enumerable.Range(0, Map.cities_count).ToList();
            Helper.shuffle(seq);
            return new Chromosome(seq);
        }

        private static List<Chromosome> spawn_generation(){
            List<Chromosome> result = new List<Chromosome>();
            for(int i = 0; i < GlobalVars.POPULATION_COUNT; i++){
                result.Add(generate_chromosome());
            }

            return result;
        }

        private static Chromosome get_best_chromosome(List<Chromosome> generation){
            return generation.OrderBy(c => c.get_fitness()).First();
        }

        private static Chromosome tournament_selection(List<Chromosome> generation){
            Chromosome chrom_a = generation[rnd.Next(GlobalVars.POPULATION_COUNT)];
            List<Chromosome> gen_remainder = Helper.exclude(generation, chrom_a);
            Chromosome chrom_b = gen_remainder[rnd.Next(GlobalVars.POPULATION_COUNT - 1)];

            return chrom_a.get_fitness() > chrom_b.get_fitness() ? chrom_b : chrom_a;
        }

        static void breed(){
            List<Chromosome> next_gen = new List<Chromosome>();

            for(int i = 0; i < GlobalVars.POPULATION_COUNT; i++){
                Chromosome father = tournament_selection(current_generation);
                List<Chromosome> gen_remainder = Helper.exclude(current_generation, father);
                Chromosome mother = tournament_selection(gen_remainder);

                // Crossover
                // Mutation
            }

            current_generation = next_gen;
        }

        public static void Main(){
            while(Console.ReadLine() is string input){
                string[] args = input.Split(' ');
                Map.add_city(args[0], Int32.Parse(args[1]), Int32.Parse(args[2]));
            }

            current_generation = spawn_generation();
            for(int i = 0; i < GlobalVars.GENERATIONS_COUNT; i++){
                breed();
            }
        }
    }
}