namespace Aufgaben_Managment_Tool
{
    internal class UserRepository
    {
        private const string FilePath = "users.json";

        public List<User> LoadUsers()
        {
            return StorageManager<User>.Load(FilePath);
        }

        public void SaveUsers(List<User> users)
        {
            StorageManager<User>.Save(FilePath, users);
        }

    }
}
