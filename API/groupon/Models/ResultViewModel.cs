using System;

namespace groupon.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class RequestErrorViewModel
    {
        public string Error { get; set; }

        public RequestErrorViewModel(string error)
        {
            Error = error;
        }
    }

    public class RequestSuccessViewModel
    {
        public string StatusCode { get; set; }

        public RequestSuccessViewModel(string statusCode)
        {
            StatusCode = statusCode;
        }
    }

    public class ResultViewModel
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public ResultViewModel(bool success)
        {
            Success = true;
        }

        public ResultViewModel(string rez, string error)
        {
            Success = false;
            Error = error;
        }
    }
}