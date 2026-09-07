using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public int? StatusCode { get; }
        public string? Message { get; }

        protected Result(bool isSuccess, int? statusCode, string? message)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
        }

        public static Result Success(int code = 200, string message = "Успех") => new Result(true, code, message);
        public static Result Failure(string message, int code) => new Result(false, code, message);
    }
    public class Result<T> : Result
    {
        public T? Value { get; }
        public Result(bool isSuccess, T? value, int? code, string? message) : base(isSuccess, code, message)
        {
            Value = value;
        }

        public static Result<T> Success(T value, int code = 200, string message = "Успех") => new Result<T>(true, value, code, message);
        public new static Result<T> Failure(string message, int code) => new Result<T>(false, default, code, message);
    }
}