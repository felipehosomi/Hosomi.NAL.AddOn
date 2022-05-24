using SBO.Hub;
using System;
using System.Windows.Forms;

namespace Hosomi.NAL.AddOn.Core.BLL
{
    public class InitializeBLL
    {
        public static void Initialize()
        {
            UserFieldsBLL.CreateUserFields();
            EventFilterBLL.SetDefaultEvents();
            GlobalsBLL.GetGlobals();
            try
            {
                SBO.Hub.Helpers.MenuHelper.LoadFromXML($"{Application.StartupPath}\\Menu\\Menu.xml");
            }
            catch (Exception ex)
            {
                SBOApp.Application.SetStatusBarMessage($"Erro ao criar menu: {ex.Message}");
            }
        }
    }
}
