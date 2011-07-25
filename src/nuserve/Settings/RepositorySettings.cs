using System;

namespace nuserve.Settings
{
    public class RepositorySettings
    {
        private string pathToServerPackageRepository;

        public string PathToServerPackageRepository
        {
            get { return pathToServerPackageRepository; }
            set
            {
                bool newPathIsValid = String.IsNullOrWhiteSpace(value) == false;

                if (newPathIsValid)
                {
                    pathToServerPackageRepository = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the RepositorySettings class.
        /// </summary>
        public RepositorySettings()
        {
            PathToServerPackageRepository = "~/Packages";
        }
    }
}
