using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BoilerPlateWeb.Models
{
    [Serializable]
    public class LookupTableField
    {
        public string field, lookupTable, lookupField;
        public DropDownList dropDownList = new DropDownList();
        public DropDownList triggeringDDL;
        public Dictionary<string, DropDownList> editIDDropDownLists = new Dictionary<string, DropDownList>();

        public LookupTableField(string lookupTableField)
        {
            var lookupTableFieldComponents = lookupTableField.Split('-');
            field = lookupTableFieldComponents[0];
            lookupTable = lookupTableFieldComponents[1];
            lookupField = lookupTableFieldComponents[2];
        }
    }
}