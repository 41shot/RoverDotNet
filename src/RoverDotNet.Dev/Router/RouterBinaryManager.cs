using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using RoverDotNet.Dev.Exceptions;

namespace RoverDotNet.Dev.Router;

/// <summary>
/// Manages locating, downloading, and installing Apollo Router binaries.
/// Mirrors the router binary management in rover's <c>src/command/dev/router/install.rs</c>.
/// </summary>
public sealed class RouterBinaryManager
{
    private const string DownloadUrlTemplate = "https://github.com/apollographql/router/releases/download/v{0}/router-v{0}-{1}.tar.gz";
    private static readonly Regex VersionPattern = new(@"router(?:-v)?(\d+\.\d+\.\d+)(?:\.exe)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Raised when a download progress update is available.
    /// </summary>
    public event EventHandler<RouterDownloadProgress>? DownloadProgressChanged;

    /// <summary>
    /// Locates the Apollo Router binary, downloading it if necessary.
    /// Searches in order:
    /// 1. {user-home}/.rover/bin/router[-v{version}][.exe]
    /// 2. Current working directory
    /// 3. PATH environment variable
    /// </summary>
    /// <param name="preferredVersion">Optional preferred version to download if not found.</param>
    /// <param name="skipDownload">
    /// When <see langword="true"/>, the method will only search for an existing installation
    /// and will not attempt to download a new binary. Throws <see cref="RouterProcessException"/>
    /// if no existing binary is found.
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>Path to the router binary.</returns>
    /// <exception cref="RouterProcessException">Thrown if the router cannot be located or downloaded.</exception>
    public async Task<string> LocateOrDownloadAsync(
        string? preferredVersion = null,
        bool skipDownload = false,
        CancellationToken cancellationToken = default)
    {
        // Try to locate existing binary
        var existingPath = TryLocateExisting();
        if (existingPath is not null)
            return existingPath;

        if (skipDownload)
        {
            throw new RouterProcessException(
                "Apollo Router binary not found on this machine and --skip-update was specified. " +
                "Install the Apollo Router first, or omit --skip-update to allow automatic download.");
        }

        // No existing binary found, download it
        var version = preferredVersion ?? await GetLatestRouterVersionAsync(cancellationToken);
        return await DownloadRouterAsync(version, cancellationToken);
    }

    /// <summary>
    /// Attempts to locate an existing router binary in standard locations.
    /// </summary>
    /// <returns>Path to the router binary, or null if not found.</returns>
    private static string? TryLocateExisting()
    {
        // 1. Check {user-home}/.rover/bin
        var roverBinDir = GetRoverBinDirectory();
        if (Directory.Exists(roverBinDir))
        {
            var roverBinFiles = Directory
                .GetFiles(roverBinDir, "router*")
                .Where(f => IsRouterExecutable(f))
                .OrderByDescending(f => ExtractVersion(f))
                .ToList();

            if (roverBinFiles.Count > 0)
                return roverBinFiles[0]; // Return the highest version
        }

        // 2. Check current working directory
        var cwd = Directory.GetCurrentDirectory();
        var cwdFiles = Directory
            .GetFiles(cwd, "router*")
            .Where(f => IsRouterExecutable(f))
            .OrderByDescending(f => ExtractVersion(f))
            .ToList();

        if (cwdFiles.Count > 0)
            return cwdFiles[0];

        // 3. Check PATH (not an exhaustive search - only where path contains "rover")
        var pathDirs = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? [];
        foreach (var dir in pathDirs.Where(d => d.Contains("rover", StringComparison.OrdinalIgnoreCase)))
        {
            if (!Directory.Exists(dir))
                continue;

            try
            {
                var pathFiles = Directory
                    .GetFiles(dir, "router*")
                    .Where(f => IsRouterExecutable(f))
                    .OrderByDescending(f => ExtractVersion(f))
                    .ToList();

                if (pathFiles.Count > 0)
                    return pathFiles[0];
            }
            catch
            {
                // Ignore access denied errors
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if a file is a router executable.
    /// </summary>
    private static bool IsRouterExecutable(string path)
    {
        var fileName = Path.GetFileName(path);
        var exeSuffix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";

        // Match: router, router.exe, router-v1.2.3, router-v1.2.3.exe, routerv1.2.3.exe, router1.2.3.exe
        return VersionPattern.IsMatch(fileName) || fileName.Equals($"router{exeSuffix}", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extracts the version from a router binary filename.
    /// Returns a comparable version (or 0.0.0 if no version found).
    /// </summary>
    private static Version ExtractVersion(string path)
    {
        var fileName = Path.GetFileName(path);
        var match = VersionPattern.Match(fileName);

        if (match.Success && Version.TryParse(match.Groups[1].Value, out var version))
            return version;

        return new Version(0, 0, 0); // No version = treat as oldest
    }

    /// <summary>
    /// Gets the standard rover bin directory: {user-home}/.rover/bin
    /// </summary>
    private static string GetRoverBinDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, ".rover", "bin");
    }

    /// <summary>
    /// Downloads the latest Apollo Router version information.
    /// </summary>
    private static async Task<string> GetLatestRouterVersionAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "RoverDotNet");

        try
        {
            // Query GitHub releases API for the latest version
            var response = await httpClient.GetStringAsync(
                "https://api.github.com/repos/apollographql/router/releases/latest",
                cancellationToken);

            // Parse JSON to extract tag_name (e.g., "v1.57.1")
            var tagMatch = Regex.Match(response, @"""tag_name""\s*:\s*""v?(\d+\.\d+\.\d+)""");
            if (tagMatch.Success)
                return tagMatch.Groups[1].Value;

            throw new RouterProcessException("Failed to parse latest router version from GitHub API.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RouterProcessException(
                $"Failed to fetch latest router version: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Downloads and installs the Apollo Router binary.
    /// </summary>
    private async Task<string> DownloadRouterAsync(string version, CancellationToken cancellationToken)
    {
        var platform = GetPlatformIdentifier();
        var downloadUrl = string.Format(DownloadUrlTemplate, version, platform);
        var binDir = GetRoverBinDirectory();
        var exeSuffix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";
        var targetPath = Path.Combine(binDir, $"router-v{version}{exeSuffix}");

        // Create bin directory if it doesn't exist
        Directory.CreateDirectory(binDir);

        RaiseProgress(0, $"Downloading Apollo Router v{version} from GitHub...");

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        httpClient.DefaultRequestHeaders.Add("User-Agent", "RoverDotNet");

        try
        {
            // Download the tar.gz file
            using var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            var downloadedBytes = 0L;

            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var memoryStream = new MemoryStream();

            var buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                downloadedBytes += bytesRead;

                if (totalBytes > 0)
                {
                    var progress = (int)((downloadedBytes * 100) / totalBytes);
                    RaiseProgress(progress, $"Downloading: {progress}% ({downloadedBytes / 1024 / 1024} MB / {totalBytes / 1024 / 1024} MB)");
                }
            }

            RaiseProgress(100, "Download complete. Extracting...");

            // Extract the router binary from the tar.gz
            memoryStream.Position = 0;
            await ExtractRouterBinaryAsync(memoryStream, targetPath, exeSuffix, cancellationToken);

            // Make executable on Unix-like systems
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var chmod = Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{targetPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (chmod is not null)
                    await chmod.WaitForExitAsync(cancellationToken);
            }

            RaiseProgress(100, $"Apollo Router v{version} installed successfully.");

            return targetPath;
        }
        catch (HttpRequestException ex)
        {
            throw new RouterProcessException(
                $"Failed to download router v{version} from {downloadUrl}. " +
                $"Ensure the version exists and your network connection is active. Error: {ex.Message}",
                ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RouterProcessException(
                $"Failed to download and install router: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Extracts the router binary from a gzipped tar archive.
    /// </summary>
    private static async Task ExtractRouterBinaryAsync(
        Stream gzipStream,
        string targetPath,
        string exeSuffix,
        CancellationToken cancellationToken)
    {
        await using var gzipExtractStream = new GZipStream(gzipStream, CompressionMode.Decompress);
        using var memoryBuffer = new MemoryStream();
        await gzipExtractStream.CopyToAsync(memoryBuffer, cancellationToken);
        memoryBuffer.Position = 0;

        // Parse the tar archive manually (simple implementation)
        var routerBinary = ExtractRouterFromTar(memoryBuffer, exeSuffix);

        if (routerBinary is null)
        {
            throw new RouterProcessException(
                "Failed to locate router binary in the downloaded archive. The archive may be corrupt.");
        }

        await File.WriteAllBytesAsync(targetPath, routerBinary, cancellationToken);
    }

    /// <summary>
    /// Extracts the router binary from a tar archive.
    /// Simple tar parser looking for "dist/router" or "router" entries.
    /// </summary>
    private static byte[]? ExtractRouterFromTar(Stream tarStream, string exeSuffix)
    {
        var buffer = new byte[512]; // Tar blocks are 512 bytes

        while (tarStream.Read(buffer, 0, 512) == 512)
        {
            // Check if this is a valid tar header (not all zeros)
            if (buffer.All(b => b == 0))
                break;

            // Parse filename (first 100 bytes, null-terminated)
            var nameBytes = buffer.Take(100).TakeWhile(b => b != 0).ToArray();
            var name = System.Text.Encoding.ASCII.GetString(nameBytes);

            // Parse file size (bytes 124-135, octal)
            var sizeBytes = buffer.Skip(124).Take(11).TakeWhile(b => b != 0 && b != 32).ToArray();
            var sizeStr = System.Text.Encoding.ASCII.GetString(sizeBytes).Trim();
            var size = Convert.ToInt64(sizeStr, 8);

            // Check if this is the router binary
            if (name.EndsWith($"router{exeSuffix}", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith($"dist/router{exeSuffix}", StringComparison.OrdinalIgnoreCase))
            {
                var fileData = new byte[size];
                var totalBytesRead = 0;
                while (totalBytesRead < size)
                {
                    var bytesRead = tarStream.Read(fileData, totalBytesRead, (int)(size - totalBytesRead));
                    if (bytesRead == 0)
                        throw new InvalidOperationException("Unexpected end of tar stream while reading router binary.");
                    totalBytesRead += bytesRead;
                }
                return fileData;
            }

            // Skip to next block (files are padded to 512-byte boundaries)
            var padding = (512 - (size % 512)) % 512;
            tarStream.Position += size + padding;
        }

        return null;
    }

    /// <summary>
    /// Gets the platform identifier for downloading the correct router binary.
    /// </summary>
    private static string GetPlatformIdentifier()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x86_64-pc-windows-msvc",
                Architecture.Arm64 => "aarch64-pc-windows-msvc",
                _ => throw new PlatformNotSupportedException($"Unsupported Windows architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x86_64-unknown-linux-gnu",
                Architecture.Arm64 => "aarch64-unknown-linux-gnu",
                _ => throw new PlatformNotSupportedException($"Unsupported Linux architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x86_64-apple-darwin",
                Architecture.Arm64 => "aarch64-apple-darwin",
                _ => throw new PlatformNotSupportedException($"Unsupported macOS architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        throw new PlatformNotSupportedException($"Unsupported operating system: {RuntimeInformation.OSDescription}");
    }

    private void RaiseProgress(int percentage, string message)
    {
        DownloadProgressChanged?.Invoke(this, new RouterDownloadProgress(percentage, message));
    }
}

/// <summary>
/// Contains progress information for router download operations.
/// </summary>
public sealed record RouterDownloadProgress(int Percentage, string Message);
