namespace KosmoGraph.Desktop.View
{
    using Microsoft.Practices.Prism;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.MefExtensions;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class KosmoGraphBootstrapper : MefBootstrapper
    {
        #region Create and initialize this applications shell

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<KosmoGraphWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (KosmoGraphWindow)(this.Shell);
            Application.Current.MainWindow.Show();
        }

        #endregion 

        #region Configure Dependency Injection container 

        override protected void ConfigureContainer()
        {
            this.CreateAggregateCatalog();
            this.RegisterBootstrapperProvidedTypes();
            this.AggregateCatalog.Catalogs.Add(new DirectoryCatalog("."));

            //this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(KosmoGraphBootstrapper).Assembly));
            //var serviceLocator = this.Container.GetExportedValue<Microsoft.Practices.ServiceLocation.IServiceLocator>();
        }

        private void RegisterBootstrapperProvidedTypes()
        {
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
            this.Container.ComposeExportedValue<IModuleCatalog>(this.ModuleCatalog);
            this.Container.ComposeExportedValue<IServiceLocator>(new MefServiceLocatorAdapter(this.Container));
            this.Container.ComposeExportedValue<AggregateCatalog>(this.AggregateCatalog);
        }

        protected override Microsoft.Practices.Prism.Regions.RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            //var tmp =  base.ConfigureRegionAdapterMappings();
            //return tmp;
            return null;
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var moduleCatalog = new DirectoryModuleCatalog();
            moduleCatalog.ModulePath = @".\Modules";
            return moduleCatalog;
        }

        #endregion 
    }
}
