using FluentAssertions;
using gOldCleaner.Domain;
using gOldCleaner.DomainServices;
using gOldCleaner.InfrastructureServices;
using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace gOldCleaner.Tests
{
    [Trait("Common", "Arch Test")]
    // ReSharper disable once TestFileNameWarning 
    public class ArchTests
    {
        protected static Assembly ApplicationAssembly = typeof(FolderItem).Assembly;

        [Fact]
        public void Domain_Classes_Should_Be_Sealed()
        {

            var types = Types.InAssembly(ApplicationAssembly);

             var result = types.That().ResideInNamespace("gOldCleaner.Domain").And().AreNotInterfaces().Should().BeSealed().GetResult();

             result.FailingTypeNames.Should().BeNullOrEmpty("Domain class should be sealed");
        }

        [Fact]
        public void App_Classes_Should_Not_Depend_On_Tests()
        {
            var types = Types.InAssembly(ApplicationAssembly);

            var result = types
                .That()
                .ResideInNamespace("gOldCleaner")
                .ShouldNot()
                .HaveDependencyOn("gOldCleaner.Tests")
                .GetResult();

            result.FailingTypeNames.Should().BeNullOrEmpty("Domain can't depend on DomainServices");
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void All_Interfaces_Must_Start_With_I()
        {
            var types = Types.InAssembly(ApplicationAssembly);
            var result = types.That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I").GetResult();

            result.FailingTypeNames.Should().BeNullOrEmpty("Interfaces must start with I");
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact] public void All_Implementations_Of_IServices_Must_Have_Service_End()
        {
            var types = Types.InAssembly(ApplicationAssembly);
            var result = types.That()
                .ImplementInterface(typeof(IDomainService))
                .Or()
                .ImplementInterface(typeof(IInfrastructureService))
                .Should()
                .HaveNameEndingWith("Service").GetResult();

            result.FailingTypeNames.Should().BeNullOrEmpty("Service implementation must have Service at end of name");
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Classes_Should_Not_Depend_On_DomainServices_Layer()
        {
            var types = Types.InAssembly(ApplicationAssembly);
            
            var result = types
                .That()
                .ResideInNamespaceMatching(@"^gOldCleaner\.Domain$")
                .Should()
                .NotHaveDependencyOn("gOldCleaner.DomainServices")
                .GetResult();

            result.FailingTypeNames.Should().BeNullOrEmpty("Domain can't depend on DomainServices");
            result.IsSuccessful.Should().BeTrue();
        }
        
        [Fact]
        public void Domain_Classes_Should_Not_Depend_On_InfrastructureServices_Layer()
        {
            var types = Types.InAssembly(ApplicationAssembly);

            var tt = Types.InAssembly(ApplicationAssembly).That().ResideInNamespace("gOldCleaner.Domain");

            var result = types
                .That()
                .ResideInNamespaceMatching(@"^gOldCleaner\.Domain$")
                .Should()
                .NotHaveDependencyOn("gOldCleaner.InfrastructureServices")
                .GetResult();

            result.FailingTypeNames.Should().BeNullOrEmpty("Domain can't depend on InfrastructureServices");
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
