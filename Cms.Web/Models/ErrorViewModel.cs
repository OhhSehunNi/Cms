namespace Cms.Web.Models;

/// <summary>
/// 错误视图模型
/// 用于错误页面显示请求ID
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// 请求ID
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// 是否显示请求ID
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
