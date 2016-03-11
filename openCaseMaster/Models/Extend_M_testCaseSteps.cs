using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.Models
{
    public static class Extend_M_testCaseSteps
    {


        public static Dictionary<string, string> getParamDictionary(this M_testCaseSteps mtc)
        {

            XElement xe = XElement.Parse(mtc.stepXML);

            var pbs = xe.ParamDictionary();


            XElement pxe = XElement.Parse(mtc.paramXML);
            var ppbs = pxe.Descendants();

            string[] keys = new string[pbs.Count];

            pbs.Keys.CopyTo(keys, 0);

            foreach (var key in keys)
            {
                foreach (var ppb in ppbs)
                {
                    if (key == (string)ppb.Attribute("name"))
                    {
                        pbs[key] = (string)ppb.Attribute("value");
                        break;
                    }
                }
            }

            return pbs;
        }



       
    }
}