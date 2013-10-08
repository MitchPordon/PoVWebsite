using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PoVWebsite.Models;

namespace PoVWebsite.Content.Utility
{
    public static class General
    {
        internal static bool ValidUserCredentials(string username, string password, int id)
        {
            PoVEntities db = new PoVEntities();
            User user = db.Users.SingleOrDefault(m => m.id == id);
            if (user != null)
                if (user.username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && user.password.Equals(password, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;

        }

        internal static Dictionary<string, string> DataToDictionary(string input, string[] firstSeperators, string[] secondSeperators)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] firstSplit = input.Split(firstSeperators, StringSplitOptions.RemoveEmptyEntries);
            for (int x = 0; x < firstSplit.Length; x++)
            {
                string[] temp = firstSplit[x].Split(secondSeperators, StringSplitOptions.RemoveEmptyEntries);
                result.Add(temp[0], temp[1]);
            }
            return result;
        }
    }
}