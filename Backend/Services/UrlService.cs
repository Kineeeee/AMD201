using URLShortener.Models;
using URLShortener.Data;
using StackExchange.Redis;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace URLShortener.Services
{
    public class UrlService
    {
        private readonly IMongoCollection<UrlModel> _urls;
        private readonly IDatabase _cache;
        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromDays(30);

        public UrlService(MongoDbContext dbContext, IDatabase cache)
        {
            _urls = dbContext.Urls;
            _cache = cache;
        }

        private static string GenerateShortUrl(string originalUrl)
        {
            using var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(originalUrl));

            return ConvertToBase62(BitConverter.ToUInt64(hash, 0));
        }

        private static string ConvertToBase62(ulong value)
        {
            var shortUrl = new StringBuilder(8);

            for (int i = 0; i < 8; i++)
            {
                shortUrl.Append(Base62Chars[(int)(value % 62)]);
                value /= 62;
            }

            return shortUrl.ToString();
        }

        public UrlModel CreateShortUrl(string originalUrl)
        {
            string cacheKey = $"url:{originalUrl}";

            // Kiểm tra Redis Cache
            string cachedShortUrl = _cache.StringGet(cacheKey);
            if (!string.IsNullOrEmpty(cachedShortUrl))
            {
                return new UrlModel { OriginalUrl = originalUrl, ShortUrl = cachedShortUrl };
            }

            // Kiểm tra database
            var existingUrl = _urls.Find(u => u.OriginalUrl == originalUrl).FirstOrDefault();
            if (existingUrl != null)
            {
                _cache.StringSet(cacheKey, existingUrl.ShortUrl, CACHE_DURATION);
                return existingUrl;
            }

            // Tạo URL ngắn mới
            var shortUrl = GenerateShortUrl(originalUrl);
            var urlModel = new UrlModel { OriginalUrl = originalUrl, ShortUrl = shortUrl };

            _urls.InsertOne(urlModel);
            _cache.StringSet(cacheKey, shortUrl, CACHE_DURATION);

            return urlModel;
        }

        public UrlModel? GetOriginalUrl(string shortUrl) =>
            _urls.Find(u => u.ShortUrl == shortUrl).FirstOrDefault();

        public List<UrlModel> GetAllUrls() =>
            _urls.Find(_ => true).ToList();
    }
}
