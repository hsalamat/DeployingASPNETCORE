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
C:\Hooman\DeployingASPNETCORE> dotnet new mvc -o HelloCoreWorldCLI

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
2. csproj include dependecies, framwork
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


