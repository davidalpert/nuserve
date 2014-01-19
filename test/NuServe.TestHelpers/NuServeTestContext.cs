using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace NuServe.TestHelpers
{
    public class NuServeTestContext
    {
        private DirectoryInfo _testPackageRoot;

        public DirectoryInfo ExecutingDirectory { get; private set; }
        public DirectoryInfo PackagesRoot { get; private set; }
        public FileInfo[] TestPackages { get; private set; }

        public NuServeTestContext(Assembly executingAssembly)
        {
            ExecutingDirectory = new DirectoryInfo(Path.GetDirectoryName(executingAssembly.Location));

            InitializePackagesRoot();

            InitializeTestPackageList();
        }

        private void InitializePackagesRoot()
        {
            PackagesRoot = new DirectoryInfo(@".\Packages");

            if (PackagesRoot.Exists) PackagesRoot.Delete(true);

            PackagesRoot.Create();
        }
        
        private void InitializeTestPackageList()
        {
            _testPackageRoot = FindNearestPackagesFolder();

            TestPackages = (_testPackageRoot == null || _testPackageRoot.Exists == false
                                ? Enumerable.Empty<FileInfo>()
                                : _testPackageRoot.GetDirectories()
                                                  .Select(s => s.GetFiles("*.nupkg")
                                                                .FirstOrDefault())
                           ).ToArray();
        }

        private DirectoryInfo FindNearestPackagesFolder()
        {
            string packageLocation = @"..\..\..\..\..\packages";
            return new DirectoryInfo(packageLocation);

            // TODO: locate source folder (via stacktrace?) and scan upwards to the solution packages folder
            var directoryToSearch = ExecutingDirectory;
            while (directoryToSearch.Exists)
            {
                var packagesDirectory = directoryToSearch.GetDirectories("packages").FirstOrDefault();

                if (packagesDirectory != null) return packagesDirectory;

                directoryToSearch = directoryToSearch.Parent;
            }

            return null;
        }

        public void Cleanup()
        {
            // TODO
        }
    }
}