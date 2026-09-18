using ERP.Server.Models.Common;

namespace ERP.Server.Common.Helpers
{
    public static class PaginationHelper
    {
        #region 规范化分页参数Normalize
        public static void Normalize(PageRequest request)
        {
            if(request.Page <= 0)
            {
                request.Page = 1;
            }
            if(request.PageSize <= 0)
            {
                request.PageSize = 10;
            }
            if(request.PageSize > 200)
            {
                request.PageSize = 200;
            }
        }
        #endregion
    }
}