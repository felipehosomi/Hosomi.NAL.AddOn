using SAPbouiCOM;
using SBO.Hub.Helpers;

namespace Hosomi.NAL.AddOn.Core.BLL
{
    class EventFilterBLL
    {
        public static void SetDefaultEvents()
        {
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_CLICK);
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_VALIDATE);
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_RIGHT_CLICK);
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_FORM_DATA_ADD);
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_FORM_DATA_LOAD);
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_CHOOSE_FROM_LIST);
            EventFilterHelper.SetFormEvent("FrmAtoConcessorio", BoEventTypes.et_MENU_CLICK);

            EventFilterHelper.SetFormEvent("133", BoEventTypes.et_FORM_LOAD);
            EventFilterHelper.SetFormEvent("133", BoEventTypes.et_ITEM_PRESSED);
            EventFilterHelper.SetFormEvent("133", BoEventTypes.et_FORM_DATA_ADD);
            EventFilterHelper.SetFormEvent("133", BoEventTypes.et_FORM_DATA_LOAD);

            EventFilterHelper.EnableEvents();
        }
    }
}
