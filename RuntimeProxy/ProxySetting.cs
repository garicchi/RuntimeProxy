using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials.UI;

namespace RuntimeProxy
{
    public static class ProxySetting
    {
        private static NetworkCredential Credential;

        public static bool IsCredential = false;

        /// <summary>
        /// プロキシ接続を構成します
        /// </summary>
        /// <param name="testUri">テスト接続用Uri</param>
        public static async Task SetProxyAsync(Uri testUri)
        {
            var config = await NetworkInformation.GetProxyConfigurationAsync(testUri);
            if (config.CanConnectDirectly==false)
            {
                NetworkCredential tempCredential = null;
                if (Credential == null)
                {
                    CredentialPickerOptions credPickerOptions = new CredentialPickerOptions();
                    credPickerOptions.Message = "認証情報を入力してください";
                    credPickerOptions.Caption = "プロキシネットワークに接続します";
                    credPickerOptions.TargetName = "target";
                    credPickerOptions.AlwaysDisplayDialog = true;
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Basic;
                    var pickResult = await CredentialPicker.PickAsync(credPickerOptions);

                    tempCredential = new NetworkCredential(pickResult.CredentialUserName, pickResult.CredentialPassword);
                    if (pickResult.CredentialSaved)
                    {
                        Credential = tempCredential;
                    }
                }
                else 
                {
                    tempCredential = Credential;
                }
                
                HttpClientHandler handler = new HttpClientHandler();
                handler.Proxy = WebRequest.DefaultWebProxy;
                handler.Proxy.Credentials = tempCredential;
                handler.UseProxy=true;

                IsCredential = true;
            }
            
        }


    }
}
