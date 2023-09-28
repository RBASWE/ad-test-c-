using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ad.api.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    ILogger<LoginController> _logger;

    private string RemoteIpAdress => HttpContext.Connection.RemoteIpAddress.ToString();
    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet("HelloWorld")]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [Authorize]
    [HttpGet("HelloWorldAuth")]
    public string HelloWorldAuth()
    {
        return "Hello World now with Auth";
    }


    [HttpGet("Username")]
    public async Task<string> GetUsername()
    {
        string message = "";
        var user = HttpContext.User.Identity!;
        var userIdNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        Domain domain = Domain.GetCurrentDomain();
        //await WindowsIdentity.RunImpersonatedAsync(user.AccessToken, async () =>
        //{
        //    var impersonatedUser = WindowsIdentity.GetCurrent();
        //    message =
        //        $"User: {impersonatedUser.Name}\t" +
        //        $"State: {impersonatedUser.ImpersonationLevel}";

        //});

        return $"{user.IsAuthenticated} {user.Name} {user.AuthenticationType}";
    }

    [HttpPost(Name = "login")]
    public async Task<IActionResult>Login([FromBody] LoginModel creds)
    {
        creds.IpAdress = RemoteIpAdress;
        bool isAuthenticated = await ValidateUser(creds);

        if (isAuthenticated) return Ok(creds);
        else return BadRequest("not authenticated");
    }


    private async Task<bool> ValidateUser(LoginModel creds)
    {
        string domainName = "";
        try
        {
            Domain domain = Domain.GetCurrentDomain();
            domainName = domain.Name;
            Console.WriteLine("Domain Name: " + domainName);
        }
        catch (ActiveDirectoryObjectNotFoundException ex)
        {
            throw new Exception("Domain not found: " + ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }

        try
        {
            using var context = new PrincipalContext(ContextType.Domain, domainName);
            bool isAuthenticated = context.ValidateCredentials(creds.Username, creds.Password);
            if (!isAuthenticated) throw new Exception("User not found");
            else return isAuthenticated;
        }
        catch (Exception ex)
        {
            // TODO
            _logger.LogError(ex.Message);
            return false;
        }
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? IpAdress { get; set; }
}
