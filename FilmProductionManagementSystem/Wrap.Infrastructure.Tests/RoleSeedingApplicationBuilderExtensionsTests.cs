namespace Wrap.Infrastructure.Tests;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Moq;
using NUnit.Framework;

using Data.Seeding.Interfaces;
using Web.Infrastructure.Extensions;

[TestFixture]
public class RoleSeedingApplicationBuilderExtensionsTests
{
    [Test]
    public void UseRolesSeeder_WhenSeederIsResolved_CallsSeedRolesAsyncAndReturnsSameApplicationBuilder()
    {
        // Arrange
        Mock<IApplicationBuilder> applicationBuilderMock = new(MockBehavior.Strict);
        Mock<IServiceProvider> rootServiceProviderMock = new(MockBehavior.Strict);
        Mock<IServiceScopeFactory> scopeFactoryMock = new(MockBehavior.Strict);
        Mock<IServiceScope> scopeMock = new(MockBehavior.Strict);
        Mock<IServiceProvider> scopedServiceProviderMock = new(MockBehavior.Strict);
        Mock<IApplicationRoleSeeder> roleSeederMock = new(MockBehavior.Strict);

        roleSeederMock
            .Setup(rs => rs.SeedRolesAsync())
            .Returns(Task.CompletedTask);

        applicationBuilderMock
            .Setup(ab => ab.ApplicationServices)
            .Returns(rootServiceProviderMock.Object);

        rootServiceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);

        scopeFactoryMock
            .Setup(sf => sf.CreateScope())
            .Returns(scopeMock.Object);

        scopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(scopedServiceProviderMock.Object);

        scopedServiceProviderMock
            .Setup(sp => sp.GetService(typeof(IApplicationRoleSeeder)))
            .Returns(roleSeederMock.Object);

        scopeMock
            .Setup(s => s.Dispose());

        // Act
        IApplicationBuilder result = applicationBuilderMock.Object.UseRolesSeeder();

        // Assert
        Assert.That(result, Is.SameAs(applicationBuilderMock.Object));

        roleSeederMock.Verify(rs => rs.SeedRolesAsync(), Times.Once);
        applicationBuilderMock.Verify(ab => ab.ApplicationServices, Times.Once);
        rootServiceProviderMock.Verify(sp => sp.GetService(typeof(IServiceScopeFactory)), Times.Once);
        scopeFactoryMock.Verify(sf => sf.CreateScope(), Times.Once);
        scopeMock.Verify(s => s.ServiceProvider, Times.Once);
        scopedServiceProviderMock.Verify(sp => sp.GetService(typeof(IApplicationRoleSeeder)), Times.Once);
        scopeMock.Verify(s => s.Dispose(), Times.Once);

        applicationBuilderMock.VerifyNoOtherCalls();
        rootServiceProviderMock.VerifyNoOtherCalls();
        scopeFactoryMock.VerifyNoOtherCalls();
        scopeMock.VerifyNoOtherCalls();
        scopedServiceProviderMock.VerifyNoOtherCalls();
        roleSeederMock.VerifyNoOtherCalls();
    }

    [Test]
    public void UseRolesSeeder_WhenSeederThrows_DisposesScopeAndRethrows()
    {
        // Arrange
        Mock<IApplicationBuilder> applicationBuilderMock = new(MockBehavior.Strict);
        Mock<IServiceProvider> rootServiceProviderMock = new(MockBehavior.Strict);
        Mock<IServiceScopeFactory> scopeFactoryMock = new(MockBehavior.Strict);
        Mock<IServiceScope> scopeMock = new(MockBehavior.Strict);
        Mock<IServiceProvider> scopedServiceProviderMock = new(MockBehavior.Strict);
        Mock<IApplicationRoleSeeder> roleSeederMock = new(MockBehavior.Strict);

        InvalidOperationException expectedException = new("Seeding failed.");

        roleSeederMock
            .Setup(rs => rs.SeedRolesAsync())
            .ThrowsAsync(expectedException);

        applicationBuilderMock
            .Setup(ab => ab.ApplicationServices)
            .Returns(rootServiceProviderMock.Object);

        rootServiceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);

        scopeFactoryMock
            .Setup(sf => sf.CreateScope())
            .Returns(scopeMock.Object);

        scopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(scopedServiceProviderMock.Object);

        scopedServiceProviderMock
            .Setup(sp => sp.GetService(typeof(IApplicationRoleSeeder)))
            .Returns(roleSeederMock.Object);

        scopeMock
            .Setup(s => s.Dispose());

        // Act
        InvalidOperationException? actualException = Assert.Throws<InvalidOperationException>(
            () => applicationBuilderMock.Object.UseRolesSeeder());

        // Assert
        Assert.That(actualException, Is.SameAs(expectedException));

        roleSeederMock.Verify(rs => rs.SeedRolesAsync(), Times.Once);
        scopeMock.Verify(s => s.Dispose(), Times.Once);

        applicationBuilderMock.Verify(ab => ab.ApplicationServices, Times.Once);
        rootServiceProviderMock.Verify(sp => sp.GetService(typeof(IServiceScopeFactory)), Times.Once);
        scopeFactoryMock.Verify(sf => sf.CreateScope(), Times.Once);
        scopeMock.Verify(s => s.ServiceProvider, Times.Once);
        scopedServiceProviderMock.Verify(sp => sp.GetService(typeof(IApplicationRoleSeeder)), Times.Once);

        applicationBuilderMock.VerifyNoOtherCalls();
        rootServiceProviderMock.VerifyNoOtherCalls();
        scopeFactoryMock.VerifyNoOtherCalls();
        scopeMock.VerifyNoOtherCalls();
        scopedServiceProviderMock.VerifyNoOtherCalls();
        roleSeederMock.VerifyNoOtherCalls();
    }
}