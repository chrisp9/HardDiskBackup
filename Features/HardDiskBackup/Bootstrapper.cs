using Autofac;
using Autofac.Builder;
using Domain;
using HardDiskBackup.ViewModel;
using Registrar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using SystemWrapper.IO;

namespace HardDiskBackup
{
    public class Bootstrapper
    {
        private ContainerBuilder _containerBuilder;

        public Bootstrapper(ContainerBuilder cb)
        {
            _containerBuilder = cb;
        }

        public ContainerBuilder Bootstrap()
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                allAssemblies.Add(Assembly.LoadFile(dll));
            allAssemblies.Add(Assembly.GetExecutingAssembly());

            foreach (var assembly in allAssemblies)
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    var attribs = type.GetCustomAttributes(typeof(Register), true);

                    foreach (Register registerAttribute in type.GetCustomAttributes(typeof(Register), true))
                    {
                        if (type.Name.ToLowerInvariant().Contains("viewmodel"))
                            RegisterSelf(type, registerAttribute.Scope);
                        else
                            Register(type, type.GetInterfaces(), registerAttribute.Scope);
                    }
                }
            }

            _containerBuilder.RegisterGeneric(typeof(WindowPresenter<,>)).AsImplementedInterfaces();

            _containerBuilder.RegisterType<FileWrap>().As<IFileWrap>().InstancePerDependency();/// Need manual
            _containerBuilder.RegisterType<DirectoryWrap>().As<IDirectoryWrap>().InstancePerDependency(); // Need manual
            _containerBuilder.RegisterType<DriveInfoWrap>().As<IDriveInfoWrap>().InstancePerDependency(); // Need manual
            _containerBuilder.RegisterType<EnvironmentWrap>().As<IEnvironmentWrap>().InstancePerDependency(); // Need manual

            _containerBuilder.RegisterInstance<DefaultScheduler>(DefaultScheduler.Instance).As<IScheduler>();

            return _containerBuilder;
        }

        private void Register(Type concrete, Type[] interfaces, LifeTime scope)
        {
            if (interfaces.Count() == 0)
                RegisterSelf(concrete, scope);
            else
                interfaces.ToList().ForEach(x => Register(concrete, x, scope));
        }

        private void RegisterSelf(Type concrete, LifeTime scope)
        {
            Register(_containerBuilder.RegisterType(concrete).AsSelf(), scope);
        }

        private void Register(Type concrete, Type interf, LifeTime scope)
        {
            if (scope == LifeTime.SingleInstance)
                Register(_containerBuilder.RegisterType(concrete).As(interf), scope);
            else if (scope == LifeTime.Transient)
                Register(_containerBuilder.RegisterType(concrete).As(interf), scope);
            else
                throw new ArgumentException("Invalid scope");
        }

        private void Register(
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> regBuilder,
            LifeTime scope)
        {
            switch (scope)
            {
                case LifeTime.Transient:
                    regBuilder.InstancePerDependency();
                    return;

                case LifeTime.SingleInstance:
                    regBuilder.SingleInstance();
                    return;

                default:
                    throw new ArgumentException("Invalid scope");
            }
        }
    }
}