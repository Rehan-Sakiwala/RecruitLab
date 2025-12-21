namespace recruitlab.server.Services
{
    public class HtmlTemplateService
    {
        private readonly IWebHostEnvironment _env;

        public HtmlTemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> LoadTemplateAsync(
            string folder,
            string fileName,
            Dictionary<string, string> data)
        {
            var path = Path.Combine(
                _env.ContentRootPath,
                folder,
                fileName
            );

            var html = await File.ReadAllTextAsync(path);

            foreach (var item in data)
            {
                html = html.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            return html;
        }
    }
}
