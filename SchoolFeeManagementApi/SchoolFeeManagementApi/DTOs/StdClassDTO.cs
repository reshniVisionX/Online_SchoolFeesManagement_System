namespace SchoolFeeManagementApi.DTOs
{
    public class StdClassDTO
    {
        public string sName { get; set; }
        public string sImage { get; set; }
        public string dob { get; set; }
        public string bloodGrp { get; set; }
        public string parAddress { get; set; }
        public string parPhone { get; set; }
        public string parEmail { get; set; }
        public string category { get; set; }
        public bool isSports { get; set; }
        public bool isMerit { get; set; }
        public bool isFG { get; set; }
        public bool isWaiver { get; set; }
        public int roleId { get; set; }
        public int courseId { get; set; }
    }

}
