using Hosomi.NAL.AddOn.Core.DAO.Resources;
using Hosomi.NAL.AddOn.Core.Model;
using SAPbobsCOM;
using SBO.Hub;
using SBO.Hub.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hosomi.NAL.AddOn.Core.BLL
{
    public class InvoiceBLL
    {
        public double ValidateQuantity(string itemCode)
        {
            try
            {
                return Convert.ToDouble(CrudDAO.ExecuteScalar(String.Format(SQL.AtoConcessorio_GetQuantity, itemCode)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AddAtoConcessorio(int docEntry)
        {
            Documents invoice = SBOApp.Company.GetBusinessObject(BoObjectTypes.oInvoices);
            bool hasChanges = false;
            List<AtoConcessorioModel> updateList = new List<AtoConcessorioModel>();

            try
            {
                invoice.GetByKey(docEntry);
                int originalRowCount = invoice.Lines.Count;

                for (int i = 0; i < originalRowCount; i++)
                {
                    invoice.Lines.SetCurrentLine(i);
                    if (GlobalsBLL.UsageDrawback != invoice.Lines.Usage)
                    {
                        continue;
                    }

                    List<AtoConcessorioModel> atoList = new CrudDAO().FillModelListFromSql<AtoConcessorioModel>(String.Format(SQL.AtoConcessorio_Get, invoice.Lines.ItemCode));

                    if (atoList.Count > 0)
                    {
                        hasChanges = true;
                        int index = 0;
                        // Se não tem quantidade suficiente no ato concessório, não faz nada
                        if (atoList.Sum(m => m.Quantidade) < invoice.Lines.Quantity)
                        {
                            continue;
                        }
                        double quantity = invoice.Lines.Quantity;

                        string itemCode = "";
                        string usage = "";
                        double price = 0;
                        string taxCode = "";
                        string supplierCatNum = "";

                        if (quantity > atoList[index].Quantidade)
                        {
                            while (quantity > atoList[index].Quantidade)
                            {
                                if (!String.IsNullOrEmpty(itemCode))
                                {
                                    invoice.Lines.ItemCode = itemCode;
                                    invoice.Lines.Usage = usage;
                                    invoice.Lines.Price = price;
                                    invoice.Lines.TaxCode = taxCode;
                                    invoice.Lines.ItemDescription = supplierCatNum + " - " + invoice.Lines.ItemDescription;
                                }

                                invoice.Lines.Quantity = atoList[index].Quantidade;
                                invoice.Lines.FreeText = atoList[index].AtoConcessorio;

                                quantity -= atoList[index].Quantidade;
                                atoList[index].QtdeUtilizada = atoList[index].Quantidade;
                                updateList.Add(atoList[index]);

                                itemCode = invoice.Lines.ItemCode;
                                usage = invoice.Lines.Usage;
                                price = invoice.Lines.Price;
                                taxCode = invoice.Lines.TaxCode;
                                supplierCatNum = invoice.Lines.SupplierCatNum;

                                invoice.Lines.Add();
                                if (quantity > atoList[index].Quantidade)
                                {
                                    index++;
                                }
                            }

                            invoice.Lines.ItemCode = itemCode;
                            invoice.Lines.Usage = usage;
                            invoice.Lines.Price = price;
                            invoice.Lines.TaxCode = taxCode;
                            invoice.Lines.FreeText = atoList[index].AtoConcessorio;
                            invoice.Lines.ItemDescription = supplierCatNum + " - " + invoice.Lines.ItemDescription;

                            invoice.Lines.Quantity = quantity;

                            atoList[index].QtdeUtilizada = atoList[index].Quantidade - quantity;
                            updateList.Add(atoList[index]);
                        }
                        else
                        {
                            invoice.Lines.FreeText = atoList[index].AtoConcessorio;
                            atoList[index].QtdeUtilizada = atoList[index].Quantidade - quantity;
                            updateList.Add(atoList[index]);
                        }
                    }
                }
                if (hasChanges)
                {
                    if (invoice.Update() != 0)
                    {
                        throw new Exception("DI API - " + SBOApp.Company.GetLastErrorDescription());
                    }
                    else
                    {
                        foreach (var atoModel in updateList.Where(m => m.QtdeUtilizada != 0))
                        {
                            CrudB1DAO.ExecuteNonQuery(String.Format(SQL.AtoConcessorio_Update, atoModel.QtdeUtilizada.ToString().Replace(",", "."), atoModel.Code, atoModel.LineId));
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "Erro ao setar Ato Concessório: " + ex.Message;
            }
            finally
            {
                Marshal.ReleaseComObject(invoice);
                invoice = null;
            }
        }

        public string UpdateItemDescriptionAtoConcessorio(int docEntry)
        {         
            try
            {
                CrudDAO.ExecuteNonQuery(String.Format(SQL.Invoice_UpdateItemDescription, docEntry));
                return "";
            }
            catch (Exception ex)
            {
                return "Erro ao setar atualizar descrição do item Concessório: " + ex.Message;
            }
        }
    }
}
