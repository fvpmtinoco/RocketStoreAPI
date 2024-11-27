using System;
using System.Diagnostics.CodeAnalysis;

namespace RocketStoreApi.Managers
{
    /// <summary>
    /// Describes the result of an operation.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public partial class Result<T>
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the result represents a failure.
        /// </summary>
        public bool Failed
        {
            get
            {
                return !string.IsNullOrEmpty(this.ErrorCode);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        public string ErrorDescription
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        private Result()
        {
        }

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
        public static Result<T> Success(T value)
        {
            return new Result<T>()
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
        public static Result<T> Failure(string errorCode, string errorDescription)
        {
            errorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            errorDescription = errorDescription ?? throw new ArgumentNullException(nameof(errorDescription));

            return new Result<T>()
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
        public bool FailedWith(string errorCode)
        {
            return string.Compare(this.ErrorCode, errorCode, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
