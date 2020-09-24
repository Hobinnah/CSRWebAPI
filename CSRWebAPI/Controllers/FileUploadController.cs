using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<FileUploadController> logger;

        public FileUploadController(ILogger<FileUploadController> logger, IMemoryCache cache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // GET: api/FileUpload
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("ImageUploader")]
        public async Task<IActionResult> UpLoader([FromForm] IFormCollection collection)
        {
            string filename = string.Empty;
            string newFilePath = string.Empty;
            string globalAccessPath = string.Empty;
            string[] supportedFormat = { ".jpeg", ".jpg", ".png" };

            var files = collection.Files;

            if (files.Count > 0)
            {
                if (!Directory.Exists(this.webHostEnvironment.WebRootPath + "\\Uploads\\"))
                {
                    Directory.CreateDirectory(this.webHostEnvironment.WebRootPath + "\\Uploads\\");
                }

                foreach (IFormFile source in files)
                {
                    filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

                    var fileExtension = Path.GetExtension(filename).ToLower();
                    if (supportedFormat.Contains(Path.GetExtension(filename).ToLower()))
                    {
                        string newFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(filename).ToLower()}";

                        newFilePath = this.webHostEnvironment.WebRootPath + "\\Uploads\\" + newFileName;
                        globalAccessPath = $"{ this.configuration.GetValue<string>("domain:domainUrl") }/Uploads/{ newFileName }";

                        using (FileStream output = System.IO.File.Create(newFilePath))
                        {
                            await source.CopyToAsync(output);
                        }
                    }
                    else
                        return this.Unauthorized();
                }

                long size = files.Sum(f => f.Length);
                cache.Set(globalAccessPath, newFilePath);

                return Ok(new { path = globalAccessPath });
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("PDFUploader")]
        public async Task<IActionResult> PDFUpLoader([FromForm] IFormCollection collection)
        {
            string filename = string.Empty;
            string newFilePath = string.Empty;
            string[] supportedFormat = { ".pdf" };

            var files = collection.Files;

            if (files.Count > 0)
            {
                if (!Directory.Exists(this.webHostEnvironment.WebRootPath + "\\SharePointUploads\\"))
                {
                    Directory.CreateDirectory(this.webHostEnvironment.WebRootPath + "\\SharePointUploads\\");
                }

                foreach (IFormFile source in files)
                {
                    filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

                    var fileExtension = Path.GetExtension(filename).ToLower();
                    if (supportedFormat.Contains(Path.GetExtension(filename).ToLower()))
                    {
                        string newFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(filename).ToLower()}";

                        newFilePath = this.webHostEnvironment.WebRootPath + "\\SharePointUploads\\" + newFileName;

                        using (FileStream output = System.IO.File.Create(newFilePath))
                        {
                            await source.CopyToAsync(output);
                        }
                    }
                    else
                        return this.Unauthorized();
                }

                long size = files.Sum(f => f.Length);
                cache.Set(filename, newFilePath);

                return this.Ok(newFilePath);
            }

            return this.Unauthorized();
        }

    }
}