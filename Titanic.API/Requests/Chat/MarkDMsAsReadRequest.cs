using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class MarkDMsAsReadRequest : APIRequest<List<PrivateMessageModel>>
    {
        public int TargetId { get; set; }

        public MarkDMsAsReadRequest(int targetId)
        {
            TargetId = targetId;
        }

        protected override List<PrivateMessageModel> Execute(TitanicAPI api)
        {
            return api.Post<List<PrivateMessageModel>>($"/chat/dms/{TargetId}/messages/read", null);
        }
    }
}
