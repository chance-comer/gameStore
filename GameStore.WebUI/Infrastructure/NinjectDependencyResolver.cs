using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moq;
using Ninject;
using System.Web.Mvc;
using GameStore.Domain.Entities;
using GameStore.Domain.Abstract;
using GameStore.WebUI.Infrastructure.Abstract;
using GameStore.WebUI.Infrastructure.Concrete;
using GameStore.Domain.Concrete;
using System.Configuration;

namespace GameStore.WebUI.Infrastructure {
    public class NinjectDependencyResolver : IDependencyResolver {
        IKernel kernel;

        public NinjectDependencyResolver(IKernel _kernel) {
            kernel = _kernel;
            AddBindings();
        }

        public object GetService(Type serviceType) {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings() {
            // Здесь размещаются привязки
            /*Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { Name = "SimCity", Price = 1499 },
                new Game { Name = "TITANFALL", Price=2299 },
                new Game { Name = "Battlefield 4", Price=899.4M }
            });
            kernel.Bind<IGameRepository>().ToConstant(mock.Object);*/
            kernel.Bind<IGameRepository>().To<EFGameRepository>();

            EmailSettings settings = new EmailSettings {
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            };
            kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", settings);

            kernel.Bind<IAuthProvider>().To<FormAuthProvider>();
        }
    }
}