using DB_WS.db;
using DB_WS.entities;
using System.Collections.Generic;
using System.Web.Services;
namespace DB_WS.api
{
 /// <summary>
 /// Summary description for CityWS
 /// </summary>
 [WebService(Namespace = "http://tempuri.org/")]
 [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
 [System.ComponentModel.ToolboxItem(false)]
 // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the
following line.
 // [System.Web.Script.Services.ScriptService]
 public class CityWS : System.Web.Services.WebService
 {
 [WebMethod]
 public List<City> getAllCities()
 {
 CityRepository countryRepository = new CityRepository();
 return countryRepository.getAllCities();
 }
 }
}