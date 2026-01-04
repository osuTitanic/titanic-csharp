using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class CreateSubscriptionRequest : APIRequest<ForumSubscriptionModel>
    {
        public int TopicId { get; set; }

        public CreateSubscriptionRequest(int topicId)
        {
            TopicId = topicId;
        }

        protected override ForumSubscriptionModel Execute(TitanicAPI api)
        {
            return api.Post<ForumSubscriptionModel>("/forum/subscriptions", new { topic_id = TopicId });
        }
    }
}
