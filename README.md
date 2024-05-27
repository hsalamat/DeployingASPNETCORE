ASP.NET Core Applications and Deployment

# 1. Understanding the Basics

## Server Architecture Review
1. FrameWork
2. Server
3. Handling Internet Traffic

### FrameWork Choices
1. .NET Framework: Older libraries or legacy code
2. .NET Core: Cross-platform, Fast/Optmized 

### Server Choices
1. ASP.NET Core apps run on top of a server library
2. Similar to OWIN self-hosted model
3. it converts raw HTTP into structures that ASP.NET Core is looking for
4. ASP.NET Core ships with 2 different servers
a. HTTP.sys: Windows only, Support Windows authentication
b. Kestrel: Cross-platform, Highly optimized
5. For most project Kestrel is the best choice

### Handling Internet Traffic
1. Serve Directly: Both Kestrel and HTTP.sys can serve traffic directly. It's called Edge server.
2. Or sit behind a load balancer and reverse proxy

If you have a simple application and simple server Internet   ----> Server (Kestrel or HTTP.sys). You need a certificate to add to Kestrel or HTTP.sys to enable HTTPS. The problem with this approach is not meant for scaling or using with lod balancer.

Or you can use Reverse Proxy like IIS or NgineX. A reverse proxy is a server that sits in front of web servers and forwards client (e.g. web browser) requests to those web servers. Reverse proxies are typically implemented to help increase security, performance, and reliability.

A forward proxy, often called a proxy, proxy server, or web proxy, is a server that sits in front of a group of client machines. When those computers make requests to sites and services on the Internet, the proxy server intercepts those requests and then communicates with web servers on behalf of those clients, like a middleman.

Client 1,2,.. <---> Forward Proxy <---> Internet <---> Web Servers Origin 1,2,...

There are a few reasons one might want to use a forward proxy:

To avoid state or institutional browsing restrictions - Some governments, schools, and other organizations use firewalls to give their users access to a limited version of the Internet. A forward proxy can be used to get around these restrictions, as they let the user connect to the proxy rather than directly to the sites they are visiting.
To block access to certain content - Conversely, proxies can also be set up to block a group of users from accessing certain sites. For example, a school network might be configured to connect to the web through a proxy which enables content filtering rules, refusing to forward responses from Facebook and other social media sites.
To protect their identity online.


A reverse proxy is a server that sits in front of one or more web servers, intercepting requests from clients. This is different from a forward proxy, where the proxy sits in front of the clients. With a reverse proxy, when clients send requests to the origin server of a website, those requests are intercepted at the network edge by the reverse proxy server. The reverse proxy server will then send requests to and receive responses from the origin server.

Client 1,2,.. <---> Internet <---> Reverse Proxy <---> Web Serve Origin 1,2,..

Below we outline some of the benefits of a reverse proxy:

Load balancing - A popular website that gets millions of users every day may not be able to handle all of its incoming site traffic with a single origin server. Instead, the site can be distributed among a pool of different servers, all handling requests for the same site. In this case, a reverse proxy can provide a load balancing solution which will distribute the incoming traffic evenly among the different servers to prevent any single server from becoming overloaded. In the event that a server fails completely, other servers can step up to handle the traffic.
Protection from attacks - With a reverse proxy in place, a web site or service never needs to reveal the IP address of their origin server(s). This makes it much harder for attackers to leverage a targeted attack against them, such as a DDoS attack. Instead the attackers will only be able to target the reverse proxy, such as Cloudflare’s CDN, which will have tighter security and more resources to fend off a cyber attack.
Global server load balancing (GSLB) - In this form of load balancing, a website can be distributed on several servers around the globe and the reverse proxy will send clients to the server that’s geographically closest to them. This decreases the distances that requests and responses need to travel, minimizing load times.
Caching - A reverse proxy can also cache content, resulting in faster performance. For example, if a user in Paris visits a reverse-proxied website with web servers in Los Angeles, the user might actually connect to a local reverse proxy server in Paris, which will then have to communicate with an origin server in L.A. The proxy server can then cache (or temporarily save) the response data. Subsequent Parisian users who browse the site will then get the locally cached version from the Parisian reverse proxy server, resulting in much faster performance.
SSL encryption - Encrypting and decrypting SSL (or TLS) communications for each client can be computationally expensive for an origin server. A reverse proxy can be configured to decrypt all incoming requests and encrypt all outgoing responses, freeing up valuable resources on the origin server.

## Choose a deployment strategy
1. IIS on Windows. IIS acting a reverse proxy to Kestrel. You can use VS to copy the binaries or copy them manually.
2. Kestrel on Linux: Use Kestrel + a proxy like NGINEX or Apache. Just like IIS, you have to copy the bianries to IIS.
3. Azure App Service: You can publish your application to Azure App Service. You can upload the binaries using VS or set up a continous integration.
4. Kestrel on Docker Container. This involves creating a docker image from your application binaries independencies and running that image in a Docker container.

Which one to choose?

1. If you have an existing IIS and Windows infrastructure, publishing to IIS might be your best option.
2. If you have a linux-heavy infrastructure such as Amazon web servies, you can set Krestel and .NET CORE on Linux box to run your application.
3. Docker is the most powerful option. Once you produce a docker image for your application, you can run it in any docker host. You won't need to manually copy binaries.

## Project Setup
1. Create a ASP.NET Core Web APP (Model-View-Controller)
2. or via visual studio code
C:\Hooman\DeployingASPNETCORE> dotnet new mvc -o HelloCoreWorldDocker

## Write code for development and production
1. If you go to project properties > Debug tab, you see that ASPNETCORE_ENVIRONMENT is set to Development. Which means when you run your app from visual studio, this is environment that you are running from.  This has been setup in LaunchSettings.json

"IIS Express": {
  "commandName": "IISExpress",
  "launchBrowser": true,
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}

## Include Static Content
1. wwwroot is the folder where all the static files (css, html, js, etc..) are stored.
2. These files are automatically included when you deploy your application. For example, wwwroot/js/site.js will be deployed to hoomanator.com/js/site.js

## Project file settings
1. Right Click on your project file and select Edit.
2. csproj include dependecies, framework
3. You can include/exclude files. For example, if you have a static file that is outside wwwroot folder, it won't get automatically included in the published output. So you add me as if they are in the wwwroot.

 <ItemGroup>
   <ResolvedFileToPublish Include="readme.md">
     <RelativePath>wwwroot\readme.md</RelativePath>
   </ResolvedFileToPublish>
 </ItemGroup>

 5. To execlude txt files, you add this to your ItemGroup tag:

     <Content Update="wwwwroot\**\*.txt" CopyToPublishDirectory="Never">
      
    </Content>
    6. More on that https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-8.0
    
# 2. Https and ASP.NET Core

## HTTPS overview
1. HTTPS is enabled by default in ASP.NET Core
2. If you look at Project Properties, IIS Express Profile tab, SSL is enabled with separate HTTPS port for the application.
3. If you are application is configured for Edge Server with no reverse proxy, you configure and install the certificate at the Kestrel or Http.sys layer. That means the server encrypt the response and sends it back to browsers or clients.
4. If you are using a load balancer or reverse proxy, you'll install and configure the certificate at the proxy layer instead meaning the certificate is installed in the reverse proxy. The proxy will forward requests over plain HTTP to your internal servers along with special headers that indicates tha the request is being forwarded. You will need to add few lines to your ASP.NET Core application to recognize theese headers.

## Configure Kestrel as an edge server
1. You can add some options to Kestrel to give it your own HTTPS certicates.
2. To add a certificate, first you can get a certificate (even for free from Let's Encrypt letsencrypt.org), and then Go to appsettings.json file
"Kestrel": {
"Endpoints": {
"Https": {
"Url": "https://*:7003",
"Certificate": {
"Path": "path-to-your-pfx-file.pfx",
"Password": "password"
}
}
}
}
3. With this configuration, Kestrel will use this certicaite for all HTTPS responses

## How Forwading Works
1. It's common to have multiple web servers behind a reverse proxy or a load balancer because it makes scaling to handle more traffic much easier
2. It also makes it easier to handle HTTPS and certificates.
3. Instead of copying the certificate and adding the right configuration to each server, you only need to configure the certificate at the reverse proxy level.
4. When a new connection arrives, the proxy handles the HTTPS connection, and then it turns around and make an unencrypted HTTP connection internally to your web servers. That way your web server will let the proxy to do all the work and don't have to worry about HTTPS, certificates and encryption.
5. Their responses are relayed back over HTTPS by the proxy.
6. The proxy and load balancer will include one or more headers on the "Internal HTTP connection" so that your web servers can understand if your requests started on HTTPS.

Client --> https://..  --> Reverse Proxy --> HTTP + Headers --> Application Code Hosted on Server (Http Sys or Kestrel)
Client <-- https://..  <-- Reverse Proxy <-- https://... + Forwarded Headers <-- Application Code Hosted on Server (Http Sys or Kestrel)

7. If you use a proxy or a load balancer, it's important to configure ASP.NET Core it look for these forward headers:

X-Forwarded-For: 203.0.113.195
X-Forwarded-Host: Hoomanator.com
X-Forwarded-Proto: https

## Configure Forwarded Headers Middleware
1. Forwarded Headers Middleware helps your ASP.NET Core application to look for headers added by an upstream proxy or load balancer.
2. How you configure middleware depends on how you plan to host your application.
3. If you are hosting on IIS, you don't have to do anything, this middleware is already enabled by default.
4. If you are using another proxy besides IIS, open up your program.cs file and add the following to the configure method. This tells ASP.NET Core to look for XForwardedFor and XForwardedProto headers. With this in place, any application code that verifies the user is on an https connection will succeed because ASP.NET Core will be able to understand that the proxy, the application code is sending this http connection to, is adding these headers to the request.

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
}
    );

The configuration for XForwardedFor and XForwardedProto is a typical configuration for most proxies, but you might have to check the docuemntation for proxies to see exactly which headers it's going to forward to ASP.NET Core and update the line here.

5. Forwarded Headers Middleware is high up in the middlewere pipeline above

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

6. Usually it should be below the exception handling middleware.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

# 3. Deploying to IIS

## Setup IIS
1. In a Windows environment, IIS acts as a reverse proxy that forwards requests to Kestrel, which is hosting your ASP.NET Core Application.
2. ASP.NET Core Module (ANCM) provides this functionality as a plugin inside of IIS.
3. IIS and ANCM Don't support Http.sys! They only support Kestrel.
Internet --> IIS --> ANCM --> Kestrel --> ASP.NET Core App
4. If you are using HTTP.Sys as your web server on a Windows environment, you'll need to either use it as a edge server or put it behind a different proxy server.
5. Before deploying ASP.NET Core Application to IIS, you need to do a bit of configuration on your server machine.
5. Setting up IIS on Windows 11:
a. Install IIS (if necessary)
Open the Start menu.
Type "features" and select Turn Windows features on or off.
Tick the Internet Information Services checkbox and select OK. Make sure IIS Maangement Console is turned on!
Wait for the installation to complete and select Close.
b. Install .NET Core hosting bundle (this package includes .NET core runtime as well as ASP.NET Core Module for IIS)
https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/hosting-bundle?view=aspnetcore-8.0#install-the-net-core-hosting-bundle

https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.4-windows-hosting-bundle-installer


c. Restart IIS
To manually restart IIS, stop the Windows Process Activation Service (WAS) and then restart the World Wide Web Publishing Service (W3SVC) and any dependent services. Execute the following commands in an elevated command shell (as an administrator):

net stop was /y
net start w3svc

## Create a site and app pool
1. Once IIS and .Net Core hosting module is installed, we can host ASP.NET Core Application.
2. Open up the IIS Manager from the task bar.
3. Right click on Sites and add a web site (HelloCoreWorld).
4. Create a directory C:\Hooman\DeployingASPNETCORE\websites\HelloCoreWorld where you want to publish your application.
5. Use localhost for the host name.
6. IIS will create a website and create an app pool to go along with it.
7. For ASP.NET application, we need to configure the setting for the app pool.
8. Click on the Application Pools and right click on HelloCoreWorld basic settings
9. Change .NET CLR Version to "No Managed Code". Basically we are saying IIS is not managing our code! Our code is managed by Kestrel while the requests will simply be forwarded along by IIS.
10. Go to WebSite, seleect HelloCoreWorld, and clcik on "Browse Website".
11. This shows an 403.14 error, becuase we do not have any file under ..\websites\HelloCoreWorld.

## Setup Data Protection
Take advantage of the easy-to-use cryptographic API in ASP.Net Core to secure sensitive data in your applications.
The Data protection stack in ASP.Net Core provides an easy-to-use cryptographic API for protecting data, including the necessary mechanisms for encryption and decryption. This section shows how we can work with this API when building our ASP.Net Core applications.
Encryption and hashing are two important concepts related to security that are often used interchangeably, but incorrectly. Encryption is a technique of converting data from one form to another using a cryptographic algorithm. It is a two-way function as the data that has been encrypted can only be decrypted using a proper key. The encrypted data is known as cipher text. Encryption is by far the most effective way to secure data in today’s communication systems.
By contrast, hashing is a technique that generates a unique message digest from a string of text. It should be noted that the hashed data is always unique; you cannot produce the same hash value from different text. Further, it is almost impossible to get back the original text from the hashed value. So, while encryption is a two-way technique that includes both encryption and decryption of data using a key, hashing is a one-way technique that changes a string of plain text to a unique digest that cannot easily be reversed back to the original text.

Steps:
- Install the Microsoft.AspNetCore. DataProtection NuGet package:
   Install-Package Microsoft.AspNetCore.DataProtection
- Configure the Data Protection API in ASP.Net Core:
   The AddDataProtection extension method can be used to configure the Data Protection API.
- Encrypting data with the Data Protection API in ASP.Net Core



1. ASP.NET Core uses the data protection API (DP API) to encrypt and store keys that are used for authtication in your application.
2. When hosting your application with IIS, you'll need to run a small script to create a registry hive for these keys, otherwise the keys will be regenerated when you restart your application which will invalidate any user sessions or cookies that were encrypted with the previous old keys. (A hive is a logical group of keys, subkeys, and values in the registry that has a set of supporting files loaded into memory when the operating system is started or a user logs in.)
3. To create a data protection registry hive, 
oldway: you'll need a powershell script that's provided by the ASP.NET Core Team (https://github.com/aspnet/DataProtection/blob/master/Provision-AutoGenKeys.ps1).
it seems there is a new way to do so: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/using-data-protection?view=aspnetcore-8.0

a. Right click on the project, Manage Nuget Packages > Browser > Search for "microsoft.aspnetcore.dataprotection"
b. In Program.cs

var builder = WebApplication.CreateBuilder(args);

//Add data protection
builder.Services.AddDataProtection();

/If you would like to store the key in the file system, you need to configure the data protection
//builder.Services.AddDataProtection().PersistKeysToFileSystem(
//    new System.IO.DirectoryInfo(@"C:\Hooman\Temp")
//    );

//To configure the system to use a default key lifetime of 14 days instead of 90 days
//builder.Services.AddDataProtection().SetDefaultKeyLifetime(TimeSpan.FromDays(14));


// Add services to the container.
builder.Services.AddControllersWithViews();

c. Go to HomeController.cs
 public class HomeController : Controller
 {
     private readonly ILogger<HomeController> _logger;
     //step1
     IDataProtector _dataProtector;


     //step2
     //public HomeController(ILogger<HomeController> logger)
     public HomeController(ILogger<HomeController> logger, IDataProtectionProvider provider)
     {
         _logger = logger;

         //step3
         _dataProtector = provider.CreateProtector(GetType().FullName);
     }

d. Right click on "Model" folder and add a TestModel class     

namespace HelloCoreWorld.Models
{
    public class TestModel
    {
        public string ProtectedData { get; set; }
        public string UnProtectedData { get; set; }
    }
}

e. Go back to HomeController, update Index() method to display our encrypted hello world.

        public IActionResult Index()
        {
            TestModel testModel = new TestModel();
            var protectedData = _dataProtector.Protect("Hello World");
            testModel.ProtectedData = "Protected Data: " + protectedData;

            var unProtectedData = _dataProtector.Unprotect(protectedData);
            testModel.UnProtectedData = "UnProtected Data: " + unProtectedData;

            return View(testModel);
        }

f. Go to Views folder > home > Open index.cshtml and replace it with:

@model HelloCoreWorld.Models.TestModel
@{
    ViewData["Title"] = "Home Page";
}

<p>@Model.ProtectedData</p>
<br/>
<br/>
<p>@Model.UnProtectedData</p>

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>

h. Run your application to see the encrypted "Hello World" in the index page. 

## Publish your app with Visual Studio
1. Right click on the project and select "publish"
2. The first time you publish, you have to create a publish profile
3. If you are publishing locally, you choose "Folder".
4. Remember that we created a folder: C:\Hooman\DeployingASPNETCORE\websites\HelloCoreWorld
we use that folder to publish to.
5. We leave the current settings (except clean all the existing files prior to publish so the deployment directory starts from a clean slate) and click on Publish button.
6. All of these settings are saved in a file called a publish profile under Properties > PublishedProfiles in the solution explorer.
7. to test it, go to any browser: http://localhost

## Publish your app via the command line
1. dotnet publish tool
2. Opent he powsershell
3. Cd to project folder: C:\Hooman\DeployingASPNETCORE\HelloCoreWorld\HelloCoreWorld
4. type: dotnet restore
this restores any NuGet packages for the project.
5. dotnet build
6. dotnet publish -c Release
7. The binaries are in the bin folder
8. explorer .
9. Everything we need is in the publish folder:
C:\Hooman\DeployingASPNETCORE\HelloCoreWorld\HelloCoreWorld\bin\Release\net8.0\publish
10. You can copy the content of this folder to our website manually C:\Hooman\DeployingASPNETCORE\websites\HelloCoreWorld

## Understand Web.config
1. Notice there is a web.config file in the publish directory
2. ASP.NET core doesn't use Web.config! It uses a new configuration model based on appsettings.json.
3. But... IIS still needs this web.config even if ASP.NET Core isn't using it.
4. Web.config is used to configure ASP.NET Core module that IIS uses to act as a reverse proxy to Kestrel.
5. When you run dotnet publish, Web.Config is generated for you automatically. You shouldn't touch it or modify it yourself.
6. Web.config should be in the root for IIS to properly host your ASP.NET Core application.
7. Web.config prevents IIS from accidently serving the content from the project directory, which could contain sensitive configuration files!

# 4. Deploying to Azure

## Get Started With Azure
1. Microsoft provides plugins for visual studio that makes it easy to deploy your application.
2. Sign up for free/student Azure account: [portal.azure.com](https://portal.azure.com/#home)
3. Open Visual Studio Installer and validate that the workloads Azure development and ASP.NET and web development are installed. https://learn.microsoft.com/en-us/dotnet/azure/configure-visual-studio

## Deploy to Azure With Visual Studio
1. Azure app service is a managed hosting service for your application, which means you don't have to worry about setting up virtual machines or servers.
2. You deploy your ASP.NET Core application directly to Azure App Service.
3. We need to set up an Azure App Service Deployment Profile.
4. Righ click on the project, select "publish", and create a new "Azure" profile.
5. Select "Azure App Service (Windows)
6. Enter your Azure credentials
7. Once you log in, you need to create a resource group for your ASP.NET CORE application. A resource group is kind of a high level container for resources you provision for the application in Azure, such as the App Service Instance, databases, and so on.
8. Click on "Create new" to create a new windows App Service.
9. Update the app or resource group names to match your app (for testing you can leave it as it is). For Hosting Plan, click on "new" and change the size to "Free" if you do not want to pay!
10. When I click "Create", these resources will be provisioned in Azure, which can take a few seconds. Then Click "Finish" to create the profile. Now it's ready to publish. 
11. click publish! Visual Studio is going to compile your application and upload it directly into Azure into that new App Service instance we just created.
12. The URL name is in the format of appname.azurewebsites.net. Once it spins up, you'll see your application running on Azure.
https://hoomanator.azurewebsites.net

## Continous deployment with Azure
https://learn.microsoft.com/en-us/azure/app-service/deploy-continuous-deployment?tabs=github%2Cgithubactions

### Prepare your repository
1. To get automated builds from Azure App Service build server, make sure that your repository root has the correct files in your project.(For ASP.NET Core, you'll need	*.sln or *.csproj)

### Configure the deployment source
1. We showed you how to publish and deploy your application using Visual Studio. Now we see how to set up a continuous deployment from a source control repository.
2. In the Azure portal, go to the management page for your App Service app.
3. In the left pane, select Deployment Center. Then select Settings.
4. If you're deploying from GitHub for the first time, select Authorize and follow the authorization prompts. If you want to deploy from a different user's repository, select Change Account.
5. After you authorize your Azure account with GitHub, select the Organization, Repository, and Branch you want.
6. Under Authentication type, select User-assigned identity for better security. 

### Github Actions
The GitHub Actions build provider is available only for GitHub deployment. When configured from the app's Deployment Center, it completes these actions to set up CI/CD:

Deposits a GitHub Actions workflow file into your GitHub repository to handle build and deploy tasks to App Service.
For basic authentication, adds the publish profile for your app as a GitHub secret. The workflow file uses this secret to authenticate with App Service.
For user-assigned identity, Creates a federated credential between a user-assigned managed identity in Azure and your selected repository and branch in GitHub.
Creates the secrets AZURE_CLIENT_ID, AZURE_TENANT_ID, and AZURE_SUBSCRIPTION_ID from the federated credential in your selected GitHub repository.
Assigns the identity to your app.
In a GitHub Actions workflow in your GitHub repository, you can then use the Azure/login action to authenticate with your app by using OpenID Connect.
Captures information from the workflow run logs and displays it on the Logs tab in the Deployment Center.

1. We have a workflow YAML file which specifies events, jobs, runners, steps, and actions
2. An event is a trigger for the workflow.
3. Common event in a repository when someone pushes new code.
name: Build and deploy ASP.Net Core app to Azure Web App - Hoomanator

on:
  push

4. When an event occured, it's going to run all the jobs within the workflow. Here we have a simple job "build" that specify multiple steps and actions. For example, here the first action is to setup a .NET Core. The second action is to build the project. This job runs on "windows-latest" (the Runner!).

on:
  push:
    branches:
      - main
  workflow_dispatch:
jobs:
  build:
    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v4

      - name: Set up .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '3.1'
          include-prerelease: true

      - name: Build with dotnet
        run: dotnet build --configuration Release

      - name: dotnet publish
        run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp

      - name: Upload artifact for deployment job
        uses: actions/upload-artifact@v3
        with:
          name: .net-app
          path: ${{env.DOTNET_ROOT}}/myapp

5. In your github, the root directory should have a folder .github/workflows where this YAML file (.yml) exist.   
6. The deault dotnet version is 3-1 which uses old version of Node, which "fails" your build! Update the dotnt-version to 8.0 (or a proper version)
7. All your actions, gets recorded in the github. You can check them out:
https://github.com/[your_repository]/[your_app]/actions/ 
https://github.com/hsalamat/hoomanator/actions/       


### Test the deployment
1. Switch back to visual studio, we can make a small change to the index view.
<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <b>Automatically deployed!</b>
</div>

2. A the bottom of the solution explorer, select "Github changes", Stage the Index.cshtml and commit. Sync and push it to the Github remote repository.
3. It may take couple of minutes to show up.
4. Continous deployment is a great way to automate the process of shipping code.

# 6. Deploying with Docker
## Docker Overview
Containerization is a new technology that many ASP.NET developers may not yet have a lot of experience with. Docker is the most popular containerization tool today. I'll explain how Docker works at a high level and why it's a good choice for deploying ASP.NET Core applications. When you set up a server or virtual machine to host your application, think of all the stuff that you have to do. Install dependencies like .NET and third-party libraries, configure things like IIS and NGINX, add environment variables and so on. If you have multiple servers, you have to do this setup manually on each one. Because of this, adding and maintaining servers becomes a complex task. With Docker, you instead create an image from your application that includes all of the required dependencies, files, and setup steps. The Docker image contains everything needed to take a machine from a blank slate all the way up to running your application. You can then use this image to create one or more Docker containers. To use a programming metaphor, think of images as classes. A container is a process that runs on the Docker host and is isolated from other running processes on the machine. The container is a live version of the image, so to continue the programming metaphor, think of containers as instances of the classes. The Docker host can run many containers at once, all isolated from each other. This approach has a few benefits. Instead of managing servers or virtual machines that have been carefully set up to run your application, you only need a server that can run Docker. The dependencies and setup steps required to host and run your application are explicitly defined in the Docker image. That means that Docker images become the fundamental unit of deployment. When you build a new version of your application, you create an updated Docker image and push that out to your running Docker containers. Images can be versions tagged and swapped in and out of containers easily, so adding more servers just means spinning up more containers from the same image. All of this means that using Docker makes deploying and managing your application servers much easier. The .NET Core team at Microsoft has created a set of base images that make it straightforward to deploy your ASP.NET Core applications using Docker.
## Create a Docker image
1. The first step to creating Docker images is installing Docker on your development machine. The official Docker website https://hub.docker.com has instructions for Windows, Mac, and Linux.  
2. Docker registries are used to host and distribute Docker Images. Docker Hub is Docker's official cloud-based registry. To get started with Docker Hub you can pull (download) an image or push (upload) one of your local images.
3. Docker Desktop is a one-click-install tool focused on developers writing applications for containers and micro-services. It provides a nice, friendly GUI and CLI to manage your container images and containers running locally.
4.  Go to https://hub.docker.com, and install Docker Desktop Installer.
5. When you finish installing Docker and restart your machine, you can open up PowerShell and run docker --version to make sure Docker is installed and running. 
6. The next thing that you need is a Docker file. The Docker file is like a recipe that tells Docker how to build an image for your application. You can create a Docker file from inside Visual Studio by right clicking on your project and choosing add, Docker support. I used Linux option (because I have Linux context) and Docker file
https://learn.microsoft.com/en-us/visualstudio/containers/container-build?view=vs-2022


7. Open Visual Studio 2022 Developer PowerShell v17.9.7
8. PS C:\Users\USER\source\repos> cd C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker
PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> MSBuild HelloCoreWorldDocker.csproj /t:ContainerBuild /p:Configuration=Release

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker images
REPOSITORY             TAG       IMAGE ID       CREATED         SIZE
hellocoreworlddocker   latest    8f667ccea273   7 minutes ago   225MB


Note: So assuming you're running a recent version of docker.exe (v20+):
Get List all contexts: meaning what is your "container OS"
PS> docker context ls
you can change the context:
PS> docker context use desktop-linux


******************************************
********************************************
6 option b) this is not working!!!! You can also write this file by hand if you aren't using Visual Studio. 
Create "Dockerfile" with Notepad. It's important that this file be saved in the root of the project, in the same directory as the csproj or program.cs files.  If you're on Windows, use quotes to save the file with no extension. 
The docker file will start with 

FROM microsoft/dotnet:8.0-sdk AS build 
WORKDIR /src
COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:8.0-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS http://*:5000
ENTRY ["dotnet", "HelloCoreWorldDocker.dll"]



First, this tells Docker that we're starting from the Microsoft .NET Core sdk base image. Then we'll say WORKDIR/src to move into a virtual directory inside of the Docker image. We wanna copy all of our source files into the Docker image temporarily so we can build the application. We'll start with just the csproj file first. Copy anything .csproj into the Docker image, and then we're gonna RUN dotnet restore to pull down any packages that we need to build the application. After that restore step, we'll COPY the rest of the rest of the source files, and then RUN dotnet publish -c Release, and say that output should go into another virtual directory called /app. Splitting up the restore and publish steps in this way allows Docker to optimize how the packages get pulled down when we're doing the NuGet restore. Now that we've built the application, we'll say FROM microsoft/dotnet:8.0-aspnetcore-runtime AS runtime, since we don't need the sdk any longer. Switch into that /app directory, COPY things from the build step into /app, we need to set an environment variable, so we'll say ENV, the environment variable is called ASPNETCORE_URLS, this tells ASP.NET Core what ports and URLS it should bind to. We'll stick with the default configuration of binding to port 5000 by saying http://*:5000. You can also bind to https if you have an https certificate configured in your Docker container by saying, https://*5001, for example, we'll stick with http to keep it simple for now. Then we finally need to say ENTRYPOINT and give the command to start the application, which is "dotnet", "HelloCoreWorld.dll". Save the file. 

8. Switch to PowerShell. From the application directory, I need to say Docker build, give it a tag, we'll say -t hellocoreworlddocker just as a name for the image, and then a period to say that we wanna build this image from the current directory. 

PS C:\Users\USER> cd C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker
PS> docker build -t hellocoreworlddocker .

9. Once the image has been built, it can be run locally on this machine, or on any Docker host.
PS> docker images

***********************************************************
***********************************************************

## Test the Docker image locally
1. Now that we've built an image with Docker build, we can test it on our local machine. 
2. You can run that from visual studio, by choosing "Container (Dockerfile)" next to green "play" arrow.
3. Or
4. hellocoreworlddocker is the name of the image I created before. I can run this image using docker run. 



I'll use the -it flag to tell Docker to take all of the output from the container and pipe it to this console window. I'll also use the -p flag to map port 808 from inside of the container, which was exposed with the expose command in the Docker file, to port 8080 on this machine, on my local machine. And finally I'll specify the name of the image that I want to spin up. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker run -it -p 8080:8080 hellocoreworlddocker

When I hit enter, Docker will take this image, create a container from it, and then run the container and show me the output. And it works really fast, so in just that amount of time I have the container spun up, Kestrel started up, and it's listening for requests. If I switch over to a browser and browse to localhost:8080, I should start interacting with that container and getting a response back. As you can see, testing Docker images and containers is really straightforward. Next I'll explore running and monitoring this container as a background process.

Note: You should see 2 images in the docker desktop: one you created by visual studio and one in the command line

## Run and monitor a container
1. On a real server, you'll wanna run the container, or multiple containers as background processes. If we use Docker run with the -d flag, it'll start the container in the background. We still need -p to map the ports from inside the container to our local machine, and we'll specify the name of the image to start up. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker run -d -p 8080:8080 hellocoreworlddocker

2 We can use Docker ps to monitor the status of this running container. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker ps
CONTAINER ID   IMAGE                      COMMAND                  CREATED          STATUS          PORTS
                               NAMES
5ab544174817   hellocoreworlddocker       "dotnet HelloCoreWor…"   39 seconds ago   Up 38 seconds   0.0.0.0:8080->8080/tcp, 8081/tcp                   focused_golick
a41c4b7c7604   hellocoreworlddocker:dev   "tail -f /dev/null"      23 minutes ago   Up 23 minutes   0.0.0.0:32769->8080/tcp, 0.0.0.0:32768->8081/tcp   HelloCoreWorldDocker

3. The output of Docker ps is usually too wide for a single window, so I like to use the --format command to make it a little bit easier to read. I prefer using table .names .image .status, and. ports, this just customizes what output Docker ps will send to the screen. So that's a little bit more readable. This tells us that this image, HelloCoreWorldDocker was spun up into the container called focused_golick, it's been up for about 3 minutes, and it has internal port 8080 mapped to external port 8080. The dev one is created by visual studio!!!

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker ps --format 'table {{.Names}}\t{{.Image}}\t{{.Status}}\t{{.Ports}}'
NAMES                  IMAGE                      STATUS          PORTS
focused_golick         hellocoreworlddocker       Up 3 minutes    0.0.0.0:8080->8080/tcp, 8081/tcp
HelloCoreWorldDocker   hellocoreworlddocker:dev   Up 26 minutes   0.0.0.0:32769->8080/tcp, 0.0.0.0:32768->8081/tcp

4. We can use Docker ps to monitor the status of this container, or we can use Docker stop, followed by the generated container name, focused_golick in this case, to shut the container down. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker stop focused_golick
focused_golick

Remove the visual studio image
C:\Hooman\DeployingASPNETCORE>docker rmi --force fae80b320b3a

Next we'll explore using NGINX to expose our application to the internet.

## Docker Compose overview
We now have Kestrel running in a Docker container but it's not accepting traffic on port 80 (we are using 8080 or maybe 5000!). As I mentioned earlier, the best practice is to put a reverse proxy in front of Kestrel. We can use Docker to create another container running NGINX that will proxy requests to our Kestrel container. And you might be wondering, why would we use a Linux server like NGINX if I'm developing this on Windows? The reason is that under the hood Docker runs these containers on top of a small Linux Kernel, even on Windows. When I spin up a container for my Kestrel image, which is based on the dot net core base image, it's actually starting up a small Debian Linux environment. Docker includes a tool called Docker Compose that helps you create multi container applications. We'll use it to create a simple container that runs NGINX, pair that with our existing Kestrel container, and then configure NGINX to proxy requests to Kestrel.

## Kestrel and NGINX with Compose
**** this section is not working
1. First we need to create a Docker file for the new Nginx container we need. To keep things organized, I'll create a folder in my project folder called nginx. 
2. I'll use Notepad to create the new Docker file and save it in the nginx folder as Dockerfile with no extension. There's already a public base image for Nginx, so we can use FROM nginx to get started really quickly. 

FROM nginx

3. The only thing we need to do is customize the Nginx configuration, so we'll copy our own nginx.conf file into the image. We need to copy it to /etc/nginx/nginx.conf. And that takes care of the docker file. 

COPY nginx.cong /etc/nginx/nginx.conf

4. Now we need to create nginx.conf. Save this also in the nginx folder as nginx.conf with quotes to preserve the extension. This Nginx configuration will be very similar to the configuration we used earlier when we configured Nginx on a Linux machine. This time it includes a few more elements. We need an events group that specifies the maximum number of concurrent worker connections. This means that up to 1024 active connections can be handled by Nginx at once. And we need an http group which will contain a server group. We want, of course, to listen on port 80. We're gonna send those requests to a location that we'll define here. We'll proxy those requests to Kestrel port 5000. We'll come back to this in a moment. I also need to set some boilerplate things, the HTTP version to 1.1, set the upgrade header, set the connection header, set the host header, and set cache bypass to follow the upgrade setting. That takes care of nginx.conf. 

events { worker_connection 1024; }
http {
       server
              { 
                 listen 80;
		 location / {
			  proxy_pass http://kestrel:5000;
			  proxy_http_version 1.1;
			  proxy_set_header Connection 'keep-alive';
			  proxy_set_header HOST $host;
			  proxy_cache_bypass $http_upgrade;
	          }
	       }
	      	
}


5. Next we'll need a set of instructions for Docker Compose that tells Compose how we wanna structure our multi-container application. Compose looks for a YAML file called docker-compose.yml. I'll create that and save it in the root of my project. 

 6. This file specifies the containers that we wanna spin up when we run Docker Compose, and in our case we need two. We need one container for Nginx. We'll build this from the Docker file in the current directory slash nginx subdirectory, and we need to link this container to another container that we'll define in a moment called kestrel. This is where that host name comes from. We want to expose port 80 internally from the container and externally to our physical machine. And then for our other container, we'll build it from the Docker file in the root directory. So that's our Kestrel image. And we need to expose port 5000 internally. So in this case, it means that port 5000 will be exposed from our Kestrel container, but only inside of Docker. When we spin up both of these containers together, port 80 will be exposed on our local machine. Port 5000 will not be. It will only be exposed inside of the Docker environment. And that's necessary so that the Nginx container can proxy requests to the Kestrel container. 
 

 
 7. Switch over to PowerShell. From my project root directory, I can run docker-compose up, which will read that YAML file, build the images, and then spin up the containers that we need. At the end of the build process, we should have Nginx listening on port 80 proxying those requests over to Kestrel. We can test this in a browser. If I switch over to Chrome and navigate to localhost, notice there's no port there so I'm hitting port 80, and there's my ASP.NET Core application. As we've seen, docker-compose makes it possible to build complex, multi-container applications with Docker.

 PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker-compose up

 ## Save an image to a file
1. We've build a number of images using Docker build, but these are all stored locally on our machine. What if we need to transfer this image to another machine to push it to a production server, for example. That's where the Docker save command comes in. 

2. If we run Docker images, we can see all the images that I've stored locally on this machine. To export or save one of these, we can run Docker save -o to specify an output filename, in this case I'll call it hellocore.tar, and specify the image I wanna save out. This command will save a tar ball of the image as a single file on the file system that you can then easily transport to another computer. This command can take a few minutes.

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker images
REPOSITORY             TAG       IMAGE ID       CREATED        SIZE
hellocoreworlddocker   latest    8f667ccea273   22 hours ago   225MB
PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker save -o hellocore.tar hellocoreworlddocker

 3. When it's done we can see the file on the file system. The opposite of the save command is the load command, I can use Docker load -i and the filename to load that image back into Docker and get it ready for deployment. You can also use Docker hub to share images between machines. I'll show you how to do that next.

 PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker load -i hellocore.tar
Loaded image: hellocoreworlddocker:latest

## Publish an image to Docker Hub
1. Docker Hub is an online repository for sharing Docker images. You can push images that you've built up to Docker Hub, and then pull them down at a later time, or on a different machine. Storing public images on Docker Hub is free, so it's a useful way to share and maintain images that you use. 
2. To use Docker Hub, sign up for an account at hub.docker.com, then in the terminal, run Docker login and enter your credentials. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker login
Authenticating with existing credentials...
Login Succeeded

3. I've already logged in on this machine. Images you push to Docker Hub must follow a naming scheme, where the name of your Docker Hub repository comes first, followed by a slash, and then the name of the image. Your personal repository name is your username, so I need to rebuild my image to include my username. I'll rebuild my project image by running Docker build -t, and for the name, I'll use my username, hsalamat/ and then the image name, hellocoreworldcore, and a period to build it from the current directory. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker build -t hsalamat/hellocoreworlddocker .


4. Once the image is built, I can push it up to Docker Hub by using Docker push, and then the image name. 

PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker push hsalamat/hellocoreworlddocker

5. When the process is done, my image will be visible online at hub.docker.com, and to pull that image back down. 


6. I can just run Docker pull and the image name. 
PS C:\Hooman\DeployingASPNETCORE\HelloCoreWorldDocker> docker pull hsalamat/hellocoreworlddocker

7. Since ASP.NET Core runs natively on the Linux environments Docker uses, Docker's a great choice for deploying ASP.NET Core applications.

## Next steps
If you wanna dive deeper into Docker, I'd recommend the course Learning Docker by Arthur Ulfeldt, and also the course Docker for .NET Developers by Lee Brandt. You can always find up to date information about ASP.NET Core on the official documentation site at docs.asp.net. There are a number of additional tools that you could use to manage and automate your workflow, such as Octopus Deploy for automated deployment, Travis and CI for building code and running tests, and Jenkins for continuous delivery. These tools build on top of the skills that you've already learned in this course. I recommend doing some research to see if these tools could help your deployment process even further. Thanks for watching my course and happy deploying.