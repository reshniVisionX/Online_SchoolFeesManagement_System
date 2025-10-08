using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.Data;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Repository
{
    public class SchoolRepository : ISchool
    {
        private readonly SchoolContext _context;

        public SchoolRepository(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<School>> GetAllSchoolsAsync()
        {
            return await _context.Schools.ToListAsync();
        }
    }
}
