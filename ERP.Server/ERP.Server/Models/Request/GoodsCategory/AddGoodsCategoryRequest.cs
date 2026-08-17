namespace ERP.Server.Models.Request;

public class AddGoodsCategoryRequest
{
    public int? ParentID { get; set; }//上级分类ID
    public string CategoryName { get; set; } = string.Empty;//分类名称
    public string CategoryCode { get; set; } = string.Empty;//分类编码
    public int Sort { get; set; }//排序
}