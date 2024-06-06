namespace HomeBankingNet8.Utils
{
    public class Response<T>
    {
        public T data { get; set; }
        public int statusCode { get; set; }

        public Response(T Data,int status)
        {
            data = Data;
            statusCode = status;
        }
    }
}
