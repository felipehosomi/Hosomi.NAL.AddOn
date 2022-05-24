using Hosomi.NAL.AddOn.Core.BLL;
using Hosomi.NAL.AddOn.Core.DAO.Resources;
using Hosomi.NAL.AddOn.Core.Model;
using SAPbouiCOM;
using SBO.Hub;
using SBO.Hub.Attributes;
using SBO.Hub.DAO;
using SBO.Hub.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hosomi.NAL.AddOn.Core.Forms
{
    [Form("133")]
    class FrmInvoice : SystemForm
    {
        private static List<AtoConcessorioModel> UpdateList = new List<AtoConcessorioModel>();
        //private static bool AtoCalculated = false;

        public FrmInvoice(ItemEvent itemEventInfo)
        {
            this.ItemEventInfo = itemEventInfo;
        }

        public FrmInvoice(BusinessObjectInfo businessObject)
        {
            this.BusinessObjectInfo = businessObject;
        }

        public override bool ItemEvent()
        {
            if (ItemEventInfo.BeforeAction && ItemEventInfo.EventType == BoEventTypes.et_ITEM_PRESSED && ItemEventInfo.ItemUID == "1")
            {
                try
                {
                    Form.Freeze(true);
                    UpdateList = new List<AtoConcessorioModel>();
                    DBDataSource ds_Line = Form.DataSources.DBDataSources.Item("INV1");
                    int originalRowCount = ds_Line.Size;
                    List<AtoConcessorioModel> atoList = new List<AtoConcessorioModel>();

                    for (int i = 0; i < originalRowCount; i++)
                    {
                        if (GlobalsBLL.UsageDrawback == ds_Line.GetValue("Usage", i))
                        {
                            Matrix mt_Line = Form.Items.Item("38").Specific;
                            
                            string itemCode = ds_Line.GetValue("ItemCode", i);
                            double quantity = Convert.ToDouble(ds_Line.GetValue("Quantity", i).Replace(".", ","));

                            if (!atoList.Any(m => m.ItemCode == itemCode))
                            {
                                atoList.AddRange(new CrudDAO().FillModelListFromSql<AtoConcessorioModel>(String.Format(SQL.AtoConcessorio_Get, itemCode)));
                            }

                            List<AtoConcessorioModel>  atoListItem = new List<AtoConcessorioModel>();
                            atoListItem = atoList.Where(m => m.ItemCode == itemCode).ToList();

                            if (atoListItem.Count > 0)
                            {
                                if (atoListItem.Sum(m => m.Quantidade) < quantity)
                                {
                                    int msgResult = SBOApp.Application.MessageBox($"Saldo insuficiente no ato concessório para o item {itemCode}. Deseja continuar?", 1, "Sim", "Não");
                                    if (msgResult != 1)
                                    {
                                        for (int j = 0; j < mt_Line.RowCount; j++)
                                        {
                                            ((EditText)mt_Line.GetCellSpecific("163", j + 1)).Value = "";
                                        }

                                        return false;
                                    }
                                }
                                else
                                {
                                    atoListItem = atoListItem.Where(m => m.Quantidade > 0).ToList();

                                    int index = 0;
                                    if (quantity > atoListItem[index].Quantidade)
                                    {
                                        while (quantity > atoListItem[index].Quantidade)
                                        {
                                            ((EditText)mt_Line.GetCellSpecific("11", i + 1)).Value = atoListItem[index].Quantidade.ToString();
                                            ((EditText)mt_Line.GetCellSpecific("163", i + 1)).Value = atoListItem[index].AtoConcessorio;
                                            quantity -= atoListItem[index].Quantidade;
                                            atoListItem[index].QtdeUtilizada += atoListItem[index].Quantidade;
                                            atoListItem[index].Quantidade = 0;
                                            UpdateList.Add(atoListItem[index]);

                                            //mt_Line.SelectRow(i + 1, true, false);
                                            mt_Line.Columns.Item("0").Cells.Item(i + 1).Click();
                                            //Form.EnableMenu("1287", true);
                                            //SBOApp.Application.Menus.Item("1287").Activate();
                                            SBOApp.Application.ActivateMenuItem("1294");
                                            if (quantity > atoListItem[index].Quantidade)
                                            {
                                                index++;
                                            }
                                            i++;
                                        }

                                        ((EditText)mt_Line.GetCellSpecific("11", i + 1)).Value = quantity.ToString();
                                        ((EditText)mt_Line.GetCellSpecific("163", i + 1)).Value = atoListItem[index].AtoConcessorio;

                                        atoListItem[index].QtdeUtilizada += quantity;
                                        atoListItem[index].Quantidade -= quantity;
                                        UpdateList.Add(atoListItem[index]);
                                    }
                                    else
                                    {
                                        ((EditText)mt_Line.GetCellSpecific("163", i + 1)).Value = atoListItem[index].AtoConcessorio;
                                        atoListItem[index].QtdeUtilizada += quantity;
                                        atoListItem[index].Quantidade -= quantity;
                                        UpdateList.Add(atoListItem[index]);
                                    }
                                }
                            }
                        }
                    }
                    //AtoCalculated = true;
                }
                catch (Exception ex)
                {
                    SBOApp.Application.SetStatusBarMessage(ex.Message);
                }
                finally
                {
                    Form.Freeze(false);
                }
            }
            else if (ItemEventInfo.BeforeAction && ItemEventInfo.EventType == BoEventTypes.et_FORM_LOAD)
            {
                UpdateList = new List<AtoConcessorioModel>();
            }
            return true;
        }

        public override bool FormDataEvent()
        {

            if (!BusinessObjectInfo.BeforeAction)
            {
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                {
                    if (BusinessObjectInfo.ActionSuccess)
                    {
                        foreach (var atoModel in UpdateList)
                        {
                            CrudB1DAO.ExecuteNonQuery(String.Format(SQL.AtoConcessorio_Update, atoModel.QtdeUtilizada.ToString().Replace(",", "."), atoModel.Code, atoModel.LineId));
                        }
                        UpdateList = new List<AtoConcessorioModel>();

                        InvoiceBLL invoiceBLL = new InvoiceBLL();
                        DBDataSource ds_OINV = Form.DataSources.DBDataSources.Item("OINV");
                        string docEntry = ds_OINV.GetValue("DocEntry", ds_OINV.Offset);
                        string error = invoiceBLL.UpdateItemDescriptionAtoConcessorio(Convert.ToInt32(docEntry));
                        if (!String.IsNullOrEmpty(error))
                        {
                            SBOApp.Application.MessageBox(error);
                        }
                    }
                }
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD)
                {
                    UpdateList = new List<AtoConcessorioModel>();
                }
            }

            return true;
        }
    }
}
