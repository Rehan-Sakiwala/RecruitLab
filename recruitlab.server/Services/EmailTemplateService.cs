public class EmailTemplateService
{
    private readonly IWebHostEnvironment _env;

    public EmailTemplateService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> GetAsync(
        string templateName,
        Dictionary<string, string> data)
    {
        var path = Path.Combine(
            _env.ContentRootPath,
            "EmailTemplates",
            "ApplicationStatus",
            templateName + ".html"
        );

        var html = await File.ReadAllTextAsync(path);

        foreach (var item in data)
        {
            html = html.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return html;
    }
}
