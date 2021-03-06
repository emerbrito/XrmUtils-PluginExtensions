Dynamics 365 Plugin Extensions
==============================

[![Build status](https://ci.appveyor.com/api/projects/status/2tmf1yi6ioi93n0h?svg=true)](https://ci.appveyor.com/project/emerbrito/xrmutils-pluginextensions)
[![NuGet](https://img.shields.io/nuget/v/XrmUtils.CrmSdk.PluginExtensions.svg)](http://www.nuget.org/packages/XrmUtils.CrmSdk.PluginExtensions)

Base classes for Dynamics 365 plugins and workflows. Wraps boilerplate code and offers a series of extension methods to streamline plugin and custom activities development.

Get it from Nuget:
```
    PM> Install-Package XrmUtils.CrmSdk.PluginExtensions 
```

Documentation
----------------

Documentation will be provide soon.
Bellow is a summary commonly used functionality.

Plugin Base Class
-----------------

When creating a plugin, inherit from `PluginBase` instead of implementing the `IPlugin` interface from the SDK.

In this example, the plugin class is decorated with a series of attributes that not only self-documents the plugin registration but are used at runtime to validate the registration. If registration is different than expected, an `InvalidPluginExecutionException` will be thrown before the `Execute` method is reached.

The `LocalPluginContext` passed as an argument to the `Execute` method has all the services you would normally use in a plugin (such as `ITracingService` and `IOrganizationService`).

```csharp
    using XrmUtils.Extensions;
    using XrmUtils.Extensions.Plugins;

    [Message("update")]
    [PrimaryEntity("account")]    
    [ExecutionMode(ExecutionMode.Asynchronous)]
    [Stage(PipelineStage.PostOperation)]
    public class TrackCreditLimit : PluginBase
    {
        protected override void Execute(LocalPluginContext localContext)
        {
            // your implementation here
        }

    }
```

A fully implemented plugin, note the `Execute`method only has the necessary business logic:

```csharp
    [Message("update")]
    [PrimaryEntity("account")]    
    [ExecutionMode(ExecutionMode.Asynchronous)]
    [Stage(PipelineStage.PostOperation)]
    public class TrackCreditLimit : PluginBase
    {
        protected override void Execute(LocalPluginContext localContext)
        {
            var target = localContext.GetTargetEntity();
            var image = localContext.GetPreImage("MyImage", throwIfNull: true);

            if(target.AttributeHasChanged("creditlimit", image))
            {
                Entity oldLimit;

                localContext.Trace("Credit limit has changed. Creating audit log.");

                oldLimit = new Entity("creditlimittracking");
                oldLimit.AddOrUpdateAttribute("limit", image.GetAttributeValue<Money>("creditlimit"));

                localContext.Trace("Creating new record.");
                localContext.OrganizationService.Create(oldLimit);
            }
            else
            {
                localContext.Trace("Credit limit didn't change.");
            }
        }

    }
```


### Injecting your own Tracing Service

In the above examples, all calls to `localContext.Trace` are routed to the default instance of Dynamics 365 `ITracingService`.
There may be an instance where you want to wrap the tracing service so you can le's say, dump additional information on a custom entity.

You can inject yout own service by overriding the base class method `CreateTracingService`.
This method received the original tracing service which you can use to initialize your custom implementation if required.

```csharp
    public class TrackCreditLimit : PluginBase
    {
        protected override ITracingService CreateTracingService(ITracingService crmTracingService)
        {
            return MyCustomFactory.CreateService(crmTracingService);
        }

        protected override void Execute(LocalPluginContext localContext)
        {
            // this call you use your own tracing service.
            localContext.Trace("Using my own trace.");
        }
    }
```

Workflow Activity Base Class
----------------------------

When creating a custom workflow activity, inherit from `WorkflowActivityBase` instead of `CodeActivity`.

The `LocalWorkflowContext` passed as an argument to the `Execute` method has all the services you would normally use in a custom activity (such as `ITracingService` and `IOrganizationService`).

```csharp
    using XrmUtils.Extensions.Plugins;

    class MyCustomActivity : WorkflowActivityBase
    {
        [Input("Full name")]
        public InArgument<string> FullName { get; set; }

        [Output("Result")]
        public OutArgument<string> Result { get; set; }

        protected override void Execute(LocalWorkflowContext localContext)
        {
            var name = FullName.Get(localContext.ExecutionContext);
            var greetings = $"Hello {name}!";

            Result.Set(localContext.ExecutionContext, greetings);
        }
    }
```

License
--------
While this project os dostributed under MIT license, it may have dependencies on 3rd party software distributed under different licenses.

