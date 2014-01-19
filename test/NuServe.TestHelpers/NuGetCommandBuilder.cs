using System.Linq;
using System.Collections.Generic;
using NSubstitute;
using NuGet;
using NuGet.Commands;

namespace NuServe.TestHelpers
{
    public static class NuGetCommandBuilder
    {
        public static ListCommand BuildListCommandFor(string source)
        {
            var sourceProvider = Substitute.For<IPackageSourceProvider>();
            sourceProvider.LoadPackageSources()
                          .Returns(call => new List<PackageSource>
                              {
                                  new PackageSource(source)
                              });
            var packageRepositoryFactory = new PackageRepositoryFactory();

            return new ListCommand(packageRepositoryFactory, sourceProvider)
                {
                    Console = new InMemoryConsole()
                };
        }

        public static PublishCommand BuildPublishCommandFor(string source)
        {
            var sourceProvider = Substitute.For<IPackageSourceProvider>();
            sourceProvider.LoadPackageSources()
                          .Returns(call => new List<PackageSource>
                              {
                                  new PackageSource(source)
                              });

            return new PublishCommand(sourceProvider)
                {
                    Console = new InMemoryConsole(),
                    Source = source
                };
        }
    }
}