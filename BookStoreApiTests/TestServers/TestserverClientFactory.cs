using BookStoreApi.Contracts;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.TestServers {
    /// <summary>
    /// Normally it is forbidden to change this, but it is the test project, and I work alone :)
    /// </summary>
    /// <typeparam name="TDataSeeder"></typeparam>
    public abstract class TestserverClientFactory : ITestClientFactoryAsync, IDisposable {
        protected Task<TestServer> _server = null;
        protected Task<HttpClient> _client = null;

        protected abstract TestServer CreateServer();

        protected TestserverClientFactory() {
            _client = InitializeTestInfrastruture();
        }


        #region Interface implementation
        /// <summary>
        /// Creates test client in async way
        /// </summary>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public async virtual Task<HttpClient> GetTestClientAsync(Func<HttpClient, Task> initAction = null) {
            var result = await _client;
            if(initAction != null)
                await initAction.Invoke(result);
            return result;
        }
        #endregion



        protected virtual Task<TestServer> CreateServerAsync() {
            if (_server == null) {
                _server = Task.Factory.StartNew(() => CreateServer());
            }
            return _server;
        }


        protected virtual Task<HttpClient> InitializeTestInfrastruture() {
            return CreateServerAsync().ContinueWith<HttpClient>((serverCreationTask) => {
                try {
                    if (serverCreationTask.Status == TaskStatus.RanToCompletion) {
                        TestServer server = serverCreationTask.Result;
                        return server.CreateClient();
                    }
                    else
                        throw new OperationCanceledException("Failed to create test server");
                }
                catch (AggregateException e) {
                    throw e.Flatten();
                }
            });
        }

        #region Dispose

        private bool _Disposed = false;
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_Disposed)
                return;
            if (_server != null && disposing) {
                _server.Dispose();
                _server = null;
            }

            if (_client != null && disposing) {
                _client.Dispose();
                _client = null;
            }
            _Disposed = true;
        }


        #endregion

    }
}
