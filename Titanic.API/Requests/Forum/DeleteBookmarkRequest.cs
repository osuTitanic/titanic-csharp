namespace Titanic.API.Requests
{
    public class DeleteBookmarkRequest : APIRequest<object>
    {
        public int TopicId { get; set; }

        public DeleteBookmarkRequest(int topicId)
        {
            TopicId = topicId;
        }

        protected override object Execute(TitanicAPI api)
        {
            return api.Delete<object>($"/forum/bookmarks/{TopicId}");
        }
    }
}
