// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.IO.LibreOffice.Exceptions
{
    public class LibreOfficeException : Exception
    {
        public LibreOfficeException(int exitCode)
            : base($"LibreOffice has failed. Exit code: {exitCode}")
        {
        }
    }
}
