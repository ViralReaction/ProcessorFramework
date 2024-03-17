using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace ProcessorFramework
{
    public class BonusOutput
    {
        public BonusOutput()
        {
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1) Log.Error("PF: RandomProductList configured incorrectly");
            else
            {
                string str = xmlRoot.FirstChild.Value;
                str = str.TrimStart(new char[]
                {
                    '('
                });
                str = str.TrimEnd(new char[]
                {
                    ')'
                });
                string[] array = str.Split(new char[]
                {
                    ','
                });
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                chance = Convert.ToSingle(array[0], invariantCulture);
                amount = Convert.ToInt32(array[1], invariantCulture);
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name, null, null);
            }
        }

        public ThingDef thingDef;
        public float chance;
        public int amount;
    }
}
