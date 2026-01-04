using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetForumRequest : APIRequest<ForumModel>
    {
        public int ForumId { get; set; }

        public GetForumRequest(int forumId)
        {
            ForumId = forumId;
        }

        protected override ForumModel Execute(TitanicAPI api)
        {
            return api.Get<ForumModel>($"/forum/{ForumId}");
        }
    }
}
