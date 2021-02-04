using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace BookStoreApiTests.Mocks {
    public class MockLogger<TBase> : ILogger<TBase> where TBase:class {

        private readonly Mock<ILogger<TBase>> _Mock;
        public MockLogger() {
            _Mock = new Mock<ILogger<TBase>>();
          
            
        }

        public IDisposable BeginScope<TState>(TState state) => _Mock.Object.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            string info = formatter?.Invoke(state, exception);
            if (string.IsNullOrWhiteSpace(info))
                info = state?.ToString() ?? "Empty info"; 
            Trace.WriteLine($"{logLevel}: {info}");
        }


    }
}
