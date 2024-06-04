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
This chapter covers

Understanding middleware
Serving static files using middleware
Adding functionality using middleware
Combining middleware to form a pipeline
Handling exceptions and errors with middleware
In chapter 3 you had a whistle-stop tour of a complete ASP.NET Core application to see how the components come together to create a web application. In this chapter, we’ll focus on one small subsection: the middleware pipeline.

In ASP.NET Core, middleware consists of C# classes or functions that handle an HTTP request or response. Middleware is chained together, with the output of one acting as the input to the next to form a pipeline.

The middleware pipeline is one of the most important parts of configuration for defining how your application behaves and how it responds to requests. Understanding how to build and compose middleware is key to adding functionality to your applications.

In this chapter you’ll learn what middleware is and how to use it to create a pipeline. You’ll see how you can chain multiple middleware components together, with each component adding a discrete piece of functionality. The examples in this chapter are limited to using existing middleware components, showing how to arrange them in the correct way for your application. In chapter 31 you’ll learn how to build your own middleware components and incorporate them into the pipeline.

We’ll begin by looking at the concept of middleware, all the things you can achieve with it, and how a middleware component often maps to a cross-cutting concern. These functions of an application cut across multiple different layers. Logging, error handling, and security are classic cross-cutting concerns that are required by many parts of your application. Because all requests pass through the middleware pipeline, it’s the preferred location to configure and handle this functionality.

In section 4.2 I’ll explain how you can compose individual middleware components into a pipeline. You’ll start out small, with a web app that displays only a holding page. From there, you’ll learn how to build a simple static-file server that returns requested files from a folder on disk.

Next, you’ll move on to a more complex pipeline containing multiple middleware. In this example you’ll explore the importance of ordering in the middleware pipeline, and you’ll see how requests are handled when your pipeline contains multiple middleware.

In section 4.3 you’ll learn how you can use middleware to deal with an important aspect of any application: error handling. Errors are a fact of life for all applications, so it’s important that you account for them when building your app.

You can handle errors in a few ways. Errors are among the classic cross-cutting concerns, and middleware is well placed to provide the required functionality. In section 4.3 I’ll show how you can handle exceptions with middleware provided by Microsoft. In particular, you’ll learn about two different components:

DeveloperExceptionPageMiddleware—Provides quick error feedback when building an application

ExceptionHandlerMiddleware—Provides a generic error page in production so that you don’t leak any sensitive details

You won’t see how to build your own middleware in this chapter; instead, you’ll see that you can go a long way by using the components provided as part of ASP.NET Core. When you understand the middleware pipeline and its behavior, you’ll find it much easier to understand when and why custom middleware is required. With that in mind, let’s dive in!

## 4.1 Defining middleware
The word middleware is used in a variety of contexts in software development and IT, but it’s not a particularly descriptive word.

In ASP.NET Core, middleware is a C# class1 that can handle an HTTP request or response. Middleware can

Handle an incoming HTTP request by generating an HTTP response

Process an incoming HTTP request, modify it, and pass it on to another piece of middleware

Process an outgoing HTTP response, modify it, and pass it on to another piece of middleware or to the ASP.NET Core web server

You can use middleware in a multitude of ways in your own applications. A piece of logging middleware, for example, might note when a request arrived and then pass it on to another piece of middleware. Meanwhile, a static-file middleware component might spot an incoming request for an image with a specific name, load the image from disk, and send it back to the user without passing it on.

The most important piece of middleware in most ASP.NET Core applications is the EndpointMiddleware class. This class normally generates all your HTML and JavaScript Object Notation (JSON) responses, and is the focus of most of this book. Like image-resizing middleware, it typically receives a request, generates a response, and then sends it back to the user (figure 4.1).



Figure 4.1 Example of a middleware pipeline. Each middleware component handles the request and passes it on to the next middleware component in the pipeline. After a middleware component generates a response, it passes the response back through the pipeline. When it reaches the ASP.NET Core web server, the response is sent to the user’s browser.

__DEFINITION__ This arrangement—whereby a piece of middleware can call another piece of middleware, which in turn can call another, and so on—is referred to as a pipeline. You can think of each piece of middleware as being like a section of pipe; when you connect all the sections, a request flows through one piece and into the next.

One of the most common use cases for middleware is for the cross-cutting concerns of your application. These aspects of your application need to occur for every request, regardless of the specific path in the request or the resource requested, including

Logging each request

Adding standard security headers to the response

Associating a request with the relevant user

Setting the language for the current request

In each of these examples, the middleware receives a request, modifies it, and then passes the request on to the next piece of middleware in the pipeline. Subsequent middleware could use the details added by the earlier middleware to handle the request in some way. In figure 4.2, for example, the authentication middleware associates the request with a user. Then the authorization middleware uses this detail to verify whether the user has permission to make that specific request to the application.



Figure 4.2 Example of a middleware component modifying a request for use later in the pipeline. Middleware can also short-circuit the pipeline (do the process more quickly avoid some parts of the pipeline), returning a response before the request reaches later middleware.

If the user has permission, the authorization middleware passes the request on to the endpoint middleware to allow it to generate a response. If the user doesn’t have permission, the authorization middleware can short-circuit the pipeline, generating a response directly; it returns the response to the previous middleware, and the endpoint middleware never sees the request. This scenario is an example of the chain-of-responsibility design pattern.

__DEFINITION__ When a middleware component short-circuits the pipeline and returns a response, it’s called terminal middleware.

A key point to glean from this example is that the pipeline is bidirectional. The request passes through the pipeline in one direction until a piece of middleware generates a response, at which point the response passes back through the pipeline, passing through each piece of middleware a second time, in reverse order, until it gets back to the first piece of middleware. Finally, the first/last piece of middleware passes the response back to the ASP.NET Core web server.

### The HttpContext object

I mentioned the HttpContext in chapter 3, and it’s sitting behind the scenes here, too. The ASP.NET Core web server constructs an HttpContext for each request, which the ASP.NET Core application uses as a sort of storage box for a single request. Anything that’s specific to this particular request and the subsequent response can be associated with and stored in it. Examples are properties of the request, request-specific services, data that’s been loaded, or errors that have occurred. The web server fills the initial HttpContext with details of the original HTTP request and other configuration details, and then passes it on to the middleware pipeline and the rest of the application.

All middleware has access to the HttpContext for a request. It can use this object to determine whether the request contains any user credentials, to identify which page the request is attempting to access, and to fetch any posted data, for example. Then it can use these details to determine how to handle the request.

When the application finishes processing the request, it updates the HttpContext with an appropriate response and returns it through the middleware pipeline to the web server. Then the ASP.NET Core web server converts the representation to a raw HTTP response and sends it back to the reverse proxy, which forwards it to the user’s browser.

As you saw in chapter 3, you define the middleware pipeline in code as part of your initial application configuration in Program.cs. You can tailor the middleware pipeline specifically to your needs; simple apps may need only a short pipeline, whereas large apps with a variety of features may use much more middleware. Middleware is the fundamental source of behavior in your application. Ultimately, the middleware pipeline is responsible for responding to any HTTP requests it receives.

Requests are passed to the middleware pipeline as HttpContext objects. As you saw in chapter 3, the ASP.NET Core web server builds an HttpContext object from an incoming request, which passes up and down the middleware pipeline. When you’re using existing middleware to build a pipeline, this detail is one that you’ll rarely have to deal with. But as you’ll see in the final section of this chapter, its presence behind the scenes provides a route to exerting extra control over your middleware pipeline.

You can also think of your middleware pipeline as being a series of concentric components, similar to a traditional matryoshka (Russian) doll, as shown in figure 4.3. A request progresses through the pipeline by heading deeper into the stack of middleware until a response is returned. Then the response returns through the middleware, passing through the components in reverse order from the request.

![Figure 4.3](/ASPNETCORE8Tutorial/images/Figure4_3.png?raw=true "Figure 4.3")

Figure 4.3 You can also think of middleware as being a series of nested components; a request is sent deeper into the middleware, and the response resurfaces from it. Each middleware component can execute logic before passing the response on to the next middleware component and can execute logic after the response has been created, on the way back out of the stack.

### Middleware vs. HTTP modules and HTTP handlers

In the previous version of ASP.NET, the concept of a middleware pipeline isn’t used. Instead, you have HTTP modules and HTTP handlers.

An HTTP handler is a process that runs in response to a request and generates the response. The ASP.NET page handler, for example, runs in response to requests for .aspx pages. Alternatively, you could write a custom handler that returns resized images when an image is requested.

HTTP modules handle the cross-cutting concerns of applications, such as security, logging, and session management. They run in response to the life-cycle events that a request progresses through when it’s received by the server. Examples of events include BeginRequest, AcquireRequestState, and PostAcquireRequestState.

This approach works, but sometimes it’s tricky to reason about which modules will run at which points. Implementing a module requires relatively detailed understanding of the state of the request at each individual life-cycle event.

The middleware pipeline makes understanding your application far simpler. The pipeline is defined completely in code, specifying which components should run and in which order. Behind the scenes, the middleware pipeline in ASP.NET Core is simply a chain of method calls, with each middleware function calling the next in the pipeline.

That’s pretty much all there is to the concept of middleware. In the next section, I’ll discuss ways you can combine middleware components to create an application and how to use middleware to separate the concerns of your application.

## 4.2 Combining middleware in a pipeline
Generally speaking, each middleware component has a single primary concern; it handles only one aspect of a request. Logging middleware deals only with logging the request, authentication middleware is concerned only with identifying the current user, and static-file middleware is concerned only with returning static files.

Each of these concerns is highly focused, which makes the components themselves small and easy to reason about. This approach also gives your app added flexibility. Adding static-file middleware, for example, doesn’t mean you’re forced to have image-resizing behavior or authentication; each of these features is an additional piece of middleware.

To build a complete application, you compose multiple middleware components into a pipeline, as shown in section 4.1. Each middleware component has access to the original request, as well as any changes made to the HttpContext by middleware earlier in the pipeline. When a response has been generated, each middleware component can inspect and/or modify the response as it passes back through the pipeline before it’s sent to the user. This feature allows you to build complex application behaviors from small, focused components.

In the rest of this section, you’ll see how to create a middleware pipeline by combining various middleware components. Using standard middleware components, you’ll learn to create a holding page and to serve static files from a folder on disk. Finally, you’ll take a look at a more complex pipeline such as you’d get in a minimal API application with multiple middleware, routing, and endpoints.

### 4.2.1 Simple pipeline scenario 1: A holding page
For your first app in this chapter and your first middleware pipeline, you’ll learn how to create an app consisting of a holding page. Adding a holding page can be useful occasionally when you’re setting up your application to ensure that it’s processing requests without errors.

In previous chapters, I mentioned that the ASP.NET Core framework is composed of many small individual libraries. You typically add a piece of middleware by referencing a package in your application’s .csproj project file and configuring the middleware in Program.cs. Microsoft ships many standard middleware components with ASP.NET Core for you to choose among; you can also use third-party components from NuGet and GitHub, or you can build your own custom middleware. You can find the list of built-in middleware at https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#built-in-middleware.

__NOTE__ I discuss building custom middleware in chapter 31.

In this section, you’ll see how to create one of the simplest middleware pipelines, consisting only of WelcomePageMiddleware. WelcomePageMiddleware is designed to provide a sample HTML page quickly when you’re first developing an application, as you can see in figure 4.4. You wouldn’t use it in a production app, as you can’t customize the output, but it’s a single, self-contained middleware component you can use to ensure that your application is running correctly.

![Figure 4.4](/ASPNETCORE8Tutorial/images/Figure4_5.png?raw=true "Figure 4.4")

Figure 4.4 The Welcome-page middleware response. Every request to the application, at any path, will return the same Welcome-page response.

__TIP__ WelcomePageMiddleware is included as part of the base ASP.NET Core framework, so you don’t need to add a reference to any additional NuGet packages.

Even though this application is simple, the same process you’ve seen before occurs when the application receives an HTTP request, as shown in figure 4.5.

![Figure 4.5](/ASPNETCORE8Tutorial/images/Figure4_5.png?raw=true "Figure 4.5")

Figure 4.5 WelcomePageMiddleware handles a request. The request passes from the reverse proxy to the ASP.NET Core web server and finally to the middleware pipeline, which generates an HTML response.

The request passes to the ASP.NET Core web server, which builds a representation of the request and passes it to the middleware pipeline. As it’s the first (only!) middleware in the pipeline, WelcomePageMiddleware receives the request and must decide how to handle it. The middleware responds by generating an HTML response, no matter what request it receives. This response passes back to the ASP.NET Core web server, which forwards it to the reverse proxy and then to the user to display in their browser.

As with all ASP.NET Core applications, you define the middleware pipeline in Program.cs by calling Use* methods on the WebApplication instance. To create your first middleware pipeline, which consists of a single middleware component, you need a single method call. The application doesn’t need any extra configuration or services, so your whole application consists of the four lines in the following listing.

Listing 4.1 Program.cs for a Welcome-page middleware pipeline

```
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);  ❶
WebApplication app = builder.Build();                                ❶
 
app.UseWelcomePage();                                                ❷
 
app.Run();                                                           ❸

```


❶ Uses the default WebApplication configuration

❷ The only custom middleware in the pipeline

❸ Runs the application to handle requests

You build up the middleware pipeline in ASP.NET Core by calling methods on WebApplication (which implements IApplicationBuilder). WebApplication doesn’t define methods like UseWelcomePage itself; instead, these are extension methods.

Using extension methods allows you to add functionality to the WebApplication class, while keeping the implementation isolated from it. Under the hood, the methods typically call another extension method to add the middleware to the pipeline. Behind the scenes, for example, the UseWelcomePage method adds the WelcomePageMiddleware to the pipeline by calling

```
UseMiddleware<WelcomePageMiddleware>();
```

This convention of creating an extension method for each piece of middleware and starting the method name with Use is designed to improve discoverability when you add middleware to your application.2 ASP.NET Core includes a lot of middleware as part of the core framework, so you can use IntelliSense in Visual Studio and other integrated development environments (IDEs) to view all the middleware that’s available, as shown in figure 4.6.

![Figure 4.6](/ASPNETCORE8Tutorial/images/Figure4_6.png?raw=true "Figure 4.6")

Figure 4.6 IntelliSense makes it easy to view all the available middleware to add to your middleware pipeline.

Calling the UseWelcomePage method adds the WelcomePageMiddleware as the next middleware in the pipeline. Although you’re using only a single middleware component here, it’s important to remember that the order in which you make calls to IApplicationBuilder in Configure defines the order in which the middleware will run in the pipeline.

__WARNING__ When you’re adding middleware to the pipeline, always take care to consider the order in which it will run. A component can access only data created by middleware that comes before it in the pipeline.

This application is the most basic kind, returning the same response no matter which URL you navigate to, but it shows how easy it is to define your application behavior with middleware. Next, we’ll make things a little more interesting by returning different responses when you make requests to different paths.

### 4.2.2 Simple pipeline scenario 2: Handling static files
In this section, I’ll show you how to create one of the simplest middleware pipelines you can use for a full application: a static-file application. Most web applications, including those with dynamic content, serve some pages by using static files. Images, JavaScript, and CSS stylesheets are normally saved to disk during development and are served up when requested from the special wwwroot folder of your project, normally as part of a full HTML page request.

__DEFINITION__ By default, the wwwroot folder is the only folder in your application that ASP.NET Core will serve files from. It doesn’t serve files from other folders for security reasons. The wwwroot folder in an ASP.NET Core project is typically deployed as is to production, including all the files and folders it contains.

You can use StaticFileMiddleware to serve static files from the wwwroot folder when requested, as shown in figure 4.7. In this example, an image called moon.jpg exists in the wwwroot folder. When you request the file using the /moon.jpg path, it’s loaded and returned as the response to the request.

![Figure 4.7](/ASPNETCORE8Tutorial/images/Figure4_7.png?raw=true "Figure 4.7 Serving a static image file using the static-file middleware")

If the user requests a file that doesn’t exist in the wwwroot folder, such as missing.jpg, the static-file middleware won’t serve a file. Instead, a 404 HTTP error code response will be sent to the user’s browser, which displays its default “File Not Found” page, as shown in figure 4.8.

__NOTE__ How this page looks depends on your browser. In some browsers, you may see a blank page.

![Figure 4.8](/ASPNETCORE8Tutorial/images/Figure4_8.png?raw=true "Figure 4.8 Returning a 404 to the browser when a file doesn’t exist. The requested file didn’t exist in the wwwroot folder, so the ASP.NET Core application returned a 404 response. Then the browser (Microsoft Edge, in this case) shows the user a default “File Not Found” error page.")

Building the middleware pipeline for this simple static-file application is easy. The pipeline consists of a single piece of middleware, StaticFileMiddleware, as you can see in the following listing. You don’t need any services, so configuring the middleware pipeline with UseStaticFiles is all that’s required.

Listing 4.2 Program.cs for a static-file middleware pipeline

```
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
app.UseStaticFiles();    ❶
 
app.Run();
```

❶ Adds the StaticFileMiddleware to the pipeline

__TIP__ Remember that you can view the application code for this book in the GitHub repository.

When the application receives a request, the ASP.NET Core web server handles it and passes it to the middleware pipeline. StaticFileMiddleware receives the request and determines whether it can handle it. If the requested file exists, the middleware handles the request and returns the file as the response, as shown in figure 4.9.

![Figure 4.9](/ASPNETCORE8Tutorial/images/Figure4_9.png?raw=true "Figure 4.9 StaticFileMiddleware handles a request for a file. The middleware checks the wwwroot folder to see if whether requested moon.jpg file exists. The file exists, so the middleware retrieves it and returns it as the response to the web server and, ultimately, to the browser.")

If the file doesn’t exist, the request effectively passes through the static-file middleware unchanged. But wait—you added only one piece of middleware, right? Surely you can’t pass the request through to the next middleware component if there isn’t another one.

ASP.NET Core automatically adds a dummy piece of middleware to the end of the pipeline. This middleware always returns a 404 response if it’s called.

TIP If no middleware generates a response for a request, the pipeline automatically returns a simple 404 error response to the browser.

### HTTP response status codes

Every HTTP response contains a status code and, optionally, a reason phrase describing the status code. Status codes are fundamental to the HTTP protocol and are a standardized way of indicating common results. A 200 response, for example, means that the request was successfully answered, whereas a 404 response indicates that the resource requested couldn’t be found. You can see the full list of standardized status codes at https://www.rfc-editor.org/rfc/rfc9110#name-status-codes.

Status codes are always three digits long and are grouped in five classes, based on the first digit:

1xx—Information. This code is not often used; it provides a general acknowledgment.

2xx—Success. The request was successfully handled and processed.

3xx—Redirection. The browser must follow the provided link to allow the user to log in, for example.

4xx—Client error. A problem occurred with the request. The request sent invalid data, for example, or the user isn’t authorized to perform the request.

5xx—Server error. A problem on the server caused the request to fail.

These status codes typically drive the behavior of a user’s browser. The browser will handle a 301 response automatically, for example, by redirecting to the provided new link and making a second request, all without the user’s interaction.

Error codes are in the 4xx and 5xx classes. Common codes include a 404 response when a file couldn’t be found, a 400 error when a client sends invalid data (such as an invalid email address), and a 500 error when an error occurs on the server. HTTP responses for error codes may include a response body, which is content to display when the client receives the response.

This basic ASP.NET Core application makes it easy to see the behavior of the ASP.NET Core middleware pipeline and the static-file middleware in particular, but it’s unlikely that your applications will be this simple. It’s more likely that static files will form one part of your middleware pipeline. In the next section you’ll see how to combine multiple middleware components as we look at a simple minimal API application.

### 4.2.3 Simple pipeline scenario 3: A minimal API application
By this point, you should have a decent grasp of the middleware pipeline, insofar as you understand that it defines your application’s behavior. In this section you’ll see how to combine several standard middleware components to form a pipeline. As before, you do this in Program.cs by adding middleware to the WebApplication object.

You’ll begin by creating a basic middleware pipeline that you’d find in a typical ASP.NET Core minimal APIs template and then extend it by adding middleware. Figure 4.10 shows the output you see when you navigate to the home page of the application—identical to the sample application in chapter 3.

![Figure 4.10](/ASPNETCORE8Tutorial/images/Figure4_10.png?raw=true "Figure 4.10 A simple minimal API application. The application uses only four pieces of middleware: routing middleware to choose the endpoint to run, endpoint middleware to generate the response from a Razor Page, static-file middleware to serve image files, and exception-handler middleware to capture any errors.")

Creating this application requires only four pieces of middleware: routing middleware to choose a minimal API endpoint to execute, endpoint middleware to generate the response, static-file middleware to serve any image files from the wwwroot folder, and exception-handler middleware to handle any errors that might occur. Even though this example is still a Hello World! example, this architecture is much closer to a realistic example. The following listing shows an example of such an application.

Listing 4.3 A basic middleware pipeline for a minimal APIs application

```
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
UseDeveloperExceptionPage();         ❶
app.UseStaticFiles();                    ❷
app.UseRouting();                        ❸
 
app.MapGet("/", () => "Hello World!");   ❹
 
app.Run();
```

❶ This call isn’t strictly necessary, as it’s already added by WebApplication by default.

❷ Adds the StaticFileMiddleware to the pipeline

❸ Adds the RoutingMiddleware to the pipeline

❹ Defines an endpoint for the application

The addition of middleware to WebApplication to form the pipeline should be familiar to you now, but several points are worth noting in this example:

Middleware is added with Use*() methods.

MapGet defines an endpoint, not middleware. It defines the endpoints that the routing and endpoint middleware can use.

WebApplication automatically adds some middleware to the pipeline, such as the EndpointMiddleware.

The order of the Use*() method calls is important and defines the order of the middleware pipeline.

First, all the methods for adding middleware start with Use. As I mentioned earlier, this is thanks to the convention of using extension methods to extend the functionality of WebApplication; prefixing the methods with Use should make them easier to discover.

Second, it’s important to understand that the MapGet method does not add middleware to the pipeline; it defines an endpoint in your application. These endpoints are used by the routing and endpoint middleware. You’ll learn more about endpoints and routing in chapter 5.

__TIP__ You can define the endpoints for your app by using MapGet() anywhere in Program.cs before the call to app.Run(), but the calls are typically placed after the middleware pipeline definition.

In chapter 3, I mentioned that WebApplication automatically adds middleware to your app. You can see this process in action in listing 4.3 automatically adding the EndpointMiddleware to the end of the middleware pipeline. WebApplication also automatically adds the developer exception page middleware to the start of the middleware pipeline when you’re running in development. As a result, you can omit the call to UseDeveloperExceptionPage() from listing 4.3, and your middleware pipeline will be essentially the same.

### WebApplication and autoadded middleware

WebApplication and WebApplicationBuilder were introduced in .NET 6 to try to reduce the amount of boilerplate code required for a Hello World! ASP.NET Core application. As part of this initiative, Microsoft chose to have WebApplication automatically add various middleware to the pipeline. This decision alleviates some of the common getting-started pain points of middleware ordering by ensuring that, for example, UseRouting() is always called before UseAuthorization().

Everything has trade-offs, of course, and for WebApplication the trade-off is that it’s harder to understand exactly what’s in your middleware pipeline without having deep knowledge of the framework code itself.

Luckily, you don’t need to worry about the middleware that WebApplication adds for the most part. If you’re new to ASP.NET Core, generally you can accept that WebApplication will add the middleware only when it’s necessary and safe to do so.

Nevertheless, in some cases it may pay to know exactly what’s in your pipeline, especially if you’re familiar with ASP.NET Core. In .NET 7, WebApplication automatically adds some or all of the following middleware to the start of the middleware pipeline:

HostFilteringMiddleware—This middleware is security-related. You can read more about why it’s useful and how to configure it at https://andrewlock.net/adding-host-filtering-to-kestrel-in-aspnetcore/.

ForwardedHeadersMiddleware—This middleware controls how forwarded headers are handled. You can read more about it in chapter 27.

DeveloperExceptionPageMiddleware—As already discussed, this middleware is added when you run in a development environment.

RoutingMiddleware—If you add any endpoints to your application, UseRouting() runs before you add any custom middleware to your application.

AuthenticationMiddleware—If you configure authentication, this middleware authenticates a user for the request. Chapter 23 discusses authentication in detail.

AuthorizationMiddleware—The authorization middleware runs after authentication and determines whether a user is permitted to execute an endpoint. If the user doesn’t have permission, the request is short-circuited. I discuss authorization in detail in chapter 24.

EndpointMiddleware—This middleware pairs with the RoutingMiddleware to execute an endpoint. Unlike the other middleware described here, the EndpointMiddleware is added to the end of the middleware pipeline, after any other middleware you configure in Program.cs.

Depending on your Program.cs configuration, WebApplication may not add all this middleware. Also, if you don’t want some of this automatic middleware to be at the start of your middleware pipeline, generally you can override the location. In listing 4.3, for example, we override the automatic RoutingMiddleware location by calling UseRouting() explicitly, ensuring that routing occurs exactly where we need it.

Another important point about listing 4.3 is that the order in which you add the middleware to the WebApplication object is the order in which the middleware is added to the pipeline. The order of the calls in listing 4.3 creates a pipeline similar to that shown in figure 4.11.

![Figure 4.11](/ASPNETCORE8Tutorial/images/Figure4_11.png?raw=true "Figure 4.11 The middleware pipeline for the example application in listing 4.3. The order in which you add the middleware to WebApplication defines the order of the middleware in the pipeline.")

The ASP.NET Core web server passes the incoming request to the developer exception page middleware first. This exception-handler middleware ignores the request initially; its purpose is to catch any exceptions thrown by later middleware in the pipeline, as you’ll see in section 4.3. It’s important for this middleware to be placed early in the pipeline so that it can catch errors produced by later middleware.

The developer exception page middleware passes the request on to the static-file middleware. The static-file handler generates a response if the request corresponds to a file; otherwise, it passes the request on to the routing middleware. The routing middleware selects a minimal API endpoint based on the endpoints defined and the request URL, and the endpoint middleware executes the selected minimal API endpoint. If no endpoint can handle the requested URL, the automatic dummy middleware returns a 404 response.

In chapter 3, I mentioned that WebApplication adds the RoutingMiddleware to the start of the middleware pipeline automatically. So you may be wondering why I explicitly added it to the pipeline in listing 4.3 using UseRouting().

The answer, again, is related to the order of the middleware. Adding an explicit call to UseRouting() tells WebApplication not to add the RoutingMiddleware automatically before the middleware defined in Program.cs. This allows us to “move” the RoutingMiddleware to be placed after the StaticFileMiddleware. Although this step isn’t strictly necessary in this case, it’s good practice. The StaticFileMiddleware doesn’t use routing, so it’s preferable to let this middleware check whether the incoming request is for a static file; if so, it can short-circuit the pipeline and avoid the unnecessary call to the RoutingMiddleware.

__NOTE__ In versions 1.x and 2.x of ASP.NET Core, the routing and endpoint middleware were combined in a single Model-View-Controller (MVC) middleware component. Splitting the responsibilities for routing from execution makes it possible to insert middleware between the routing and endpoint middleware. I discuss routing further in chapters 6 and 14.

The impact of ordering is most obvious when you have two pieces of middleware that are listening for the same path. The endpoint middleware in the example pipeline currently responds to a request to the home page of the application (with the / path) by returning the string "Hello World!", as shown in figure 4.10. Figure 4.12 shows what happens if you reintroduce a piece of middleware that you saw previously, WelcomePageMiddleware, and configure it to respond to the / path as well.

![Figure 4.12](/ASPNETCORE8Tutorial/images/Figure4_12.png?raw=true "Figure 4.12 The Welcome-page middleware response. The Welcome-page middleware comes before the endpoint middleware, so a request to the home page returns the Welcome-page middleware instead of the minimal API response.")

As you saw in section 4.2.1, WelcomePageMiddleware is designed to return a fixed HTML response, so you wouldn’t use it in a production app, but it illustrates the point nicely. In the following listing, it’s added to the start of the middleware pipeline and configured to respond only to the "/" path.

Listing 4.4 Adding WelcomePageMiddleware to the pipeline
```
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
app.UseWelcomePage("/");                  ❶
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();                         ❷
 
app.MapGet("/", () => "Hello World!");    ❷
 
app.Run();
```
❶ WelcomePageMiddleware handles all requests to the “/” path and returns a sample HTML response.

❷ Requests to “/” will never reach the endpoint middleware, so this endpoint won’t be called.

Even though you know that the endpoint middleware can also handle the "/" path, WelcomePageMiddleware is earlier in the pipeline, so it returns a response when it receives the request to "/", short-circuiting the pipeline, as shown in figure 4.13. None of the other middleware in the pipeline runs for the request, so none has an opportunity to generate a response.

![Figure 4.13](/ASPNETCORE8Tutorial/images/Figure4_13.png?raw=true "Figure 4.13 Overview of the application handling a request to the "/" path. The Welcome-page middleware is first in the middleware pipeline, so it receives the request before any other middleware. It generates an HTML response, short-circuiting the pipeline. No other middleware runs for the request.")

As WebApplication automatically adds EndpointMiddleware to the end of the middleware pipeline, the WelcomePageMiddleware will always be ahead of it, so it always generates a response before the endpoint can execute in this example.

__TIP__ You should always consider the order of middleware when adding it to WebApplication. Middleware added earlier in the pipeline will run (and potentially return a response) before middleware added later.

All the examples shown so far try to handle an incoming request and generate a response, but it’s important to remember that the middleware pipeline is bidirectional. Each middleware component gets an opportunity to handle both the incoming request and the outgoing response. The order of middleware is most important for those components that create or modify the outgoing response.

In listing 4.3, I included DeveloperExceptionPageMiddleware at the start of the application’s middleware pipeline, but it didn’t seem to do anything. Error-handling middleware characteristically ignores the incoming request as it arrives in the pipeline; instead, it inspects the outgoing response, modifying it only when an error has occurred. In the next section, I discuss the types of error-handling middleware that are available to use with your application and when to use them.

## 4.3 Handling errors using middleware
Errors are a fact of life when you’re developing applications. Even if you write perfect code, as soon as you release and deploy your application, users will find a way to break it, by accident or intentionally! The important thing is that your application handles these errors gracefully, providing a suitable response to the user and not causing your whole application to fail.

The design philosophy for ASP.NET Core is that every feature is opt-in. So because error handling is a feature, you need to enable it explicitly in your application. Many types of errors could occur in your application, and you have many ways to handle them, but in this section I focus on a single type of error: exceptions.

Exceptions typically occur whenever you find an unexpected circumstance. A typical (and highly frustrating) exception you’ll no doubt have experienced before is NullReferenceException, which is thrown when you attempt to access a variable that hasn’t been initialized.3 If an exception occurs in a middleware component, it propagates up the pipeline, as shown in figure 4.14. If the pipeline doesn’t handle the exception, the web server returns a 500 status code to the user.

![Figure 4.14](/ASPNETCORE8Tutorial/images/Figure4_14.png?raw=true "Figure 4.14 An exception in the endpoint middleware propagates through the pipeline. If the exception isn’t caught by middleware earlier in the pipeline, a 500 “Server error” status code is sent to the user’s browser.")

In some situations, an error won’t cause an exception. Instead, middleware might generate an error status code. One such case occurs when a requested path isn’t handled. In that situation, the pipeline returns a 404 error.

For APIs, which typically are consumed by apps (as opposed to end users), that result probably is fine. But for apps that typically generate HTML, such as Razor Pages apps, returning a 404 typically results in a generic, unfriendly page being shown to the user, as you saw in figure 4.8. Although this behavior is correct, it doesn’t provide a great experience for users of these types of applications.

Error-handling middleware attempts to address these problems by modifying the response before the app returns it to the user. Typically, error-handling middleware returns either details on the error that occurred or a generic but friendly HTML page to the user. You’ll learn how to handle this use case in chapter 13 when you learn about generating responses with Razor Pages.

The remainder of this section looks at the two main types of exception-handling middleware that’s available for use in your application. Both are available as part of the base ASP.NET Core framework, so you don’t need to reference any additional NuGet packages to use them.

#### 4.3.1 Viewing exceptions in development: DeveloperExceptionPage
When you’re developing an application, you typically want access to as much information as possible when an error occurs somewhere in your app. For that reason, Microsoft provides DeveloperExceptionPageMiddleware, which you can add to your middleware pipeline by using
```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In development, this is already added by WebApplication
// Note: You should NEVER do this in Production as it can leak secrets
app.UseDeveloperExceptionPage();
app.MapGet("/", () => BadService.GetValues());

app.Run();

class BadService
{
    public static string? GetValues()
    {
        throw new Exception("Oops, something bad happened!");
    }
}
```
__NOTE__ As shown previously, WebApplication automatically adds this middleware to your middleware pipeline when you’re running in the Development environment, so you don’t need to add it explicitly. You’ll learn more about environments in chapter 10.

When an exception is thrown and propagates up the pipeline to this middleware, it’s captured. Then the middleware generates a friendly HTML page, which it returns with a 500 status code, as shown in figure 4.15. This page contains a variety of details about the request and the exception, including the exception stack trace; the source code at the line the exception occurred; and details on the request, such as any cookies or headers that were sent.

![Figure 4.15](/ASPNETCORE8Tutorial/images/Figure4_15.png?raw=true "Figure 4.15 The developer exception page shows details about the exception when it occurs during the process of a request. The location in the code that caused the exception, the source code line itself, and the stack trace are all shown by default. You can also click the Query, Cookies, Headers, and Routing buttons to reveal further details about the request that caused the exception.")

Having these details available when an error occurs is invaluable for debugging a problem, but they also represent a security risk if used incorrectly. You should never return more details about your application to users than absolutely necessary, so you should use DeveloperExceptionPage only when developing your application. The clue is in the name!

__WARNING__ Never use the developer exception page when running in production. Doing so is a security risk, as it could publicly reveal details about your application’s code, making you an easy target for attackers. WebApplication uses the correct behavior by default and adds the middleware only when running in development.

If the developer exception page isn’t appropriate for production use, what should you use instead? Luckily, you can use another type of general-purpose error-handling middleware in production: ExceptionHandlerMiddleware.

### 4.3.2 Handling exceptions in production: ExceptionHandlerMiddleware
The developer exception page is handy when you’re developing your applications, but you shouldn’t use it in production, as it can leak information about your app to potential attackers. You still want to catch errors, though; otherwise, users will see unfriendly error pages or blank pages, depending on the browser they’re using.

You can solve this problem by using ExceptionHandlerMiddleware. If an error occurs in your application, the user will see a custom error response that’s consistent with the rest of the application but provides only necessary details about the error. For a minimal API application, that response could be JSON or plain text, as shown in figure 4.16.

![Figure 4.16](/ASPNETCORE8Tutorial/images/Figure4_16.png?raw=true "Figure 4.16 Using the ExceptionHandlerMiddleware, you can return a generic error message when an exception occurs, ensuring that you don’t leak any sensitive details about your application in production.")

For Razor Pages apps, you can create a custom error response, such as the one shown in figure 4.17. You maintain the look and feel of the application by using the same header, displaying the currently logged-in user, and displaying an appropriate message to the user instead of full details on the exception.

![Figure 4.17](/ASPNETCORE8Tutorial/images/Figure4_17.png?raw=true "Figure 4.17 A custom error page created by ExceptionHandlerMiddleware. The custom error page can have the same look and feel as the rest of the application by reusing elements such as the header and footer. More important, you can easily control the error details displayed to users.")

Given the differing requirements for error handlers in development and production, most ASP.NET Core apps add their error-handler middleware conditionally, based on the hosting environment. WebApplication automatically adds the developer exception page when running in the development hosting environment, so you typically add ExceptionHandlerMiddleware when you’re not in the development environment, as shown in the following listing.

Listing 4.5 Adding exception-handler middleware when in production

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseExceptionHandler("/error");

// normally this would only be used in Production, and you'd rely on the DeveloperExceptionPage middleware
// that's automatically added by WebApplication in Development
// e.g.:
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.MapGet("/", () => BadService.GetValues());
app.MapGet("/error", () => "Sorry, there was a problem processing your request");

app.Run();

class BadService
{
    public static string? GetValues()
    {
        throw new Exception("Oops, something bad happened!");
    }
}



❶ In development, WebApplication automatically adds the developer exception page middleware.

❷ Configures a different pipeline when not running in development

❸ The ExceptionHandlerMiddleware won’t leak sensitive details when running in production.

❹ This error endpoint will be executed when an exception is handled.

As well as demonstrating how to add ExceptionHandlerMiddleware to your middleware pipeline, this listing shows that it’s perfectly acceptable to configure different middleware pipelines depending on the environment when the application starts. You could also vary your pipeline based on other values, such as settings loaded from configuration.

__NOTE__ You’ll see how to use configuration values to customize the middleware pipeline in chapter 10.

When adding ExceptionHandlerMiddleware to your application, you typically provide a path to the custom error page that will be displayed to the user. In the example in listing 4.5, you used an error handling path of "/error":

app.UseExceptionHandler("/error");
ExceptionHandlerMiddleware invokes this path after it captures an exception to generate the final response. The ability to generate a response dynamically is a key feature of ExceptionHandlerMiddleware; it allows you to reexecute a middleware pipeline to generate the response sent to the user.

Figure 4.18 shows what happens when ExceptionHandlerMiddleware handles an exception. It shows the flow of events when the minimal API endpoint for the "/" path generates an exception. The final response returns an error status code but also provides an error string, using the "/error" endpoint.

![Figure 4.18](/ASPNETCORE8Tutorial/images/Figure4_18.png?raw=true "Figure 4.18 ExceptionHandlerMiddleware handling an exception to generate a JSON response. A request to the / path generates an exception, which is handled by the middleware. The pipeline is reexecuted, using the /error path to generate the JSON response.")

The sequence of events when an unhandled exception occurs somewhere in the middleware pipeline (or in an endpoint) after ExceptionHandlerMiddleware is as follows:

A piece of middleware throws an exception.

ExceptionHandlerMiddleware catches the exception.

Any partial response that has been defined is cleared.

The ExceptionHandlerMiddleware overwrites the request path with the provided error-handling path.

The middleware sends the request back down the pipeline, as though the original request had been for the error-handling path.

The middleware pipeline generates a new response as normal.

When the response gets back to ExceptionHandlerMiddleware, it modifies the status code to a 500 error and continues to pass the response up the pipeline to the web server.

One of the main advantages of reexecuting the pipeline for Razor Page apps is the ability to have your error messages integrated into your normal site layout, as shown in figure 4.17. It’s certainly possible to return a fixed response when an error occurs without reexecuting the pipeline, but you wouldn’t be able to have a menu bar with dynamically generated links or display the current user’s name in the menu, for example. By reexecuting the pipeline, you ensure that all the dynamic areas of your application are integrated correctly, as though the page were a standard page of your site.

__NOTE__ You don’t need to do anything other than add ExceptionHandlerMiddleware to your application and configure a valid error-handling path to enable reexecuting the pipeline, as shown in figure 4.18. The middleware will catch the exception and reexecute the pipeline for you. Subsequent middleware will treat the reexecution as a new request, but previous middleware in the pipeline won’t be aware that anything unusual happened.

Reexecuting the middleware pipeline is a great way to keep consistency in your web application for error pages, but you should be aware of some gotchas. First, middleware can modify a response generated farther down the pipeline only if the response hasn’t yet been sent to the client. This situation can be a problem if, for example, an error occurs while ASP.NET Core is sending a static file to a client. In that case, ASP.NET Core may start streaming bytes to the client immediately for performance reasons. When that happens, the error-handling middleware won’t be able to run, as it can’t reset the response. Generally speaking, you can’t do much about this problem, but it’s something to be aware of.

A more common problem occurs when the error-handling path throws an error during the reexecution of the pipeline. Imagine that there’s a bug in the code that generates the menu at the top of the page in a Razor Pages app:

When the user reaches your home page, the code for generating the menu bar throws an exception.

The exception propagates up the middleware pipeline.

When reached, ExceptionHandlerMiddleware captures it, and the pipe is reexecuted, using the error-handling path.

When the error page executes, it attempts to generate the menu bar for your app, which again throws an exception.

The exception propagates up the middleware pipeline.

ExceptionHandlerMiddleware has already tried to intercept a request, so it lets the error propagate all the way to the top of the middleware pipeline.

The web server returns a raw 500 error, as though there were no error-handling middleware at all.

Thanks to this problem, it’s often good practice to make your error-handling pages as simple as possible to reduce the possibility that errors will occur.

__WARNING__ If your error-handling path generates an error, the user will see a generic browser error. It’s often better to use a static error page that always works than a dynamic page that risks throwing more errors. You can see an alternative approach using a custom error handling function in this post: http://mng.bz/0Kmx.

Another consideration when building minimal API applications is that you generally don’t want to return HTML. Returning an HTML page to an application that’s expecting JSON could easily break it. Instead, the HTTP 500 status code and a JSON body describing the error are more useful to a consuming application. Luckily, ASP.NET Core allows you to do exactly this when you create minimal APIs and web API controllers.

NOTE I discuss how to add this functionality with minimal APIs in chapter 5 and with web APIs in chapter 20.

That brings us to the end of middleware in ASP.NET Core for now. You’ve seen how to use and compose middleware to form a pipeline, as well as how to handle exceptions in your application. This information will get you a long way when you start building your first ASP.NET Core applications. Later, you’ll learn how to build your own custom middleware, as well as how to perform complex operations on the middleware pipeline, such as forking it in response to specific requests. In chapter 5, you’ll look in depth at minimal APIs and at how they can be used to build JSON APIs.

__Summary__
- Middleware has a similar role to HTTP modules and handlers in ASP.NET but is easier to reason about.

- Middleware is composed in a pipeline, with the output of one middleware passing to the input of the next.

- The middleware pipeline is two-way: requests pass through each middleware on the way in, and responses pass back through in reverse order on the way out.

- Middleware can short-circuit the pipeline by handling a request and returning a response, or it can pass the request on to the next middleware in the pipeline.

- Middleware can modify a request by adding data to or changing the HttpContext object.

- If an earlier middleware short-circuits the pipeline, not all middleware will execute for all requests.

- If a request isn’t handled, the middleware pipeline returns a 404 status code.

- The order in which middleware is added to WebApplication defines the order in which middleware will execute in the pipeline.

- The middleware pipeline can be reexecuted as long as a response’s headers haven’t been sent.

- When it’s added to a middleware pipeline, StaticFileMiddleware serves any requested files found in the wwwroot folder of your application.

- DeveloperExceptionPageMiddleware provides a lot of information about errors during development, but it should never be used in production.

- ExceptionHandlerMiddleware lets you provide user-friendly custom error-handling messages when an exception occurs in the pipeline. It’s safe for use in production, as it doesn’t expose sensitive details about your application.

Microsoft provides some common middleware, and many third-party options are available on NuGet and GitHub.

1. Technically, middleware needs to be a function, as you’ll see in chapter 31, but it’s common to implement middleware as a C# class with a single method.

2. The downside to this approach is that it can hide exactly which middleware is being added to the pipeline. When the answer isn’t clear, I typically search for the source code of the extension method directly in GitHub (https://github.com/aspnet/aspnetcore).

3. C# 8.0 introduced non-nullable reference types, which provide a way to handle null values more clearly, with the promise of finally ridding .NET of NullReferenceExceptions! The ASP.NET Core framework libraries in .NET 7 have fully embraced nullable reference types. See the documentation to learn more: (https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types).



## 5. Creating a JSON API with minimal APIs
This chapter covers

- Creating a minimal API application to return JSON to clients
- Generating responses with IResult
- Using filters to perform common actions like validation
- Organizing your APIs with route groups
So far in this book you’ve seen several examples of minimal API applications that return simple Hello World! responses. These examples are great for getting started, but you can also use minimal APIs to build full-featured HTTP API applications. In this chapter you’ll learn about HTTP APIs, see how they differ from a server- rendered application, and find out when to use them.

Section 5.2 starts by expanding on the minimal API applications you’ve already seen. You’ll explore some basic routing concepts and show how values can be extracted from the URL automatically. Then you’ll learn how to handle additional HTTP verbs such as POST and PUT, and explore various ways to define your APIs.

In section 5.3 you’ll learn about the different return types you can use with minimal APIs. You’ll see how to use the Results and TypedResults helper classes to easily create HTTP responses that use status codes like 201 Created and 404 Not Found. You’ll also learn how to follow web standards for describing your errors by using the built-in support for Problem Details.

Section 5.4 introduces one of the big features added to minimal APIs in .NET 7: filters. You can use filters to build a mini pipeline (similar to the middleware pipeline from chapter 4) for each of your endpoints. Like middleware, filters are great for extracting common code from your endpoint handlers, making your handlers easier to read.

You’ll learn about the other big .NET 7 feature for minimal APIs in section 5.5: route groups. You can use route groups to reduce the duplication in your minimal APIs, extracting common routing prefixes and filters, making your APIs easier to read, and reducing boilerplate. In conjunction with filters, route groups address many of the common complaints raised against minimal APIs when they were released in .NET 6.

One great aspect of ASP.NET Core is the variety of applications you can create with it. The ability to easily build a generalized HTTP API presents the possibility of using ASP.NET Core in a greater range of situations than can be achieved with traditional web apps alone. But should you build an HTTP API, and if so, why? In the first section of this chapter, I’ll go over some of the reasons why you may—or may not—want to create a web API.

### 5.1 What is an HTTP API, and when should you use one?
Traditional web applications handle requests by returning HTML, which is displayed to the user in a web browser. You can easily build applications like that by using Razor Pages to generate HTML with Razor templates, as you’ll learn in part 2 of this book. This approach is common and well understood, but the modern application developer has other possibilities to consider (figure 5.1), as you first saw in chapter 2.

![Figure 5.1](/ASPNETCORE8Tutorial/images/Figure5_1.png?raw=true "Figure 5.1 Modern developers have to consider several consumers of their applications. As well as traditional users with web browsers, these users could be single-page applications, mobile applications, or other apps.")

Client-side single-page applications (SPAs) have become popular in recent years with the development of frameworks such as Angular, React, and Vue. These frameworks typically use JavaScript running in a web browser to generate the HTML that users see and interact with. The server sends this initial JavaScript to the browser when the user first reaches the app. The user’s browser loads the JavaScript and initializes the SPA before loading any application data from the server.

__NOTE__ Blazor WebAssembly is an exciting new SPA framework. Blazor lets you write an SPA that runs in the browser like other SPAs, but it uses C# and Razor templates instead of JavaScript by using the new web standard, WebAssembly. I don’t cover Blazor in this book, so to find out more, I recommend Blazor in Action, by Chris Sainty (Manning, 2022).

Once the SPA is loaded in the browser, communication with a server still occurs over HTTP, but instead of sending HTML directly to the browser in response to requests, the server-side application sends data—normally, in the ubiquitous JavaScript Object Notation (JSON) format—to the client-side application. Then the SPA parses the data and generates the appropriate HTML to show to a user, as shown in figure 5.2. The server-side application endpoint that the client communicates with is sometimes called an HTTP API, a JSON API, or a REST API, depending on the specifics of the API’s design.

![Figure 5.2](/ASPNETCORE8Tutorial/images/Figure5_2.png?raw=true "Figure 5.2 A sample client-side SPA using Blazor WebAssembly. The initial requests load the SPA files into the browser, and subsequent requests fetch data from a web API, formatted as JSON.")

__DEFINITION__ An HTTP API exposes multiple URLs via HTTP that can be used to access or change data on a server. It typically returns data using the JSON format. HTTP APIs are sometimes called web APIs, but as web API refers to a specific technology in ASP.NET Core, in this book I use HTTP API to refer to the generic concept.

These days, mobile applications are common and, from the server application’s point of view, similar to client-side SPAs. A mobile application typically communicates with a server application by using an HTTP API, receiving data in JSON format, just like an SPA. Then it modifies the application’s UI depending on the data it receives.

One final use case for an HTTP API is where your application is designed to be partially or solely consumed by other backend services. Imagine that you’ve built a web application to send emails. By creating an HTTP API, you can allow other application developers to use your email service by sending you an email address and a message. Virtually all languages and platforms have access to an HTTP library they could use to access your service from code.

That’s all there is to an HTTP API: it exposes endpoints (URLs) that client applications can send requests to and retrieve data from. These endpoints are used to power the behavior of the client apps, as well as to provide all the data the client apps need to display the correct interface to a user.

__NOTE__ You have even more options when it comes to creating APIs in ASP.NET Core. You can create remote procedure call APIs using gRPC, for example, or provide an alternative style of HTTP API using the GraphQL standard. I don’t cover those technologies in this book, but you can read about gRPC at https://docs.microsoft.com/aspnet/core/grpc and find out about GraphQL in Building Web APIs with ASP.NET Core, by Valerio De Sanctis (Manning, 2023).

Whether you need or want to create an HTTP API for your ASP.NET Core application depends on the type of application you want to build. Perhaps you’re familiar with client-side frameworks, or maybe you need to develop a mobile application, or you already have an SPA build pipeline configured. In each case, you’ll most likely want to add HTTP APIs for the client apps to access your application.

One selling point for using an HTTP API is that it can serve as a generalized backend for all your client applications. You could start by building a client-side application that uses an HTTP API. Later, you could add a mobile app that uses the same HTTP API, making little or no modification to your ASP.NET Core code.

If you’re new to web development, HTTP APIs can also be easier to understand initially, as they typically return only JSON. Part 1 of this book focuses on minimal APIs so that you can focus on the mechanics of ASP.NET Core without needing to write HTML or CSS.

In part 3, you’ll learn how to use Razor Pages to create server-rendered applications instead of minimal APIs. Server-rendered applications can be highly productive. They’re generally recommended when you have no need to call your application from outside a web browser or when you don’t want or need to make the effort of configuring a client-side application.

__NOTE__ Although there’s been an industry shift toward client-side frameworks, server-side rendering using Razor is still relevant. Which approach you choose depends largely on your preference for building HTML applications in the traditional manner versus using JavaScript (or Blazor!) on the client.

Having said that, whether to use HTTP APIs in your application isn’t something you necessarily have to worry about ahead of time. You can always add them to an ASP.NET Core app later in development, as the need arises.

### SPAs with ASP.NET Core

The cross-platform, lightweight design of ASP.NET Core means that it lends itself well to acting as a backend for your SPA framework of choice. Given the focus of this book and the broad scope of SPAs in general, I won’t be looking at Angular, React, or other SPAs here. Instead, I suggest checking out the resources appropriate to your chosen SPA. Books are available from Manning for all the common client-side JavaScript frameworks, as well as Blazor:

React in Action, by Mark Tielens Thomas (Manning, 2018)

Angular in Action, by Jeremy Wilken (Manning, 2018)

Vue.js in Action, by Erik Hanchett with Benjamin Listwon (Manning, 2018)

Blazor in Action, by Chris Sainty (Manning, 2022)

After you’ve established that you need an HTTP API for your application, creating one is easy, as it’s the default application type in ASP.NET Core! In the next section we look at various ways you can create minimal API endpoints and ways to handle multiple HTTP verbs.

### 5.2 Defining minimal API endpoints
Chapters 3 and 4 gave you an introduction to basic minimal API endpoints. In this section, we’ll build on those basic apps to show how you can handle multiple HTTP verbs and explore various ways to write your endpoint handlers.

### 5.2.1 Extracting values from the URL with routing
You’ve seen several minimal API applications in this book, but so far, all the examples have used fixed paths to define the APIs, as in this example:

```
app.MapGet("/", () => "Hello World!");
app.MapGet("/person", () => new Person("Hooman", "Salamat");
```


These two APIs correspond to the paths / and /person, respectively. This basic functionality is useful, but typically you need some of your APIs to be more dynamic. It’s unlikely, for example, that the /person API would be useful in practice, as it always returns the same Person object. What might be more useful is an API to which you can provide the user’s first name, and the API returns all the users with that name.

You can achieve this goal by using parameterized routes for your API definitions. You can create a parameter in a minimal API route using the expression {someValue}, where someValue is any name you choose. The value will be extracted from the request URL’s path and can be used in the lambda function endpoint.

__NOTE__ I introduce only the basics of extracting values from routes in this chapter. You’ll learn a lot more about routing in chapter 6, including why we use routing and how it fits into the ASP.NET Core pipeline, as well as the syntax you can use.

If you create an API using the route template /person/{name}, for example, and send a request to the path /person/Andrew, the name parameter will have the value "Andrew". You can use this feature to build more useful APIs, such as the one shown in the following listing.

Listing 5.1 A minimal API that uses a value from the URL

```
using System.Xml.Linq;
using System;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
var people = new List<Person>                             ❶
{                                                         ❶
    new("Tom", "Hanks"),                                  ❶
    new("Denzel", "Washington"),                          ❶
    new("Leondardo", "DiCaprio"),                         ❶
    new("Al", "Pacino"),                                  ❶
    new("Morgan", "Freeman"),                             ❶
};                                                        ❶
 
app.MapGet("/person/{name}", (string name) =>             ❷
    people.Where(p => p.FirstName.StartsWith(name)));     ❸
 
app.Run();
public record Person(string FirstName, string LastName);
```

❶ Creates a list of people as the data for the API

❷ The route is parameterized to extract the name from the URL.

❸ The extracted value can be injected into the lambda handler.

If you send a request to /person/Al for the app defined in listing 5.1, the name parameter will have the value "Al", and the API will return the following JSON:

[{"firstName":"Al","lastName":"Pacino"}]
NOTE By default, minimal APIs serialize C# objects to JSON. You’ll see how to return other types of results in section 5.3.

The ASP.NET Core routing system is quite powerful, and we’ll explore it in more detail in chapter 6. But with this simple capability, you can already build more complex applications.

5.2.2 Mapping verbs to endpoints
So far in this book we’ve defined all our minimal API endpoints by using the MapGet() function. This function matches requests that use the GET HTTP verb. GET is the most-used verb; it’s what a browser uses when you enter a URL in the address bar of your browser or follow a link on a web page.

You should use GET only to get data from the server, however. You should never use it to send data or to change data on the server. Instead, you should use an HTTP verb such as POST or DELETE. You generally can’t use these verbs by navigating web pages in the browser, but they’re easy to send from a client-side SPA or mobile app.

__TIP__ If you’re new to web programming or are looking for a refresher, Mozilla Developer Network (MDN), maker of the Firefox web browser, has a good introduction to HTTP at https://developer.mozilla.org/en-US/docs/Web/HTTP/Overview.

In theory, each of the HTTP verbs has a well-defined purpose, but in practice, you may see apps that only ever use POST and GET. This is often fine for server-rendered applications like Razor Pages, as it’s typically simpler, but if you’re creating an API, I recommend that you use the HTTP verbs with the appropriate semantics wherever possible.

You can define endpoints for other verbs with minimal APIs by using the appropriate Map* functions. To map a POST endpoint, for example, you’d use MapPost(). Table 5.1 shows the minimal API Map* methods available, the corresponding HTTP verbs, and the typical semantic expectations of each verb on the types of operations that the API performs.

![Table 5.1](/ASPNETCORE8Tutorial/images/Table5_1.png?raw=true "Table 5.1 The minimal API map endpoints and the corresponding HTML verbs")

RESTful applications (as described in chapter 2) typically stick close to these verb uses where possible, but some of the actual implementations can differ, and people can easily get caught up in pedantry. Generally, if you stick to the expected operations described in table 5.1, you’ll create a more understandable interface for consumers of the API.

__NOTE__ You may notice that if you use the MapMethods() and Map() methods listed in table 5.1, your API probably doesn’t correspond to the expected operations of the HTTP verbs it supports, so I avoid these methods where possible. MapFallback() doesn’t have a path and is called only if no other endpoint matches. Fallback routes can be useful when you have a SPA that uses client-side routing. See https://weblog.west-wind.com/posts/2020/Jul/12/Handling-SPA-Fallback-Paths-in-a-Generic-ASPNET-Core-Server for a description of the problem and an alternative solution.

As I mentioned at the start of section 5.2.2, testing APIs that use verbs other than GET is tricky in the browser. You need to use a tool that allows sending arbitrary requests such as Postman (https://www.postman.com) or the HTTP Client plugin in JetBrains Rider. In chapter 11 you’ll learn how to use a tool called Swagger UI to visualize and test your APIs.

__TIP__ The HTTP client plugin in JetBrains Rider makes it easy to craft HTTP requests from inside your API, and even discovers all the endpoints in your application automatically, making them easier to test. You can read more about it at https://www.jetbrains.com/help/rider/Http_client_in__product__ code_editor.html.

As a final note before we move on, it’s worth mentioning the behavior you get when you call a method with the wrong HTTP verb. If you define an API like the one in listing 5.1

app.MapGet("/person/{name}", (string name) =>
  people.Where(p => p.FirstName.StartsWith(name)));
and call it by using a POST request to /person/Al instead of a GET request, the handler won’t run, and the response you get will have status code 405 Method Not Allowed.

__TIP__ You should never see this response when you’re calling the API correctly, so if you receive a 405 response, make sure to check that you’re using the right HTTP verb and the right path. Often when I see a 405, I’ve used the correct verb but made a typo in the URL!

In all the examples in this book so far, you provide a lambda function as the handler for an endpoint. But in section 5.2.3, you’ll see that there are many ways to define the handler.

### 5.2.3 Defining route handlers with functions
For basic examples, using a lambda function as the handler for an endpoint is often the simplest approach, but you can take many approaches, as shown in the following listing. This listing also demonstrates creating a simple CRUD (Create, Read, Update, Delete) API using different HTTP verbs, as discussed in section 5.2.1.

Listing 5.2 Creating route handlers for a simple CRUD API

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapGet("/fruit", () => Fruit.All);

var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", getFruit);

app.MapPost("/fruit/{id}", Handlers.AddFruit);

Handlers handlers = new();
app.MapPut("/fruit/{id}", handlers.ReplaceFruit);

Handlers handlers2 = new();
app.MapPatch("/fruit/{id}", handlers2.ReplaceFruit);

app.MapDelete("/fruit/{id}", DeleteFruit);

app.Run();

void DeleteFruit(string id)
{
    Fruit.All.Remove(id);
}

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
};

class Handlers
{
    public void ReplaceFruit(string id, Fruit fruit)
    {
        Fruit.All[id] = fruit;
    }

    public static void AddFruit(string id, Fruit fruit)
    {
        Fruit.All.Add(id, fruit);
    }
}

Exercise: 
1. Run the application
2. Go to postman
3. select "Post" verb
4. https://localhost:7178/fruit/f1
5. in the body: {"Name": "Apple", "Stock" : "45"}
6. Hit "Send"
7. select "Post" verb
8. https://localhost:7178/fruit/f2
9. in the body: {"Name": "Orange", "Stock" : "35"}
10. Hit "Send"
11. Select "Get" verb
12. https://localhost:7178/fruit/
13. {
    "f1": {
        "name": "Apple",
        "stock": 45
    },
    "f2": {
        "name": "Orange",
        "stock": 35
    }
}

14. Select "Put" verb (if put doesn't find the key, it adds one!)
15. https://localhost:7178/fruit/fruit2
16. in the body: {"Name": "banana", "Stock" : "25"}
17. Select "Get" verb
18. https://localhost:7178/fruit/
19. {
    "f1": {
        "name": "Apple",
        "stock": 45
    },
    "f2": {
        "name": "Orange",
        "stock": 35
    },
    "fruit2": {
        "name": "banana",
        "stock": 25
    }
}
20. select "Delete" verb
21. https://localhost:7178/fruit/fruit2
22. Select "Get" verb
23. https://localhost:7178/fruit/ 
24. select "Put" verb
25. https://localhost:7178/fruit/f2
26. {"Name": "banana", "Stock" : "25"}
27. Select "Get" verb
28. https://localhost:7178/fruit/
29. {
    "f1": {
        "name": "Apple",
        "stock": 45
    },
    "f2": {
        "name": "banana",
        "stock": 25
    }
}
30. Select "Patch" verb (partial change)
31. https://localhost:7178/fruit/f1
32. {"Name": "Apple2"}
33. Select "Get" verb
34. https://localhost:7178/fruit/
35. {
    "f1": {
        "name": "Apple2",
        "stock": 0
    }
}

Listing 5.2 demonstrates the various ways you can pass handlers to an endpoint by simulating a simple API for interacting with a collection of Fruit items:

A lambda expression, as in the MapGet("/fruit") endpoint

A Func<T, TResult> variable, as in the MapGet("/fruit/{id}") endpoint

A static method, as in the MapPost endpoint

A method on an instance variable, as in the MapPut endpoint

A local function, as in the MapDelete endpoint

All these approaches are functionally identical, so you can use whichever pattern works best for you.

Each Fruit record in listing 5.2 has a Name and a Stock level and is stored in a dictionary with an id. You call the API by using different HTTP verbs to perform the CRUD operations against the dictionary.

__WARNING__ This API is simple. It isn’t thread-safe, doesn’t validate user input, and doesn’t handle edge cases. We’ll remedy some of those deficiencies in section 5.3.

The handlers for the POST and PUT endpoints in listing 5.2 accept both an id parameter and a Fruit parameter, showing another important feature of minimal APIs. Complex types—that is, types that can’t be extracted from the URL by means of route parameters—are created by deserializing the JSON body of a request.

NOTE By contrast with APIs built using ASP.NET and ASP.NET Core web API controllers (which we cover in chapter 20), minimal APIs can bind only to JSON bodies and always use the System.Text.Json library for JSON deserialization.

Figure 5.3 shows an example of a POST request sent with Postman. Postman sends the request body as JSON, which the minimal API automatically deserializes into a Fruit instance before calling the endpoint handler. You can bind only a single object in your endpoint handler to the request body in this way. I cover model binding in detail in chapter 7.

![Figure 5.3](/ASPNETCORE8Tutorial/images/Figure5_3.png?raw=true "Figure 5.3 Sending a POST request with Postman. The minimal API automatically deserializes the JSON in the request body to a Fruit instance before calling the endpoint handler.")

Minimal APIs leave you free to organize your endpoints any way you choose. That flexibility is often cited as a reason to not use them, due to the fear that developers will keep all the functionality in a single file, as in most examples (such as listing 5.2). In practice, you’ll likely want to extract your endpoints to separate files so as to modularize them and make them easier to understand. Embrace that urge; that’s the way they were intended to be used!

Now you have a simple API, but if you try it out, you’ll quickly run into scenarios in which your API seems to break. In section 5.3 you learn how to handle some of these scenarios by returning status codes.

### 5.3 Generating responses with IResult
You’ve seen the basics of minimal APIs, but so far, we’ve looked only at the happy path, where you can handle the request successfully and return a response. In this section we look at how to handle bad requests and other errors by returning different status codes from your API.

The API in listing 5.2 works well as long as you perform only operations that are valid for the current state of the application. If you send a GET request to /fruit, for example, you’ll always get a 200 success response, but if you send a GET request to /fruit/f1 before you create a Fruit with the id f1, you’ll get an exception and a 500 Internal Server Error response, as shown in figure 5.4.

![Figure 5.4](/ASPNETCORE8Tutorial/images/Figure5_4.png?raw=true "Figure 5.4 If you try to retrieve a fruit by using a nonexistent id for the simplistic API in listing 5.2, the endpoint throws an exception. This exception is handled by the DeveloperExceptionPage-Middleware but provides a poor experience.")

Throwing an exception whenever a user requests an id that doesn’t exist clearly makes for a poor experience all round. A better approach is to return a status code indicating the problem, such as 404 Not Found or 400 Bad Request. The most declarative way to do this with minimal APIs is to return an IResult instance.

All the endpoint handlers you’ve seen so far in this book have returned void, a string, or a plain old CLR object (POCO) such as Person or Fruit. There is one other type of object you can return from an endpoint: an IResult implementation.

In summary, the endpoint middleware handles each return type as follows:

- void or Task—The endpoint returns a 200 response with no body.

- string or Task<string>—The endpoint returns a 200 response with the string serialized to the body as text/plain.

- IResult or Task<IResult>—The endpoint executes the IResult.ExecuteAsync method. Depending on the implementation, this type can customize the response, returning any status code.

- T or Task<T>—All other types (such as POCO objects) are serialized to JSON and returned in the body of a 200 response as application/json.

- The IResult implementations provide much of the flexibility in minimal APIs, as you’ll see in section 5.3.1.

### 5.3.1 Returning status codes with Results and TypedResults
A well-designed API uses status codes to indicate to a client what went wrong when a request failed, as well as potentially provide more descriptive codes when a request is successful. You should anticipate common problems that may occur when clients call your API and return appropriate status codes to indicate the causes to users.

ASP.NET Core exposes the simple static helper types Results and TypedResults in the namespace Microsoft.AspNetCore.Http. You can use these helpers to create a response with common status codes, optionally including a JSON body. Each of the methods on Results and TypedResults returns an implementation of IResult, which the endpoint middleware executes to generate the final response.

NOTE Results and TypedResults perform the same function, as helpers for generating common status codes. The only difference is that the Results methods return an IResult, whereas TypedResults return a concrete generic type, such as Ok<T>. There’s no difference in terms of functionality, but the generic types are easier to use in unit tests and in OpenAPI documentation, as you’ll see in chapters 36 and 11. TypedResults were added in .NET 7.

The following listing shows an updated version of listing 5.2, in which we address some of the deficiencies in the API and use Results and TypedResults to return different status codes to clients.

Listing 5.3 Using Results and TypedResults in a minimal API

using System.Collections.Concurrent;
 
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
var _fruit = new ConcurrentDictionary<string, Fruit>();        ❶
 
app.MapGet("/fruit", () => _fruit);
 
app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)                      ❷
        ? TypedResults.Ok(fruit)                               ❸
        : Results.NotFound());                                 ❹
 
 
app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)                                   ❺
        ? TypedResults.Created($"/fruit/{id}", fruit)          ❻
        : Results.BadRequest(new                               ❼
            { id = "A fruit with this id already exists" }));  ❼
 
app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
{
 
    _fruit[id] = fruit;
    return Results.NoContent();                                ❽
});
 
app.MapDelete("/fruit/{id}", (string id) =>
{
    _fruit.TryRemove(id, out _);                               ❾
    return Results.NoContent();                                ❾
});
 
app.Run();
record Fruit(string Name, int stock);
❶ Uses a concurrent dictionary to make the API thread-safe

❷ Tries to get the fruit from the dictionary. If the ID exists in the dictionary, this returns true . . .

❸ . . . and we return a 200 OK response, serializing the fruit in the body as JSON.

❹ If the ID doesn’t exist, returns a 404 Not Found response

❺ Tries to add the fruit to the dictionary. If the ID hasn’t been added yet. this returns true . . .

❻ . . . and we return a 201 response with a JSON body and set the Location header to the given path.

❼ If the ID already exists, returns a 400 Bad Request response with an error message

❽ After adding or replacing the fruit, returns a 204 No Content response

❾ After deleting the fruit, always returns a 204 No Content response

Listing 5.3 demonstrates several status codes, some of which you may not be familiar with:

- 200 OK—The standard successful response. It often includes content in the body of the response but doesn’t have to.

- 201 Created—Often returned when you successfully created an entity on the server. The Created result in listing 5.3 also includes a Location header to describe the URL where the entity can be found, as well as the JSON entity itself in the body of the response.

- 204 No Content—Similar to a 200 response but without any content in the response body.

- 400 Bad Request—Indicates that the request was invalid in some way; often used to indicate data validation failures.

- 404 Not Found—Indicates that the requested entity could not be found.

These status codes more accurately describe your API and can make an API easier to use. That said, if you use only 200 OK responses for all your successful responses, few people will mind or think less of you! You can see a summary of all the possible status codes and their expected uses at http://mng.bz/jP4x.

NOTE The 404 status code in particular causes endless debate in online forums. Should it be only used if the request didn’t match an endpoint? Is it OK to use 404 to indicate a missing entity (as in the previous example)? There are endless proponents in both camps, so take your pick!

Results and TypedResults include methods for all the common status code results you could need, but if you don’t want to use them for some reason, you can always set the status code yourself directly on the HttpResponse, as in listing 5.4. In fact, the listing shows how to define the entire response manually, including the status code, the content type, and the response body. You won’t need to take this manual approach often, but it can be useful in some situations.

Listing 5.4 Writing the response manually using HttpResponse

using System.Net.Mime
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
app.MapGet("/teapot", (HttpResponse response) =>          ❶
{
    response.StatusCode = 418;                            ❷
    response.ContentType = MediaTypeNames.Text.Plain;     ❸
    return response.WriteAsync("I'm a teapot!");          ❹
});
 
app.Run();
❶ Accesses the HttpResponse by including it as a parameter in your endpoint handler

❷ You can set the status code directly on the response.

❸ Defines the content type that will be sent in the response

❹ You can write data to the response stream manually.

HttpResponse represents the response that will be sent to the client and is one of the special types that minimal APIs know to inject into your endpoint handlers (instead of trying to create it by deserializing from the request body). You’ll learn about the other types you can use in your endpoint handlers in chapter 7.

5.3.2 Returning useful errors with Problem Details
In the MapPost endpoint of listing 5.3, we checked to see whether an entity with the given id already existed. If it did, we returned a 400 response with a description of the error. The problem with this approach is that the client—typically, a mobile app or SPA—must know how to read and parse that response. If each of your APIs has a different format for errors, that arrangement can make for a confusing API. Luckily, a web standard called Problem Details describes a consistent format to use.

DEFINITION Problem Details is a web specification (https://www.rfc-editor.org/rfc/rfc7807.html) for providing machine-readable errors for HTTP APIs. It defines the required and optional fields that should be in the JSON body for errors.

ASP.NET Core includes two helper methods for generating Problem Details responses from minimal APIs: Results.Problem() and Results.ValidationProblem() (plus their TypedResults counterparts). Both of these methods return Problem Details JSON. The only difference is that Problem() defaults to a 500 status code, whereas ValidationProblem() defaults to a 400 status and requires you to pass in a Dictionary of validation errors, as shown in the following listing.

Listing 5.5 Returning Problem Details using Results.Problem

using System.Collections.Concurrent;
 
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
app.MapGet("/fruit", () => _fruit);
 
app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404));                           ❶
 
 
app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.ValidationProblem(new Dictionary<string, string[]>   ❷
          {                                                            ❷
              {"id", new[] {"A fruit with this id already exists"}}    ❷
          }));                                                         ❷
❶ Returns a Problem Details object with a 404 status code

❷ Returns a Problem Details object with a 400 status code and includes the validation errors

The ProblemHttpResult returned by these methods takes care of including the correct title and description based on the status code, and generates the appropriate JSON, as shown in figure 5.5. You can override the default title and description by passing additional arguments to the Problem() and ValidationProblem() methods.

CH05_F05_Lock3

Figure 5.5 You can return a Problem Details response by using the Problem and ValidationProblem methods. The ValidationProblem response shown here includes a description of the error, along with the validation errors in a standard format. This example shows the response when you try to create a fruit with an id that has already been used.

Deciding on an error format is an important step whenever you create an API, and as Problem Details is already a web standard, it should be your go-to approach, especially for validation errors. Next, you’ll learn how to ensure that all your error responses are Problem Details.

5.3.3 Converting all your responses to Problem Details
In section 5.3.2 you saw how to use the Results.Problem() and Results.ValidationProblem() methods in your minimal API endpoints to return Problem Details JSON. The only catch is that your minimal API endpoints aren’t the only thing that could generate errors. In this section you’ll learn how to make sure that all your errors return Problem Details JSON, keeping the error responses consistent across your application.

A minimal API application could generate an error response in several ways:

Returning an error status code from an endpoint handler

Throwing an exception in an endpoint handler, which is caught by the ExceptionHandlerMiddleware or the DeveloperExceptionPageMiddleware and converted to an error response

The middleware pipeline returning a 404 response because a request isn’t handled by an endpoint

A middleware component in the pipeline throwing an exception

A middleware component returning an error response because a request requires authentication, and no credentials were provided

There are essentially two classes of errors, which are handled differently: exceptions and error status code responses. To create a consistent API for consumers, we need to make sure that both error types return Problem Details JSON in the response.

Converting exceptions to Problem Details

In chapter 4 you learned how to handle exceptions with the ExceptionHandlerMiddleware. You saw that the middleware catches any exceptions from later middleware and generates an error response by executing an error-handling path. You could add the middleware to your pipeline with an error-handling path of "/error":

app.UseExceptionHandler("/error");
ExceptionHandlerMiddleware invokes this path after it captures an exception to generate the final response. The trouble with this approach for minimal APIs is that you need a dedicated error endpoint, the sole purpose of which is to generate a Problem Details response.

Luckily, in .NET 7, you can configure the ExceptionHandlerMiddleware (and DeveloperExceptionPageMiddleware) to convert an exception to a Problem Details response automatically. In .NET 7, you can add the new IProblemDetailsService to your app by calling AddProblemDetails() on WebApplicationBuilder.Services. When the ExceptionHandlerMiddleware is configured without an error-handling path, it automatically uses the IProblemDetailsService to generate the response, as shown in figure 5.6.

WARNING Calling AddProblemDetails() registers the IProblemDetailsService service in the dependency injection container so that other services and middleware can use it. If you configure ExceptionHandlerMiddleware without an error-handling path but forget to call AddProblemDetails(), you’ll get an exception when your app starts. You’ll learn more about dependency injection in chapters 8 and 9.

CH05_F06_Lock3

Figure 5.6 The ExceptionHandlerMiddleware catches exceptions that occur later in the middleware pipeline. If the middleware isn’t configured to reexecute the pipeline, it generates a Problem Details response by using the IProblemDetailsService.

Listing 5.6 shows how to configure Problem Details generation in your exception handlers. Add the required IProblemDetailsService service to your app, and call UseExceptionHandler() without providing an error-handling path, and the middleware will generate a Problem Details response automatically when it catches an exception.

Listing 5.6 Configuring ExceptionHandlerMiddleware to use Problem Details

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();                ❶
 
WebApplication app = builder.Build();
 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();                     ❷
}
 
app.MapGet("/", void () => throw new Exception());   ❸
 
app.Run();
❶ Adds the IProblemDetailsService implementation

❷ Configures the ExceptionHandlerMiddleware without a path so that it uses the IProblemDetailsService

❸ Throws an exception to demonstrate the behavior

As discussed in chapter 4, WebApplication automatically adds the DeveloperExceptionPageMiddleware to your app in the development environment. This middleware similarly supports returning Problem Details when two conditions are satisfied:

You’ve registered an IProblemDetailsService with the app (by calling AddProblemDetails() in Program.cs).

The request indicates that it doesn’t support HTML. If the client supports HTML, middleware uses the HTML developer exception page from chapter 4 instead.

The ExceptionHandlerMiddleware and DeveloperExceptionPageMiddleware take care of converting all your exceptions to Problem Details responses, but you still need to think about nonexception errors, such as the automatic 404 response generated when a request doesn’t match any endpoints.

Converting error status codes to Problem Details

Returning error status codes is the common way to communicate errors to a client with minimal APIs. To ensure a consistent API for consumers, you should return a Problem Details response whenever you return an error. Unfortunately, as already mentioned, you don’t control all the places where an error code may be created. The middleware pipeline automatically returns a 404 response when an unmatched request reaches the end of the pipeline, for example.

Instead of generating a Problem Details response in your endpoint handlers, you can add middleware to convert responses to Problem Details automatically by using the StatusCodePagesMiddleware, as shown in figure 5.7. Any response that reaches the middleware with an error status code and doesn’t already have a body has a Problem Details body added by the middleware. The middleware converts all error responses automatically, regardless of whether they were generated by an endpoint or from other middleware.

CH05_F07_Lock3

Figure 5.7 The StatusCodePagesMiddleware intercepts responses with an error status code that have no response body and adds a Problem Details response body.

NOTE You can also use the StatusCodePagesMiddleware to reexecute the middleware pipeline with an error handling path, as you can with the ExceptionHandlerMiddleware (chapter 4). This technique is most useful for Razor Pages applications when you want to have a different error page for specific status codes, as you’ll see in chapter 15.

Add the StatusCodePagesMiddleware to your app by using the UseStatusCodePages() extension method, as shown in the following listing. Ensure that you also add the IProblemDetailsService to your app by using AddProblemDetails().

Listing 5.7 Using StatusCodePagesMiddleware to return Problem Details

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();        ❶
 
WebApplication app = builder.Build();
 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
 
app.UseStatusCodePages();                    ❷
 
app.MapGet("/", () => Results.NotFound());   ❸
 
app.Run();
❶ Adds the IProblemDetailsService implementation

❷ Adds the StatusCodePagesMiddleware

❸ The StatusCodePagesMiddleware automatically adds a Problem Details body to the 404 response.

The StatusCodePagesMiddleware, coupled with exception-handling middleware, ensures that your API returns a Problem Details response for all error responses.

TIP You can also customize how the Problem Details response is generated by passing parameters to the AddProblemDetails() method or by implementing your own IProblemDetailsService.

So far in section 5.3, I’ve described returning objects as JSON, returning a string as text, and returning custom status codes and Problem Details by using Results. Sometimes, however, you need to return something bigger, such as a file or a binary. Luckily, you can use the convenient Results class for that task too.

5.3.4 Returning other data types
The methods on Results and TypedResults are convenient ways of returning common responses, so it’s only natural that they include helpers for other common scenarios, such as returning a file or binary data:

Results.File()—Pass in the path of the file to return, and ASP.NET Core takes care of streaming it to the client.

Results.Byte()—For returning binary data, you can pass this method a byte[] to return.

Results.Stream()—You can send data to the client asynchronously by using a Stream.

In each of these cases, you can provide a content type for the data, and a filename to be used by the client. Browsers offer to save binary data files using the suggested filename. The File and Byte methods even support range requests by specifying enableRangeProcessing as true.

DEFINITION Clients can create range requests using the Range header to request a specific range of bytes from the server instead of the whole file, reducing the bandwidth required for a request. When range requests are enabled for Results.File() or Results.Byte(), ASP.NET Core automatically handles generating an appropriate response. You can read more about range requests at http://mng.bz/Wzd0.

If the built-in Results helpers don’t provide the functionality you need, you can always fall back to creating a response manually, as in listing 5.4. If you find yourself creating the same manual response several times, you could consider creating a custom IResult type to encapsulate this logic. I show how to create a custom IResult that returns XML and registers it as an extension in this blog post: http://mng.bz/8rNP.

5.4 Running common code with endpoint filters
In section 5.3 you learned how to use Results to return different responses when the request isn’t valid. We’ll look at validation in more detail in chapter 7, but in this section, you’ll learn how to use filters to extract common code that executes before (or after) an endpoint executes.

Let’s start by adding some extra validation to the fruit API from listing 5.5. The following listing adds an additional check to the MapGet endpoint to ensure that the provided id isn’t empty and that it starts with the letter f.

Listing 5.8 Adding basic validation to minimal API endpoints

using System.Collections.Concurrent;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
app.MapGet("/fruit/{id}", (string id) =>
{
    if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))     ❶
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            {"id", new[] {"Invalid format. Id must start with 'f'"}}
        });
    }
 
    return _fruit.TryGetValue(id, out var fruit)
            ? TypedResults.Ok(fruit)
            : Results.Problem(statusCode: 404);
});
 
app.Run()
❶ Adds extra validation that the provided id has the required format

Even though this check is basic, it starts to clutter our endpoint handler, making it harder to read what the endpoint is doing. One improvement would be to move the validation code to a helper function. But you’re still inevitably going to clutter your endpoint handlers with calls to methods that are tangential to the main function of your endpoint.

NOTE Chapter 7 discusses additional validation patterns in detail.

It’s common to perform various cross-cutting activities for every endpoint. I’ve already mentioned validation; other cross-cutting activities include logging, authorization, and auditing. ASP.NET Core has built-in support for some of these features, such as authorization (chapter 24), but you’re likely to have some common code that doesn’t fit into the specific pigeonholes of validation or authorization.

Luckily, ASP.NET Core includes a feature in minimal APIs for running these tangential concerns: endpoint filters. You can specify a filter for an endpoint by calling AddEndpointFilter()on the result of a call to MapGet (or similar) and passing in a function to execute. You can even add multiple calls to AddEndpointFilter(), which builds up an endpoint filter pipeline, analogous to the middleware pipeline. Figure 5.8 shows that the pipeline is functionally identical to the middleware pipeline in figure 4.3.

CH05_F08_Lock3

Figure 5.8 The endpoint filter pipeline. Filters execute code and then call next(context) to invoke the next filter in the pipeline. If there are no more filters in the pipeline, the endpoint handler is invoked. After the handler has executed, the filters may run further code.

Each endpoint filter has two parameters: a context parameter, which provides details about the selected endpoint handler, and the next parameter, which represents the filter pipeline. When you invoke the methodlike next parameter by calling next(context), you invoke the remainder of the filter pipeline. If there are no more filters in the pipeline, you invoke the endpoint handler, as shown in figure 5.8.

Listing 5.9 shows how to run the same validation logic you saw in listing 5.8 in an endpoint filter. The filter function accesses the endpoint method arguments by using the context.GetArgument<T>() function, passing in a position; 0 is the first argument of your endpoint handler, 1 is the second argument, and so on. If the argument isn’t valid, the filter function returns an IResult object response. If the argument is valid, the filter calls await next(context) instead, executing the endpoint handler.

Listing 5.9 Using AddEndpointFilter to extract common code

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404))
    .AddEndpointFilter(ValidationHelper.ValidateId);            ❶
 
app.Run();
 
class ValidationHelper
{
    internal static async ValueTask<object?> ValidateId(        ❷
        EndpointFilterInvocationContext context,                ❸
        EndpointFilterDelegate next)                            ❹
    {
        var id = context.GetArgument<string>(0);                ❺
        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {"id", new[]{"Invalid format. Id must start with 'f'"}}
                });
        }
 
        return await next(context);                             ❻
    }
 }
❶ Adds the filter to the endpoint using AddEndpointFilter

❷ The method must return a ValueTask.

❸ context exposes the endpoint method arguments and the HttpContext.

❹ next represents the filter method (or endpoint) that will be called next.

❺ You can retrieve the method arguments from the context.

❻ Calling next executes the remaining filters in the pipeline.

NOTE The EndpointFilterDelegate is a named delegate type. It’s effectively a Func<EndpointFilterInvocationContext, ValueTask<object?>>.

There are many parallels between the middleware pipeline and the filter endpoint pipeline, and we’ll explore them in section 5.4.1.

5.4.1 Adding multiple filters to an endpoint
The middleware pipeline is typically the best place for handling cross-cutting concerns such as logging, authentication, and authorization, as these functions apply to all requests. Nevertheless, it can be common to have additional cross-cutting concerns that are endpoint-specific, as we’ve already discussed. If you need many endpoint-specific operations, you might consider using multiple endpoint filters.

As you saw in figure 5.8, adding multiple filters to an endpoint builds up a pipeline. Like the middleware pipeline, the endpoint filter pipeline can execute code both before and after the rest of the pipeline executes. Similarly, the filter pipeline can short-circuit in the same way as the middleware pipeline by returning a result and not calling next.

NOTE You’ve already seen an example of a short circuit in the filter pipeline. In listing 5.9 we short-circuit the pipeline if the id is invalid by returning a Problem Details object instead of calling next(context).

As with middleware, the order in which you add filters to the endpoint filter pipeline is important. The filters you add first are called first in the pipeline, and filters you add last are called last. On the return journey through the pipeline, after the endpoint handler is invoked, the filters are called in reverse order, as with the middleware pipeline. As an example, consider the following listing, which adds an extra filter to the endpoint shown in listing 5.9.

Listing 5.10 Adding multiple filters to the endpoint filter pipeline

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404))
    .AddEndpointFilter(ValidationHelper.ValidateId)              ❶
    .AddEndpointFilter(async (context, next) =>                  ❷
    {
        app.Logger.LogInformation("Executing filter...");        ❸
        object? result = await next(context);                    ❹
        app.Logger.LogInformation($"Handler result: {result}");  ❺
        return result;                                           ❻
    });
 
app.Run();
❶ Adds the validation filter as before

❷ Adds a new filter using a lambda function

❸ Logs a message before executing the rest of the pipeline

❹ Executes the remainder of the pipeline and the endpoint handler

❺ Logs the result returned by the rest of the pipeline

❻ Returns the result unmodified

The extra filter is implemented as a lambda function and simply writes a log message when it executes. Then it runs the rest of the filter pipeline (which contains only the endpoint handler in this example) and logs the result returned by the pipeline. Chapter 26 covers logging in detail. For this example, we’ll look at the logs written to the console.

Figure 5.9 shows the log messages written when we send two requests to the API in listing 5.10. The first request is for an entry that exists, so it returns a 200 OK result. The second request uses an invalid id format, so the first filter rejects it. Figure 5.9 shows that neither the second filter nor the endpoint handler runs in this case; the filter pipeline has been short-circuited.

CH05_F09_Lock3

Figure 5.9 Sending two requests to the API from listing 5.10. The first request is valid, so both filters execute. An invalid id is provided in the second request, so the first filter short-circuits the requests, and the second filter doesn’t execute.

By adding calls to AddEndpointFilter, you can create arbitrarily large endpoint filter pipelines, but the fact that you can doesn’t mean you should. Moving code to filters can reduce clutter in your endpoints, but it makes the flow of your application harder to understand. I suggest that you avoid using filters unless you find duplicated code in multiple endpoints, and then favor a filter over a simple method call only if it significantly simplifies the code required.

5.4.2 Filters or middleware: Which should you choose?
The endpoint filter pipeline is similar to the middleware pipeline in many ways, but you should consider several subtle differences when deciding which approach to use. The similarities include three main parallels:

Requests pass through a middleware component on the way in, and responses pass through again on the way out. Similarly, endpoint filters can run code before calling the next filter in the pipeline and can run code after the response is generated, as shown in figure 5.8.

Middleware can short-circuit a request by returning a response instead of passing it on to later middleware. Filters can also short-circuit the filter pipeline by returning a response.

Middleware is often used for cross-cutting application concerns, such as logging, performance profiling, and exception handling. Filters also lend themselves to cross-cutting concerns.

By contrast, there are three main differences between middleware and filters:

Middleware can run for all requests; filters will run only for requests that reach the EndpointMiddleware and execute the associated endpoint.

Filters have access to additional details about the endpoint that will execute, such as the return value of the endpoint, for example an IResult. Middleware in general won’t see these intermediate steps, so it sees only the generated response.

Filters can easily be restricted to a subset of requests, such as a single endpoint or a group of endpoints. Middleware generally applies to all requests (though you can achieve something similar with custom middleware components).

That’s all well and good, but how should we interpret these differences? When should we choose one over the other?

I like to think of middleware versus filters as a question of specificity. Middleware is the more general concept, operating on lower-level primitives such as HttpContext, so it has wider reach. If the functionality you need has no endpoint-specific requirements, you should use a middleware component. Exception handling is a great example; exceptions could happen anywhere in your application, and you need to handle them, so using exception-handling middleware makes sense.

On the other hand, if you do need access to endpoint details, or if you want to behave differently for some requests, you should consider using a filter. Validation is a good example. Not all requests need the same validation. Requests for static files, for example, don’t need parameter validation, the way requests to an API endpoint do. Applying validation to the endpoints via filters makes sense in this case.

TIP Where possible, consider using middleware for cross-cutting concerns. Use filters when you need different behavior for different endpoints or where the functionality relies on endpoint concepts such as IResult objects.

So far, the filters we’ve looked at have been specific to a single endpoint. In section 5.4.3 we look at creating generic filters that you can apply to multiple endpoints.

5.4.3 Generalizing your endpoint filters
One common problem with filters is that they end up closely tied to the implementation of your endpoint handlers. Listing 5.9, for example, assumes that the id parameter is the first parameter in the method. In this section you’ll learn how to create generalized versions of filters that work with multiple endpoint handlers.

The fruit API we’ve been working with in this chapter contains several endpoint handlers that take multiple parameters. The MapPost handler, for example, takes a string id parameter and a Fruit fruit parameter:

app.MapPost("/fruit/{id}", (string id, Fruit fruit) => { /* */ });
In this example, the id parameter is listed first, but there’s no requirement for that to be the case. The parameters to the handler could be reversed, and the endpoint would be functionally identical:

app.MapPost("/fruit/{id}", (Fruit fruit, string id) => { /* */ });
Unfortunately, with this order, the ValidateId filter described in listing 5.9 won’t work. The ValidateId filter assumes that the first parameter to the handler is id, which isn’t the case in our revised MapPost implementation.

ASP.NET Core provides a solution that uses a factory pattern for filters. You can register a filter factory by using the AddEndpointFilterFactory() method. A filter factory is a method that returns a filter function. ASP.NET Core executes the filter factory when it’s building your app and incorporates the returned filter into the filter pipeline for the app, as shown in figure 5.10. You can use the same filter-factory function to emit a different filter for each endpoint, with each filter tailored to the endpoint’s parameters.

CH05_F10_Lock3

Figure 5.10 A filter factory is a generalized way to add endpoint filters. The factory reads details about the endpoint, such as its method signature, and builds a filter function. This function is incorporated into the final filter pipeline for the endpoint. The build step means that a single filter factory can create filters for multiple endpoints with different method signatures.

Listing 5.11 shows an example of the factory pattern in practice. The filter factory is applied to multiple endpoints. For each endpoint, the factory first checks for a parameter called id; if it doesn’t exist, the factory returns next and doesn’t add a filter to the pipeline. If the id parameter exists, the factory returns a filter function, which is virtually identical to the filter function in listing 5.9; the main difference is that this filter handles a variable location of the id parameter.

Listing 5.11 Using a filter factory to create an endpoint filter

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404))
    .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);     ❶
 
app.MapPost("/fruit/{id}", (Fruit fruit, string id) =>
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.ValidationProblem(new Dictionary<string, string[]>
          {
              { "id", new[] { "A fruit with this id already exists" } }
        }))
    .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory);     ❶
 
app.Run();
 
class ValidationHelper
{
    internal static EndpointFilterDelegate ValidateIdFactory(
        EndpointFilterFactoryContext context,                          ❷
        EndpointFilterDelegate next)
    {
        ParameterInfo[] parameters =                                   ❸
            context.MethodInfo.GetParameters();                        ❸
        int? idPosition = null;  
        for (int i = 0; i < parameters.Length; i++)                    ❹
        {                                                              ❹
            if (parameters[i].Name == "id" &&                          ❹
                parameters[i].ParameterType == typeof(string))         ❹
            {                                                          ❹
                idPosition = i;                                        ❹
                break;                                                 ❹
            }                                                          ❹
        }                                                              ❹
 
        if (!idPosition.HasValue)                                      ❺
        {                                                              ❺
            return next;                                               ❺
        }                                                              ❺
 
        return async (invocationContext) =>                            ❻
        {
            var id = invocationContext                                 ❼
                .GetArgument<string>(idPosition.Value);                ❼
            if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))       ❼
            {                                                          ❼
                return Results.ValidationProblem(                      ❼
                    new Dictionary<string, string[]>                   ❼
                {{ "id", new[] { "Id must start with 'f'" }}});        ❼
            }                                                          ❼
      
            return await next(invocationContext);                      ❽
        };
    }
}
❶ The filter factory can handle endpoints with different method signatures.

❷ The context parameter provides details about the endpoint handler method.

❸ GetParameters() provides details about the parameters of the handler being called.

❹ Loops through the parameters to find the string id parameter and record its position

❺ If the id parameter isn’t not found, doesn’t add a filter, but returns the remainder of the pipeline

❻ If the id parameter exists, returns a filter function (the filter executed for the endpoint)

❼ If the id isn’t valid, returns a Problem Details result

❽ If the id is valid, executes the next filter in the pipeline

The code in listing 5.11 is more complex than anything else we’ve seen so far, as it has an extra layer of abstraction. The endpoint middleware passes an EndpointFilterFactoryContext object to the factory function, which contains extra details about the endpoint in comparison to the context passed to a normal filter function. Specifically, it includes a MethodInfo property and an EndpointMetadata property.

NOTE You’ll learn about endpoint metadata in chapter 6.

The MethodInfo property can be used to control how the filter is created based on the definition of the endpoint handler. Listing 5.11 shows how you can loop through the parameters to check for the details you need—a string id parameter, in this case—and customize the filter function you return.

If you find all these method signatures to be confusing, I don’t blame you. Remembering the difference between an EndpointFilterFactoryContext and EndpointFilterInvocationContext and then trying to satisfy the compiler with your lambda methods can be annoying. Sometimes, you yearn for a good ol’ interface to implement. Let’s do that now.

5.4.4 Implementing the IEndpointFilter interface
Creating a lambda method for AddEndpointFilter() that satisfies the compiler can be a frustrating experience, depending on the level of support your integrated development environment (IDE) provides. In this section you’ll learn how to sidestep the issue by defining a class that implements IEndpointFilter instead.

You can implement IEndpointFilter by defining a class with an InvokeAsync() that has the same signature as the lambda defined in listing 5.9. The advantage of using IEndpointFilter is that you get IntelliSense and autocompletion for the method signature. The following listing shows how to implement an IEndpointFilter class that’s equivalent to listing 5.9.

Listing 5.12 Implementing IEndpointFilter

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404))
    .AddEndpointFilter<IdValidationFilter>();         ❶
 
app.Run();
 
class IdValidationFilter : IEndpointFilter            ❷
{
    public async ValueTask<object?> InvokeAsync(      ❸
        EndpointFilterInvocationContext context,      ❸
        EndpointFilterDelegate next)                  ❸
    {
        var id = context.GetArgument<string>(0);
        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {"id", new[]{"Invalid format. Id must start with 'f'"}}
                });
        }
 
        return await next(context);
    }
}
❶ Adds the filter using the generic AddEndpointFilter method

❷ The filter must implement IEndpointFilter . . .

❸ . . . which requires implementing a single method.

Implementing IEndpointFilter is a good option when your filters become more complex, but note that there’s no equivalent interface for the filter-factory pattern shown in section 5.4.3. If you want to generalize your filters with a filter factory, you’ll have to stick to the lambda (or helper-method) approach shown in listing 5.11.

5.5 Organizing your APIs with route groups
One criticism levied against minimal APIs in .NET 6 was that they were necessarily quite verbose, required a lot of duplicated code, and often led to large endpoint handler methods. .NET 7 introduced two new mechanisms to address these critiques:

Filters—Introduced in section 5.4, filters help separate validation checks and cross-cutting functions such as logging from the important logic in your endpoint handler functions.

Route groups—Described in this section, route groups help reduce duplication by applying filters and routing to multiple handlers at the same time.

When designing APIs, it’s important to maintain consistency in the routes you use for your endpoints, which often means duplicating part of the route pattern across multiple APIs. As an example, all the endpoints in the fruit API described throughout this chapter (such as in listing 5.3) start with the route prefix /fruit:

MapGet("/fruit", () => {/* */})

MapGet("/fruit/{id}", (string id) => {/* */})

MapPost("/fruit/{id}", (Fruit fruit, string id) => {/* */})

MapPut("/fruit/{id}", (Fruit fruit, string id) => {/* */})

MapDelete("/fruit/{id}", (string id) => {/* */})

Additionally, the last four endpoints need to validate the id parameter. This validation can be extracted to a helper method and applied as a filter, but you still need to remember to apply the filter when you add a new endpoint.

All this duplication can be removed by using route groups. You can use route groups to extract common path segments or filters to a single location, reducing the duplication in your endpoint definitions. You create a route group by calling MapGroup("/fruit") on the WebApplication instance, providing a route prefix for the group ("/fruit", in this case), and MapGroup() returns a RouteGroupBuilder.

When you have a RouteGroupBuilder, you can call the same Map* extension methods on RouteGroupBuilder as you do on WebApplication. The only difference is that all the endpoints you define on the group will have the prefix "/fruit" applied to each endpoint you define, as shown in figure 5.11. Similarly, you can call AddEndpointFilter() on a route group, and all the endpoints on the group will also use the filter.

CH05_F11_Lock3

Figure 5.11 Using route groups to simplify the definition of endpoints. You can create a route group by calling MapGroup() and providing a prefix. Any endpoints created on the route group inherit the route template prefix, as well as any filters added to the group.

You can even create nested groups by calling MapGroup() on a group. The prefixes are applied to your endpoints in order, so the first MapGroup() call defines the prefix used at the start of the route. app.MapGroup("/fruit").MapGroup("/citrus"), for example, would have the prefix "/fruit/citrus".

TIP If you don’t want to add a prefix but still want to use the route group for applying filters, you can pass the prefix "/" to MapGroup().

Listing 5.13 shows an example of rewriting the fruit API to use route groups. It creates a top-level fruitApi, which applies the "/fruit" prefix, and creates a nested route group called fruitApiWithValidation for the endpoints that require a filter. You can find the complete example comparing the versions with and without route groups in the source code for this chapter.

Listing 5.13 Reducing duplication with route groups

using System.Collections.Concurrent;
 
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();
 
var _fruit = new ConcurrentDictionary<string, Fruit>();
 
RouteGroupBuilder fruitApi = app.MapGroup("/fruit");                 ❶
 
fruitApi.MapGet("/", () => _fruit);                                  ❷
 
RouteGroupBuilder fruitApiWithValidation = fruitApi.MapGroup("/")    ❸
    .AddEndpointFilter(ValidationHelper.ValidateIdFactory);          ❹
 
fruitApiWithValidation.MapGet("/{id}", (string id) =>                ❺
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404));
 
fruitApiWithValidation.MapPost("/{id}", (Fruit fruit, string id) =>  ❺
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.ValidationProblem(new Dictionary<string, string[]>
          {
              { "id", new[] { "A fruit with this id already exists" } }
        }));
 
fruitApiWithValidation.MapPut("/{id}", (string id, Fruit fruit) =>   ❺
{
    _fruit[id] = fruit;
    return Results.NoContent();
});
 
fruitApiWithValidation.MapDelete("/fruit/{id}", (string id) =>       ❺
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent();
});
 
app.Run();
❶ Creates a route group by calling MapGroup and providing a prefix

❷ Endpoints defined on the route group will have the group prefix prepended to the route.

❸ You can create nested route groups with multiple prefixes.

❹ You can add filters to the route group . . .

❺ . . . and the filter will be applied to all the endpoints defined on the route group.

In .NET 6, minimal APIs were a bit too verbose to be generally recommended, but with the addition of route groups and filters, minimal APIs have come into their own. In chapter 6 you’ll learn more about routing and route template syntax, as well as how to generate links to other endpoints.

Summary
HTTP verbs define the semantic expectation for a request. GET is used to fetch data, POST creates a resource, PUT creates or replaces a resource, and DELETE removes a resource. Following these conventions will make your API easier to consume.

Each HTTP response includes a status code. Common codes include 200 OK, 201 Created, 400 Bad Request, and 404 Not Found. It’s important to use the correct status code, as clients use these status codes to infer the behavior of your API.

An HTTP API exposes methods or endpoints that you can use to access or change data on a server using the HTTP protocol. An HTTP API is typically called by mobile or client-side web applications.

You define minimal API endpoints by calling Map* functions on the WebApplication instance, passing in a route pattern to match and a handler function. The handler functions runs in response to matching requests.

There are different extension methods for each HTTP verb. MapGet handles GET requests, for example, and MapPost maps POST requests. You use these extension methods to define how your app handles a given route and HTTP verb.

You can define your endpoint handlers as lambda expressions, Func<T, TResult> and Action<T> variables, local functions, instance methods, or static methods. The best approach depends on how complex your handler is, as well as personal preference.

Returning void from your endpoint handler generates a 200 response with no body by default. Returning a string generates a text/plain response. Returning an IResult instance can generate any response. Any other object returned from your endpoint handler is serialized to JSON. This convention helps keep your endpoint handlers succinct.

You can customize the response by injecting an HttpResponse object into your endpoint handler and then setting the status code and response body. This approach can be useful if you have complex requirements for an endpoint.

The Results and TypedResults helpers contain static methods for generating common responses, such as a 404 Not Found response using Results.NotFound(). These helpers simplifying returning common status codes.

You can return a standard Problem Details object by using Results.Problem() and Results.ValiationProblem(). Problem() generates a 500 response by default (which can be changed), and ValidationProblem() generates a 400 response, with a list of validation errors. These methods make returning Problem Details objects more concise than generating the response manually.

You can use helper methods to generate other common result types on Results, such as File() for returning a file from disk, Bytes() for returning arbitrary binary data, and Stream() for returning an arbitrary stream.

You can extract common or tangential code from your endpoint handlers by using endpoint filters, which can keep your endpoint handlers easy to read.

Add a filter to an endpoint by calling AddEndpointFilter() and providing the lambda function to run (or use a static/instance method). You can also implement IEndpointFilter and call AddEndpointFilter<T>(), where T is the name of your implementing class.

You can generalize your filter functions by creating a factory, using the overload of AddEndpointFilter() that takes an EndpointFilterFactoryContext. You can use this approach to support endpoint handlers with various method signatures.

You can reduce duplication in your endpoint routes and filter configuration by using route groups. Call MapGroup() on WebApplication, and provide a prefix. All endpoints created on the returned RouteGroupBuilder will use the prefix in their route templates.

You can also call AddEndpointFilter() on route groups. Any endpoints defined on the group will also have the filter, as though you defined them on the endpoint directly, removing the need to duplicate the call on each endpoint.


