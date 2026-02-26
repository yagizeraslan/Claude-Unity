using System;

namespace YagizEraslan.Claude.Unity
{
    /// <summary>
    /// A generic result wrapper that provides explicit success/failure handling
    /// instead of returning null on errors.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public string Error { get; }
        public Exception Exception { get; }
        public int? HttpStatusCode { get; }

        private Result(T value, bool isSuccess, string error, Exception exception = null, int? httpStatusCode = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Exception = exception;
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Creates a successful result with the given value.
        /// </summary>
        public static Result<T> Success(T value) => new Result<T>(value, true, null);

        /// <summary>
        /// Creates a failed result with the given error message.
        /// </summary>
        public static Result<T> Failure(string error, Exception exception = null, int? httpStatusCode = null)
            => new Result<T>(default, false, error, exception, httpStatusCode);

        /// <summary>
        /// Executes the appropriate callback based on whether the result is a success or failure.
        /// </summary>
        public void Match(Action<T> onSuccess, Action<string> onFailure)
        {
            if (IsSuccess)
                onSuccess?.Invoke(Value);
            else
                onFailure?.Invoke(Error);
        }

        /// <summary>
        /// Returns the value if successful, otherwise returns the default value.
        /// </summary>
        public T GetValueOrDefault(T defaultValue = default) => IsSuccess ? Value : defaultValue;

        /// <summary>
        /// Maps the success value to a new type using the provided function.
        /// </summary>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            if (IsFailure)
                return Result<TNew>.Failure(Error, Exception, HttpStatusCode);

            try
            {
                return Result<TNew>.Success(mapper(Value));
            }
            catch (Exception ex)
            {
                return Result<TNew>.Failure(ex.Message, ex);
            }
        }
    }
}
