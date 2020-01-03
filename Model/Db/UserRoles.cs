namespace authService.Model.Db
{
    public sealed class UserRole
    {
        private readonly string Role;

        private UserRole(string role)
        {
            Role = role;
        }
    }
}
