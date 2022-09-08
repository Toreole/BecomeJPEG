using System;

namespace BecomeJPEG
{
    [Serializable] //this could be a struct, but i just enjoy having a paramless ctor, which isnt a feature for structs in this version of C#.
    internal class QualityTemplate
    {
        internal string templateName;
        internal float frameDropChance;
        internal int compressionQuality;
        internal int frameLagTime;
        internal int frameLagRandom;

        //default values provided by constructor
        internal QualityTemplate()
        {
            frameDropChance = 0.85f;
            compressionQuality = 0;
            frameLagTime = 100;
            frameLagRandom = 400;
        }

        internal QualityTemplate(string name) : this()
        {
            templateName = name;
        }

        internal QualityTemplate(string name, float dropRate, int quality, int lagTime, int lagRandom)
        {
            templateName = name;
            frameDropChance = dropRate;
            compressionQuality = quality;
            frameLagTime = lagTime;
            frameLagRandom = lagRandom;
        }
    }
}
