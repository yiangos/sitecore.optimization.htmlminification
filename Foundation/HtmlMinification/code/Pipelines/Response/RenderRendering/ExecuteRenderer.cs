using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LinakisDigital.Foundation.Minification.Services;

namespace LinakisDigital.Foundation.Minification.Pipelines.Response.RenderRendering
{
    public class ExecuteRenderer: Sitecore.Mvc.Pipelines.Response.RenderRendering.ExecuteRenderer
    {
        private IMarkupMinifierService markupMinifierService;

        private static string[] excludeURLsContainingKeywords;

        public static string[] ExcludeURLsContainingKeywords
        {
            get
            {
                if (ExecuteRenderer.excludeURLsContainingKeywords == null || ExecuteRenderer.excludeURLsContainingKeywords.Length < 1)
                {
                    string setting = Settings.GetSetting(Constants.ExcludeURLKeywords, "/sitecore");
                    ExecuteRenderer.excludeURLsContainingKeywords = setting.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                }
                return ExecuteRenderer.excludeURLsContainingKeywords;
            }
        }
        public ExecuteRenderer(): base()
        {
            this.markupMinifierService = (ServiceLocator.ServiceProvider.GetService(typeof(IMarkupMinifierService)) as IMarkupMinifierService);
        }

        public ExecuteRenderer(IRendererErrorStrategy errorStrategy) : base(errorStrategy)
        {
            this.markupMinifierService = (ServiceLocator.ServiceProvider.GetService(typeof(IMarkupMinifierService)) as IMarkupMinifierService);
        }
        public ExecuteRenderer(IRendererErrorStrategy errorStrategy, IMarkupMinifierService markupMinifierService): base(errorStrategy)
        {
            this.markupMinifierService = markupMinifierService;
        }

        protected override bool Render(Renderer renderer, TextWriter writer, RenderRenderingArgs args)
        {
            if (!Settings.GetBoolSetting(Constants.Enabled, false) || this.ExcludedURL(args.PageContext.RequestContext.HttpContext.Request.RawUrl.ToLower()))
            {
                return base.Render(renderer, writer, args);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(stringBuilder))
                {
                    renderer.Render((TextWriter)stringWriter);
                }
                string value = markupMinifierService.Minify(stringBuilder.ToString(), 
                    Settings.GetBoolSetting(Constants.RemoveWhiteSpaces, true), 
                    Settings.GetBoolSetting(Constants.RemoveLineBreaks, true), 
                    Settings.GetBoolSetting(Constants.RemoveHTMLComments, true));
                writer.Write(value);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to render rendering", ex, (object)this);
                if (!IsHandledByErrorStrategy(renderer, ex, writer))
                {
                    throw;
                }
            }
            return true;
        }

        private bool ExcludedURL(string url)
        {
            string[] array = ExecuteRenderer.ExcludeURLsContainingKeywords;
            foreach (string str in array)
            {
                if (Regex.IsMatch(url, "(?<=^([^\"]|\"[^\"]*\")*)" + str))
                {
                    return true;
                }
            }
            return false;
        }
    }
}


