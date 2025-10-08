namespace SchoolFeeManagementApi.Interface
{
    public interface IUser
    {
        Task<string> Login(string username, string password);
       

    }
}
