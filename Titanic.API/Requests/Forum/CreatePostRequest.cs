using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class CreatePostRequest : APIRequest<ForumPostModel>
    {
        public int ForumId { get; set; }
        public int TopicId { get; set; }
        public ForumPostCreateRequest Post { get; set; }

        public CreatePostRequest(int forumId, int topicId, ForumPostCreateRequest post)
        {
            ForumId = forumId;
            TopicId = topicId;
            Post = post;
        }

        protected override ForumPostModel Execute(TitanicAPI api)
        {
            return api.Post<ForumPostModel>($"/forum/{ForumId}/topics/{TopicId}/posts", Post);
        }
    }
}
