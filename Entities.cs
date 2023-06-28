namespace Entities{
    class Chromosome{
        public List<int> perm {get; set;}

        public Chromosome(List<int> seq){
            this.perm = seq;
        }

        public double get_fitness(){
            double distance = 0;

            for(int i = 1; i < perm.Count; i++){
                (int, int) from = Map.get_coords(perm[i - 1]);
                (int, int) to = Map.get_coords(perm[i]);

                int dx = to.Item1 - from.Item1;
                int dy = to.Item2 - from.Item2;
                distance += Math.Sqrt(dx*dx + dy*dy);
            }

            return distance;
        }

        public override string ToString(){
            string res = "";
            for(int i = 0; i < perm.Count; i++){
                res += Convert.ToString(perm[i]);
            }

            return res;
        }

        public override bool Equals(Object? obj){
            if ((obj == null) || !this.GetType().Equals(obj.GetType())){
                return false;
            } else {
                Chromosome ch = (Chromosome) obj;

                if(ch.perm.Count != this.perm.Count) return false;
                for(int i = 0; i < perm.Count; i++){
                    if(perm[i] != ch.perm[i]) return false;
                }

                return true;
            }
        }
    }

    static class Map{
        static private Dictionary<string, (int, int)> cities = new Dictionary<string, (int, int)>();

        static public void add_city(string name, int x, int y){
            cities.Add(name, (x, y));
        }

        static public (int, int) get_coords(int city_index){
            List<string> names = new List<string>(cities.Keys);
            return cities[names[city_index]];
        }

        static public int cities_count {
            get => cities.Keys.Count;
        }
    }
}