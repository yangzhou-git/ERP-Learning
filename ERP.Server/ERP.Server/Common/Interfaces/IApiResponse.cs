namespace ERP.Server.Common.Interfaces
{
    public interface IApiResponse
    {
        public int Code { get; }
        public string Message { get; }
    }
}
