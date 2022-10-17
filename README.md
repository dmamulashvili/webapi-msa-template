# webapi-msa
ASP.NET Core Web API Microservice Template using Clean Architecture, CQRS, Structure by Feature, Specifications, PostgreSQL, MassTranist with AmazonSQS & OutBox, JWT Auth 
> **Note**  
> Reference used: <https://github.com/dotnet-architecture/eShopOnContainers> 

## Install
Clone repo
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
## Configure:
Update PostgreSQL writer(Master) & reader(Slave) connection strings in case you're not using local one.
```json
  "ConnectionStrings": {
    "MasterDbContext": "Server=localhost;Port=5432;Database=MyCompany.MyProject.MyOrderingDb;User Id=postgres;password=postgres",
    "SlaveDbContext": "Server=localhost;Port=5432;Database=MyCompany.MyProject.MyOrderingDb;User Id=postgres;password=postgres;"
  },
```
Create aws user with Programmatic access & read/write permissions to SNS/SQS.
>**Warning**  
>The following characters are accepted in QueueName: alphanumeric characters, hyphens (-), and underscores (_).
```json
"AmazonSqsConfiguration": {
    "AccessKey": "",
    "SecretKey": "",
    "RegionEndpointSystemName": "eu-central-1",
    "Scope": "MyProject",
    "QueueName" : "MyCompany_MyProject_MyOrdering"
  },
```
Configure JWT
>**Warning**  
>ValidateAudience is disabled by default in `Program.cs`, you can leave it empty.
```json
"JWT": {
    "ValidAudience": "",
    "ValidIssuer": "",
    "Secret": ""
  }
```
