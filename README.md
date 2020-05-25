# Project Template of ASP.NET Core Web API with API documentation
This is a C# project template for the .NET Core Template Engine. It contains a simple ASP.NET Core Web API with API documentation by NSwag.

## Installation
You can install this template via NuGet package.
```
dotnet new -i Ycode.RestApi
```
After the installation completes, this command will show a list of templates. You can find `restapi` in it.

## How to use template
You can create a working ASP.NET Core Web API project using this template.
```
mkdir Example
cd Example
dotnet new restapi --swaggerTitle="My API"
dotnet run
```
Now, open [https://localhost:62182/swagger](https://localhost:62182/swagger) in your browser, then you will see Swagger UI of your new Web API.

You can try running the API via this web UI. Expand `Example` and `GET /api/v1/Example`, and click on `Try it out` button and `Execute` button, then you will see the response from the API.
![API Document Web UI](https://github.com/y-code/RestApi/raw/master/doc/images/swagger-ui.png)
