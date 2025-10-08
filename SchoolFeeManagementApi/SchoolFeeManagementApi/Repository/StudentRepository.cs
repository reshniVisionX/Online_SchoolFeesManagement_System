using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.Data;
using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Utilities;
using SchoolFeeManagementApi.Service;

namespace SchoolFeeManagementApi.Repository
{
    public class StudentRepository : IStudent
    {
        private readonly SchoolContext _context;
        private readonly PaymentService pay_repo;
        public StudentRepository(SchoolContext context, PaymentService rep)
        {
            _context = context;
            pay_repo = rep;
        }

        public async Task<Student> AddStudent(Student student, StudentDetails details)
        {
            string plainPassword = GeneratePassword(6, 8);
            student.Password = AESEncryption.Encrypt(plainPassword);

            student.StudentDetails = details;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            var notifier = new NotifyTwilio();
            await notifier.SendSmsAsync(student.ParPhone,
                $"Welcome {student.SName}, your portal password is: {plainPassword}");
            return student;
        }


        public async Task<Student?> GetStudentById(int id)
        {
            return await _context.Students
                .Include(s => s.Course)
                .Include(s => s.Role)
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.SId == id);
        }

        public async Task<ICollection<Student>> GetAllStudents()
        {
            return await _context.Students
                .Include(s => s.Course)
                .Include(s => s.StudentDetails)
                .ToListAsync();
        }

        public async Task<ICollection<Student>> FilterStudent(string name)
        {
            return await _context.Students
                .Include(s => s.StudentDetails)
                .Where(s => s.SName.Contains(name))
                .ToListAsync();
        }

        public async Task<bool> DeleteStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.SId == id);

            if (student == null) return false;

            if (student.StudentDetails != null)
            {
                student.StudentDetails.IsActive = false; // soft delete
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<Student?> UpdateStudent(int id, Student student, StudentDetails details)
        {
            // Fetch existing student with details
            var existing = await _context.Students
                .Include(s => s.StudentDetails)
                .FirstOrDefaultAsync(s => s.SId == id);

            if (existing == null) return null;

            // ✅ Update base student fields only if provided
            if (!string.IsNullOrWhiteSpace(student.SName))
                existing.SName = student.SName;

            if (student.CourseId != 0)
                existing.CourseId = student.CourseId;

            if (student.SImage != null)
                existing.SImage = student.SImage;

            if (student.Dob != default)
                existing.Dob = student.Dob;

            if (!string.IsNullOrWhiteSpace(student.BloodGrp))
                existing.BloodGrp = student.BloodGrp;

            if (!string.IsNullOrWhiteSpace(student.ParAddress))
                existing.ParAddress = student.ParAddress;

            if (!string.IsNullOrWhiteSpace(student.ParPhone))
                existing.ParPhone = student.ParPhone;

            if (!string.IsNullOrWhiteSpace(student.ParEmail))
                existing.ParEmail = student.ParEmail;

            // ✅ Update StudentDetails only if provided
            if (existing.StudentDetails != null)
            {
                if (details.Category != 0)
                    existing.StudentDetails.Category = details.Category;

                // ⚡ Use conditionals for each bool
                if (details.IsSports != existing.StudentDetails.IsSports)
                    existing.StudentDetails.IsSports = details.IsSports;

                if (details.IsMerit != existing.StudentDetails.IsMerit)
                    existing.StudentDetails.IsMerit = details.IsMerit;

                if (details.IsFG != existing.StudentDetails.IsFG)
                    existing.StudentDetails.IsFG = details.IsFG;

                if (details.IsWaiver != existing.StudentDetails.IsWaiver)
                    existing.StudentDetails.IsWaiver = details.IsWaiver;

                if (details.IsActive != existing.StudentDetails.IsActive)
                    existing.StudentDetails.IsActive = details.IsActive;
            }
            else
            {
                existing.StudentDetails = details;
            }

            await _context.SaveChangesAsync();
            await pay_repo.RefreshPaymentForStudentAsync(id);
            return existing;
        }

        public async Task<ICollection<Student>> FilterStudent(
           string? name,
           bool? isActive,
           string? aid,
           StudentCategory? category,
           string? bloodGroup,
           DateOnly? dob
        )
        {
            var query = _context.Students
                .Include(s => s.StudentDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => s.SName.Contains(name));

            if (isActive != null)
                query = query.Where(s => s.StudentDetails.IsActive == isActive);

            if (aid != null)
                query = query.Where(s => s.AdmissionId == aid);

            if (category != null)
                query = query.Where(s => s.StudentDetails.Category == category);

            if (!string.IsNullOrWhiteSpace(bloodGroup))
                query = query.Where(s => s.BloodGrp == bloodGroup);

            if (dob != null)
                query = query.Where(s => s.Dob == dob);

            return await query.ToListAsync();
        }

        public async Task<string> GenerateAdmissionIdAsync()
        {
            // Fetch the latest AdmissionId from the DB
            var lastStudent = await _context.Students
                .OrderByDescending(s => s.SId)   // order by primary key or AdmissionId
                .FirstOrDefaultAsync();

            if (lastStudent == null || string.IsNullOrEmpty(lastStudent.AdmissionId))
                return "GVS001"; // first student

            // Extract the numeric part (after "GVS")
            string lastId = lastStudent.AdmissionId.Replace("GVS", "");
            if (int.TryParse(lastId, out int number))
            {
                number++; // increment
                return "GVS" + number.ToString("D3"); // pad with leading zeros (3 digits)
            }

            // fallback if format is wrong
            return "GVS001";
        }

        public async Task<ICollection<Student>> GetStudentsByCourseId(int courseId)
        {
            var students = _context.Students
                 .Include(s => s.StudentDetails)
                 .Where(s => s.CourseId == courseId);
            return await students.ToListAsync();
        }


        private string GeneratePassword(int minLength = 6, int maxLength = 8)
        {
            var random = new Random();
            int length = random.Next(minLength, maxLength + 1);

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";

            // Ensure at least one of each type
            var passwordChars = new List<char>
            {
                upper[random.Next(upper.Length)],
                lower[random.Next(lower.Length)],
                digits[random.Next(digits.Length)],
               special[random.Next(special.Length)]
             };

            // Fill remaining characters randomly from all categories
            string allChars = upper + lower + digits + special;
            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters
            return new string(passwordChars.OrderBy(x => random.Next()).ToArray());
        }


        public async Task<(bool Success, List<string> Errors)> BulkInsertStudentsAsync(ICollection<StdClassDTO> stud)
        {
            var errors = new List<string>();

            if (stud == null || stud.Count == 0)
                return (false, new List<string> { "Student list cannot be empty" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var s in stud)
                {
                    try
                    {

                        if (s.roleId != 1)
                        {
                            errors.Add($"Invalid RoleId for student {s.sName}. RoleId must be 1.");
                            continue;
                        }

                        bool emailExists = await _context.Students.AnyAsync(x => x.ParEmail == s.parEmail);
                        if (emailExists)
                        {
                            errors.Add($"Email '{s.parEmail}' already exists for student {s.sName}.");
                            continue;
                        }

                        var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == s.courseId);
                        if (course == null)
                        {
                            errors.Add($"CourseId '{s.courseId}' does not exist for student {s.sName}.");
                            continue;
                        }
                        if (!course.IsActive)
                        {
                            errors.Add($"CourseId '{s.courseId}' is not active for student {s.sName}.");
                            continue;
                        }

                        if (errors.Count > 0)
                        {
                            await transaction.RollbackAsync();
                            return (false, errors);
                        }
                        // Create StudentDetails
                        var studentDetails = new StudentDetails
                        {
                            Category = Enum.Parse<StudentCategory>(s.category),
                            IsSports = s.isSports,
                            IsMerit = s.isMerit,
                            IsFG = s.isFG,
                            IsWaiver = s.isWaiver,
                            AdmissionDate = DateTime.UtcNow,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.StudentDetails.Add(studentDetails);
                        await _context.SaveChangesAsync();

                        // Handle image safely
                        byte[]? imageBytes = null;
                        if (!string.IsNullOrEmpty(s.sImage))
                        {
                            try
                            {
                                if (File.Exists(s.sImage))
                                {
                                    imageBytes = File.ReadAllBytes(s.sImage);
                                }
                                else
                                {
                                    errors.Add($"Image file not found for {s.sName}: {s.sImage}");
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error reading image for {s.sName}: {ex.Message}");
                            }
                        }
                        string aid = await GenerateAdmissionIdAsync();

                        var plainPassword = GeneratePassword(6, 8);
                        var student = new Student
                        {
                            SName = s.sName,
                            SImage = imageBytes,
                            Dob = DateOnly.Parse(s.dob),
                            BloodGrp = s.bloodGrp,
                            AdmissionId = aid,
                            ParAddress = s.parAddress,
                            ParPhone = s.parPhone,
                            ParEmail = s.parEmail,
                            RoleId = 1,
                            CourseId = s.courseId,
                            SdId = studentDetails.SdId,
                            Password = AESEncryption.Encrypt(plainPassword),
                            StudentDetails = studentDetails
                        };

                        try
                        {
                            var notifier = new NotifyTwilio();
                            await notifier.SendSmsAsync(student.ParPhone,
                                $"---- Welcome {student.SName}, your FeePortal password is: {plainPassword} ---");
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Failed to send SMS to {student.SName} {e.Message}");
                        }

                        _context.Students.Add(student);
                        await _context.SaveChangesAsync();

                        decimal totalFees = course.TutionFees + course.DonationFee + course.BusFees;
                        if (studentDetails.Category == StudentCategory.Hostel)
                            totalFees += course.HostelFees;

                        decimal totWaiver = 0;
                        if (studentDetails.IsSports) totWaiver += course.Sports;
                        if (studentDetails.IsMerit) totWaiver += course.Merit;
                        if (studentDetails.IsFG) totWaiver += course.FG;
                        if (studentDetails.IsWaiver) totWaiver += course.Waiver;

                        decimal feesToPay = Math.Max(0, totalFees - totWaiver);

                        var feePayment = new FeePayment
                        {
                            SId = student.SId,
                            CourseId = course.CourseId,
                            Penalty = 0,
                            TotalFees = totalFees,
                            TotWaiver = totWaiver,
                            FeesToPay = feesToPay,
                            PaidAmt = 0,
                            Balance = feesToPay,
                            IsPending = true,
                            IsActive = true,
                            DueDate = DateTime.UtcNow.AddMonths(1),
                            AcademicYear = DateTime.UtcNow.Year.ToString(),
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.FeePayments.Add(feePayment);
                    }
                    catch (Exception exStudent)
                    {
                        errors.Add($"Failed to insert student {s.sName}: {exStudent.Message}");
                    }
                }
                if (errors.Count > 0)
                {
                    await transaction.RollbackAsync();
                    return (false, errors);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                errors.Add($"Bulk insert failed: {ex.Message}");
                return (false, errors);
            }
        }

        public async Task<Student?> GetStudentByAdmissionId(string admissionId)
        {
            return await _context.Students
                .Include(s => s.Course)
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.AdmissionId == admissionId);
        }

        public Task<bool> ValidatePassword(int id, string password)
        {
            var student = _context.Students.Find(id);
            if (student == null) return Task.FromResult(false);
            Console.WriteLine($"---Password is : {AESEncryption.Decrypt(student.Password)} ---");
            return Task.FromResult(AESEncryption.Decrypt(student.Password) == password);
        }
        public async Task<IEnumerable<StudentCredentialDTO>> GetAllUserCredentialsAsync()
        {
            var users = await _context.Students.Include(c => c.Course).ToListAsync();

            return users.Select(u => new StudentCredentialDTO
            {
                SName = u.SName,
                Sclass = u.Course!.CourseName,
                AdmissionId = u.AdmissionId,
                Password = AESEncryption.Decrypt(u.Password)
            });
        }

    }
}
