using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace CommerceBuilder.Taxes.Providers.AvaTax
{
    public class AvalaraSecurityHeaderBehavior : IEndpointBehavior
    {
        string _username;
        string _password;

        public AvalaraSecurityHeaderBehavior(string username, string password)
        {
            _username = username;
            _password = password;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        { }

        void IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MessageInspector(_username, _password));
        }
        void IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        { }

        void IEndpointBehavior.Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
        { }

    }
}
