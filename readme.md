console app on .NET8 that act as mvc server
just a simple rest api that built in c#
it uses packages Newtonsoft.Json,Microsoft.AspNetCore.App,Microsoft.AspNetCore.Hosting

endpoints:
    POST /api/login
    requires body: {"username":"example","password":"example"}
    returns body: {"token":"example","statusCode":"example","username":"example"}

    POST /api/register
    requires body: {"username":"example","password":"example"}
    returns body: {"statusCode":"example"}

    POST /api/logout
    requires body: {"token":"example"}
    returns body: {"statusCode":"example"}

    GET /api/test
    returns body: true/false
