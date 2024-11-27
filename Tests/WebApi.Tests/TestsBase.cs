using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketStoreApi.Tests
{
    /// <summary>
    /// Defines the base class for test classes.
    /// </summary>
    public abstract partial class TestsBase
    {
        #region Public Methods

        /// <summary>
        /// Gets the content from the specified response.
        /// </summary>
        /// <typeparam name="T">The content response.</typeparam>
        /// <param name="response">The response.</param>
        /// <returns>
        /// The <see cref="Task{TResult}"/> that represents the asynchronous operation.
        /// The <typeparamref name="T"/> instance.
        /// </returns>
        public virtual async Task<T> GetResponseContentAsync<T>(HttpResponseMessage response)
        {
            System.ArgumentNullException.ThrowIfNull(response);

            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json);
        }

        #endregion
    }
}
