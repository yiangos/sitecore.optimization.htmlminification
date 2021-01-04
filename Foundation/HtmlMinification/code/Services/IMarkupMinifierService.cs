namespace Yiangos.Foundation.Minification.Services
{
    public interface IMarkupMinifierService
    {
        string Minify(string content);

        string Minify(string content, bool removeWhitespaces);

        string Minify(string content, bool removeWhitespaces, bool removeLineBreaks);

        string Minify(string content, bool removeWhitespaces, bool removeLineBreaks, bool removeHtmlComments);
    }
}