using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.Data;
using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Repository
{
    public class PaymentRepository : IPayment
    {
        private readonly SchoolContext _context;

        public PaymentRepository(SchoolContext context)
        {
            _context = context;
        }

        public async Task<ICollection<FeePayment>> GetAllPayment()
        {
            return await _context.FeePayments
                .Include(f => f.Student)
                .Include(f => f.Course)
                .Include(f => f.Transactions)
                .ToListAsync();
        }

 
        public async Task<ICollection<FeePaymentDTO>> GetAllPaymentByCourseId(int courseId)
        {
            
            var payments = await _context.FeePayments
                .Include(f => f.Course)
                .Where(f => f.CourseId == courseId)
                .ToListAsync();

            var today = DateTime.Now.Date;

            foreach (var payment in payments)
            {
                if (payment.DueDate < today && payment.IsPending)
                {
                    int overdueDays = (today - payment.DueDate.Date).Days;
                    payment.Penalty = overdueDays * 100;
                  
                    payment.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            return await _context.FeePayments
                .Where(f => f.CourseId == courseId)
                .Select(f => new FeePaymentDTO
                {
                    SId = f.Student!.SId,
                    StudentName = f.Student!.SName,
                    CourseName = f.Course!.CourseName,
                    AcademicYear = f.AcademicYear,
                    Penalty = f.Penalty,
                    TotalFees = f.TotalFees,
                    DueDate = f.DueDate,
                    TotWaiver = f.TotWaiver,
                    FeesToPay = f.FeesToPay,
                    PaidAmt = f.PaidAmt,
                    Balance = f.Balance,
                    IsPending = f.IsPending,
                    UpdatedAt = f.UpdatedAt
                })
                .ToListAsync();
        }


        public async Task<FeePayment?> GetPaymentById(int sId)
        {
            var payment = await _context.FeePayments
                .Include(f => f.Student)
                .Include(f => f.Course)
                .FirstOrDefaultAsync(f => f.SId == sId);

            if (payment != null && payment.DueDate < DateTime.Now.Date && payment.IsPending)
            {
                int overdueDays = (DateTime.Now.Date - payment.DueDate.Date).Days;
                payment.Penalty = overdueDays * 100;
                payment.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return payment; 
        }



        public async Task<bool> RefreshPaymentsForCourse(int courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null) return false;

                var students = await _context.Students
                    .Include(s => s.StudentDetails)
                    .Include(s => s.FeePayments)
                    .Where(s => s.CourseId == courseId)
                    .ToListAsync();

                foreach (var student in students)
                {
                    var sd = student.StudentDetails!;
                    if (sd == null) continue; 

                    decimal totalFeesNew = course.TutionFees + course.DonationFee + course.BusFees;
                    if (sd.Category == StudentCategory.Hostel)
                        totalFeesNew += course.HostelFees;

                    decimal totWaiverNew = 0;
                    if (sd.IsSports) totWaiverNew += course.Sports;
                    if (sd.IsMerit) totWaiverNew += course.Merit;
                    if (sd.IsFG) totWaiverNew += course.FG;
                    if (sd.IsWaiver) totWaiverNew += course.Waiver;

                    decimal feesToPayNew = Math.Max(0, totalFeesNew - totWaiverNew);

                    var feePayment = student.FeePayments.FirstOrDefault()
                                     ?? new FeePayment { SId = student.SId, CourseId = course.CourseId };

                    decimal feeToPayOld = feePayment.TotalFees - feePayment.TotWaiver;

                    Console.WriteLine(" Course - Old Fee " + feeToPayOld + " New fees " + feesToPayNew);
                 
                    decimal diff = feesToPayNew - feeToPayOld;
                    Console.WriteLine("Diff : "+ diff);
                    if (diff != 0)
                    {
                        feePayment.TotalFees = totalFeesNew;
                        feePayment.TotWaiver = totWaiverNew;

                        decimal totPay = feePayment.TotalFees - feePayment.TotWaiver;
                        feePayment.FeesToPay = totPay - feePayment.PaidAmt;

                        Console.WriteLine("Fee to pay for course : "+ feePayment.FeesToPay);
                        if (feePayment.FeesToPay <= 0 || (feePayment.PaidAmt == totPay))
                        {
                            feePayment.FeesToPay = 0;
                            feePayment.Balance = totPay - feePayment.PaidAmt;
                            feePayment.IsPending = false;
                        }else if (diff > 0 && !feePayment.IsPending)
                            feePayment.IsPending = true;
                    }

                     
                    feePayment.UpdatedAt = DateTime.UtcNow;

                    if (feePayment.FeeId == 0)
                        _context.FeePayments.Add(feePayment);
                    else
                        _context.FeePayments.Update(feePayment);
                }

                // 9️⃣ Save all changes
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> RefreshPaymentForStudent(int studentId)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.StudentDetails)
                    .Include(s => s.Course)
                    .Include(s => s.FeePayments)
                    .FirstOrDefaultAsync(s => s.SId == studentId);

                if (student == null || student.StudentDetails == null || student.Course == null)
                    return false;

                var course = student.Course;
                var sd = student.StudentDetails;

                decimal totalFeesNew = course.TutionFees + course.DonationFee + course.BusFees;
                if (sd.Category == StudentCategory.Hostel)
                    totalFeesNew += course.HostelFees;

             

                decimal totWaiverNew = 0;
                if (sd.IsSports) totWaiverNew += course.Sports;
                if (sd.IsMerit) totWaiverNew += course.Merit;
                if (sd.IsFG) totWaiverNew += course.FG;
                if (sd.IsWaiver) totWaiverNew += course.Waiver;

               

             decimal feesToPayNew = Math.Max(0, totalFeesNew - totWaiverNew);
    
                var feePayment = student.FeePayments.FirstOrDefault()
                                 ?? new FeePayment { SId = student.SId, CourseId = course.CourseId };

                decimal feeToPayOld = feePayment.TotalFees - feePayment.TotWaiver;

                Console.WriteLine("Old Fee " + feeToPayOld+ " New fees " + feesToPayNew);
               
                decimal diff = feesToPayNew - feeToPayOld  ;

                Console.WriteLine("Diff : "+ diff);
                if (diff != 0)
                {
                    feePayment.TotalFees = totalFeesNew;
                    feePayment.TotWaiver = totWaiverNew;

                    decimal totPay = feePayment.TotalFees - feePayment.TotWaiver;
                    feePayment.FeesToPay = totPay - feePayment.PaidAmt;

                    Console.WriteLine("Fee to pay : "+ feePayment.FeesToPay);

                    if (feePayment.FeesToPay <= 0 || (feePayment.PaidAmt == totPay))
                    {
                        feePayment.FeesToPay = 0;
                        feePayment.IsPending = false;
                        feePayment.Balance = totPay - feePayment.PaidAmt;
                    }else  if (diff > 0 && !feePayment.IsPending)
                        feePayment.IsPending = true;
                }

                feePayment.UpdatedAt = DateTime.UtcNow;


                if (feePayment.FeeId == 0)
                    _context.FeePayments.Add(feePayment);
                else
                    _context.FeePayments.Update(feePayment);

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
