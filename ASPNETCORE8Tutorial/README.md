## 1. Create your first ASP.NET Core
### Creating a new minimal API application
1. Create an empty ASP.NET CORE project in visual studio
or
Creating a new minimal API application with the .NET CLI
dotnet new sln -n WebAppliaction1
dotnet new web -o WebApplication1
dotnet sln add WebApplication1

2. To compile:
a) Ctrl+Shift+B
b) dotnet build in command line
c) dotnet build in the package manger console

3) To run the application
PM> cd .\TestASPNETCORE    //CD to the project folder
donet restore  //Ensure NuGet dependencies have been downloaded locally and referenced correctly
dotnet build 
PM> dotnet run

4. All dependencies are in the csproj in PackageReferenceNode. dotnet restore uses this file to download NuGet packages.

5. dotnet run   runs  dotnet build  implicitly
6. dotnet build  runs donet restore implicitly
7. dotnet build --no-restore
8. dotnet run --no-restore --no-build  //if you do not want to build and restore explicitly!
9. Properties folder has a single file: launchSettings.json tells how visual studio runs and debug the application.

### NuGet
1. NuGet is the library package manager for .nET, where libraries are packaged in NugET packages and published to https://www.nuget.org
2. You can use these packages in your project by referencing the unique package name in your .csproject.

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.DataProtection" Version="8.0.4" />
  </ItemGroup>
</Project>

3. You can also add a package in CLI, which updates the project file.
dotnet add Microsoft.AspNetCore.DataProtection

### Program.cs file: Defining your application
All ASP.NET core applications start life as a .NET console application. This means a program is written with top-level statements, in which the startup code for your application is written directly in a file instead of a static void Main function.

### Top-level statements 
Top-level statements enable you to avoid the extra ceremony required by placing your program's entry point in a static method in a class. The typical starting point for a new console application looks like the following code:

using System;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}

The preceding code is the result of running the dotnet new console command and creating a new console application. Those 11 lines contain only one line of executable code. You can simplify that program with the new top-level statements feature and the compiler creates the rest for you. That enables you to remove all but two of the lines in this program:

Console.WriteLine("Hello, World!");

args are still available!
foreach(string arg in args)
{
    Console.WriteLine(arg);
}

### Web Applications
For web applications, we use builder object to configure an object, deplay its creation untill all configuration has finished.

var builder = WebApplication.CreateBuilder(args); //create a WebApplication builder instance
var app = builder.Build(); //Builds and returns an instance of the web application

app.MapGet("/", () => "Hello World!"); //Defines an endpoint for your application, which returns Hello World when "/" is called

app.Run(); //Runs the web application to start listening for requests and generating responses

## 2. WebApplicationbuilder
WebApplicationbuilder configures a lot of things by default, including:
1. Configuration: load configurations like database from JSON files
2. Logging:
3. Services: Any classes your application needs, must be registered so they can be instantiated correctly at runtime. WebApplicationBuilder configures minimal set of services needed for an ASP.NET Core App.
4. Hosting: ASP.NET Core uses the Kestrel web server by default to handle requests.

### Middleware
After configuring the WebApplicationBuilder, you call buid() function to create an instance of the web application. This instance handles and responds to requests using two building blocks:

1. Middleware: These small components execute in sequence when application receives an HTTP request. the can perform a whole host of functions such as logging, identifying the current user, serving static files, and handling errors.
2. Endpoints: Endpoints define how the response should be generated for a specific request to a URL in your app.


Here, there is no middleware, but we define a single endpoint using a call to MapGet.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!"); //handle reuqests using Get HTTP verb (we have MapPost)

app.Run();

## 3. The overall structure of a typical ASP.NET Core app entry point
1. Create a WebApplicationBuilder instance
2. Register the required servces and configuration with the WebApplicationBuilder
3. Call Build() on the builderi instance to create a WebApplication instance.
4. Add middleware to the WebApplication to create a pipeline.
5. Map the endpoints in your application.
6. Call Run() on the WebApplication to start the server and handle requests.

### Learning 2 new features

1. When running in the development environment, details about each request are logged using the HttpLoggingMiddleware.
2. Create an additional endpoint that creates an instance of the C# record called Person and serializes it in the response as JSON

__Note__:
A record in C# is a class or struct that provides special syntax and behavior for working with data models. The record modifier instructs the compiler to synthesize members that are useful for types whose primary role is storing data. 



using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(opts 
    => opts.LoggingFields = HttpLoggingFields.RequestProperties);

// The following filter is already configured in appsettings.Development.json
// builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/person", () => new Person("Hooman", "Salamat"));

app.Run();

public record Person(string FirstName, string LastName);


3. If you want to see the request in console, update your appsettings.development.json to include "Microsoft.AspNetCore.HttpLogging": "Information"

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.HttpLogging": "Information"
    }
  }
}

The following filter is already configured in appsettings.Development.json, but you can configure in the code as well!
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

### Adding and Configuring services
1. Services are small modular components that allow individual features to evolve separately.
2. Services refer to any class that provide functionality to an application.
3. Services could be classes exposed by a library or code you've written fior your application.
4. Each services provides a small piece of independent functionality, but you can combine them to cretae a complete application.
5. This design methodology scenario is known as the single-responsibility principle (SRP).
6. SRP states that every class should be responsible for only a single piece of functionality.
7. SRP is one of the five main design principle promoted by Robert C. Martin in Agile Software Development, Principles, Patterns, and Pratices.
8. When writing a service, you can declare your dependencies and let another class fill those dependecies for you. Your service can focus on the functionality for which it was designed instead of trying to work out how to build its dependecies. This technique is called dependency injection or the inversion of the control (IoC) principle.
9. Typically, you'll register the dependencies of your application into a container, which you can use to create any serivce.
10. You can use the container to create both your own custom application services and the framework services used by ASP.NET Core.
11. In ASP.NET Core application, this registration is performed by using the Services property of WebApplicationBuilder.
12. Whenever you use a new ASP.NET Core feature in your application, you need to come back to Program.cs and add the necessary services.
13. builder.Services.AddHttpLogging(opts 
    => opts.LoggingFields = HttpLoggingFields.RequestProperties);
    Calling AddHttpLogging() add the necessary services for HTTP logging middleware to the IoC container and customizes the options used by the middleware for what to display. AddHttpLogging is an extension method that encapsulate all the code required to setup HTTP logging.
14. As well as registering framework-related services, the Services property is where you'd register any custom services you have in your application. 
15. The services property is an IServiceCollection which is a list of every known service that your application will need to use.
16. By adding a new service to IServiceCollection, you ensure that whenever a class declares a dependency on your service, the IoC container will know how to provide it.
17. After you call build(), you can't register any more services or changing your logging information, the servie defined for the WebApplication instance are set in stone.

### How requests are handled with middleware and endpoints?
After registering your services with IoC container on WebApplicationBuilder, you create an application instance. You can do three things with the application instance:
a. Add middleware wo the pipeline.
b. Map endpoints that generate a reponse for a request.
c. Run the applciation by calling Run().

#### Middleware 
1. Middleware consists of small components that execute in sequence when the application receives an HTTP request such as loggin, identifying the user, serving static files, an handling the errors.
2. Middleware is typically added to WebApplication by calling Use* extension methods.
if(app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
}
3. We added only a single piece of middleware to the pipeline. but when you are addingmutiple middleware, the order of the Use* calls matters!
4. The WebApplicationbuilder builds an IWebHostEnvironment object and sets it on the Environment property.
##### IWebHostEnvironment 
IWebHostEnvironment exposes several environment-related properties such as:
1. ContentRootPath: Location of the working directory for the app
2. WebRootPath: Location of the wwwroot path
3. EnvironmentName: The current environemnt.

##### Error-Handling, Rotuing and endpoint middlewares
1. Routing middleware: it's added automatically to the start of the pipeline. For each request, the routing middleware uses the request's URL to determine which endpoint to invoke.
2. Endpoint middleware: it's added automatically to the end of the pipeline. The middleware pipeline executes until the request reaches the endpoint middleware, at which point the endpoint middleware executes the endpoint to generate the final response.
Example1: app.MapGet("/", () => "Hello World!");
You send a Get request to "/", the routing middleware selects the "Hello World!" endpoint. The endpoint middleware executes the lambda and returns the string value in the response body.
Example2: app.MapGet("/person", () => new Person("Hooman", "Salamat"));
This minimal API endpoint defines a lambda to run for GET requests to the "/person" path, but it return a C# object. The object is serialized to JSON via the minimal API infrastructure and returned in the response body. With minimal API, a simple function is used to generate a response.
3. ErrorHandling middleware: it's add automatically when you are running in the development environment.

## 4. Handling requests with the middleware pipeline