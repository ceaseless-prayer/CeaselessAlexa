# Ceaseless Alexa Skill

The Alexa skill is comprised of two components: (1) the server code and (2) the Alexa skill configuration.

## Server Code

The server code for the skill is written in C# and runs on .NET Core 1.0. The server code is then deployed to AWS Lambda and ran as a serverless function.

[![Build status](https://ci.appveyor.com/api/projects/status/ueprnsv2ade3l0b7/branch/master?svg=true)](https://ci.appveyor.com/project/adriangodong/ceaselessalexa/branch/master)

### Developer Workflow for Server Code

0. Download and install [.NET Core SDK](https://www.microsoft.com/net/download/core)
1. Build and test using the following command:

       dotnet restore && dotnet build && dotnet test Ceaseless.Tests/Ceaseless.Tests.csproj

2. To publish code to AWS Lambda, run the following command:

       cd Ceaseless
       dotnet lambda deploy-function

### Server Code Components

The server code is further split into the following components:

1. `Function` contains the code to handle requests coming from Alexa / AWS Lambda. This is the "controller" in an MVC application.

2. `Engine` contains the code to interface with external systems (S3, REST API, etc.). This is the "repository".

3. Test project is under `Ceaseless.Tests`. To enable continuous testing while you code, run the following command:

       cd Ceaseless.Tests
       dotnet watch test

## Alexa Skill Configuration

The skill configuration is managed through [Amazon Developer Console](https://developer.amazon.com/edw/home.html#/skills). Configure the skill endpoint to hit the Lambda function we deployed from the previous section.

    // TODO: source control the Interaction Model JSON file.