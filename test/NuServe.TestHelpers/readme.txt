

Thanks for downloading StronglyTypedContext, I hope the library is useful.  For more info check out:

github:  https://github.com/kevholditch/StronglyTypedContext
blog:    http://kevinholditch.co.uk/2013/12/10/strongly-typed-scenariocontext-in-specflow/

Instructions:

1) Derive your step definition class (the one with the [Binding] attribute) from the StronglyTypedContext.BaseBinding abstract class
2) Create an interface to hold your context
3) Create a public virtual property for the interface you created in step 2
4) Mark the property with the [ScenarioContext] attribute

That's it you are good to go.  The property will no automatically be proxied into the ScenarioContext for you.