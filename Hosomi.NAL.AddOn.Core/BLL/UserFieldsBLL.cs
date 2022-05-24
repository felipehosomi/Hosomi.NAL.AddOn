using SBO.Hub.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hosomi.NAL.AddOn.Core.BLL
{
    class UserFieldsBLL
    {
        public static void CreateUserFields()
        {
            UserObject userObject = new UserObject();
            userObject.CreateUserTable("NAL_ATOCONC_CONFIG", "Ato concessório - Config", SAPbobsCOM.BoUTBTableType.bott_NoObject);
            userObject.InsertUserField("@NAL_ATOCONC_CONFIG", "Usage", "Cód. Utilização", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10);

            userObject.CreateUserTable("NAL_ATOCONC", "Ato concessório", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            userObject.InsertUserField("@NAL_ATOCONC", "ItemCode", "Cód. Item", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 60);
            userObject.InsertUserField("@NAL_ATOCONC", "ItemName", "Desc. Item", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 254);
            userObject.InsertUserField("@NAL_ATOCONC", "BPLId", "Filial", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 3);

            userObject.CreateUserTable("NAL_ATOCONC1", "Ato concessório - Linha", SAPbobsCOM.BoUTBTableType.bott_MasterDataLines);
            userObject.InsertUserField("@NAL_ATOCONC1", "AtoConc", "Ato concessório", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 254);
            userObject.InsertUserField("@NAL_ATOCONC1", "Obs", "Observação", SAPbobsCOM.BoFieldTypes.db_Memo, SAPbobsCOM.BoFldSubTypes.st_None, 1000);
            userObject.InsertUserField("@NAL_ATOCONC1", "DataValidade", "Valido Até", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10);
            userObject.InsertUserField("@NAL_ATOCONC1", "Quantidade", "Quantidade", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10);
            userObject.InsertUserField("@NAL_ATOCONC1", "QtdeUtil", "Qtde. Utilizada", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10);
            userObject.InsertUserField("@NAL_ATOCONC1", "Ativo", "Ativo", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, true, "N");

            userObject.CreateUserObject("NAL_ATOCONC", "Ato concessório", "NAL_ATOCONC", SAPbobsCOM.BoUDOObjType.boud_MasterData, true, false, true);
            userObject.AddChildTableToUserObject("NAL_ATOCONC", "NAL_ATOCONC1");
        }
    }
}

