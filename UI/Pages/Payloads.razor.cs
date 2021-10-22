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
        private OrbitalHttpClient OrbitalHttpClient { get; set; }

        private List<PayloadsPageTab> Tabs { get; set; } = new List<PayloadsPageTab>();

        private bool IsSpellsDialogOpen { get; set; } = false;

        private Payload SelectedPayload { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SetTabsData();
        }

        private static List<Payload> FilterPayloads(List<Payload> payloads, PayloadType type)
        {
            return payloads == null ? new List<Payload>() : payloads.FindAll(p => p.PayloadType == type);
        }

        private async Task SetTabsData()
        {
            List<PayloadsPageTab> tabs = new();

            foreach (PayloadType payloadType in Enum.GetValues(typeof(PayloadType)))
            {
                var tab = new PayloadsPageTab();
                var payloads = await OrbitalHttpClient.GetResourceListFromOrbital<List<Payload>>("Payloads/");
                tab.PayloadsToDisplay = FilterPayloads(payloads, payloadType);
                tab.Label = payloadType switch
                {
                    PayloadType.NativeExecutable => "Native executables",
                    PayloadType.NativeLibrary => "Native libraries",
                    PayloadType.AssemblyExecutable => "Assembly executables",
                    PayloadType.AssemblyLibrary => "Assembly libraries",
                    _ => "Others"
                };

                tabs.Add(tab);
            }
            Tabs = tabs;
        }

        private void CloseSpellDialog()
        {
            IsSpellsDialogOpen = false;
        }

        private void OpenSpellDialog(Payload payload)
        {
            SelectedPayload = payload;
            IsSpellsDialogOpen = true;
        }
    }


    public class PayloadsPageTab
    {
        public string Label { get; set; }

        public List<Payload> PayloadsToDisplay { get; set; }
    }

}
