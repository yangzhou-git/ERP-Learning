using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Request.BusinessCodeSeed
{
    public class UpdateBusinessCodeSeedRequest
    {
        public int BusinessCodeSeedID { get; set; }
        public BusinessCodeType BusinessType { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public int NumberLength { get; set; }
        public BusinessCodeResetType ResetType { get; set; }
    }
}
