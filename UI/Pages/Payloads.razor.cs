using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Shared.Enums;
using Shared.Dtos;
using Shared.Static;
using Ui.Services;
using System.Net.Http;
using MatBlazor;

namespace Ui.Pages
{
    public partial class Payloads
    {
        [Inject]
        private IMatToaster Toaster { get; set; }

        [Inject]
        private RodinHttpClient RodinHttpClient { get; set; }

        private List<PayloadsPageTab> Tabs { get; set; } = new List<PayloadsPageTab>();

        private bool IsSpellsDialogOpen { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await SetTabsData();
        }


        private async Task<List<Payload>> GetPayloads()
        {
            var endpoint = "Payloads/";
            var payloads = new List<Payload>();
            var rodinClient = RodinHttpClient.Client;
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await rodinClient.GetAsync(endpoint);
            }
            catch (Exception ex)
            {
                Toaster.Add(ex.Message, MatToastType.Danger);
                return payloads;
            }

            if (!response.IsSuccessStatusCode)
            {
                await RodinHttpClient.ShowAndLogError(response);
            }

            using Stream responseStream = await response.Content.ReadAsStreamAsync();

            payloads = await JsonHelper.DeserializeAsync<List<Payload>>(responseStream);


            return payloads;
        }

        private List<Payload> FilterPayloads(List<Payload> payloads, PayloadType type)
        {
            if (payloads == null) return new List<Payload>();
            return payloads.FindAll(p => p.PayloadType == type);
        }

        private async Task SetTabsData()
        {
            List<PayloadsPageTab> tabs = new List<PayloadsPageTab>();

            foreach (PayloadType payloadType in Enum.GetValues(typeof(PayloadType)))
            {
                var tab = new PayloadsPageTab();
                var payloads = await GetPayloads();
                tab.PayloadsToDisplay = FilterPayloads(payloads, payloadType);
                switch (payloadType)
                {
                    case PayloadType.kNativeExecutable:
                        tab.Label = "Native executables";
                        break;
                    case PayloadType.kNativeLibrary:
                        tab.Label = "Native libraries";
                        break;
                    case PayloadType.kAssemblyExecutable:
                        tab.Label = "Assembly executables";
                        break;
                    case PayloadType.kAssemblyLibrary:
                        tab.Label = "Assembly libraries";
                        break;
                    default:
                        break;
                }

                tabs.Add(tab);
            }
            Tabs = tabs;
        }

        private void CloseSpellDialog()
        {
            IsSpellsDialogOpen = false;
        }
    }


    public class PayloadsPageTab
    {
        public string Label { get; set; }

        public List<Payload> PayloadsToDisplay { get; set; }
    }

}
