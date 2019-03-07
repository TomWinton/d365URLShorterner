using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Net;
using System.IO;

namespace ShortenUrl
{

    public sealed class ShortenURL : CodeActivity
    {
        [RequiredArgument]
      
        [Input("URL")] public InArgument<string> inputURL { get; set;}               
        [Output("ShortURL")] public OutArgument<string> outputURL { get; set; }
        


        //Declare input/outputs
        //

        protected override void Execute(CodeActivityContext executionContext)
        {
            
            //Build the connection
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory =
            executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service =
            serviceFactory.CreateOrganizationService(context.UserId);
           
            if (inputURL != null) 
                 
            {
                //Build the request
                string longURL = this.inputURL.Get(executionContext).ToString();

                //Update to your URL here
                string firebaseURL = "'https://yourfirebasedynamicslinkhere.page.link/?link=" + longURL + "'";
                string message = "{'longDynamicLink':" + firebaseURL + ",'suffix': { 'option': 'SHORT'} }";
                //Send the request
                // Update to your web api secret here
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://firebasedynamiclinks.googleapis.com/v1/shortLinks?key=YOURKEYHERE);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(message);
                    streamWriter.Close();
                }
                //Read the response
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd().ToString();
                    
                    //Set the response
                    outputURL.Set(executionContext, result.Substring(18, 34));
                } 
            }
        }
    }
}
