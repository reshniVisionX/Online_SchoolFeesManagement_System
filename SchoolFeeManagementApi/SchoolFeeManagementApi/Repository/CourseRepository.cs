using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Data;
using System;
using SchoolFeeManagementApi.Service;

namespace SchoolFeeManagementApi.Repository
{
    public class CourseRepository : ICourse
    {
        private readonly SchoolContext _context;
        private readonly PaymentService pay_repo;
        public CourseRepository(SchoolContext context, PaymentService pay_repo)
        {
            _context = context;
            this.pay_repo = pay_repo;
        }

        public async Task<Course> AddCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;
            course.IsActive = false;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ICollection<Course>> FilterCourse(string? name, bool? active)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.CourseName.Contains(name));
            }

            if (active.HasValue)
            {
                query = query.Where(c => c.IsActive == active.Value);
            }

            return await query.ToListAsync();
        }


        public async Task<ICollection<Course>> GetAllCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseById(int id)
        {
            var course =await  _context.Courses.FindAsync(id);
            return course;
        }

        public async Task<Course?> UpdateCourse(int id, Course course)
        {
            var existing = await _context.Courses.FindAsync(id);
            if (existing == null) return null;

            // Update only if values are provided, else keep existing
            existing.CourseName = string.IsNullOrWhiteSpace(course.CourseName) ? existing.CourseName : course.CourseName;
            existing.TutionFees = course.TutionFees == 0 ? existing.TutionFees : course.TutionFees;
            existing.BusFees = course.BusFees == 0 ? existing.BusFees : course.BusFees;
            existing.HostelFees = course.HostelFees == 0 ? existing.HostelFees : course.HostelFees;
            existing.DonationFee = course.DonationFee == 0 ? existing.DonationFee : course.DonationFee;
            existing.Sports = course.Sports == 0 ? existing.Sports : course.Sports;
            existing.Merit = course.Merit == 0 ? existing.Merit : course.Merit;
            existing.FG = course.FG == 0 ? existing.FG : course.FG;
            existing.Waiver = course.Waiver == 0 ? existing.Waiver : course.Waiver;

            // Handle IsActive if provided (bool? works well here)
            if (course.IsActive != null)
                existing.IsActive = course.IsActive;

            // Handle CreatedAt only if provided
            if (course.CreatedAt != default)
                existing.CreatedAt = course.CreatedAt;

            await _context.SaveChangesAsync();
            await pay_repo.RefreshPaymentsForCourseAsync(id);
            return existing;
        }

        public async Task<bool> ToggleCourseStatus(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            // Toggle the IsActive value
            course.IsActive = !course.IsActive;

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
