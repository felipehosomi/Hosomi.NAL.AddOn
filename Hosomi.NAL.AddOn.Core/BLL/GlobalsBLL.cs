using Hosomi.NAL.AddOn.Core.DAO.Resources;
using SBO.Hub;
using SBO.Hub.DAO;
using System;

namespace Hosomi.NAL.AddOn.Core.BLL
{
    public class GlobalsBLL
    {
        public static string UsageDrawback;

        public static void GetGlobals()
        {
            try
            {
                UsageDrawback = CrudDAO.ExecuteScalar(SQL.AtoConcessorioConfig_Get).ToString();
                if (UsageDrawback == "")
                {
                    SBOApp.Application.SetStatusBarMessage("ID Utilização Drawback não encontrada, por favor verifique a tabela @NAL_ATOCONC_CONFIG");
                }
            }
            catch
            {
                SBOApp.Application.SetStatusBarMessage("ID Utilização Drawback não encontrada, por favor verifique a tabela @NAL_ATOCONC_CONFIG");
            }
        }
    }
}
