using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using URLShortener.Services;
using URLShortener.Models;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("")]
    public class UrlController : ControllerBase
    {
        private readonly UrlService _urlService;

        public UrlController(UrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] string originalUrl)
        {
            if (!IsValidUrl(originalUrl))
            {
                return BadRequest(new { message = "URL không hợp lệ!" });
            }

            var result = _urlService.CreateShortUrl(originalUrl);

            return Ok(new
            {
                result.OriginalUrl,
                result.ShortUrl,
                FullShortUrl = $"{Request.Scheme}://{Request.Host}/{result.ShortUrl}"
            });
        }

        [HttpGet("expand/{shortUrl}")]
        public IActionResult ExpandUrl(string shortUrl)
        {
            var result = _urlService.GetOriginalUrl(shortUrl);
            return result == null
                ? NotFound(new { message = "URL không tồn tại!" })
                : Ok(new { result.OriginalUrl });
        }

        [HttpGet("{shortUrl}")]
        public IActionResult RedirectToOriginalUrl(string shortUrl)
        {
            var result = _urlService.GetOriginalUrl(shortUrl);
            return result == null ? NotFound(new { message = "URL không tồn tại!" }) : Redirect(result.OriginalUrl);
        }

        [HttpGet("list")]
        public IActionResult GetAllUrls()
        {
            var urls = _urlService.GetAllUrls();

            return Ok(urls.Select(u => new
            {
                u.OriginalUrl,
                u.ShortUrl,
                FullShortUrl = $"{Request.Scheme}://{Request.Host}/{u.ShortUrl}"
            }));
        }

        private static bool IsValidUrl(string url)
        {
            // Regex kiểm tra URL hợp lệ, bao gồm cả localhost
            string pattern = @"^(http|https)://((localhost|\d{1,3}(\.\d{1,3}){3})|([\w-]+(\.[a-zA-Z]{2,})+))(:\d{1,5})?(\/.*)?$";
            return Regex.IsMatch(url, pattern);
        }

    }
}
