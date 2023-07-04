namespace JWT_post.Models;

public static class UserConstants
{
    public static List<UserModel> Users = new()
    {
        new UserModel {Username = "login", Password = "pass", Role = "Admin"}
    };
}