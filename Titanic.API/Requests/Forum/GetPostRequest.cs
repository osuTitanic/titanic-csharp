using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetPostRequest : APIRequest<ForumPostModel>
    {
        public int ForumId { get; set; }
        public int TopicId { get; set; }
        public int PostId { get; set; }

        public GetPostRequest(int forumId, int topicId, int postId)
        {
            ForumId = forumId;
            TopicId = topicId;
            PostId = postId;
        }

        protected override ForumPostModel Execute(TitanicAPI api)
        {
            return api.Get<ForumPostModel>($"/forum/{ForumId}/topics/{TopicId}/posts/{PostId}");
        }
    }
}
