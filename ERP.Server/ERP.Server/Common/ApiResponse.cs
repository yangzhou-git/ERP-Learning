namespace ERP.Server.Common
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public T? Data { get; set; }

        public ApiResponse(int code, string message, T? data = default)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> Success(T data, string message = "操作成功")
            => new ApiResponse<T>(200, message, data);

        public static ApiResponse<T> Error(string message, int code = 500)
            => new ApiResponse<T>(code, message);
    }
}
