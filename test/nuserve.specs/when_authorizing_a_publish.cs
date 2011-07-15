using System;
using Machine.Specifications;
using nuserve.Settings;

namespace nuserve.specs
{
    public class when_authorizing_a_publish_with_a_valid_apikey : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("topSecretKey42", "somePackage");

        It should_authorize = () => result.ShouldBeTrue();
    }

    public class when_authorizing_a_publish_with_a_valid_apikey_of_different_casing : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("tOpsecREtkeY42", "somePackage");

        It should_ignore_case = () => result.ShouldBeTrue();
    }

    public class when_authorizing_a_publish_with_an_invalid_apikey : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("firstattempt", "somePackage");

        It should_not_authorize = () => result.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_empty_apikey : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("", "somePackage");

        It should_not_authorize = () => result.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_empty_package_name : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("topSecretKey42", "");

        It should_not_authorize = () => result.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_a_whitespace_apikey : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("  ", "somePackage");

        It should_not_authorize = () => result.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_a_whitespace_package_name : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("topSecretKey42", "  ");

        It should_not_authorize = () => result.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_null_apikey : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage(null, "somePackage");

        It should_not_authorize = () => result.ShouldBeFalse();
    }

    public class when_authorizing_a_publish_with_an_null_package_name : AuthorizePackageOperations_Specs
    {
        Establish context = () => settings = new ApiSettings { ApiKey = "topSecretKey42" };

        Because of = () => result = sut.ClientCanPublishPackage("topSecretKey42", null);

        It should_not_authorize = () => result.ShouldBeFalse();
    }
}
