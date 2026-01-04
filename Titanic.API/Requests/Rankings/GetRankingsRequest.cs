using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetRankingsRequest : APIRequest<List<RankingEntryModel>>
    {
        public string Order { get; set; }
        public int Mode { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Country { get; set; }

        public GetRankingsRequest(string order, int mode, int offset = 0, int limit = 50, string country = null)
        {
            Order = order;
            Mode = mode;
            Offset = offset;
            Limit = limit;
            Country = country;
        }

        protected override List<RankingEntryModel> Execute(TitanicAPI api)
        {
            string endpoint = $"/rankings/{Order}/{Mode}?offset={Offset}&limit={Limit}";
            if (!string.IsNullOrEmpty(Country))
                endpoint += $"&country={Country}";
            return api.Get<List<RankingEntryModel>>(endpoint);
        }
    }
}
