using System;
using developwithpassion.specifications.nsubstitue;
using Machine.Specifications;
using Should;
using nuserve.Infrastructure;
using nuserve.Infrastructure.Implementation;
using nuserve.Settings;

namespace nuserve.specs
{
    public abstract class AuthorizePackageOperations_Specs : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        protected static ApiSettings settings;
        protected static bool result;

        Establish context = () =>
        {
            sut_factory.create_using(() =>
            {
                settings.ShouldNotBeNull("Did you remember to populate the 'settings' field with a new ApiSettings object in your Establish clause?");

                return new AuthorizePackageOperations(settings);
            });
        };
    }
}