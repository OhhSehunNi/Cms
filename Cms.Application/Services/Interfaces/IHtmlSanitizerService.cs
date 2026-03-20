namespace Cms.Application.Services
{
    /// <summary>
    /// HTML 清洗服务接口
    /// </summary>
    public interface IHtmlSanitizerService
    {
        /// <summary>
        /// 清洗 HTML 内容
        /// </summary>
        /// <param name="html">原始 HTML 内容</param>
        /// <returns>清洗后的 HTML 内容</returns>
        string SanitizeHtml(string html);

        /// <summary>
        /// 提取纯文本
        /// </summary>
        /// <param name="html">HTML 内容</param>
        /// <returns>纯文本内容</returns>
        string ExtractPlainText(string html);

        /// <summary>
        /// 计算字数
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns>字数</returns>
        int CalculateWordCount(string text);
    }
}
