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

    public class UpdateProfileResult
    {
        public string Authorized { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Field { get; set; }
        public string WorkExperience { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Picture { get; set; }
        public string CurrentlyWorking { get; set; }
    }
}