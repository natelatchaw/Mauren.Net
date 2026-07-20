using System;
using System.Reflection;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Specifies the name for an <see cref="IConfigurationSection"/>
    /// containing values corresponding to the 
    /// <see cref="ConfigurationSectionAttribute"/>'s target.
    /// </summary>
    /// 
    /// <param name="name">
    /// The name of the <see cref="IConfigurationSection"/>.
    /// </param>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationSectionAttribute : Attribute
    {
        /// <summary>
        /// The name of the <see cref="IConfigurationSection"/> which the target will bind to.
        /// </summary>
        public required String Name { get; set; }
    }
}

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationSectionAttributeExtensions
    {
        /// <summary>
        /// Gets the <see cref="IConfigurationSection"/> which <typeparamref name="TOptions"/> will bind against.
        /// </summary>
        /// 
        /// <typeparam name="TOptions">
        /// The options type to be configured.
        /// </typeparam>
        /// 
        /// <param name="configurationRoot">
        /// The configuration root.
        /// </param>
        /// 
        /// <returns>
        /// The retrieved <see cref="IConfigurationSection"/>.
        /// </returns>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Thrown if <typeparamref name="TOptions"/> is missing a <see cref="ConfigurationSectionAttribute"/>.
        /// </exception>
        public static IConfigurationSection GetSection<TOptions>(this IConfigurationRoot configurationRoot)
            where TOptions : class
        {
            // Get the type of TOptions
            Type type = typeof(TOptions);
            // If the type is missing the necessary attribute
            if (type.GetCustomAttribute<ConfigurationSectionAttribute>() is not ConfigurationSectionAttribute configurationSection)
                // Throw an exception if the attribute was not found
                throw new InvalidOperationException($"{type.Name} is missing the {nameof(ConfigurationSectionAttribute)}");
            // Get the configuration section with the name specified by the ConfigurationSectionAttribute
            IConfigurationSection section = configurationRoot.GetRequiredSection(configurationSection.Name);
            // Return the configuration section
            return section;
        }

        extension<TOptions>(TOptions) where TOptions : class
        {
            /// <summary>
            /// Retrieves the <see cref="IConfigurationSection"/> name of <typeparamref name="TOptions"/>
            /// via the <see cref="ConfigurationSectionAttribute"/>.
            /// </summary>
            /// 
            /// <returns>
            /// The name of the <see cref="IConfigurationSection"/>.
            /// </returns>
            /// 
            /// <exception cref="InvalidOperationException">
            /// Thrown if <paramref name="type"/> is missing a <see cref="ConfigurationSectionAttribute"/>.
            /// </exception>
            public static String GetSectionName()
            {
                // Get the type of TOptions
                Type type = typeof(TOptions);
                // If the type is missing the necessary attribute
                if (type.GetCustomAttribute<ConfigurationSectionAttribute>() is not ConfigurationSectionAttribute attribute)
                    // Throw a CustomAttributeFormatException
                    throw new InvalidOperationException($"{type.Name} is missing the {nameof(ConfigurationSectionAttribute)}");
                // If the attribute's value is missing
                if (String.IsNullOrWhiteSpace(attribute?.Name))
                    // Throw a CustomAttributeFormatException
                    throw new InvalidOperationException($"{type.Name} is missing the {nameof(ConfigurationSectionAttribute)} {nameof(ConfigurationSectionAttribute.Name)}");
                // Return the attribute name
                return attribute.Name;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="IConfigurationSection"/> name of <paramref name="type"/>
        /// via the <see cref="ConfigurationSectionAttribute"/>.
        /// </summary>
        /// 
        /// <param name="type">
        /// The <see cref="Type"/> to retrieve the <see cref="IConfigurationSection"/> name of.
        /// </param>
        /// 
        /// <returns>
        /// The name of the <see cref="IConfigurationSection"/>.
        /// </returns>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="type"/> is missing a <see cref="ConfigurationSectionAttribute"/>.
        /// </exception>
        public static String GetSectionName(this Type type)
        {
            // If the type is missing the necessary attribute
            if (type.GetCustomAttribute<ConfigurationSectionAttribute>() is not ConfigurationSectionAttribute attribute)
                // Throw a CustomAttributeFormatException
                throw new InvalidOperationException($"{type.Name} is missing the {nameof(ConfigurationSectionAttribute)}");
            // If the attribute's value is missing
            if (String.IsNullOrWhiteSpace(attribute?.Name))
                // Throw a CustomAttributeFormatException
                throw new InvalidOperationException($"{type.Name} is missing the {nameof(ConfigurationSectionAttribute)} {nameof(ConfigurationSectionAttribute.Name)}");
            // Return the attribute name
            return attribute.Name;
        }
    }
}
