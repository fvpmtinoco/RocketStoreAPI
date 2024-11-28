using RestSharp;
using System.IO;
using System.Text;
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
        public virtual async Task<T> GetResponseContentAsync<T>(RestResponse response)
        {
            // Check if the response is null
            if (response == null) return default;

            // Convert the response content to a stream
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
            {
                // Use the async DeserializeAsync method to deserialize the JSON content
                return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
