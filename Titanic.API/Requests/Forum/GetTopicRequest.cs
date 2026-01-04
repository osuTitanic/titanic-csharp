using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetTopicRequest : APIRequest<ForumTopicModel>
    {
        public int ForumId { get; set; }
        public int TopicId { get; set; }

        public GetTopicRequest(int forumId, int topicId)
        {
            ForumId = forumId;
            TopicId = topicId;
        }

        protected override ForumTopicModel Execute(TitanicAPI api)
        {
            return api.Get<ForumTopicModel>($"/forum/{ForumId}/topics/{TopicId}");
        }
    }
}
