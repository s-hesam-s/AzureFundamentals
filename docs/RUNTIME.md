# Runtime and dependency maintenance

All application and test projects target **.NET 10 LTS**. Install the latest supported .NET 10 SDK and runtime patch. `global.json` permits stable .NET 10 feature-band updates; CI installs `10.0.x`. A successful source build does not update a developer machine's installed runtime.

## Dependency policy

`Directory.Build.props` audits direct and transitive NuGet dependencies. Restore fails on advisory severities Low through Critical, and when vulnerability data cannot be obtained. Feed access is required; an unavailable audit is not a clean result. The check uses the advisories available at restore time and does not certify vendored JavaScript, operating-system packages or a deployed host.

SQL Server projects pin `System.Security.Cryptography.Pkcs` 10.0.14 to override the vulnerable version otherwise selected by SqlClient. Projects using older Stripe packages explicitly resolve Newtonsoft.Json 13.0.4. Scaffolding projects pin patched NuGet.Protocol/NuGet.Packaging 6.12.5. These are focused transitive fixes, rather than provider API rewrites.

```powershell
dotnet restore
dotnet list package --vulnerable --include-transitive --no-restore
```

For repositories with multiple entry points, pass the solution or project path shown in the README. Build and isolated tests do not run database migrations, seeds, payments, email or cloud consumers. Review the configured development targets before starting the original hosts.

## References

- [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
- [NuGet audit](https://learn.microsoft.com/en-us/nuget/concepts/auditing-packages)

## Azure Functions migration

`AzureTangyFunc` uses `Azure.Functions.Sdk` 1.0.0, Worker 2.52.0 and .NET 10 in the isolated worker model. `Program.cs` configures ASP.NET Core HTTP integration and the existing SQL context; it does not automatically migrate or seed the database. `local.settings.json` selects `dotnet-isolated` and leaves resource credentials empty.

HTTP/queue/blob/timer function names and routes are retained. Queue output now uses a typed multi-output result; blob output returns encoded bytes; SendGrid output returns JSON. The queue name is normalized to `salesrequestinbound`, as required by Azure Queue Storage. Update local resources consistently before invoking triggers.

ImageSharp is replaced with SkiaSharp 4.154.0 and matching Linux native assets. The thumbnail helper accepts PNG/JPEG only, limits encoded input to 8 MiB and decoded dimensions to 16 million pixels, rejects incomplete decoding, and produces 300 x 200 output in the original PNG/JPEG format. TIFF and other formats are rejected. SkiaSharp has its own MIT and bundled native-library notices; preserve those when distributing it.

`tests/Image.Tests` exercises native image processing and generated function metadata without starting the Functions host. Ten functions and their queue/HTTP/blob/SendGrid output bindings are checked. These tests do not verify Azure deployment, storage triggers, SQL writes or email delivery. Linux native execution remains a separate environment check. For .NET 10 on Linux, review the current supported hosting plans before deploying.
