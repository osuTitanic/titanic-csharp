using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class PostChannelMessageRequest : APIRequest<MessageModel>
    {
        public string Target { get; set; }
        public string Message { get; set; }

        public PostChannelMessageRequest(string target, string message)
        {
            Target = target;
            Message = message;
        }

        protected override MessageModel Execute(TitanicAPI api)
        {
            return api.Post<MessageModel>($"/chat/channels/{Target}/messages", new { message = Message });
        }
    }
}
