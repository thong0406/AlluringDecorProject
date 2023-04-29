using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlluringDecors.Models;
using Newtonsoft.Json;

namespace AlluringDecors
{
    public class Misc
    {
        public static bool UserIsRole(string username, UserRoles role)
        {
            using (var db = new AlluringDecorEntities())
            {
                user u = db.users.Where(e => e.Username == username).FirstOrDefault();
                user_role ur = db.user_role.Where(e => e.userId == u.Id).FirstOrDefault();
                switch (role)
                {
                    case (UserRoles.User):
                        return (ur.role.name == "Client");
                    case (UserRoles.Admin):
                        return (ur.role.name == "Admin");
                }
            }
            return false;
        }
        public static void AddToJson(string filePath, string key, string value)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string finalDir = dir + filePath;
            Dictionary<string, string> dict = LoadJson(filePath);
            dict.Add(key, value);
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            System.IO.File.WriteAllText(finalDir, json);
        }
        public static void UpdateJson(string filePath, string key, string value)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string finalDir = dir + filePath;
            Dictionary<string, string> dict = LoadJson(filePath);
            dict[key] = value;
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            System.IO.File.WriteAllText(finalDir, json);
        }
        public static void DeleteFromJson(string filePath, string key)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string finalDir = dir + filePath;
            Dictionary<string, string> dict = LoadJson(filePath);
            dict.Remove(key);
            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            System.IO.File.WriteAllText(finalDir, json);
        }
        public static Dictionary<string, string> LoadJson(string filePath)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string finalDir = dir + filePath;
            Dictionary<string, string> dict = null;
            using (System.IO.StreamReader r = new System.IO.StreamReader(finalDir))
            {
                string json = r.ReadToEnd();
                dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            return dict;
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public enum UserRoles
        {
            User,
            Admin
        }
    }
}