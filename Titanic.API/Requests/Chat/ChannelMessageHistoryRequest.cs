using System;
using System.Collections.Generic;
using System.Text;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class ChannelMessageHistoryRequest : APIRequest<List<MessageModel>>
    {
        private string ChannelEncoded => Uri.EscapeDataString(Channel);
        public string Channel { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public ChannelMessageHistoryRequest(string channel, int offset = 0, int limit = 50)
        {
            Channel = channel;
            Offset = offset;
            Limit = limit;
        }

        protected override List<MessageModel> Execute(TitanicAPI api)
        {
            return api.Get<List<MessageModel>>($"/chat/channels/{ChannelEncoded}/messages?offset={Offset}&limit={Limit}", null);
        }
    }
}
