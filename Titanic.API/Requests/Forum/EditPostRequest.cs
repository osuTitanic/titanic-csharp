using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class EditPostRequest : APIRequest<ForumPostModel>
    {
        public int ForumId { get; set; }
        public int TopicId { get; set; }
        public int PostId { get; set; }
        public ForumPostUpdateRequest Update { get; set; }

        public EditPostRequest(int forumId, int topicId, int postId, ForumPostUpdateRequest update)
        {
            ForumId = forumId;
            TopicId = topicId;
            PostId = postId;
            Update = update;
        }

        protected override ForumPostModel Execute(TitanicAPI api)
        {
            return api.Patch<ForumPostModel>($"/forum/{ForumId}/topics/{TopicId}/posts/{PostId}", Update);
        }
    }
}
