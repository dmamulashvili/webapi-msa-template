# webapi-msa

ASP.NET Core Web API Microservice Template using Clean Architecture, CQRS, Structure by Feature, Specifications,
PostgreSQL, MassTranist with AmazonSQS & OutBox, JWT Auth
> **Note**  
> Reference used: <https://github.com/dotnet-architecture/eShopOnContainers>

## Install

Clone repository

```console
git clone https://github.com/dmamulashvili/webapi-msa-template.git
```

Install template

```console
cd webapi-msa-template
dotnet new --install .
```

Create Solution

```console
cd /<DIRECTORY_TO_CREATE_SOLUTION_AT>
dotnet new webapi-msa -o "MyCompany.MyProject.MyOrdering"
```

## Configure launchSettings.json

Configure PostgreSQL writer(Master) & reader(Slave) connection strings.

```json
"environmentVariables": {
  "ConnectionString_MasterDbContext": "Server=localhost;Port=5432;Database=MyCompany.MyProject.MyOrderingDb;User Id=postgres;password=postgres",
  "ConnectionString_SlaveDbContext": "Server=localhost;Port=5432;Database=MyCompany.MyProject.MyOrderingDb;User Id=postgres;password=postgres;"
},
```

Create aws user with Programmatic access & read/write permissions to SNS/SQS & configure creds.

```json
"environmentVariables": {
  "AmazonSQS_AccessKey": "",
  "AmazonSQS_SecretKey": "",
  "AmazonSQS_RegionEndpointSystemName": ""
},
```

Configure JWT
> **Warning**  
> ValidateAudience is disabled by default in `Program.cs`, you can leave it empty.

```json
"environmentVariables": {
  "JWT_ValidAudience": "",
  "JWT_ValidIssuer": "",
  "JWT_Secret": ""
}
```

## Configure appsettings.json
> **Warning**
> The following characters are accepted in ShortName: alphanumeric characters, hyphens (-), and underscores (_).

```json
"Project": {
  "ShortName": "Template_API"
},
```

> **Note**
> ASP.NET Core Web API Audit Microservice template: <https://github.com/dmamulashvili/webapi-msa-audit-template.git>
