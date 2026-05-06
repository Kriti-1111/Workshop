using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Workshop.Data;
using Workshop.Models;

namespace Workshop.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // IEnumerable version - fetches all then filters in memory
        [HttpGet("filtered-enumerable")]
        public IActionResult GetFilteredEnumerable(int age, string address)
        {
            var students = _context.Students
                .AsEnumerable()
                .Where(s => s.Age == age && s.Address == address)
                .Select(s => new { s.Name, s.Age, s.Address });

            return Ok(students);
        }

        // IQueryable version - filters in SQL (optimized)
        [HttpGet("filtered")]
        public IActionResult GetFiltered(int age, string address)
        {
            var students = _context.Students
                .Where(s => s.Age == age && s.Address == address)
                .Select(s => new { s.Name, s.Age, s.Address });

            return Ok(students);
        }
    }
}