namespace Titanic.API.Requests
{
    public class RenderBBCodeRequest : APIRequest<string>
    {
        public string Input { get; set; }

        public RenderBBCodeRequest(string input)
        {
            Input = input;
        }

        protected override string Execute(TitanicAPI api)
        {
            return api.Post<string>("/forum/bbcode", new { input = Input });
        }
    }
}
