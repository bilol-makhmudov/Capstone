namespace Capstone.ViewModels.User;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsLocked { get; set; }
}