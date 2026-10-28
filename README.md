# Azure Fundamentals — Learning Samples

Educational .NET 10 web and Azure Functions samples for storage, triggers, image processing, email and caching.

## Captured preview

![Azure Fundamentals — observed local output](docs/screenshots/portfolio-poster.png)

The existing ImageThumbnail.Resize runs locally: PNG 900×600 to 300×200; TIFF and input over 8 MiB are rejected. No Functions host or cloud resource starts.

[Reproduce this capture and view the static image](docs/PORTFOLIO-PREVIEW.md)

## Sample map

| Project | Focus | Target |
|---|---|---|
| [AzureBlobProject](AzureBlobProject) | Blob/container web operations | .NET 10 |
| [AzureSpookyLogic](AzureSpookyLogic) | Storage-backed web workflow | .NET 10 |
| [RedisCache](RedisCache) | MVC data access and Redis caching | .NET 10 |
| [AzureFunctionTangyWeb](AzureFunctionTangyWeb) | Function/storage web client | .NET 10 |
| [AzureTangyFunc](AzureTangyFunc) | HTTP, queue, blob and timer exercises | .NET 10, Functions v4 isolated worker |

These are separate learning exercises. Configure only the resources required by the sample you run. See [runtime and migration notes](docs/RUNTIME.md) for the isolated-worker migration and dependency policy.

## Build and isolated checks

Install the latest supported .NET 10 SDK/runtime patch. NuGet feeds and advisory data must be reachable.

```powershell
git clone https://github.com/s-hesam-s/AzureFundamentals.git
cd AzureFundamentals
dotnet restore AzureFundamentals.sln
dotnet restore AzureTangyFunc/AzureTangyFunc.csproj
dotnet build AzureFundamentals.sln --configuration Release --no-restore
dotnet test tests/Image.Tests/Image.Tests.csproj --configuration Release
pwsh -File scripts/Test-PublicConfiguration.ps1
```

The separate Functions restore is required by Azure.Functions.Sdk to generate its extension project. Image tests process in-memory PNG/JPEG data with SkiaSharp and inspect generated metadata for all ten functions and their output bindings. They do not start the host, execute timers, connect to SQL/storage, or send email.

## Configuration

Read [SECURITY-SETUP.md](SECURITY-SETUP.md). Tracked credential fields are empty. Supply your own development values through the documented configuration keys; do not recover old values from history.

- Storage web samples read `BlobConnection`; AzureFunctionTangyWeb reads `AzureWebJobsStorage`.
- RedisCache reads `ConnectionStrings:DefaultConnection` and `ConnectionStrings:AzureRedisConnection`.
- Functions read environment keys including `AzureWebJobsStorage`, `AzureSqlDatabase` and `CustomSendGridKeyAppSettingName`. They are not an ASP.NET Core `Values__...` section.
- [local.settings.json](AzureTangyFunc/local.settings.json) selects `dotnet-isolated`. Supply only isolated development resources before starting Functions Core Tools v4.

HTTP functions retain their function names and GroceryList routes. Queue bindings use lowercase `salesrequestinbound`. Thumbnail processing accepts PNG/JPEG only, with an 8 MiB encoded-input limit and a 16 million pixel limit. TIFF is rejected. See [ImageThumbnail.cs](AzureTangyFunc/ImageThumbnail.cs).

## Run a configured sample

Web sample HTTPS profiles:

| Project | Address |
|---|---|
| AzureBlobProject | `https://localhost:7033` |
| AzureSpookyLogic | `https://localhost:7294` |
| RedisCache | `https://localhost:7186` |
| AzureFunctionTangyWeb | `https://localhost:7115` |

After reviewing and preparing the development resources:

```powershell
dotnet run --project AzureBlobProject/AzureBlobProject.csproj --launch-profile https
```

For Functions, use current compatible Azure Functions Core Tools v4:

```powershell
cd AzureTangyFunc
func start
```

Starting the Functions host activates queue/blob/timer listeners, and invoking functions can write to SQL/storage and send email. This is not a dependency-free preview. A build does not provision resources or validate deployment. Actual Azure startup, Linux native execution and cloud integrations have not been verified here. Review current hosting-plan support before deploying .NET 10 to Azure.

## Continuous integration

The [CI workflow](.github/workflows/ci.yml) uses .NET 10 to restore and build the solution in Release mode, run image/metadata regressions and check public configuration. NuGet audit includes transitive dependencies and fails on known advisories or unavailable audit data. CI does not start application hosts or connect to development/cloud resources.

## Project scope and reuse

This is an educational repository. It does not claim a deployed commercial service or measured production results. Third-party libraries and assets retain their own terms; preserve SkiaSharp and bundled native-library notices when distributing it. No project-wide license is currently declared.

[Local verification snapshot](docs/VERIFICATION.md) records the build, isolated-test and dependency-audit boundaries.

## Maintenance checks

The existing CI also runs weekly on Monday and checks tracked Markdown file destinations with `scripts/check_doc_links.py`. Its negative tests cover moved documents, missing picture variants, paths outside the repository and untracked local drafts. External URLs and heading fragments are intentionally outside this offline file check. Scheduled runs do not deploy or start original application hosts.
