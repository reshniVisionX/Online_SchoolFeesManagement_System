using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.DTOs;

namespace SchoolFeeManagementApi.Interface
{
    public interface IStudent
    {
        Task<Student> AddStudent(Student student, StudentDetails details);
        Task<Student?> UpdateStudent(int id, Student student, StudentDetails details);
        Task<Student?> GetStudentById(int id);
        Task<ICollection<Student>> GetAllStudents();
        Task<ICollection<Student>> FilterStudent(string? name, bool? isActive, string? admId, StudentCategory? category, string? bloodGroup, DateOnly? dob);
        Task<bool> DeleteStudent(int id);

        Task<string> GenerateAdmissionIdAsync();

        Task<ICollection<Student>> GetStudentsByCourseId(int courseId);

        Task<(bool Success, List<string> Errors)> BulkInsertStudentsAsync(ICollection<StdClassDTO> students);

        Task<Student?> GetStudentByAdmissionId(string admissionId);

        Task<bool> ValidatePassword(int id, string password);

        Task<IEnumerable<StudentCredentialDTO>> GetAllUserCredentialsAsync();

    }
}
