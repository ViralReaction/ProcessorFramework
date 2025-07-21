using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProcessorFramework
{
	public class ProcessDef : Def
	{
        public int uniqueID; //mainly for multiplayer

        public ThingDef thingDef;
		public ThingFilter ingredientFilter = new ThingFilter(); //a ThingFilter is used here so modders can designate entire categories

        public float processDays = 6f;
        public float capacityFactor = 1;
        public float efficiency = 1f;
        public bool usesTemperature = true;
		public FloatRange temperatureSafe = new FloatRange(-1f, 32f);
		public FloatRange temperatureIdeal = new FloatRange(7f, 32f);
		public float ruinedPerDegreePerHour = 2.5f;
        public float speedBelowSafe = 0.1f;
        public float speedAboveSafe = 1f;
		public FloatRange sunFactor = new FloatRange(1f, 1f);
		public FloatRange rainFactor = new FloatRange(1f, 1f);
		public FloatRange snowFactor = new FloatRange(1f, 1f);
		public FloatRange windFactor = new FloatRange(1f, 1f);
        public float unpoweredFactor = 0f;
        public float unfueledFactor = 0f;
        public float powerUseFactor = 1f;
        public float fuelUseFactor = 1f;
        public string filledGraphicSuffix = null;
        public bool usesQuality = false;
        public QualityDays qualityDays = new QualityDays(1, 0, 0, 0, 0, 0, 0);
        public Color color = new Color(1.0f, 1.0f, 1.0f);
        public string customLabel = "";
        public float destroyChance = 0f;
        public List<BonusOutput> bonusOutputs = new List<BonusOutput>();
        public bool useStatForEfficiency = false;
        public StatDef efficiencyStat;
        public float statBaselineValue = 1f;

		public override void ResolveReferences()
		{			
			ingredientFilter.ResolveReferences();			
		}

        public override string ToString()
        {
            return thingDef?.ToString() ?? "[invalid process]";
        }
    }
}