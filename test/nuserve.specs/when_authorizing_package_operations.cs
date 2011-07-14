using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using developwithpassion.specifications.nsubstitue;
using nuserve.Infrastructure;
using Machine.Specifications;
using nuserve.Settings;

namespace nuserve.specs
{
    public class when_authorizing_a_publish_with_a_valid_apikey : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("topSecretKey42", "somePackage");

        It should_authorize = () => canPublishPackage.ShouldBeTrue();
    }

    public class when_authorizing_a_publish_with_an_invalid_apikey : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("firstattempt", "somePackage");

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_empty_apikey : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("", "somePackage");

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_empty_package_name : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("topSecretKey42", "");

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_a_whitespace_apikey : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("  ", "somePackage");

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_a_whitespace_package_name : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("topSecretKey42", "  ");

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_null_apikey : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage(null, "somePackage");

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_null_package_name : Observes<IAuthorizePackageOperations, AuthorizePackageOperations>
    {
        static bool canPublishPackage;

        Establish context = () =>
        {
            var settings = new ApiSettings { ApiKey = "topSecretKey42" };
            sut_factory.create_using(() => new AuthorizePackageOperations(settings));
        };

        Because of = () => canPublishPackage = sut.ClientCanPublishPackage("topSecretKey42", null);

        It should_not_authorize = () => canPublishPackage.ShouldBeFalse();
    }
}
