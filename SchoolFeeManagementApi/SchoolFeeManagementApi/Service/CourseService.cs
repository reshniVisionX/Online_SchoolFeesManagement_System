using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Repository;

namespace SchoolFeeManagementApi.Service
{
    public class CourseService
    {
        private readonly ICourse _courseRepo;

        public CourseService(ICourse courseRepo)
        {
            _courseRepo = courseRepo;
        }

        // ✅ Create Course
        public async Task<Course> AddCourseAsync(CourseDTO dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.CourseName))
                throw new ArgumentException("Course name is required.");

            if (dto.TutionFees < 0 || dto.BusFees < 0 || dto.HostelFees < 0 ||
                dto.DonationFee < 0 || dto.Sports < 0 || dto.Merit < 0 ||
                dto.FG < 0 || dto.Waiver < 0)
            {
                throw new ArgumentException("Fee values cannot be negative.");
            }

            var course = new Course
            {
                CourseName = dto.CourseName,
                SclId = dto.SclId ?? 1,
                TutionFees = dto.TutionFees,
                BusFees = dto.BusFees,
                HostelFees = dto.HostelFees,
                DonationFee = dto.DonationFee,
                Sports = dto.Sports,
                Merit = dto.Merit,
                FG = dto.FG,
                Waiver = dto.Waiver,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return await _courseRepo.AddCourse(course);
        }

        // ✅ Update Course (Partial)
        public async Task<Course?> UpdateCourseAsync(int id, UpdCourseDTO dto)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid course ID.");

            var existing = await _courseRepo.GetCourseById(id);
            if (existing == null)
                return null;

            // Apply only provided values
            if (!string.IsNullOrWhiteSpace(dto.CourseName))
                existing.CourseName = dto.CourseName;

            if (dto.TutionFees.HasValue)
            {
                if (dto.TutionFees.Value < 0) throw new ArgumentException("Tution fee cannot be negative.");
                existing.TutionFees = dto.TutionFees.Value;
            }

            if (dto.BusFees.HasValue)
            {
                if (dto.BusFees.Value < 0) throw new ArgumentException("Bus fee cannot be negative.");
                existing.BusFees = dto.BusFees.Value;
            }

            if (dto.HostelFees.HasValue)
            {
                if (dto.HostelFees.Value < 0) throw new ArgumentException("Hostel fee cannot be negative.");
                existing.HostelFees = dto.HostelFees.Value;
            }

            if (dto.DonationFee.HasValue)
            {
                if (dto.DonationFee.Value < 0) throw new ArgumentException("Donation fee cannot be negative.");
                existing.DonationFee = dto.DonationFee.Value;
            }

            if (dto.Sports.HasValue)
                existing.Sports = dto.Sports.Value;

            if (dto.Merit.HasValue)
                existing.Merit = dto.Merit.Value;

            if (dto.FG.HasValue)
                existing.FG = dto.FG.Value;

            if (dto.Waiver.HasValue)
                existing.Waiver = dto.Waiver.Value;

            if (dto.IsActive.HasValue)
                existing.IsActive = dto.IsActive.Value;

            if (dto.CreatedAt.HasValue)
                existing.CreatedAt = dto.CreatedAt.Value;

            return await _courseRepo.UpdateCourse(id, existing);
        }

        // ✅ Delete (soft delete)
        public async Task<bool> DeleteCourseAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid course ID.");

            return await _courseRepo.DeleteCourse(id);
        }

        // ✅ Get All
        public async Task<ICollection<Course>> GetAllCoursesAsync()
        {
            return await _courseRepo.GetAllCourses();
        }

        // ✅ Filter by name (LIKE search)
        public async Task<ICollection<Course>> FilterCourseAsync(string? name, bool? active)
        {
          

            return await _courseRepo.FilterCourse(name,active);
        }

        public async Task<bool> ToggleCourseStatus(int id)
        {
            return await _courseRepo.ToggleCourseStatus(id);
        }

        public async Task<Course> GetCourseById(int courseId)
        {
            return await _courseRepo.GetCourseById(courseId);

        }
    }
}
