using Mauren.Net;
using Mauren.Net.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mauren.Net
{
    public interface IAuthenticationStateFactory
    {
        TAuthenticationState Create<TAuthenticationState>(String name) where TAuthenticationState : IAuthenticationState, new();
        void Add<TAuthenticationState>(String key, TAuthenticationState state) where TAuthenticationState : IAuthenticationState;
    }

    public class AuthenticationStateFactory : IAuthenticationStateFactory
    {
        private readonly ConcurrentDictionary<String, IAuthenticationState> _store;

        public AuthenticationStateFactory()
        {
            _store = new ConcurrentDictionary<String, IAuthenticationState>();
        }

        TAuthenticationState IAuthenticationStateFactory.Create<TAuthenticationState>(String name)
        {
            IAuthenticationState state = _store.GetOrAdd(name, (String key) => new TAuthenticationState());
            if (state is not TAuthenticationState value)
                throw new InvalidOperationException($"Expected {typeof(TAuthenticationState).Name}, but found {state.GetType().Name}");
            return value;
        }

        void IAuthenticationStateFactory.Add<TAuthenticationState>(String key, TAuthenticationState state)
        {
            IAuthenticationState _ = _store.AddOrUpdate(key, state, (String key, IAuthenticationState state) => state);
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationStateFactoryExtensions
    {
        public static IServiceCollection AddAuthenticationState(this IServiceCollection services, Func<IDictionary<String, AuthenticationState>>? configure = default) => services
            .AddSingleton<IAuthenticationStateFactory, AuthenticationStateFactory>((IServiceProvider provider) =>
            {
                AuthenticationStateFactory factory = new AuthenticationStateFactory();
                IDictionary<String, AuthenticationState> keyValuePairs = configure switch
                {
                    Func<IDictionary<String, AuthenticationState>> values => values.Invoke(),
                    _ => new Dictionary<String, AuthenticationState>(),
                };
                foreach ((String key, AuthenticationState value) in keyValuePairs)
                    (factory as IAuthenticationStateFactory).Add(key, value);
                return factory;
            });

        public static IServiceCollection AddAuthenticationStateFactory<TAuthenticationStateFactory>(this IServiceCollection services) 
            where TAuthenticationStateFactory : class, IAuthenticationStateFactory 
            => services
                .AddSingleton<IAuthenticationStateFactory, TAuthenticationStateFactory>();
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationStateExtensions
    {
        public static IServiceCollection AddCredentials(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();

            IEnumerable<Type> types = assemblies.SelectMany((Assembly assembly) => assembly.GetTypes());

            IEnumerable<Type> found = types.Where((Type type) => typeof(ICredential).IsAssignableFrom(type))
                .Where((Type type) => type.IsClass)
                .Where((Type type) => type.IsAbstract == false);

            return services;
        }
    }
}
