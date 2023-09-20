// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

namespace Gems.IO.LibreOffice
{
    public interface ILibreOffice
    {
        Task ConvertToPdfAsync(string docxPath, string pdfPath, CancellationToken cancellationToken);
    }
}
