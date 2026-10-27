# Azure Fundamentals — Learning Samples

A collection of educational .NET web and Azure Functions samples for storage, triggers, image processing, email and caching.

## Repository map

| Project | Focus | Target |
|---|---|---|
| [AzureBlobProject](AzureBlobProject) | Blob/container web operations | .NET 8 |
| [AzureSpookyLogic](AzureSpookyLogic) | Storage-backed web workflow | .NET 8 |
| [RedisCache](RedisCache) | MVC data access and Redis caching | .NET 8 |
| [AzureFunctionTangyWeb](AzureFunctionTangyWeb) | Web client for the function/storage exercises | .NET 8 |
| [AzureTangyFunc](AzureTangyFunc) | HTTP, queue/blob, image and email trigger examples | .NET 6, Functions v4 in-process |

These projects are separate exercises. Start with one sample and configure only the resources it uses.

## Requirements and configuration

The web projects target .NET 8. The Functions project is a legacy .NET 6 in-process example with Microsoft.NET.Sdk.Functions 4.1.1. .NET 6 is out of support; use a compatible local learning environment or plan a separately tested upgrade before treating it as an active deployment target. Current Azure host compatibility is not established by this repository.

Read [SECURITY-SETUP.md](SECURITY-SETUP.md) before configuring credentials. Tracked secret values are intentionally empty. Use environment variables for your own development resources; do not restore old values from Git history.

Storage samples read `BlobConnection`. RedisCache reads `ConnectionStrings:DefaultConnection` and `ConnectionStrings:AzureRedisConnection` (environment variable `ConnectionStrings__AzureRedisConnection`). AzureFunctionTangyWeb reads `AzureWebJobsStorage`. Set your own development values using the configuration names documented in the source; the empty tracked values do not provision resources.

For AzureTangyFunc, inspect [local.settings.json](AzureTangyFunc/local.settings.json), [host.json](AzureTangyFunc/host.json) and [Startup.cs](AzureTangyFunc/Startup.cs). Functions `Values` settings are exposed as environment keys such as `AzureWebJobsStorage` and `AzureSqlDatabase`; they are not an ASP.NET Core `Values__...` section. Review SQL, storage and email targets before invoking a function: triggers can write to those resources.

## Build

```powershell
git clone https://github.com/s-hesam-s/AzureFundamentals.git
cd AzureFundamentals
```

Build the selected .NET 8 web sample first:

```powershell
dotnet restore AzureBlobProject/AzureBlobProject.csproj
dotnet build AzureBlobProject/AzureBlobProject.csproj --no-restore
```

[AzureFundamentals.sln](AzureFundamentals.sln) contains the full collection, including the legacy Functions target. A solution-wide build therefore has additional runtime/tooling requirements.

## Run a web sample

After supplying your own development storage settings:

```powershell
dotnet run --project AzureBlobProject/AzureBlobProject.csproj --launch-profile https
```

The checked-in HTTPS addresses are:

- AzureBlobProject: `https://localhost:7033`.
- AzureSpookyLogic: `https://localhost:7294`.
- RedisCache: `https://localhost:7186`.
- AzureFunctionTangyWeb: `https://localhost:7115`.

Use the corresponding project path and its `https` launch profile. Data-oriented samples need their own prepared development SQL schema.

For the legacy Functions sample, install compatible Azure Functions Core Tools v4 and the required .NET runtime, configure the local resources and then run from that project directory:

```powershell
cd AzureTangyFunc
func start
```

Use the function URLs printed by the host. Do not invoke write/trigger endpoints against a shared or production resource.

## Verification

```powershell
pwsh -File scripts/Test-PublicConfiguration.ps1
```

There is no automated behavior-test suite in this repository. A successful configuration check only checks tracked credential fields; it does not validate resource provisioning, function compatibility or connectivity. Full startup and cloud workflows require separate testing with your own development resources.

## Project scope and reuse

This is an educational repository. The implementation notes describe the code in this repository; they do not claim a deployed commercial service or measured production results. Third-party libraries and assets retain their own terms. No project-wide license is currently declared.

## Continuous integration

GitHub Actions restores and builds all samples in `AzureFundamentals.sln` on Windows. The runner installs .NET 8 and .NET 6 SDKs to compile the existing targets, including the legacy in-process Functions example; this does not upgrade its target or establish current Azure deployment support. It also runs `scripts/Test-PublicConfiguration.ps1` without printing configured values. No automated application test suite exists in this repository, so the workflow does not claim application-test coverage. The application hosts, database migrations/seeds, cloud integrations and payments are not started by CI.

The [CI workflow](.github/workflows/ci.yml) runs for pull requests and pushes to `master`, and can be started manually. It uses read-only repository permissions and does not deploy the project.
