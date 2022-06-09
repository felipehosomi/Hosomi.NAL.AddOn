using SAPbouiCOM;
using SBO.Hub;
using SBO.Hub.DAO;
using SBO.Hub.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBO.Hub.UI;
using Hosomi.NAL.AddOn.Core.DAO.Resources;

namespace Hosomi.NAL.AddOn.Core.Forms
{
    public class FrmAtoConcessorio : BaseForm
    {
        private Form Form;

        public FrmAtoConcessorio()
        {
            FormCount++;
        }

        public FrmAtoConcessorio(ItemEvent itemEventInfo)
        {
            this.ItemEventInfo = itemEventInfo;
        }

        public FrmAtoConcessorio(BusinessObjectInfo businessObject)
        {
            this.BusinessObjectInfo = businessObject;
        }

        public FrmAtoConcessorio(MenuEvent menuEvent)
        {
            this.MenuEventInfo = menuEvent;
        }

        public override object Show()
        {
            Form form = (Form)base.Show();
            form.EnableMenu("1292", true);
            form.EnableMenu("1293", true);

            ComboBox cb_Branch = form.Items.Item("cb_Branch").Specific;
            cb_Branch.AddValuesFromQuery(SQL.Filial_Get);

            AddRow(false);

            return form;
        }

        public override bool ItemEvent()
        {
            if (!ItemEventInfo.BeforeAction)
            {
                Form = SBOApp.Application.Forms.GetForm(ItemEventInfo.FormTypeEx, ItemEventInfo.FormTypeCount);
                if (ItemEventInfo.EventType == BoEventTypes.et_VALIDATE)
                {
                    if (ItemEventInfo.ItemUID == "mt_Line" && ItemEventInfo.ColUID == "cl_Ato")
                    {
                        Matrix mt_Line = Form.Items.Item("mt_Line").Specific;
                        if (mt_Line.RowCount == ItemEventInfo.Row)
                        {
                            EditText et_Ato = mt_Line.GetCellSpecific("cl_Ato", ItemEventInfo.Row);
                            if (!String.IsNullOrEmpty(et_Ato.Value))
                            {
                                this.AddRow(false);
                            }
                        }
                    }
                }
                if (ItemEventInfo.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    if (ItemEventInfo.ItemUID == "et_ItemC")
                    {
                        var cflEvent = (IChooseFromListEvent)ItemEventInfo;
                        var cflId = cflEvent.ChooseFromListUID;

                        var dataTable = cflEvent.SelectedObjects;
                        if (dataTable != null)
                        {
                            Form.DataSources.DBDataSources.Item("@NAL_ATOCONC").SetValue("U_ItemName", 0, dataTable.GetValue("ItemName", 0).ToString());
                        }
                    }
                }
            }
            return true;
        }

        public override bool FormDataEvent()
        {
            Form = SBOApp.Application.Forms.Item(BusinessObjectInfo.FormUID);
            if (BusinessObjectInfo.BeforeAction)
            {
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                {
                    Form.DataSources.DBDataSources.Item("@NAL_ATOCONC").SetValue("Code", 0, CrudDAO.GetNextCode("@NAL_ATOCONC").PadLeft(8, '0'));
                }
            }
            else
            {
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD)
                {
                    DBDataSource dt_Line = Form.DataSources.DBDataSources.Item("@NAL_ATOCONC1");
                    if (!String.IsNullOrEmpty(dt_Line.GetValue("U_AtoConc", dt_Line.Size - 1).Trim()))
                    {
                        this.AddRow();
                    }
                }
            }

            return true;
        }


        public override bool MenuRemove()
        {
            if (MenuEventInfo.BeforeAction)
            {
                Form = SBOApp.Application.Forms.ActiveForm;
                DBDataSource dt_Line = Form.DataSources.DBDataSources.Item("@NAL_ATOCONC1");

                for (int i = 0; i < dt_Line.Size; i++)
                {
                    double usedQty;
                    double.TryParse(dt_Line.GetValue("U_QtdeUtil", i), out usedQty);

                    if (usedQty > 0)
                    {
                        SBOApp.Application.SetStatusBarMessage("Impossível remover, já existem linhas quantidade utilizada");
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool MenuAddRow()
        {
            if (!MenuEventInfo.BeforeAction)
            {
                this.AddRow();
            }

            return true;
        }

        public override bool MenuRemoveRow()
        {
            if (MenuEventInfo.BeforeAction)
            {
                Form = SBOApp.Application.Forms.ActiveForm;
                Matrix mt_Line = (Matrix)Form.Items.Item("mt_Line").Specific;

                int selectedRow = mt_Line.GetNextSelectedRow();
                if (mt_Line.GetNextSelectedRow() == -1)
                {
                    CellPosition cellPosition = mt_Line.GetCellFocus();
                    if (cellPosition != null)
                    {
                        selectedRow = cellPosition.rowIndex;
                    }
                }

                if (selectedRow != -1)
                {
                    DBDataSource dt_Line = Form.DataSources.DBDataSources.Item("@NAL_ATOCONC1");
                    double usedQty;
                    double.TryParse(dt_Line.GetValue("U_QtdeUtil", selectedRow - 1), out usedQty);

                    if (usedQty > 0)
                    {
                        SBOApp.Application.SetStatusBarMessage("Impossível remover linha, já existe quantidade utilizada");
                    }
                    else
                    {
                        mt_Line.DeleteRow(selectedRow);
                        mt_Line.FlushToDataSource();
                        if (Form.Mode == BoFormMode.fm_OK_MODE)
                        {
                            Form.Mode = BoFormMode.fm_UPDATE_MODE;
                        }
                    }
                }
            }

            return false;
        }

        public override bool MenuAdd()
        {
            if (!MenuEventInfo.BeforeAction)
            {
                this.AddRow();
            }

            return true;
        }

        private void AddRow(bool setFocus = true)
        {
            Form = SBOApp.Application.Forms.ActiveForm;
            DBDataSource dt_Line = Form.DataSources.DBDataSources.Item("@NAL_ATOCONC1");
            dt_Line.Clear();

            Matrix mt_Line = (Matrix)Form.Items.Item("mt_Line").Specific;
            mt_Line.AddRow();

            CheckBox cb_Ativo = mt_Line.GetCellSpecific("cb_Ativo", mt_Line.RowCount);
            cb_Ativo.Checked = true;

            if (setFocus)
            {
                mt_Line.SetCellFocus(mt_Line.RowCount, 1);
            }
        }
    }
}
