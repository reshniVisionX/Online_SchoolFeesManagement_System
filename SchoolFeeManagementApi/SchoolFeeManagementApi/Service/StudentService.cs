using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Repository;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SchoolFeeManagementApi.Services
{
    public class StudentService
    {
        private readonly IStudent _studentRepo;

        public StudentService(IStudent studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public async Task<Student> AddStudentAsync(StudentDTO dto)
        {
            // 🔹 Validation rules
            if (string.IsNullOrWhiteSpace(dto.SName))
                throw new ArgumentException("Student name is required.");
            if (dto.CourseId <= 0)
                throw new ArgumentException("Valid CourseId is required.");
            if (dto.Dob > DateOnly.FromDateTime(DateTime.Now).AddYears(-5))
            {
                throw new ArgumentException("Invalid date of birth. Student must be at least 5 years old.");
            }

            string adId = await _studentRepo.GenerateAdmissionIdAsync();
            var student = new Student
            {
                SName = dto.SName,
                CourseId = dto.CourseId,
                RoleId = 1,
                SdId = 0,
               
                SImage = dto.SImage,
                Dob = dto.Dob,
                BloodGrp = dto.BloodGrp,
               AdmissionId = adId,
                ParAddress = dto.ParAddress,
                ParPhone = dto.ParPhone,
                ParEmail = dto.ParEmail
            };

            var details = new StudentDetails
            {
                Category = dto.Category,
                IsSports = dto.IsSports,
                IsMerit = dto.IsMerit,
                IsFG = dto.IsFG,
                IsWaiver = dto.IsWaiver,
                AdmissionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _studentRepo.AddStudent(student, details);
        }

        public async Task<Student?> UpdateStudentAsync(int id, UpdStudentsDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Validate only if the values are provided
            if (!string.IsNullOrEmpty(dto.ParPhone) && !Regex.IsMatch(dto.ParPhone, @"^\d{10}$"))
                throw new ArgumentException("ParPhone must be a 10-digit number.");

            if (!string.IsNullOrEmpty(dto.ParEmail) && !new EmailAddressAttribute().IsValid(dto.ParEmail))
                throw new ArgumentException("ParEmail is not valid.");

            if (dto.Dob.HasValue && dto.Dob.Value > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Dob cannot be in the future.");

            // Only assign fields if they are provided
            var student = new Student { SId = id };
            if (!string.IsNullOrEmpty(dto.SName)) student.SName = dto.SName;
            if (dto.CourseId.HasValue) student.CourseId = dto.CourseId.Value;
            if (dto.SImage != null) student.SImage = dto.SImage;
            if (dto.Dob.HasValue) student.Dob = dto.Dob.Value;
            if (!string.IsNullOrEmpty(dto.BloodGrp)) student.BloodGrp = dto.BloodGrp;
            if (!string.IsNullOrEmpty(dto.ParAddress)) student.ParAddress = dto.ParAddress;
            if (!string.IsNullOrEmpty(dto.ParPhone)) student.ParPhone = dto.ParPhone;
            if (!string.IsNullOrEmpty(dto.ParEmail)) student.ParEmail = dto.ParEmail;

            var details = new StudentDetails();
            if (dto.Category.HasValue) details.Category = dto.Category.Value;
            if (dto.IsSports.HasValue) details.IsSports = dto.IsSports.Value;
            if (dto.IsMerit.HasValue) details.IsMerit = dto.IsMerit.Value;
            if (dto.IsFG.HasValue) details.IsFG = dto.IsFG.Value;
            if (dto.IsWaiver.HasValue) details.IsWaiver = dto.IsWaiver.Value;
            if(dto.IsActive.HasValue) details.IsActive = dto.IsActive.Value;

            // Call repository with only the needed fields
            return await _studentRepo.UpdateStudent(id, student, details);
        }



        public Task<Student?> GetStudentByIdAsync(int id)
            => _studentRepo.GetStudentById(id);

        public Task<ICollection<Student>> GetAllStudentsAsync()
            => _studentRepo.GetAllStudents();

        public Task<bool> DeleteStudentAsync(int id)
            => _studentRepo.DeleteStudent(id);

        public async Task<ICollection<Student>> FilterStudent(
    string? name,
    bool? isActive,
    string? aid,
    StudentCategory? category,
    string? bloodGroup,
    DateOnly? dob)
        {
            if (!string.IsNullOrWhiteSpace(name) && !Regex.IsMatch(name, @"^[a-zA-Z\s]+$"))
                throw new ArgumentException("Name can only contain letters and spaces.");

            if (!string.IsNullOrWhiteSpace(bloodGroup))
            {
                var validGroups = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                if (!validGroups.Contains(bloodGroup))
                    throw new ArgumentException("Invalid blood group.");
            }

        

            if (dob != null && dob > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Date of birth cannot be in the future.");

            return await _studentRepo.FilterStudent(name, isActive, aid, category, bloodGroup, dob);
        }



        public async Task<ICollection<Student>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _studentRepo.GetStudentsByCourseId(courseId);
        }

        public async Task<(bool Success, string? ErrorMessage)> InsertBulkStudentsAsync(ICollection<StdClassDTO> students)
        {
            if (students == null || !students.Any())
                return (false, "Student list cannot be empty.");

            foreach (var stud in students)
            {
                if (string.IsNullOrWhiteSpace(stud.sName))
                    return (false, "Student name is required for one of the records.");

                if (stud.courseId <= 0)
                    return (false, $"Valid CourseId is required for student {stud.sName}.");

                if (!DateOnly.TryParse(stud.dob, out var dob))
                    return (false, $"Invalid date format for student {stud.sName}.");

                if (dob > DateOnly.FromDateTime(DateTime.Now).AddYears(-5))
                    return (false, $"Invalid date of birth for student {stud.sName}. Student must be at least 5 years old.");

                if (!string.IsNullOrWhiteSpace(stud.parEmail))
                {
                    // Regex pattern for standard email validation
                    var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z]+\.[a-zA-Z]{2,}$";

                    if (!System.Text.RegularExpressions.Regex.IsMatch(stud.parEmail, emailPattern))
                        return (false, $"Invalid parent email format for student {stud.sName}.");
                }
            }

            try
            {
                var (success, errors) = await _studentRepo.BulkInsertStudentsAsync(students);

                if (!success && errors.Any())
                    return (false, string.Join("; ", errors));

                return (success, null);
            }
            catch (Exception ex)
            {
                // Optional: log the exception here
                return (false, $"An error occurred while inserting students: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StudentCredentialDTO>> GetAllStudentsCredentialsAsync()
        {
            return await _studentRepo.GetAllUserCredentialsAsync();
        }
    }
}
