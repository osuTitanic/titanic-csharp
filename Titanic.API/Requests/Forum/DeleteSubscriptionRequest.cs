namespace Titanic.API.Requests
{
    public class DeleteSubscriptionRequest : APIRequest<object>
    {
        public int TopicId { get; set; }

        public DeleteSubscriptionRequest(int topicId)
        {
            TopicId = topicId;
        }

        protected override object Execute(TitanicAPI api)
        {
            return api.Delete<object>($"/forum/subscriptions/{TopicId}");
        }
    }
}
