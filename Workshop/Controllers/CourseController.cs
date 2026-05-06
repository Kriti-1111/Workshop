using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Workshop.Data;
using Workshop.Models;

namespace Workshop.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "courses";

        public CourseController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            if (!_cache.TryGetValue(CacheKey, out List<Course> courses))
            {
                courses = await _context.Courses.ToListAsync();
                _cache.Set(CacheKey, courses, TimeSpan.FromMinutes(5));
            }

            return Ok(courses);
        }
    }
}