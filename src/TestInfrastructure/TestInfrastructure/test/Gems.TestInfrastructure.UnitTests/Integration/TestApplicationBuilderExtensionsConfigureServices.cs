using FluentAssertions;

using Gems.TestInfrastructure.Integration;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Gems.TestInfrastructure.UnitTests.Integration
{
    public class TestApplicationBuilderExtensionsConfigureServices
    {
        [Test]
        public void RemoveServiceImplementation()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.RemoveServiceImplementation<MyServiceImplementation1>(),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(1);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                });
        }

        [Test]
        public void RemoveService()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.RemoveService<IMyService>(),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(1);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                });
        }

        [Test]
        public void ConfigureServices()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                },
                builder => builder.ConfigureServices(s => s.AddSingleton<IMyService, MyServiceImplementation1>()),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationType == typeof(MyServiceImplementation1));
                });
        }

        [Test]
        public void RemoveServiceImplementationByFullName()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.RemoveServiceImplementationByFullName(typeof(MyServiceImplementation1).FullName),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(1);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                });
        }

        [Test]
        public void RemoveServiceImplementationByName()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.RemoveServiceImplementationByName("MyServiceImplementation1"),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(1);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                });
        }

        [Test]
        public void RemoveServiceByName()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.RemoveServiceByName("IMyService"),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(1);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                });
        }

        [Test]
        public void RemoveServiceByFullName()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.RemoveServiceByFullName(typeof(IMyService).FullName),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(1);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                });
        }

        [Test]
        public void ReplaceServiceWithScoped()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.ReplaceServiceWithScoped<IMyService, MyServiceImplementation2>(),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationType == typeof(MyServiceImplementation2) &&
                            x.Lifetime == ServiceLifetime.Scoped);
                });
        }

        [Test]
        public void ReplaceServiceWithSingleton()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddTransient<IMyService, MyServiceImplementation1>();
                },
                builder => builder.ReplaceServiceWithSingleton<IMyService, MyServiceImplementation2>(),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationType == typeof(MyServiceImplementation2) &&
                            x.Lifetime == ServiceLifetime.Singleton);
                });
        }

        [Test]
        public void ReplaceServiceWithTransient()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddScoped<IMyService, MyServiceImplementation1>();
                },
                builder => builder.ReplaceServiceWithTransient<IMyService, MyServiceImplementation2>(),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationType == typeof(MyServiceImplementation2) &&
                            x.Lifetime == ServiceLifetime.Transient);
                });
        }

        [Test]
        public void ReplaceServiceWithScopedFactory()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddSingleton<IMyService, MyServiceImplementation1>();
                },
                builder => builder.ReplaceServiceWithScoped<IMyService, MyServiceImplementation2>(sp => new MyServiceImplementation2()),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationFactory.GetType() == typeof(Func<IServiceProvider, MyServiceImplementation2>) &&
                            x.Lifetime == ServiceLifetime.Scoped);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                });
        }

        [Test]
        public void ReplaceServiceWithSingletonFactory()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddTransient<IMyService, MyServiceImplementation1>();
                },
                builder => builder.ReplaceServiceWithSingleton<IMyService, MyServiceImplementation2>(sp => new MyServiceImplementation2()),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationFactory.GetType() == typeof(Func<IServiceProvider, MyServiceImplementation2>) &&
                            x.Lifetime == ServiceLifetime.Singleton);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                });
        }

        [Test]
        public void ReplaceServiceWithTransientFactory()
        {
            DoTest(
                services =>
                {
                    services.AddTransient<IDummy, Dummy>();
                    services.AddScoped<IMyService, MyServiceImplementation1>();
                },
                builder => builder.ReplaceServiceWithTransient<IMyService, MyServiceImplementation2>(sp => new MyServiceImplementation2()),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(2);
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IDummy) &&
                            x.ImplementationType == typeof(Dummy));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    services
                        .Should()
                        .Contain(x =>
                            x.ServiceType == typeof(IMyService) &&
                            x.ImplementationFactory.GetType() == typeof(Func<IServiceProvider, MyServiceImplementation2>) &&
                            x.Lifetime == ServiceLifetime.Transient);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                });
        }

        private static ITestApplicationBuilder DoTest(
            Action<IServiceCollection> arrange,
            Action<ITestApplicationBuilder> act,
            Action<Mock<ITestApplicationBuilder>, IServiceCollection> assert)
        {
            var services = new ServiceCollection();
            var mockBuilder = new Mock<ITestApplicationBuilder>();
            mockBuilder
                .Setup(x => x.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                .Callback(new InvocationAction(i =>
                {
                    var action = i.Arguments.First() as Action<IServiceCollection>;
                    action!.Invoke(services);
                }))
                .Returns(() => mockBuilder.Object);
            var builder = mockBuilder.Object;
            arrange(services);
            act(builder);
            assert(mockBuilder, services);
            return builder;
        }
    }
}
