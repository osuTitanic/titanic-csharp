using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class CreateBookmarkRequest : APIRequest<ForumBookmarkModel>
    {
        public int TopicId { get; set; }

        public CreateBookmarkRequest(int topicId)
        {
            TopicId = topicId;
        }

        protected override ForumBookmarkModel Execute(TitanicAPI api)
        {
            return api.Post<ForumBookmarkModel>("/forum/bookmarks", new { topic_id = TopicId });
        }
    }
}
