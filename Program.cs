using System;
using Entities;
using Tools;
using GA_Solver.Consts;

namespace GA_Solver{
    internal class Program{
        private static List<Chromosome> current_generation = new List<Chromosome>();
        private static Random rnd = new Random();
        private static int gen_counter = 0;
        private static int no_impr_counter = 0;
        private static double previous_best = int.MaxValue;

        private static Chromosome generate_chromosome(){
            List<int> seq = Enumerable.Range(0, Map.cities_count).ToList();
            Helper.shuffle(seq);
            return new Chromosome(seq);
        }

        private static List<Chromosome> spawn_generation(){
            List<Chromosome> result = new List<Chromosome>();
            for(int i = 0; i < Config.POPULATION_COUNT; i++){
                result.Add(generate_chromosome());
            }

            return result;
        }

        private static Chromosome get_best_chromosome(List<Chromosome> generation){
            return generation.OrderBy(c => c.get_fitness()).First();
        }

        private static Chromosome wheel_selection(List<Chromosome> generation){
            // FITNESS CONVERSION
            double fit_sum = generation.Sum(ch => ch.get_fitness());
            List<double> probability_proportions = generation.Select(ch => fit_sum / ch.get_fitness()).ToList();
            double prob_sum = probability_proportions.Sum();
            List<double> normalized_probabilities = probability_proportions.Select(p => p / prob_sum).ToList();

            // ACCUMULATING PROBABILITIES
            double accumulator = 0;
            List<double> acc_probabilities = new List<double>();
            for(int i = 0; i < normalized_probabilities.Count(); i++){
                accumulator += normalized_probabilities[i];
                acc_probabilities.Add(accumulator);
            }

            // "WHEEL-SPIN"
            double rnd_value = rnd.NextDouble();
            for(int i = 0; i < acc_probabilities.Count; i++){
               if(rnd_value <= acc_probabilities[i]) return generation[i];
            }

            throw new Exception("Probabilities-distrib error");
        }

        private static Chromosome crossover(Chromosome chrom_a, Chromosome chrom_b){
            int crossover_point = rnd.Next(1, Config.POPULATION_COUNT - 1);
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

            for(int i = 0; i < Config.POPULATION_COUNT; i++){
                // SELECTION
                Chromosome father = wheel_selection(current_generation);
                List<Chromosome> gen_remainder = Helper.exclude(current_generation, father);
                Chromosome mother = wheel_selection(gen_remainder);

                // CROSSOVER
                Chromosome offspring_a = crossover(father, mother);
                Chromosome offspring_b = crossover(mother, father);

                // MUTATION
                if(rnd.NextDouble() < Config.MUTATION_CHANCE) offspring_a = mutate(offspring_a);
                if(rnd.NextDouble() < Config.MUTATION_CHANCE) offspring_b = mutate(offspring_b);

                next_gen.Add(offspring_a);
                next_gen.Add(offspring_b);
            }

            current_generation.AddRange(next_gen);
            current_generation = current_generation
                .OrderBy(c => c.get_fitness())
                .Take(Config.POPULATION_COUNT)
                .ToList();
        }

        public static void Main(string[] args){
            // INPUT PROCESSING
            if(args.Length > 0){
                if(args.Length > 1) throw new Exception("Incorrect commandline arguments (number of them)");
                try{
                    Helper.input_generator(Convert.ToInt32(args[0]));
                } catch {
                    throw new Exception("Incorrect commandline arguments (type)"); 
                }

                for(int i = 0; i < Map.cities_count; i++){
                    (int, int) coords = Map.get_coords(i);
                    System.Console.WriteLine($"City {i} | ({coords.Item1} {coords.Item2})");
                }
            } else {
                while(Console.ReadLine() is string input){
                    string[] city_values = input.Split(' ');
                    Map.add_city(city_values[0], Int32.Parse(city_values[1]), Int32.Parse(city_values[2]));
                }
            }

            current_generation = spawn_generation();
            while(gen_counter < Config.GENERATIONS_COUNT){
                breed();
                gen_counter += 1;

                if(no_impr_counter == Config.NO_IMPROVEMENT_GEN) break;
                else {
                    double current_best = current_generation.First().get_fitness();
                    if(current_best < previous_best){
                        no_impr_counter = 0;
                        previous_best = current_best;
                    } else no_impr_counter++;
                }

                System.Console.WriteLine($"GENERATION #{gen_counter} - {Math.Round(get_best_chromosome(current_generation).get_fitness())}");
            }

            Chromosome solution = get_best_chromosome(current_generation);
            System.Console.WriteLine($"\nFound solution - {solution.get_fitness()}");
            System.Console.WriteLine(Helper.decode_path(solution.perm));

            // TESTING TOOL
            // int[,] dists = Helper.generate_dist_matrix();
            // for(int i = 0; i < Map.cities_count; i++){
            //     for(int j = 0; j < Map.cities_count; j++){
            //         System.Console.Write($"{dists[i,j]} ");
            //     }
            //     System.Console.WriteLine();
            // }
        }
    }
}