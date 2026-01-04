using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class GetMainForumsRequest : APIRequest<List<ForumModel>>
    {
        protected override List<ForumModel> Execute(TitanicAPI api)
        {
            return api.Get<List<ForumModel>>("/forum/");
        }
    }
}
