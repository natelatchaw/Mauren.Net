using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions"/> will bind against.
        /// </summary>
        /// 
        /// <typeparam name="TOptions">
        /// The options type to be configured.
        /// </typeparam>
        /// 
        /// <param name="optionsBuilder">
        /// The options builder to add the services to.
        /// </param>
        /// 
        /// <param name="configurationRoot">
        /// The configuration root.
        /// </param>
        /// 
        /// <param name="configureBinder">
        /// Used to configure the <see cref="BinderOptions"/>.
        /// </param>
        /// 
        /// <returns>
        /// The <see cref="OptionsBuilder{TOptions}"/> so that additional calls can be chained.
        /// </returns>
        /// 
        /// <remarks>
        /// The relevant <see cref="IConfigurationSection"/> will be selected using the 
        /// <see cref="ConfigurationSectionAttribute.Name">Name</see> provided by the
        /// <see cref="ConfigurationSectionAttribute"/> applied to <typeparamref name="TOptions"/>
        /// relative to <paramref name="configurationRoot"/>.
        /// </remarks>
        public static OptionsBuilder<TOptions> Bind<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, IConfigurationRoot configurationRoot)
            where TOptions : class
        {
            // Get the configuration section with the name specified by the ConfigurationSectionAttribute
            IConfigurationSection section = configurationRoot.GetSection<TOptions>();
            // Bind the configuration section to TOptions
            return optionsBuilder.Bind(section, (BinderOptions options) =>
            {

            });
        }
    }
}
