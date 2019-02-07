using System;

namespace Ccf.Ck.Libs.AutoUpdater.Http
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public string Content { get; set; }
    }
}
