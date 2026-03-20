using Ganss.Xss;
using System.Text.RegularExpressions;

namespace Cms.Application.Services
{
    /// <summary>
    /// HTML 清洗服务，用于处理富文本内容
    /// </summary>
    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();
            // 配置允许的标签和属性
            var allowedTags = new[] { "p", "div", "span", "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "li", "blockquote", "hr", "br", "strong", "em", "u", "i", "b", "a", "img" };
            foreach (var tag in allowedTags)
            {
                _sanitizer.AllowedTags.Add(tag);
            }

            var allowedAttributes = new[] { "href", "src", "alt", "title" };
            foreach (var attribute in allowedAttributes)
            {
                _sanitizer.AllowedAttributes.Add(attribute);
            }

            _sanitizer.AllowedSchemes.Add("http");
            _sanitizer.AllowedSchemes.Add("https");
        }

        /// <summary>
        /// 清洗 HTML 内容
        /// </summary>
        /// <param name="html">原始 HTML 内容</param>
        /// <returns>清洗后的 HTML 内容</returns>
        public string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // 清洗 HTML
            var sanitizedHtml = _sanitizer.Sanitize(html);

            // 移除 Word 垃圾标签
            sanitizedHtml = RemoveWordTags(sanitizedHtml);

            return sanitizedHtml;
        }

        /// <summary>
        /// 提取纯文本
        /// </summary>
        /// <param name="html">HTML 内容</param>
        /// <returns>纯文本内容</returns>
        public string ExtractPlainText(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // 清洗 HTML
            var sanitizedHtml = SanitizeHtml(html);

            // 移除 HTML 标签
            var plainText = Regex.Replace(sanitizedHtml, @"<[^>]*>", string.Empty);

            // 移除多余的空白字符
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            return plainText;
        }

        /// <summary>
        /// 计算字数
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns>字数</returns>
        public int CalculateWordCount(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // 移除 HTML 标签
            var plainText = ExtractPlainText(text);

            // 计算字数
            return plainText.Length;
        }

        /// <summary>
        /// 移除 Word 垃圾标签
        /// </summary>
        /// <param name="html">HTML 内容</param>
        /// <returns>清理后的 HTML 内容</returns>
        private string RemoveWordTags(string html)
        {
            // 移除 Word 特定的标签和属性
            html = Regex.Replace(html, @"<o:\w+[^>]*>", string.Empty);
            html = Regex.Replace(html, @"</o:\w+>", string.Empty);
            html = Regex.Replace(html, @"<!--[\s\S]*?-->", string.Empty);
            html = Regex.Replace(html, @"style=""[\s\S]*?""", string.Empty);
            html = Regex.Replace(html, @"class=""[\s\S]*?""", string.Empty);

            return html;
        }
    }
}
