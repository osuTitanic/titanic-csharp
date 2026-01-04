using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetBookmarksRequest : APIRequest<List<ForumBookmarkModel>>
    {
        protected override List<ForumBookmarkModel> Execute(TitanicAPI api)
        {
            return api.Get<List<ForumBookmarkModel>>("/forum/bookmarks");
        }
    }
}
