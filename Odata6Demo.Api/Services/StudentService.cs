using Odata6Demo.Api.Models;

namespace Odata6Demo.Api.Services
{
    public class StudentService : IStudentService
    {
        public IQueryable<Student> RetrieveAllStudents()
        {
            return new List<Student> 
            {
                new Student
                {
                    Id = Guid.NewGuid(),
                    Name = "Vishu Goli",
                    Score = 200
                },
                new Student
                {
                    Id = Guid.NewGuid(),
                    Name = "Kailu Hu",
                    Score = 160
                },
                new Student
                {
                    Id = Guid.NewGuid(),
                    Name = "Sean Hobbs",
                    Score = 170
                }
            }.AsQueryable();
        }
    }
}
