namespace DB_WS.entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string District { get; set; }
        public int Population { get; set; }
        public City() { }
        public City(int id, string name, string code, string district, int population)
        {
            Id = id;
            Name = name;
            CountryCode = code;
            District = district;
            Population = population;
        }
        public override bool Equals(object obj)
        {
            City country = obj as City;
            return Id == country.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}