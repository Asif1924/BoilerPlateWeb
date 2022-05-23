using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoilerPlateWeb.Models
{
    public class DeleteChildInfo
    {
        public string childTableName, parentField, childField, condition;

        public DeleteChildInfo(string deleteChildInfoS)
        {
            var deleteChildInfoComponents = deleteChildInfoS.Split('-');
            childTableName = deleteChildInfoComponents[0];
            parentField = deleteChildInfoComponents[1];
            childField = deleteChildInfoComponents[2];

            if (deleteChildInfoComponents.Length == 4)
            {
                condition = deleteChildInfoComponents[3];
            }
        }
    }
}