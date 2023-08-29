using Entities;
using GA_Solver.Consts;

namespace Tools{
    class Helper{
        private static Random rnd = new Random();

        public static double get_dist((int, int) from, (int, int) to){
            int dx = to.Item1 - from.Item1;
            int dy = to.Item2 - from.Item2;
            return Math.Sqrt(dx*dx + dy*dy);
        }

        public static void shuffle<T>(List<T> list){
            for(int i = list.Count - 1; i >= 0; i--){
                int j = rnd.Next(i + 1);
                T value = list[j];
                list[j] = list[i];
                list[i] = value;
            }
        }

        public static List<T> exclude<T>(List<T> list, T target){
            List<T> new_list = new List<T>();
            list.ForEach((element) => {
                if(!list.Equals(target)) new_list.Add(element);
            });

            return new_list;
        }

        // GET N RANDOM INDECIES FROM RANGE OF GIVEN LIST
        public static List<int> random_indices<T>(List<T> list, int n){
            List<int> res = new List<int>();
            while(n > 0){
                int i = rnd.Next(list.Count);
                res.Add(i);
                list = exclude(list, list[i]);
                n--;
            }

            return res;
        }

        public static string decode_path(List<int> seq){
            IEnumerable<string> cities = seq.Select(c => Map.city_name(c));
            return String.Join("->", cities);
        }

        // GENERATE RANDOM CITY INPUT, BASED ON REQUIRED NUMBER OF CITIES
        public static void input_generator(int city_count){
            for(int i = 0; i < city_count; i++){
                Map.add_city(
                    Convert.ToString(i),
                    (int) (rnd.NextDouble() * Config.MAP_WIDTH),
                    (int) (rnd.NextDouble() * Config.MAP_HEIGHT)
                );
            }
        }

        // GENERATE MATRIX OF DISTANCES BETWEEN EACH PAIR OF CITIES [FOR CORRECTNESS CHECK]
        // RESULT[I,J] - SHOWS DISTANCE BETWEEN CITIES WITH INDECIES I AND J
        public static int[,] generate_dist_matrix(){
            int[,] dists = new int[Map.cities_count, Map.cities_count];

            for(int i = 0; i < Map.cities_count; i++){
                (int, int) city_a = Map.get_coords(i);
                for(int j = i + 1; j < Map.cities_count; j++){
                    (int, int) city_b = Map.get_coords(j);

                    int dx = city_a.Item1 - city_b.Item1;
                    int dy = city_a.Item2 - city_b.Item2;
                    int d = (int) Math.Sqrt(dx*dx + dy*dy);

                    dists[i,j] = d;
                    dists[j,i] = d;
                }
            }

            return dists;
        }
    }
}