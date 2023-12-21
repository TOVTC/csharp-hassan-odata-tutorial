using Odata6Demo.Api.Models;

namespace Odata6Demo.Api.Services
{
    public interface IStudentService
    {
        IQueryable<Student> RetrieveAllStudents();
    }
}
