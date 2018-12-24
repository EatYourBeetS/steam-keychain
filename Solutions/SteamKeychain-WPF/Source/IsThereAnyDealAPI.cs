using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace SteamKeychain
{
    public class IsThereAnyDealAPI : RestClient
    {
        private const string API_KEY = "45ef13963223687a8f41546046b8e282c1898547";

        public IsThereAnyDealAPI() : base("https://api.isthereanydeal.com/")
        {

        }

        public Result<GameIdentifier> SearchGame(string title)
        {
            // https://api.isthereanydeal.com/v02/game/plain/?key=45ef13963223687a8f41546046b8e282c1898547&shop=steam&title=sonic%20cd
            var request = new RestRequest("v02/game/plain/", Method.GET, DataFormat.Json);

            request.AddQueryParameter("key", API_KEY);
            request.AddQueryParameter("shop", "steam");
            request.AddQueryParameter("title", title);

            return ExecuteInternal<GameIdentifier>(request);
        }

        public Result<Dictionary<string, GamePrices>> SearchPrice(GameIdentifier id)
        {
            // https://api.isthereanydeal.com/v01/game/prices/?key=45ef13963223687a8f41546046b8e282c1898547&plains=soniccd%2Cendorlight&region=eu2&shops=steam
            var request = new RestRequest("v01/game/prices/", Method.GET, DataFormat.Json);

            request.AddQueryParameter("key", API_KEY);
            request.AddQueryParameter("shops", "steam");
            request.AddQueryParameter("region", "eu2");
            request.AddQueryParameter("plains", id.plain);

            return ExecuteInternal<Dictionary<string, GamePrices>>(request);
        }

        private Result<T> ExecuteInternal<T>(RestRequest request)
        {
            var response = Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<Result<T>>(response.Content);
            }

            return new Result<T>()
            {
                data = default,
                meta = null
            };
        }
    }

    [JsonObject]
    public class Result<Data>
    {
        [JsonProperty(".meta")]
        protected Meta jsonPropertyWrittenInATerribleFormat { get { return meta; } set { meta = value; } }

        [JsonProperty] public Meta meta { get; set; }
        [JsonProperty] public Data data { get; set; }
    }

    public class Meta
    {
        public string currency { get; set; }
        public string match { get; set; }
        public bool? active { get; set; }
    }

    public class GameIdentifier
    {
        public string plain { get; set; }
    }

    public class GamePrices
    {
        public GamePrice[] list { get; set; }
        public GameUrl urls { get; set; }
    }

    public class GameUrl
    {
        public string game { get; set; }
    }

    public class GamePrice
    {
        public float price_new { get; set; }
        public float price_old { get; set; }
        public int price_cut { get; set; }
        public string url { get; set; }
        public GameShop shop { get; set; }
        public string[] drm { get; set; }
    }

    public class GameShop
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}