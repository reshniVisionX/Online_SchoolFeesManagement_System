namespace SchoolFeeManagementApi.DTOs
{
    public class UpdStudentsDTO
    {
        public string? SName { get; set; }

        public int? CourseId { get; set; }

        public byte[]? SImage { get; set; } 

        public DateOnly? Dob { get; set; }

        public string? BloodGrp { get; set; }


        public string? ParAddress { get; set; } = string.Empty;

        public string? ParPhone { get; set; } = string.Empty;

        public string? ParEmail { get; set; } = string.Empty;
        public StudentCategory? Category { get; set; } = StudentCategory.DayScholar;

        public bool? IsSports { get; set; } = false;

        public bool? IsMerit { get; set; } = false;

        public bool? IsFG { get; set; } = false;

        public bool? IsWaiver { get; set; } = false;
        public bool? IsActive { get; set; } = true;
    }
}
