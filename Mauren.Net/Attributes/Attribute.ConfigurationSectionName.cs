using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Specifies the key name for a configuration section.
    /// </summary>
    /// 
    /// <remarks>
    /// Initializes a new instance of <see cref="ConfigurationSectionNameAttribute"/>.
    /// </remarks>
    /// 
    /// <param name="name">The key name.</param>
    [Obsolete($"Use {nameof(ConfigurationSectionAttribute)} instead.", error: true)]
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationSectionNameAttribute(String name) : Attribute
    {
        /// <summary>
        /// The key name for a configuration section.
        /// </summary>
        public String Name { get; } = name;
    }
}

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationSectionNameAttributeExtensions
    {

        /// <summary>
        /// Configures a <typeparamref name="TConfiguration"/> object with data from the provided <paramref name="configuration"/>.
        /// The <see cref="IConfigurationSection"/> is selected from <paramref name="configuration"/> via the <typeparamref name="TConfiguration"/>'s
        /// <see cref="ConfigurationSectionNameAttribute"/>.
        /// </summary>
        /// 
        /// <typeparam name="TConfiguration">
        /// The <see cref="Type"/> to configure via <paramref name="configuration"/>.
        /// </typeparam>
        /// 
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to configure <typeparamref name="TConfiguration"/> with.
        /// </param>
        /// 
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> containing data to configure <typeparamref name="TConfiguration"/> with.
        /// </param>
        /// 
        /// <returns></returns>
        public static IServiceCollection LoadConfiguration<TConfiguration>(this IServiceCollection services, IConfiguration configuration)
            where TConfiguration : class
        {
            // Determine the configuration section name for the configuration type
            String sectionName = typeof(TConfiguration).GetSectionName();
            // Get the configuration section via the section name
            IConfigurationSection section = configuration.GetSection(sectionName);
            // Configure the TConfiguration object via the IConfigurationSection data
            services.Configure<TConfiguration>(section);
            // Return the IServiceCollection for chaining
            return services;
        }
    }
}