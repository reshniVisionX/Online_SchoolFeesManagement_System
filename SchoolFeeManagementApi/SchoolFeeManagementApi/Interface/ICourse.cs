using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Interface
{
    public interface ICourse
    {
        Task<ICollection<Course>> GetAllCourses();
        Task<ICollection<Course>> FilterCourse(string? name, bool? active);
        Task<Course> AddCourse(Course course);
        Task<Course> UpdateCourse(int id, Course course);
        Task<bool> DeleteCourse(int id);
        Task<Course> GetCourseById(int id);

        Task<bool> ToggleCourseStatus(int id);

        
    }
}
