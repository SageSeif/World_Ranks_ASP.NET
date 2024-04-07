using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorldInfo.Pages
{
    public class CountryDetailsModel : PageModel
    {
        public CountryDetailsModel()
        {
        }
        public FullCountry? CountryData { get; set; }
        public async Task OnGetAsync(string  name)
        {
            if(!string.IsNullOrEmpty(name))
            {
                string url = $"https://restcountries.com/v3.1/name/{name}";
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(json);
                        CountryData = new FullCountry();
                        CountryData.Name = jsonData[0].name.common; // No need for null check, as the name is always present
                        CountryData.Flag = jsonData[0].flags.svg; // No need for null check, as the flag is always present
                        CountryData.Population = jsonData[0].population; // No need for null check, as the population is always present
                        CountryData.Area = jsonData[0].area; // No need for null check, as the area is always present
                        CountryData.Continent = jsonData[0].region; // No need for null check, as the region is always present

                        // Need to check for capital if null or not
                        if (jsonData[0].capital != null)
                        {
                            CountryData.Capital = jsonData[0].capital[0];
                        }
                        else
                        {
                            CountryData.Capital = "This Country has no Capital";
                        }

                        // Need to check for currency if null or not
                        if (jsonData[0].currencies != null)
                        {
                            JObject currencyObj = jsonData[0]["currencies"];
                            foreach (JProperty currency in currencyObj.Properties())
                            {
                                string currencyName = ((JObject)currency.Value)["name"].ToString();

                                CountryData.Currency = currencyName;

                                break;
                            }
                        }
                        else
                        {
                            CountryData.Currency = "This Country has no Currency";
                        }
                        if (jsonData[0].languages != null)
                        {
                            JObject languageObj = jsonData[0]["languages"];
                            foreach (var item in languageObj.Properties())
                            {
                                string languageName = item.Value.ToString();
                                CountryData.Language.Add(languageName);
                            }
                        }
                        else
                        {
                            CountryData.Language.Add("This Country has no Language");
                        }
                        if (jsonData[0].borders != null)
                        {
                            foreach (var item in jsonData[0].borders)
                            {
                                string neighbourBorder = "https://restcountries.com/v3.1/alpha/";
                                neighbourBorder += item;
                                var neighbourResponse = await httpClient.GetAsync(neighbourBorder);
                                if (neighbourResponse.IsSuccessStatusCode)
                                {
                                    var neighbourJson = await neighbourResponse.Content.ReadAsStringAsync();
                                    var neighbourData = JsonConvert.DeserializeObject<dynamic>(neighbourJson);
                                    CountryData.Borders.Add((neighbourData[0].flags.svg).ToString());
                                    CountryData.Borders.Add((neighbourData[0].name.common).ToString());
                                }
                            }
                        }
                        else
                        {
                            CountryData.Borders.Add("This Country has no Borders");
                        }
                    }
                }
            }
        }
    }

    public class FullCountry
    {
        public dynamic Name { get; set; }
        public dynamic Flag { get; set; }
        public dynamic Population { get; set; }
        public dynamic Area { get; set; }
        public dynamic Continent { get; set; }
        public dynamic Capital { get; set; }
        public dynamic Currency { get; set; }
        public List<dynamic> Language { get; set; }
        public dynamic NativeName { get; set; }
        public List<dynamic> Borders { get; set; }

        public FullCountry()
        {
            Language = new List<dynamic>();
            Borders = new List<dynamic>();
        }
    }
}
