namespace Tools{
    class Helper{
        private static Random rnd = new Random();

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

        // public static string decode_path(List<int> seq){

        // }
    }
}