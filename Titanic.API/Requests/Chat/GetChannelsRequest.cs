using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetChannelsRequest : APIRequest<List<ChannelModel>>
    {
        public bool HasParticipated { get; set; }

        public GetChannelsRequest(bool hasParticipated = false)
        {
            HasParticipated = hasParticipated;
        }

        protected override List<ChannelModel> Execute(TitanicAPI api)
        {
            return api.Get<List<ChannelModel>>($"/chat/channels?has_participated={HasParticipated.ToString().ToLower()}");
        }
    }
}
