using System;
using System.Collections.Generic;
using System.Text;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class ServerStatsRequest : APIRequest<ServerStatsModel>
    {
        protected override ServerStatsModel Execute(TitanicAPI api)
        {
            return api.Get<ServerStatsModel>("/stats");
        }
    }
}
