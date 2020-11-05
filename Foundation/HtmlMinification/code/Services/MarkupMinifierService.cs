using System.Text.RegularExpressions;

namespace LinakisDigital.Foundation.Minification.Services
{
    public class MarkupMinifierService : IMarkupMinifierService
    {
        public string Minify(string content)
        {
            return this.Minify(content, true, true, true);
        }

        public string Minify(string content, bool removeWhitespaces)
        {
            return this.Minify(content, removeWhitespaces, true, true);
        }

        public string Minify(string content, bool removeWhitespaces, bool removeLineBreaks)
        {
            return this.Minify(content, removeWhitespaces, removeLineBreaks, true);
        }

        public string Minify(string content, bool removeWhitespaces, bool removeLineBreaks, bool removeHtmlComments)
        {
            if (removeWhitespaces)
            {
                content = this.RemoveWhitespaces(content);
            }
            if (removeLineBreaks)
            {
                content = this.RemoveLineBreaks(content);
            }
            if (removeHtmlComments)
            {
                content = this.RemoveHtmlComments(content);
            }
            return content;
        }

        private string RemoveHtmlComments(string content)
        {
            return Regex.Replace(content, "<!--(?!\\[[^]+])(?:(?!-->)(?!]-->).)*-->", string.Empty, RegexOptions.Multiline | RegexOptions.Compiled);
        }

        private string RemoveLineBreaks(string content)
        {
            content = Regex.Replace(content, "\\n", string.Empty, RegexOptions.Multiline | RegexOptions.Compiled);
            return Regex.Replace(content, "\\r", string.Empty, RegexOptions.Multiline | RegexOptions.Compiled);
        }

        private string RemoveWhitespaces(string content)
        {
            content = Regex.Replace(content, "^\\s*", string.Empty, RegexOptions.Multiline | RegexOptions.Compiled);
            content = Regex.Replace(content, "\\s+<", "<", RegexOptions.Multiline | RegexOptions.Compiled);
            content = Regex.Replace(content, ">\\s+", ">", RegexOptions.Multiline | RegexOptions.Compiled);
            content = Regex.Replace(content, "\\s\\s+", " ", RegexOptions.Multiline | RegexOptions.Compiled);
            return content;
        }
    }
}