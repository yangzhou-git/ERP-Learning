using ERP.Server.Common.Interfaces;

namespace ERP.Server.Common
{
    public class ApiResponse<T> : IApiResponse
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public T? Data { get; set; }

        public ApiResponse(int code, string message, T? data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> Success(T data, string message = "操作成功")
            => new ApiResponse<T>(200, message, data);

        public static ApiResponse<T> BadRequest(string message)
            => new ApiResponse<T>(400, message, default);

        public static ApiResponse<T> NotFound(string message)
            => new ApiResponse<T>(404, message, default);

        public static ApiResponse<T> Conflict(string message)
            => new ApiResponse<T>(409, message, default);

        public static ApiResponse<T> Error(string message, int code = 500)
            => new ApiResponse<T>(code, message, default);
    }
}
