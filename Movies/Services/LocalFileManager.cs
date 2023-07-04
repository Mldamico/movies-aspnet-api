using System.Net;

namespace Movies.Services;

public class LocalFileManager: IFileManager
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileManager(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
    {
        var fileName = $"{Guid.NewGuid()}{extension}";
        string folder = Path.Combine(_env.WebRootPath, container);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string route = Path.Combine(folder, fileName);
        await File.WriteAllBytesAsync(route, content);
        var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        var urlDb = Path.Combine(url, container, fileName).Replace("\\", "/");
        return urlDb;
    }

    public async Task<string> EditFile(byte[] content, string extension, string container, string route, string contentType)
    {
        await DeleteFile(route, container);
        return await SaveFile(content, extension, container, contentType);
    }

    public Task DeleteFile(string route, string container)
    {
        if (route != null)
        {
            var fileName = Path.GetFileName(route);
            string directory = Path.Combine(_env.WebRootPath, container, fileName);
            if (File.Exists(directory))
            {
                File.Delete(directory);
            }

            
        }
        return Task.FromResult(0);
    }
}