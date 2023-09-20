// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Gems.Mvc.Filters.Errors
{
    public class GenericErrorViewModel : ErrorViewModel
    {
        public GenericErrorViewModel() { }

        public GenericErrorViewModel(ExceptionContext context, bool showStackTrace)
        {
            this.Code = context.HttpContext.TraceIdentifier;
            this.Init(context.Exception, showStackTrace);
        }

        public GenericErrorViewModel(Exception ex, bool showStackTrace)
        {
            this.Init(ex, showStackTrace);
        }

        public GenericErrorViewModel InnerException { get; set; }

        public string[] StackTrace { get; set; }

        private void Init(Exception error, bool showStackTrace)
        {
            this.Message = error.Message;
            if (showStackTrace)
            {
                this.ParseStackTrace(error);
            }

            if (error.InnerException != null)
            {
                this.InnerException = new GenericErrorViewModel(error.InnerException, showStackTrace);
            }
        }

        private void ParseStackTrace(Exception exception)
        {
            this.StackTrace = exception.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
