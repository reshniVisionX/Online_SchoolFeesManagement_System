using SchoolFeeManagementApi.Models;
namespace SchoolFeeManagementApi.Interface
{
    public interface ISchool
    {
        Task<IEnumerable<School>> GetAllSchoolsAsync();
    }
}
