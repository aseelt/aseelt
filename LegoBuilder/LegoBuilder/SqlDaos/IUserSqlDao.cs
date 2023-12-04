using LegoBuilder.Models;

namespace LegoBuilder.SqlDaos
{
    public interface IUserSqlDao
    {
        string DefaultRole { get; }
        string AdminRole { get; }
        bool Inactive { get; }
        bool Active { get; }
        public User CreateUser(NewUser incomingUserParam);
        public User GetUserById(int id);
        public User GetUserByUsername(string username);

        public User IncreaseUserRole(string username);
        public User DecreaseUserRole(string username);
        public User DeactivateUser(string username);
        public User ReactivateUser(string username);
    }
}
