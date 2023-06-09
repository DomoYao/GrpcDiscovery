﻿using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Core
{
    /// <summary>
    /// Cap options extension
    /// </summary>
    public interface ICapOptionsExtension
    {
        /// <summary>
        /// Registered child service.
        /// </summary>
        /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
        void AddServices(IServiceCollection services);
    }
}
