namespace SharedCommon.DependencyInjection
{
    using SharedCommon.Logger;
    using StructureMap;

    public sealed class Injector
    {
        private static bool wasCalled;
        private static IContainer container;
        
        private Injector()
        {
        }

        public static Injector Instance { get; } = new Injector();

        public static IContainer Container => container;

        public void Setup(ILogger logger = null)
        {
            logger?.Log($"{ nameof(Injector) } | Configure DI container...");
            if (wasCalled)
            {
                logger?.Log($"{ nameof(Injector) } | DI container has been already configured, canceling...");
                return;
            }

            wasCalled = true;

            container = new StructureMap.Container();
            container.Configure(config => 
            {
                config.Scan(scanner =>
                {
                    logger?.LogToStatusLine($"{ nameof(Injector) } | DI configurator: { scanner.Description }");
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                    scanner.LookForRegistries();
                    scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                });
            });

            // logger?.Log(container.WhatDidIScan());
            logger?.Log($"{ nameof(Injector) } | DI container has been configured successfully");
        }
    }
}