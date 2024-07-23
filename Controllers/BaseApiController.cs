using System.Text;
using BackendApi.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace BackendApi_mvc;
//kestrel





[ApiController]
[Route("api")] //the url
public class BaseApiController : ControllerBase
{
    // [HttpGet] // only for the url and get request
    // public IActionResult Get()
    // {
    //     return Ok("Hello, World!");
    // }




    // [HttpGet("{text}")] // url + parameter for example http://localhost:5005/hello/segga
    // public IActionResult ReturnText(string text){
    //     return Ok(text);
    // }


    // also i didnt find a way to make it async, nvm see in startup.cs
    [HttpPost("test")]
    public IActionResult Post()
    {
        return Ok(Users.CheckDbConnection());
    }
    [HttpPost("register")]
    public IActionResult Register()
    {
        // return Ok(Users.AddUser(username, password));
        var body = Request.Body;
        using (StreamReader streamReader = new StreamReader(body))
        {
            // Read the stream and convert it to a string
            string bodydata = streamReader.ReadToEnd();

            Dictionary<string, object>? bodyjson = JsonConvert.DeserializeObject<Dictionary<string, object>>(bodydata);
            // use this checking to like remove the yellow null reference warnings
            if (bodyjson != null )
            {
                try
                {
                    bodyjson["username"].ToString();
                    bodyjson["password"].ToString();
                    string? username = bodyjson["username"].ToString();
                    string? password = bodyjson["password"].ToString();
                    return Ok(Users.AddUser(username, password));
                }
                catch{
                    //just the return is the same as later so why write again
                    
                }
            }
            return BadRequest("either username or password is null");
        }

    }
    [HttpPost("login")]
    public IActionResult Login()
    {
        var body = Request.Body;
        using (StreamReader streamReader = new StreamReader(body))
        {
            // Read the stream and convert it to a string
            string bodydata = streamReader.ReadToEnd();

            Dictionary<string, object>? bodyjson = JsonConvert.DeserializeObject<Dictionary<string, object>>(bodydata);
            // use this checking to like remove the yellow null reference warnings
            if (bodyjson != null )
            {
                try
                {
                    bodyjson["username"].ToString();
                    bodyjson["password"].ToString();
                    string? username = bodyjson["username"].ToString();
                    string? password = bodyjson["password"].ToString();
                    return Ok(Users.Login(username, password));
                }
                catch{
                    //just the return is the same as later so why write again
                }
            }
            return BadRequest("either username or password is null");
            
        }
    }
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var body = Request.Body;
        using (StreamReader streamReader = new StreamReader(body))
        {
            // Read the stream and convert it to a string
            string bodydata = streamReader.ReadToEnd();

            Dictionary<string, object>? bodyjson = JsonConvert.DeserializeObject<Dictionary<string, object>>(bodydata);
            // use this checking to like remove the yellow null reference warnings
            if (bodyjson!= null )
            {
                try
                {
                    bodyjson["accessToken"].ToString();
                    string? accessToken = bodyjson["accessToken"].ToString();
                    return Ok(Users.Logout(accessToken));
                }
                catch{
                    //just the return is the same as later so why write again
                }
            }
            return BadRequest("Access token is null");
            
        }
    }
}
