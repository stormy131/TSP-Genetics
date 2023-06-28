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
            List<int> indices = Helper.random_indices(generation, 2);
            Chromosome chrom_a = generation[indices[0]];
            Chromosome chrom_b = generation[indices[1]];

            return chrom_a.get_fitness() > chrom_b.get_fitness() ? chrom_a : chrom_b;
        }

        private static Chromosome crossover(Chromosome chrom_a, Chromosome chrom_b){
            int crossover_point = rnd.Next(1, GlobalVars.POPULATION_COUNT - 1);
            List<int> offspring_seq = chrom_a.perm.Take(crossover_point).ToList();
            HashSet<int> appeared = new HashSet<int>(offspring_seq);

            foreach(int gene in chrom_b.perm){
                if(!appeared.Contains(gene)) offspring_seq.Add(gene);
            }

            return new Chromosome(offspring_seq);
        }

        private static Chromosome swap_mutation(Chromosome original){
            List<int> seq = original.perm;
            List<int> indices = Helper.random_indices(seq, 2);
            
            int tmp = seq[indices[0]];
            seq[indices[0]] = seq[indices[1]];
            seq[indices[1]] = tmp;

            return new Chromosome(seq);
        }

        private static Chromosome rotate_mutation(Chromosome original){
            List<int> indices = Helper.random_indices(original.perm, 2);
            int i = indices.Min();
            int k = indices.Max();

            List<int> left = original.perm.Take(i).ToList();
            List<int> middle = original.perm.Skip(i).Take(k - i).ToList();
            middle.Reverse();
            List<int> right = original.perm.Skip(k).ToList();

            left.AddRange(middle);
            left.AddRange(right);
            return new Chromosome(left);
        }

        private static Chromosome mutate(Chromosome chrom){
            return rnd.NextDouble() < 0.5 ? swap_mutation(chrom) : rotate_mutation(chrom);
        }

        static void breed(){
            List<Chromosome> next_gen = new List<Chromosome>();

            for(int i = 0; i < GlobalVars.POPULATION_COUNT; i++){
                // SELECTION
                Chromosome father = tournament_selection(current_generation);
                List<Chromosome> gen_remainder = Helper.exclude(current_generation, father);
                Chromosome mother = tournament_selection(gen_remainder);

                // CROSSOVER
                Chromosome offspring_a = crossover(father, mother);
                Chromosome offspring_b = crossover(mother, father);

                // MUTATION
                if(rnd.NextDouble() < GlobalVars.MUTATION_CHANCE) offspring_a = mutate(offspring_a);
                if(rnd.NextDouble() < GlobalVars.MUTATION_CHANCE) offspring_b = mutate(offspring_b);

                next_gen.Add(offspring_a);
                next_gen.Add(offspring_b);
            }

            current_generation.AddRange(next_gen);
            current_generation = current_generation
                .OrderBy(c => c.get_fitness())
                .Take(GlobalVars.POPULATION_COUNT)
                .ToList();
        }

        public static void Main(){
            while(Console.ReadLine() is string input){
                string[] args = input.Split(' ');
                Map.add_city(args[0], Int32.Parse(args[1]), Int32.Parse(args[2]));
            }

            current_generation = spawn_generation();

            int count = 1;
            while(true){
                breed();
                System.Console.WriteLine($"GENERATION #{count} - {Math.Round(get_best_chromosome(current_generation).get_fitness())}");
                count += 1;
            }
        }
    }
}