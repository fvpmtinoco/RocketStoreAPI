using System;
using System.Diagnostics.CodeAnalysis;

namespace RocketStoreApi.Configurations
{
    /// <summary>
    /// Describes the result of an operation.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <typeparam name="TErrorCode">The type of the error code used when the operation fails. struct: This constraint means that TErrorCode must be a value type 
    /// (like an enum, struct, or primitive type). This also allows it to be nullable (e.g., TErrorCode?), as nullable types are derived from struct.</typeparam>
    public partial class Result<T, TErrorCode> where TErrorCode : struct, Enum
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the result represents a failure.
        /// </summary>
        public bool IsSuccess => ErrorCode is null;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value { get; private set; } = default!;

        /// <summary>
        /// Gets the error.
        /// </summary>
        public TErrorCode? ErrorCode { get; private set; }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        public string ErrorDescription { get; private set; } = default!;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of <see cref="Result{T}" /> that represents success.
        /// </summary>
        /// <param name="value">The result value.</param>
        /// <returns>
        /// The <see cref="Result{T}" /> instance.
        /// </returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design.")]
        public static Result<T, TErrorCode> Success(T value)
        {
            return new Result<T, TErrorCode>
            {
                Value = value
            };
        }

        /// <summary>
        /// Creates a new instance of <see cref="Result{T}" /> that represents success.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorDescription">The error description.</param>
        /// <returns>
        /// The <see cref="Result{T}" /> instance.
        /// </returns>
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design.")]
        public static Result<T, TErrorCode> Failure(TErrorCode errorCode, string errorDescription)
        {
            return new Result<T, TErrorCode>()
            {
                ErrorCode = errorCode,
                ErrorDescription = errorDescription
            };
        }

        /// <summary>
        /// Gets a value indicating whether the result represents a failure with the specified error code.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>A value indicating whether the result represents a failure with the specified error code.</returns>
        public bool FailedWith(TErrorCode errorCode)
        {
            if (ErrorCode is null)
                return false;
            else
                return errorCode.Equals(ErrorCode);
        }

        #endregion
    }
}
