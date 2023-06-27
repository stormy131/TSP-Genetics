using System;
using Entities;
using Tools;
using Program.Constants;

namespace Solver{
    internal class Program{
        static List<Chromosome> current_generation = new List<Chromosome>();

        static Chromosome generate_chromosome(){
            List<int> seq = Enumerable.Range(0, 10).ToList();
            Helper.shuffle(seq);
            return new Chromosome(seq);
        }

        static Chromosome get_best_chromosome(){
            return current_generation.OrderBy(c => c.get_fitness()).First();
        }

        static List<Chromosome> spawn_generation(){
            List<Chromosome> result = new List<Chromosome>();
            for(int i = 0; i < GlobalVars.POPULATION_COUNT; i++){
                result.Add(generate_chromosome());
            }

            return result;
        }

        static void Main(){
            while(Console.ReadLine() is string input){
                string[] args = input.Split(' ');
                Map.add_city(args[0], (Int32.Parse(args[0])), Int32.Parse(args[1]));
            }

            current_generation = spawn_generation(); 
        }
    }
}