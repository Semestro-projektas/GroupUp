using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models
{
    public class Result
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public int StatusCode { get; set; }

        public Result()
        {
            Success = true;
            StatusCode = 200;
        }

        public Result(string error, int statusCode)
        {
            Success = false;
            Error = error;
            StatusCode = statusCode;
        }

        public Result(ResultType type)
        {
            if (type == ResultType.Unauthorized)
            {
                Success = false;
                Error = "You have to be logged in to perform this action.";
                StatusCode = 401;
            }
            else if (type == ResultType.NoPermissions || type == ResultType.NotOwner)
            {
                Success = false;
                Error = "You don't have required permissions to do this action.";
                StatusCode = 403;
            }
        }

        public Result(Exception ex)
        {
            Success = false;
            Error = ex.Message;
            StatusCode = 400;
        }
    }

    public class UpdateProfileResult
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Field { get; set; }
        public string WorkExperience { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Picture { get; set; }
        public string CurrentlyWorking { get; set; }

        public UpdateProfileResult()
        {
            StatusCode = 200;
        }

        public UpdateProfileResult(ResultType type)
        {
            if (type == ResultType.Unauthorized)
            {
                StatusCode = 401;
                Error = "You must be logged in.";
            }
        }
    }

    public class UpdateCompanyResult
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Logo { get; set; }
        public bool Approved { get; set; }

        public UpdateCompanyResult(int statusCode, string error)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public UpdateCompanyResult()
        {

        }
    }

    public class UpdateGroupResult
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Hot { get; set; }

        public UpdateGroupResult()
        {
            
        }

        public UpdateGroupResult(int statusCode, string error)
        {
            StatusCode = statusCode;
            Error = error;
        }
    }

    public class CreateGroupResult
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }

        public CreateGroupResult()
        {
            
        }

        public CreateGroupResult(int statusCode, string error)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public CreateGroupResult(ResultType type)
        {
            
        }
    }

    public class CreateCompanyResult
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }

        public CreateCompanyResult()
        {

        }

        public CreateCompanyResult(int statusCode, string error)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public CreateCompanyResult(ResultType type)
        {
            if (type == ResultType.Unauthorized)
            {
                StatusCode = 401;
                Error = "You must be logged in to do this.";
            }
        }
    }

    public enum ResultType
    {
        Unauthorized,
        NotOwner,
        NoPermissions,
        BadRequest,
        BadArgument
    }
}
