namespace Aufgaben_Managment_Tool
{
    internal class UserRepository
    {
        private const string FilePath = "users.json";

        public List<User> LoadUsers()
        {
            return JsonSafeLoad<User>.Load(FilePath);
        }

        public void SaveUsers(List<User> users)
        {
            JsonSafeLoad<User>.Save(FilePath, users);
        }

    }
}
