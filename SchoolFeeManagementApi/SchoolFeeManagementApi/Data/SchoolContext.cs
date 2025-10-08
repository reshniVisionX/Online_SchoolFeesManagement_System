using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<School> Schools { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<FeePayment> FeePayments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----------------------
            // Relationships
            // ----------------------
         

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudentDetails)
                .WithOne(sd => sd.Student)
                .HasForeignKey<Student>(s => s.SdId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.School)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SclId);

            modelBuilder.Entity<FeePayment>()
                .HasOne(fp => fp.Student)
                .WithMany(s => s.FeePayments)
                .HasForeignKey(fp => fp.SId);

            modelBuilder.Entity<FeePayment>()
                .HasOne(fp => fp.Course)
                .WithMany(c => c.FeePayments)
                .HasForeignKey(fp => fp.CourseId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.SId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.FeePayment)
                .WithMany()
                .HasForeignKey(t => t.FeeId);

            // ----------------------
            // Enum conversions
            // ----------------------
            modelBuilder.Entity<StudentDetails>()
                .Property(s => s.Category)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.PayType)
                .HasConversion<string>()
                .HasMaxLength(10);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(10);

            modelBuilder.Entity<FeePayment>()
    .HasOne(fp => fp.Student)
    .WithMany(s => s.FeePayments)
    .HasForeignKey(fp => fp.SId)
    .OnDelete(DeleteBehavior.Restrict); // keep this

            modelBuilder.Entity<FeePayment>()
                .HasOne(fp => fp.Course)
                .WithMany(c => c.FeePayments)
                .HasForeignKey(fp => fp.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // avoid multiple cascade paths


            // seeding data



            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Student" },
                new Role { RoleId = 2, RoleName = "Admin" }
            );



            // ----------------------
            // Seed School
            // ----------------------
            var seedDate = new DateTime(2025, 7, 2, 12, 12, 12);
            var imageBytes = File.ReadAllBytes(@"C:\Users\sivat\Downloads\elgi.jpg");
            var logoBytes = File.ReadAllBytes(@"C:\Users\sivat\Downloads\logo.png");



            modelBuilder.Entity<School>().HasData(
                new School
                {
                    SclId = 1,
                    Name = "Green Valley International School",
                    City = "Coimbatore",
                    Logo = logoBytes,
                    Address = "123 Trinity Street, cross cut road 560066",
                    Phone = "8870203743",
                    FoundedAt = new DateOnly(2005, 6, 15),
                    Rating = 4.5,
                    Image = imageBytes,
                    CreatedAt = seedDate
                }
            );

            // ----------------------
            // Seed Courses (1st to 12th with 2 sections each)
            // ----------------------
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 101, CourseName = "1st A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 25000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 102, CourseName = "1st B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 25000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 103, CourseName = "2nd A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 26000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 104, CourseName = "2nd B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 26000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 105, CourseName = "3rd A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 27000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 106, CourseName = "3rd B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 27000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 107, CourseName = "4th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 28000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 108, CourseName = "4th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 28000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 109, CourseName = "5th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 29000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 110, CourseName = "5th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 29000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 111, CourseName = "6th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 30000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 112, CourseName = "6th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 30000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 113, CourseName = "7th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 31000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 114, CourseName = "7th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 31000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 115, CourseName = "8th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 32000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 116, CourseName = "8th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 32000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 117, CourseName = "9th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 33000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 118, CourseName = "9th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 33000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 119, CourseName = "10th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 34000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 120, CourseName = "10th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 34000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 121, CourseName = "11th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 35000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 122, CourseName = "11th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 35000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 123, CourseName = "12th A", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 36000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m },
                new Course { CourseId = 124, CourseName = "12th B", SclId = 1, CreatedAt = seedDate, IsActive = true, TutionFees = 36000m, BusFees = 15000m, HostelFees = 45000m, DonationFee = 5000m, Sports = 2000m, Merit = 5000m, FG = 3000m, Waiver = 10000m }
            );

            // ----------------------
            // Seed Students
            // ----------------------
            var std1Bytes = File.ReadAllBytes(@"C:\Users\sivat\Downloads\girl.jpg");
            var std2Bytes = File.ReadAllBytes(@"C:\Users\sivat\Downloads\kid.jpg");


            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    SId = 501,
                    SName = "Aarav Sharma",
                    CourseId = 101, // 1st A
                    RoleId = 1,
                    SdId = 1,
                    SImage = std1Bytes,
                    Password = "pass123",
                    Dob = new DateOnly(2014, 5, 12),
                    BloodGrp = "O+",
                    AdmissionId = "GVS001",
                    ParAddress = "456 Park Avenue, Indiranagar, Bangalore - 560038",
                    ParPhone = "9876543210",
                    ParEmail = "reshni1975@email.com"
                },
                new Student
                {
                    SId = 502,
                    SName = "Priya Patel",
                    CourseId = 124, // 12th B
                    RoleId = 1,
                    SdId = 2,
                    SImage = std2Bytes,
                    Password = "pass123",
                    Dob = new DateOnly(2007, 8, 23),
                    BloodGrp = "B+",
                    AdmissionId = "GVS002",
                    ParAddress = "789 Lake View Apartments, Koramangala, Bangalore - 560034",
                    ParPhone = "9876543211",
                    ParEmail = "patel.parent@email.com"
                }
            );

            // ----------------------
            // Seed StudentDetails
            // ----------------------
            modelBuilder.Entity<StudentDetails>().HasData(
                new StudentDetails
                {
                    SdId = 1,
                    Category = StudentCategory.DayScholar,

                    IsSports = true,
                    IsMerit = false,
                    IsFG = true,
                    IsWaiver = false,
                    AdmissionDate = new DateTime(2023, 6, 1),
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new StudentDetails
                {
                    SdId = 2,
                    Category = StudentCategory.Hostel,
                    IsSports = false,
                    IsMerit = true,
                    IsFG = false,
                    IsWaiver = false,
                    AdmissionDate = new DateTime(2022, 6, 1),
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );

            // ----------------------
            // Seed FeePayments
            // ----------------------
            modelBuilder.Entity<FeePayment>().HasData(
                new FeePayment
                {
                    FeeId = 1,
                    SId = 501,
                    CourseId = 101,
                    Penalty = 0,
                    TotalFees = 42000,
                    TotWaiver = 3000,
                    FeesToPay = 39000,
                    PaidAmt = 15000,
                    Balance = 24000,
                    IsPending = true,
                    IsActive = true,
                    UpdatedAt = seedDate,
                    AcademicYear = "2024-2025"
                },
                new FeePayment
                {
                    FeeId = 2,
                    SId = 502,
                    CourseId = 124,
                    Penalty = 500,
                    TotalFees = 86000,
                    TotWaiver = 5000,
                    FeesToPay = 81000,
                    PaidAmt = 81000,
                    Balance = 0,
                    IsPending = false,
                    IsActive = true,
                    UpdatedAt = seedDate,
                    AcademicYear = "2024-2025"
                }
            );

            // ----------------------
            // Seed Transactions
            // ----------------------
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    TransId = 1,
                    SId = 501,
                    FeeId = 1,
                    PayType = PaymentType.UPI,
                    Amount = 15000,
                    Status = TransactionStatus.Success,
                    DateTime = new DateTime(2024, 9, 1, 10, 30, 0)
                },
                new Transaction
                {
                    TransId = 2,
                    SId = 502,
                    FeeId = 2,
                    PayType = PaymentType.Card,
                    Amount = 40000,
                    Status = TransactionStatus.Success,
                    DateTime = new DateTime(2024, 8, 15, 14, 20, 0)
                },
                new Transaction
                {
                    TransId = 3,
                    SId = 502,
                    FeeId = 2,
                    PayType = PaymentType.Card,
                    Amount = 41000,
                    Status = TransactionStatus.Success,
                    DateTime = new DateTime(2024, 8, 16, 11, 45, 0)
                }
            );

            // ----------------------
            // Seed Feedback
            // ----------------------
            modelBuilder.Entity<Feedback>().HasData(
                new Feedback
                {
                    FeedId = 1,
                    EmailId = "reshni1975@email.com",
                    Query = "Excellent teaching staff and good facilities. The fee structure is reasonable.",
                    Rating = 5,
                    CreatedAt = new DateTime(2024, 8, 15, 14, 20, 0)
                },
                new Feedback
                {
                    FeedId = 2,
                    EmailId = "priya.parent@email.com",
                    Query = "Great hostel facilities and supportive administration. Would recommend to others.",
                    Rating = 4,
                    CreatedAt = new DateTime(2024, 8, 15, 14, 20, 0)
                }
            );

        }
    }
}
