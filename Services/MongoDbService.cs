using MongoDB.Driver;
using DevAtlasBackend.Models;

namespace DevAtlasBackend.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Course> _coursesCollection;
        private readonly IMongoCollection<CourseOutline> _courseOutlinesCollection;

        public MongoDbService(IConfiguration configuration)
        {
            // PRIORITY: Environment Variables (Azure)
            var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION")
                ?? configuration.GetConnectionString("MongoDb")
                ?? throw new InvalidOperationException("MONGO_CONNECTION not set");

            var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME")
                ?? configuration["DatabaseName"]
                ?? "DevAtlasDb";

            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);

            _usersCollection = mongoDatabase.GetCollection<User>("Users");
            _coursesCollection = mongoDatabase.GetCollection<Course>("Courses");
            _courseOutlinesCollection = mongoDatabase.GetCollection<CourseOutline>("CourseOutlines");
        }

        // User methods
        public async Task<User?> GetUserByEmailAsync(string email) =>
            await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task CreateUserAsync(User user) =>
            await _usersCollection.InsertOneAsync(user);

        // Course methods
        public async Task<List<Course>> GetCoursesAsync() =>
            await _coursesCollection.Find(_ => true).ToListAsync();

        public async Task CreateCourseAsync(Course course) =>
            await _coursesCollection.InsertOneAsync(course);

        // CourseOutline methods
        public async Task CreateCourseOutlineAsync(CourseOutline outline) =>
            await _courseOutlinesCollection.InsertOneAsync(outline);
    }
}