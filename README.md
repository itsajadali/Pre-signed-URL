## Project Overview
This project consists of two main services: **AppService** and **StorageService**, designed to work together using the provided configurations and Docker.

## Quick Start (Recommended)
If you use the dotnet-aspire branch, the setup is fully automated.

Action: Just download the branch and run it.
Benefit: No need to manually configure environment variables or Docker settings; Aspire handles the orchestration for you.

You only need to add appsettings for both **AppService** and **StorageService** 


## Manual Installation (Master Branch): 

## Getting Started

### 1. Environment Configuration
Ensure the `.env` file is located in the root directory. It should contain the following:
- `JWT_SECRET=`
- `SIGNING_SECRET=`
- `ASPNETCORE_ENVIRONMENT=Development`
- `CERT_PASSWORD=`

### 2. How to Run
Use Docker Compose to build and start both services:

bash
docker-compose build
docker-compose up

Service Configurations
<details> <summary>View <code>AppService/appsettings.json</code></summary>
{
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "",
    "Issuer": "",
    "Audience": ""
  },
  "Signing": { "Secret": "" },
  "StorageService": { "BaseUrl": "" }}
</details>

<details> <summary>View <code>StorageService/appsettings.json</code></summary>
  {
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*",
  "Signing": { "Secret": "" },
  "Storage": {
    "MaxFileSizeMB": ,
    "UploadPath": "",
    "ExpiresInMinutes": 
  },
  "StorageService": { "PublicBaseUrl": "" }
}
 
  }
}

</details>
