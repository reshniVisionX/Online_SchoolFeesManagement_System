using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Services
{
    public class SchoolService
    {
        private readonly ISchool _schoolRepository;

        public SchoolService(ISchool schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<IEnumerable<School>> GetAllSchoolsAsync()
        {
            try
            {
                return await _schoolRepository.GetAllSchoolsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching schools", ex);
            }
        }
    }
}
