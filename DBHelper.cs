using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace BoilerPlateWeb
{
    public class DBHelper
    {
        public static bool HasData(DataSet ds)
        {
            if (ds != null)
            {
                if (ds.Tables != null)
                {
                    if (ds.Tables.Count != 0)
                    {
                        if (ds.Tables[0] != null)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count != 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static DateTime GetDateFromString(string date)
        {
            if (date.Contains("/"))
            {
                int month = int.Parse(date.Substring(0, 2));
                int day = int.Parse(date.Substring(3, 2));
                int year = int.Parse(date.Substring(6, 4));
                int hour = int.Parse(date.Substring(11, 2));
                int minute = int.Parse(date.Substring(14, 2));
                int second = int.Parse(date.Substring(17, 2));

                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(5, 2));
                int day = int.Parse(date.Substring(8, 2));

                return new DateTime(year, month, day);
            }
        }
    }
}