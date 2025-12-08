namespace Aufgaben_Managment_Tool
{
    internal static class UserRepository
    {
        private const string FilePath = "users.json";

        public static List<User> LoadUsers()
        {
            return StorageManager<User>.Load(FilePath);
        }

        public static void SaveUsers(List<User> users)
        {
            StorageManager<User>.Save(FilePath, users);
        }
    }
}
