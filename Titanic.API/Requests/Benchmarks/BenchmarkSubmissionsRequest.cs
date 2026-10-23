using System.Collections.Generic;
using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class BenchmarkSubmissionsRequest : APIRequest<List<BenchmarkModel>>
    {
        private readonly int limit;
        private readonly int offset;

        public BenchmarkSubmissionsRequest(int limit = 100, int offset = 0)
        {
            this.limit = limit;
            this.offset = offset;
        }

        protected override List<BenchmarkModel> Execute(TitanicAPI api)
        {
            return api.GetList<BenchmarkModel>($"/benchmarks/submissions?limit={limit}&offset={offset}");
        }
    }
}
