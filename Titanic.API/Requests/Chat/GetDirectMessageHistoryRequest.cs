using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetDirectMessageHistoryRequest : APIRequest<List<PrivateMessageModel>>
    {
        public int TargetId { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public GetDirectMessageHistoryRequest(int targetId, int offset = 0, int limit = 50)
        {
            TargetId = targetId;
            Offset = offset;
            Limit = limit;
        }

        protected override List<PrivateMessageModel> Execute(TitanicAPI api)
        {
            return api.Get<List<PrivateMessageModel>>($"/chat/dms/{TargetId}/messages?offset={Offset}&limit={Limit}");
        }
    }
}
