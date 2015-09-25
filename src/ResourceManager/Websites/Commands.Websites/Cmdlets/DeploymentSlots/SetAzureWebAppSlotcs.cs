﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.WindowsAzure.Management.Models;

namespace Microsoft.Azure.Commands.WebApps.Cmdlets
{
    /// <summary>
    /// this commandlet will let you create a new Azure Web app using ARM APIs
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureRMWebAppSlot")]
    public class SetAzureWebAppSlotCmdlet : WebAppSlotBaseCmdlet
    {
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The name of the app service plan eg: Default1.")]
        public string AppServicePlan { get; set; }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Default documents for web app")]
        [ValidateNotNullOrEmpty]
        public string[] DefaultDocuments { get; set; }

        [Parameter(Position = 5, Mandatory = false, HelpMessage = ".NET Framework version")]
        [ValidateNotNullOrEmpty]
        public string NetFrameworkVersion { get; set; }

        [Parameter(Position = 6, Mandatory = false, HelpMessage = "PHP version")]
        [ValidateNotNullOrEmpty]
        public string PhpVersion { get; set; }

        [Parameter(Position = 7, Mandatory = false, HelpMessage = "Whether or not request tracing is enabled")]
        [ValidateNotNullOrEmpty]
        public bool RequestTracingEnabled { get; set; }

        [Parameter(Position = 8, Mandatory = false, HelpMessage = "Whether or not http logging is enabled")]
        [ValidateNotNullOrEmpty]
        public bool HttpLoggingEnabled { get; set; }

        [Parameter(Position = 9, Mandatory = false, HelpMessage = "Whether or not detailed error logging is enabled")]
        [ValidateNotNullOrEmpty]
        public bool DetailedErrorLoggingEnabled { get; set; }

        [Parameter(Position = 10, Mandatory = false, HelpMessage = "Web app settings")]
        [ValidateNotNullOrEmpty]
        public IDictionary<string, string> AppSettings { get; set; }

        [Parameter(Position = 11, Mandatory = false, HelpMessage = "Web app connection strings")]
        [ValidateNotNullOrEmpty]
        public IDictionary<string, ConnStringValueTypePair> ConnectionStrings { get; set; }

        [Parameter(Position = 12, Mandatory = false, HelpMessage = "Web app handler mappings")]
        [ValidateNotNullOrEmpty]
        public IList<HandlerMapping> HandlerMappings { get; set; }

        [Parameter(Position = 13, Mandatory = false, HelpMessage = "Web app managed pipeline mode. Allowed Values [Classic|Integrated]")]
        [ValidateSet("Classic", "Integrated")]
        public string ManagedPipelineMode { get; set; }

        [Parameter(Position = 14, Mandatory = false, HelpMessage = "Whether or not detailed error logging is enabled")]
        [ValidateNotNullOrEmpty]
        public bool WebSocketsEnabled { get; set; }

        [Parameter(Position = 15, Mandatory = false, HelpMessage = "Whether or not to use 32-bit worker process. By default worker process is 64-bit")]
        [ValidateNotNullOrEmpty]
        public bool Use32BitWorkerProcess { get; set; }

        protected override void ProcessRecord()
        {
            var parameters = new HashSet<string>(MyInvocation.BoundParameters.Keys, StringComparer.OrdinalIgnoreCase);
            var siteConfig = new SiteConfig
            {
                DefaultDocuments = parameters.Contains("DefaultDocuments") ? DefaultDocuments : null,
                NetFrameworkVersion = parameters.Contains("NetFrameworkVersion") ? NetFrameworkVersion : null,
                PhpVersion = parameters.Contains("PhpVersion") ? PhpVersion : null,
                RequestTracingEnabled = parameters.Contains("RequestTracingEnabled") ? (bool?)RequestTracingEnabled : null,
                HttpLoggingEnabled = parameters.Contains("HttpLoggingEnabled") ? (bool?)HttpLoggingEnabled : null,
                DetailedErrorLoggingEnabled = parameters.Contains("DetailedErrorLoggingEnabled") ? (bool?)DetailedErrorLoggingEnabled : null,
                HandlerMappings = parameters.Contains("HandlerMappings") ? HandlerMappings : null,
                ManagedPipelineMode = parameters.Contains("ManagedPipelineMode") ? (ManagedPipelineMode?)Enum.Parse(typeof(ManagedPipelineMode), ManagedPipelineMode) : null,
                WebSocketsEnabled = parameters.Contains("WebSocketsEnabled") ? (bool?)WebSocketsEnabled : null,
                Use32BitWorkerProcess = parameters.Contains("Use32BitWorkerProcess") ? (bool?)Use32BitWorkerProcess : null
            };

            // Update web app slot configuration
            WebsitesClient.UpdateWebAppConfiguration(ResourceGroupName, Name, Slot, siteConfig, AppSettings, ConnectionStrings);

            if (parameters.Contains("AppServicePlan"))
            {
                WebsitesClient.UpdateWebApp(ResourceGroupName, Name, Slot, AppServicePlan);
            }

            WriteObject(WebsitesClient.GetWebApp(ResourceGroupName, Name, Slot));
        }
    }
}