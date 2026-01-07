using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class BeatmapScoresRequest : APIRequest<List<ScoreModelWithoutBeatmap>>
    {
        public int BeatmapId { get; set; }

        public BeatmapScoresRequest(int beatmapId)
        {
            BeatmapId = beatmapId;
        }
        
        protected override List<ScoreModelWithoutBeatmap> Execute(TitanicAPI api)
        {
            return api.Get<List<ScoreModelWithoutBeatmap>>($"/beatmaps/{BeatmapId}/scores");
        }
    }
}
