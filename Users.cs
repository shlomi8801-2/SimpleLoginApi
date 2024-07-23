using System.Text;

namespace BackendApi.Controllers;

public  class Users
{
    private static Random rand = new Random();
    public static string GenerateRandomString(int length){
        return string.Join("",Enumerable.Range(0, length).Select(_ => Convert.ToChar(rand.Next(48,123))).ToList());
    }
    public static string ConvertToMd5(string? input)
    {
        return input != null ? Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input))) : "";
    }
    public static string ConvertToBase64(string? input){
        if(input == null) return "";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }
    public static string ConvertFromBase64(string? input)
    {
        if(input == null) return "";
        return Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }
    public static bool CheckDbConnection()
    {
        // use the run cmd in DB class to create a users table if not exist and then check if the table has content if not add user admin and generate random password of 8 letters if error return false
        try
        {
            DB.RunCMDNoOut("CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY AUTOINCREMENT, username VARCHAR(32), password VARCHAR(32), email VARCHAR(32) NULL, role VARCHAR(32) NULL,accessToken VARCHAR(32) NULL)");            
            if (DB.RunCMD("SELECT * FROM users").Count == 0){
                // add a admin user with random 8 letter password
                string password = GenerateRandomString(8);
                //one time creating user with runcmd cuz if using AddUser it causes it to be ✨recursive✨ with endless loop
                // AddUser("admin", password);
                DB.RunCMDNoOut("INSERT INTO users (username, password, role) VALUES ('"+ConvertToBase64("admin")+"', '" + ConvertToMd5(password) + "', '" + "admin" + "')");
                Console.WriteLine("table users found empty added user \nusername:admin\npassword:" + password);
            }
            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
    public static string AddUser(string? Username, string? Password, string? Email = null, string? Role = null){
        //returns status code like http :)
        // use the run cmd in DB class to add user to users table
        if (!CheckDbConnection()) return "Error connecting to database";
        if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Username)) return "Username and password are required";
        Username = Username.Trim();
        Password = Password.Trim();
        if(Email != null) Email = Email?.Trim();
        // do the security checks for short password in the frontend
        
        if (SearchUserByUsername(Username).Count !=0) return "Username already exists";
        try
        {
            var hashedPassword = ConvertToMd5(Password);
            DB.RunCMDNoOut("INSERT INTO users (username, password, email, role) VALUES ('" + ConvertToBase64(Username) + "', '" + hashedPassword + "', '" + ConvertToBase64(Email) + "', '" + Role + "')");
            return "User added successfully";
        }
        catch(Exception e)
        {
            return "Error adding user: " + e.Message;
        }
    }
    public static string DeleteUser(string Username) {
        //returns status code like http :)
        // use the run cmd in DB class to delete user from users table
        if (CheckDbConnection() == false) return "Error connecting to database";
        Username = Username.Trim();
        if (SearchUserByUsername(Username).Count == 0) return "User not found";
        try
        {
            DB.RunCMDNoOut("DELETE FROM users WHERE username = '" + ConvertToBase64(Username) + "'");
            return "User deleted successfully";
        }
        catch (Exception e)
        {
            return "Error deleting user: " + e.Message;
        }
    }
    public static Dictionary<string, string?> SearchUserByUsername(string Username) // its the same as get user
    {
        // use the run cmd in DB class to search user by username
        if (CheckDbConnection() == false) return new Dictionary<string, string?>();

        HashSet<Dictionary<string, string?>> UserObj = DB.RunCMD("SELECT * FROM users WHERE username = '" + ConvertToBase64(Username) + "'");
        if (UserObj.Count == 0) return new Dictionary<string, string?>();

        // check for multiple users with the same username
        if (UserObj.Count > 1) throw new Exception("Error: Multiple users found with the same username");
        return UserObj.First();
    }





    public static string SetAccessToken(string Username, string? AccessToken = null){
        if (CheckDbConnection() == false) return "Error connecting to database";
        if (SearchUserByUsername(Username).Count == 0) return "User not found";
        AccessToken = string.IsNullOrEmpty(AccessToken) ? GenerateRandomString(32) : AccessToken;
        DB.RunCMDNoOut("UPDATE users SET accessToken = '" + AccessToken + "' WHERE username = '" + ConvertToBase64(Username.Trim()) + "'");
        return AccessToken;
    }




    // this is a basic json
    public static Dictionary<string, string?> Login(string? Username, string? Password){
        // returns a dictionary with code username and accessToken
        Dictionary<string, string?> Output = new Dictionary<string, string?>();
        var StatusCode = "success";
        if (CheckDbConnection() == false) StatusCode = "Error connecting to database";
        if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Username)) StatusCode = "Username and password are required";
        else {
        Username = Username.Trim();
        Password = Password.Trim();
            Dictionary<string, string?> UserObj = SearchUserByUsername(Username);
            var hashedPassword = ConvertToMd5(Password);
            if (UserObj.Count == 0 || UserObj["password"] != hashedPassword){
                StatusCode = "username or password are incorrect";
            }else {
            //so far it checked for username and password and connection to database if all is good then 👍 and can continue with sending the data back
            Output.Add("username", Username);
            Output.Add("accessToken", SetAccessToken(Username));
            }
            
            };
        Output.Add("statusCode", StatusCode);
        return Output;
    }
    public static Dictionary<string, string?> Logout(string? AccessToken){
        // returns just status code
        Dictionary<string, string?> Output = new Dictionary<string, string?>();
        var StatusCode = "success";
        if (CheckDbConnection() == false) StatusCode = "Error connecting to database";
        if (string.IsNullOrEmpty(AccessToken)) StatusCode = "Access token is required";
        else DB.RunCMDNoOut("UPDATE users SET accessToken = '' WHERE accessToken = '" + AccessToken + "'");
        Output.Add("statusCode", StatusCode);
        return Output;
    }
}