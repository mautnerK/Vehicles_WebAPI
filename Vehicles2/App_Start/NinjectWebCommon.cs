[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Vehicles.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Vehicles.App_Start.NinjectWebCommon), "Stop")]

namespace Vehicles.App_Start
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Validation;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using Ninject.Web.WebApi.Filter;
    using Vehicles.Repository;
    using Vehicles.Repository.Common;
    using Vehicles.Service;
    using Vehicles.Service.Common;
    using WebApiContrib.IoC.Ninject;
    using static Vehicles2.App_Start.AutoMapperModule;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new Vehicles2.App_Start.AutoMapperModule());
            var root = kernel.Get<Root>();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                
                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind<IMakeService>().To<MakeService>();
            kernel.Bind<IModelService>().To<ModelService>();
            kernel.Bind<IModelRepository>().To<ModelRepository>();
            kernel.Bind<IMakeRepository>().To<MakeRepository>();
        }
    }
}