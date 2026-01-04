using Titanic.API.Models;

namespace Titanic.API.Requests
{
    public class PostDirectMessageRequest : APIRequest<PrivateMessageModel>
    {
        public int TargetId { get; set; }
        public string Message { get; set; }

        public PostDirectMessageRequest(int targetId, string message)
        {
            TargetId = targetId;
            Message = message;
        }

        protected override PrivateMessageModel Execute(TitanicAPI api)
        {
            return api.Post<PrivateMessageModel>($"/chat/dms/{TargetId}/messages", new { message = Message });
        }
    }
}
