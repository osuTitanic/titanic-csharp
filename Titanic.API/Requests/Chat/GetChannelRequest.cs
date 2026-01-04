using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetChannelRequest : APIRequest<ChannelModel>
    {
        public string Target { get; set; }

        public GetChannelRequest(string target)
        {
            Target = target;
        }

        protected override ChannelModel Execute(TitanicAPI api)
        {
            return api.Get<ChannelModel>($"/chat/channels/{Target}");
        }
    }
}
