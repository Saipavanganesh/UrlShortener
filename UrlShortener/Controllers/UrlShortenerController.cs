using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Migrations;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlShortenerController : ControllerBase
    {
        private readonly UrlShortenerService _urlShortenerService;
        public UrlShortenerController(UrlShortenerService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService; 
        }
        [HttpPost]
        public IActionResult CreateShortUrl([FromBody] UrlRequest urlRequest)
        {
            if(string.IsNullOrWhiteSpace(urlRequest?.LongUrl))
            {
                return BadRequest("Long URL cannot be empty");
            }
            try
            {
                var shortUrl = _urlShortenerService.CreateShortUrl(urlRequest.LongUrl);
                return Ok(shortUrl);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult GetLongurl([FromQuery] ShortUrlRequest shortUrlrequest)
        {
            if (string.IsNullOrWhiteSpace(shortUrlrequest?.shortUrl))
            {
                return BadRequest("Short URL cannot be empty");
            }
            try
            {
                var longUrl = _urlShortenerService.GetLongUrl(shortUrlrequest.shortUrl);
                return Redirect(longUrl);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Short URL not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
