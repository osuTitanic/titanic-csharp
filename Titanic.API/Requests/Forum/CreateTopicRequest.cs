using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class CreateTopicRequest : APIRequest<ForumTopicModel>
    {
        public int ForumId { get; set; }
        public ForumTopicCreateRequest Topic { get; set; }

        public CreateTopicRequest(int forumId, ForumTopicCreateRequest topic)
        {
            ForumId = forumId;
            Topic = topic;
        }

        protected override ForumTopicModel Execute(TitanicAPI api)
        {
            return api.Post<ForumTopicModel>($"/forum/{ForumId}/topics", Topic);
        }
    }
}
