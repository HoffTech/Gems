// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Gems.IO.LibreOffice.Exceptions;
using Gems.IO.LibreOffice.Options;
using Gems.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.IO.LibreOffice
{
    public class LibreOffice : ILibreOffice
    {
        private readonly ILogger<LibreOffice> logger;
        private readonly IOptions<LibreOfficeOptions> options;
        private readonly ConcurrentDictionary<string, string> documentsInProgress;

        public LibreOffice(ILogger<LibreOffice> logger, IOptions<LibreOfficeOptions> options)
        {
            this.logger = logger;
            this.options = options;

            this.documentsInProgress = new ConcurrentDictionary<string, string>();

            if (string.IsNullOrEmpty(options?.Value?.LibreOfficeExecutablePath) ||
                string.IsNullOrEmpty(options.Value?.TempUserPathForArgs) ||
                string.IsNullOrEmpty(options.Value?.TempUserDirectPathForDelete))
            {
                throw new ArgumentNullException(nameof(options), "Empty options not allowed");
            }
        }

        public Task ConvertToPdfAsync(string docxPath, string pdfPath, CancellationToken cancellationToken)
        {
            if (this.documentsInProgress.TryAdd(docxPath, pdfPath))
            {
                return AsyncAwaiter.AwaitAsync(
                    "docx-to-pdf",
                    () => this.ConvertToPdfInternalAsync(docxPath, pdfPath, cancellationToken),
                    this.options.Value.MaxConcurrentLibreOfficeInstances);
            }

            this.logger.LogWarning("Document \"{DocumentSourcePath}\" is already in progress, skipping.", docxPath);
            return Task.CompletedTask;
        }

        private static void ValidateInputFiles(string docx, string pdf)
        {
            if (string.IsNullOrEmpty(docx))
            {
                throw new ArgumentNullException(docx, "Input docx path can't be null or empty string");
            }

            if (string.IsNullOrEmpty(pdf))
            {
                throw new ArgumentNullException(pdf, "Oputput pdf file can't be null or empty string");
            }

            if (!Path.GetExtension(docx).EndsWith(".docx"))
            {
                throw new ArgumentException($"Input file extension doesn't match docx. Input File: '{docx}'", docx);
            }

            if (!Path.GetExtension(pdf).EndsWith(".pdf"))
            {
                throw new ArgumentException($"Output file extension doesn't match pdf. Output File: '{pdf}'", pdf);
            }
        }

        private async Task ConvertToPdfInternalAsync(string docxPath, string pdfPath, CancellationToken cancellationToken)
        {
            ValidateInputFiles(docxPath, pdfPath);

            var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolder);

            var userId = DateTime.UtcNow.Ticks;

            try
            {
                var arguments = this.GetCommandArguments(docxPath, userId, tempFolder);
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo(this.options.Value.LibreOfficeExecutablePath)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    RedirectStandardError = true
                };

                foreach (var arg in arguments)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }

                var errorOutput = new StringBuilder();
                process.ErrorDataReceived += (_, e) =>
                {
                    if (e.Data != null)
                    {
                        errorOutput.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                this.LogLibreOfficeOutput(errorOutput, process.ExitCode);

                if (process.ExitCode != 0)
                {
                    throw new LibreOfficeException(process.ExitCode);
                }
                else
                {
                    var pdfFile = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(docxPath) + ".pdf");
                    if (File.Exists(pdfPath))
                    {
                        File.Delete(pdfPath);
                    }

                    if (File.Exists(pdfFile))
                    {
                        File.Move(pdfFile, pdfPath ?? throw new ArgumentNullException(nameof(pdfPath)));
                    }
                }
            }
            finally
            {
                if (!this.documentsInProgress.TryRemove(docxPath, out _))
                {
                    this.logger.LogError(
                        "Can't delete in progress document key from dictionary for document \"{DocumentSourcePath}\"",
                        docxPath);
                }

                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }

                var directory = $"{this.options.Value.TempUserDirectPathForDelete}{userId}";
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        private void LogLibreOfficeOutput(StringBuilder errorOutput, int exitCode)
        {
            if (errorOutput is null || errorOutput.Length <= 0)
            {
                return;
            }

            const int successExitCode = 0;
            if (exitCode != successExitCode)
            {
                this.logger.LogError(
                    "ExitCode: {ExitCode}. LibreOffice Error Output: {ErrorOutput}",
                    exitCode,
                    errorOutput.ToString());
            }
            else
            {
                this.logger.LogWarning(
                    "ExitCode: {ExitCode}. LibreOffice Warning Output : {WarningOutput}",
                    exitCode,
                    errorOutput.ToString());
            }
        }

        private List<string> GetCommandArguments(string docx, long userId, string tempFolder)
        {
            return new List<string>
            {
                "--convert-to",
                "pdf:writer_pdf_Export",
                docx,
                Environment.OSVersion.Platform == PlatformID.Unix ?
                    $"-env:UserInstallation=file:///{this.options.Value.TempUserPathForArgs}{userId}" :
                    $"-env:UserInstallation={this.options.Value.TempUserPathForArgs}{userId}",
                "--norestore",
                "--writer",
                "--headless",
                "--outdir",
                tempFolder
            };
        }
    }
}
