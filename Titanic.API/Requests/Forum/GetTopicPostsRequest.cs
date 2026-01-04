using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetTopicPostsRequest : APIRequest<List<ForumPostModel>>
    {
        public int ForumId { get; set; }
        public int TopicId { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public GetTopicPostsRequest(int forumId, int topicId, int offset = 0, int limit = 25)
        {
            ForumId = forumId;
            TopicId = topicId;
            Offset = offset;
            Limit = limit;
        }

        protected override List<ForumPostModel> Execute(TitanicAPI api)
        {
            return api.Get<List<ForumPostModel>>($"/forum/{ForumId}/topics/{TopicId}/posts?offset={Offset}&limit={Limit}");
        }
    }
}
