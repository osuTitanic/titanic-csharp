using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetSubscriptionsRequest : APIRequest<List<ForumSubscriptionModel>>
    {
        protected override List<ForumSubscriptionModel> Execute(TitanicAPI api)
        {
            return api.Get<List<ForumSubscriptionModel>>("/forum/subscriptions");
        }
    }
}
