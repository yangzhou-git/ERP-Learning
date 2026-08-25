namespace ERP.Server.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int UserID { get; }
        string UserName { get; }
        int CompanyID { get; }
    }
}
