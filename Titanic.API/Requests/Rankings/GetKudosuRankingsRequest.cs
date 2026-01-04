using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetKudosuRankingsRequest : APIRequest<List<RankingEntryModel>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }

        public GetKudosuRankingsRequest(int offset = 0, int limit = 50)
        {
            Offset = offset;
            Limit = limit;
        }

        protected override List<RankingEntryModel> Execute(TitanicAPI api)
        {
            return api.Get<List<RankingEntryModel>>($"/rankings/kudosu?offset={Offset}&limit={Limit}");
        }
    }
}
