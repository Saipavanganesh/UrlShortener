using System.Text;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Services
{
    public class UrlShortenerService
    {
        private readonly UrlShortenerContext _context;
        private readonly SnowflakeIdGenerator _idGenerator;
        public UrlShortenerService(UrlShortenerContext context, SnowflakeIdGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        public string CreateShortUrl(string longUrl)
        {
            var existingRecord = _context.ShortUrls.FirstOrDefault(x => x.LongUrl == longUrl);
            if (existingRecord != null)
            {
                return existingRecord.ShortUrlCode;
            }
            long uniqueId = _idGenerator.GenerateId();
            string shortUrl = Base62Encode(uniqueId);
            var shortUrlRecord = new ShortUrl
            {
                Id = uniqueId,
                ShortUrlCode = shortUrl,
                LongUrl = longUrl,
            };
            _context.ShortUrls.Add(shortUrlRecord);
            _context.SaveChanges();
            return shortUrl;
        }
        public string GetLongUrl(string shortUrl)
        {
            var shortUrlRecord = _context.ShortUrls.FirstOrDefault(x => x.ShortUrlCode == shortUrl);
            if (shortUrlRecord != null)
            {
                return shortUrlRecord.LongUrl;
            }
            else
            {
                throw new KeyNotFoundException("The short URL does not exist.");
            }
        }
        private string Base62Encode(long id)
        {
            const string base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder encodedResult = new StringBuilder();
            if (id == 0)
            {
                return base62Chars[0].ToString(); // Return '0' if id is 0
            }

            while (id > 0)
            {
                int remainder = (int)(id % base62Chars.Length); //62;
                char c = base62Chars[remainder];
                encodedResult.Insert(0, c);
                id = id / base62Chars.Length; //62
            }
            return encodedResult.ToString();
        }
    }
}
