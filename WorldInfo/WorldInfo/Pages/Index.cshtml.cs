using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Newtonsoft.Json;

namespace WorldInfo.Pages
{

    public class IndexModel : PageModel
    {
        public List<Country> countries { get; set; }
        public string SearchString { get; set; }

        public IndexModel()
        {
            countries = new List<Country>();
            var countryData = new CountryData();
            //var json = countryData.GenerateCountryData();
            var jsonIteration = new JsonIteration();
            countries = jsonIteration.NewCountry(countries);
            var dumtest = countries.Count();
        }
        public async void onGet()
        {
            //SearchString = searchItem;
        }
    }

    public class CountryData
    {
        public string GenerateCountryData()
        {
            string url = "https://restcountries.com/v3.1/all";
            string json = new WebClient().DownloadString(url);
            return json;
        }
    }

    public class JsonIteration
    {
        public string Json { get; set; }

        public JsonIteration()
        {
            Json = new CountryData().GenerateCountryData();
        }
        public List<Country> NewCountry(List<Country> _countries)
        {
            var jsonData = JsonConvert.DeserializeObject<dynamic>(Json);

            foreach (var item in jsonData)
            {
                Country _country = new Country();
                var countryName = item.name.common;
                _country.Name = countryName;
                var countryFlag = item.flags.svg;
                _country.Flag = countryFlag;
                var countryPopulation = item.population;
                _country.Population = countryPopulation;
                //var countryGini = item.gini[0];
                //_country.Gini = countryGini;
                var countryArea = item.area;
                _country.Area = countryArea;
                _countries.Add(_country);
            }
            return _countries;
        }
    }

    public class Country
    {
        public string? Name { get; set; }
        public string? Flag { get; set; }
        public int Population { get; set; }
        public int Gini { get; set; }
        public int Area { get; set; }
    }
}
