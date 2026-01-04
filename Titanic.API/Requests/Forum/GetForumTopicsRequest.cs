using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetForumTopicsRequest : APIRequest<List<ForumTopicModel>>
    {
        public int ForumId { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        public GetForumTopicsRequest(int forumId, int offset = 0, int limit = 25)
        {
            ForumId = forumId;
            Offset = offset;
            Limit = limit;
        }

        protected override List<ForumTopicModel> Execute(TitanicAPI api)
        {
            return api.Get<List<ForumTopicModel>>($"/forum/{ForumId}/topics?offset={Offset}&limit={Limit}");
        }
    }
}
