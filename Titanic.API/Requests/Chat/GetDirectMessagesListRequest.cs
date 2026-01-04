using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetDirectMessagesListRequest : APIRequest<List<PrivateMessageModel>>
    {
        protected override List<PrivateMessageModel> Execute(TitanicAPI api)
        {
            return api.Get<List<PrivateMessageModel>>("/chat/dms");
        }
    }
}
