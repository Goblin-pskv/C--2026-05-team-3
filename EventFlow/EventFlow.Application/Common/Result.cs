using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common {
    public class Result {
        public bool IsSuccess {
            get;
        }
        public int StatusCode {
            get;
        }
        public string? Error {
            get;
        }

        protected Result(bool isSuccess, string? error, int statusCode = 400)
        {
            IsSuccess = isSuccess;
            Error = error;
            StatusCode = statusCode;
        }


        public static Result Success() => new Result(true, null);
        public static Result Failure(string error, int statusCode = 400) => new Result(false, error, statusCode);
    }
    public class Result<T> : Result {
        public T? Value {
            get;
        }
        public Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public new static Result<T> Failure(string error) => new Result<T>(false, default, error);
    }
}
