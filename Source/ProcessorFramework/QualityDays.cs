using RimWorld;
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
    public class QualityDays
    {
        public QualityDays()
        {
        }

        public QualityDays(float awful, float poor, float normal, float good, float excellent, float masterwork, float legendary)
        {
            this.awful = awful;
            this.poor = poor;
            this.normal = normal;
            this.good = good;
            this.excellent = excellent;
            this.masterwork = masterwork;
            this.legendary = legendary;
        }

        public float DaysForQuality(QualityCategory qualityCategory)
        {
            return qualityCategory switch
            {
                QualityCategory.Awful => awful,
                QualityCategory.Poor => poor,
                QualityCategory.Normal => normal,
                QualityCategory.Good => good,
                QualityCategory.Excellent => excellent,
                QualityCategory.Masterwork => masterwork,
                QualityCategory.Legendary => legendary,
                _ => normal,
            };
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1) Log.Error("PF: QualityDays configured incorrectly");
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
                awful = Convert.ToSingle(array[0], invariantCulture);
                poor = Convert.ToSingle(array[1], invariantCulture);
                normal = Convert.ToSingle(array[2], invariantCulture);
                good = Convert.ToSingle(array[3], invariantCulture);
                excellent = Convert.ToSingle(array[4], invariantCulture);
                masterwork = Convert.ToSingle(array[5], invariantCulture);
                legendary = Convert.ToSingle(array[6], invariantCulture);
            }
        }
        public float awful;
        public float poor;
        public float normal;
        public float good;
        public float excellent;
        public float masterwork;
        public float legendary;
    }
}
