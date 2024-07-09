namespace Article.Application.Common
{
    public class BaseResponse<T> where T : class
    {
        public long Id { get; set; }
        public bool IsError { get; set; }
        public List<string> ErrorsList { get; set; } = new List<string>();
        public T Data { get; set; }
    }
}
