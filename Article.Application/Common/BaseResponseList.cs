namespace Article.Application.Common
{
    public class BaseResponseList <T> where T : class
    {
        public long Id { get; set; }
        public bool IsError { get; set; }
        public List<string> ErrorsList { get; set; } = new List<string>();
        public List<T>? Data { get; set; }
    }
}
