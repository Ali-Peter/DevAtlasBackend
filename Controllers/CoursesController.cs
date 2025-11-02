using Microsoft.AspNetCore.Mvc;
using DevAtlasBackend.Models;
using DevAtlasBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevAtlasBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IWebHostEnvironment _environment;

        public CoursesController(MongoDbService mongoDbService, IWebHostEnvironment environment)
        {
            _mongoDbService = mongoDbService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _mongoDbService.GetCoursesAsync();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Category) || string.IsNullOrWhiteSpace(request.Level))
            {
                return BadRequest("All required fields must be provided.");
            }

            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Level = request.Level,
                ThumbnailUrl = request.ThumbnailUrl
            };

            await _mongoDbService.CreateCourseAsync(course);
            return Ok(new { Message = "Course info added successfully!", CourseId = course.Id });
        }

        [HttpPost("CourseOutline")]
        public async Task<IActionResult> CreateCourseOutline([FromForm] CourseOutlineRequest request)
        {
            if (request.Sections == null || !request.Sections.Any())
            {
                return BadRequest("At least one section is required.");
            }

            foreach (var section in request.Sections)
            {
                if (string.IsNullOrWhiteSpace(section.CourseId) || string.IsNullOrWhiteSpace(section.Title) ||
                    section.Order <= 0)
                {
                    return BadRequest($"Invalid section data: CourseId: {section.CourseId}, Title: {section.Title}, Order: {section.Order}. All must be provided and Order must be a positive number.");
                }

                string? fileUrl = null;
                string? videoUrl = null;

                // Handle file upload
                if (section.File != null)
                {
                    var fileExtension = Path.GetExtension(section.File.FileName).ToLower();
                    if (!new[] { ".pdf", ".doc", ".ppt", ".pptx" }.Contains(fileExtension))
                    {
                        return BadRequest($"Invalid file format for section '{section.Title}'. Only PDF, DOC, PPT, PPTX allowed.");
                    }

                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var uploadsPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Files");
                    Directory.CreateDirectory(uploadsPath);
                    var filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await section.File.CopyToAsync(stream);
                    }
                    fileUrl = $"/api/Uploads/Files/{fileName}";
                }

                // Handle video upload
                if (section.Video != null)
                {
                    var videoExtension = Path.GetExtension(section.Video.FileName).ToLower();
                    if (!new[] { ".mp4", ".mov", ".avi", ".mkv" }.Contains(videoExtension))
                    {
                        return BadRequest($"Invalid video format for section '{section.Title}'. Only MP4, MOV, AVI, MKV allowed.");
                    }

                    var videoName = $"{Guid.NewGuid()}{videoExtension}";
                    var uploadsPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Videos");
                    Directory.CreateDirectory(uploadsPath);
                    var videoPath = Path.Combine(uploadsPath, videoName);
                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await section.Video.CopyToAsync(stream);
                    }
                    videoUrl = $"/api/Uploads/Videos/{videoName}";
                }

                var outline = new CourseOutline
                {
                    CourseId = section.CourseId,
                    Title = section.Title,
                    Description = section.Description,
                    FileUrl = fileUrl,
                    VideoUrl = videoUrl,
                    Order = section.Order
                };

                await _mongoDbService.CreateCourseOutlineAsync(outline);
            }

            return Ok("Course outline uploaded successfully!");
        }
    }

    public class CourseRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
    }

    public class CourseOutlineSection
    {
        public string CourseId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
        public IFormFile? Video { get; set; }
        public int Order { get; set; }
    }

    public class CourseOutlineRequest
    {
        public List<CourseOutlineSection> Sections { get; set; } = new();
    }
}