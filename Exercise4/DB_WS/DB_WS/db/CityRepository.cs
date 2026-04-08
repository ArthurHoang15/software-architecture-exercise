using System.Collections.Generic;
using DB_WS.entities;
using MySqlConnector;
namespace DB_WS.db
{
    public class CityRepository
    {
        private MySqlConnection connection;
        public CityRepository()
        {
            connection = ConnectDB.getInstance();
        }
        public List<City> getAllCities()
        {
            connection.Open();
            string sql = "select * from city";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<City> cities = new List<City>();
            while (rdr.Read())
            {
                City city = new City(
                int.Parse(rdr[0].ToString()),
                rdr[1].ToString(), rdr[2].ToString(), rdr[3].ToString(),
                int.Parse(rdr[4].ToString())
                );
                cities.Add(city);
            }
            connection.Close();
            return cities;
        }
    }
}