namespace Tools{
    class Helper{
        private static Random rng = new Random();

        public static void shuffle<T>(List<T> list){
            for(int i = list.Count - 1; i >= 0; i--){
                int j = rng.Next(i + 1);
                T value = list[j];
                list[j] = list[i];
                list[i] = value;
            }
        }
    }
}