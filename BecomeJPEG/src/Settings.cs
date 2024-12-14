using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BecomeJPEG
{
    internal class Settings
    {
        private const string templateFile = "templates.txt";

        //active settings.
        internal static int frameDropChance = 55;
        private static int compressionQuality = 0;
        internal static int frameLagTime = 100;
        internal static int frameLagRandom = 400;
        internal static int repeatChance = 0;
        internal static int repeatFrameCount = 12;
        internal static int repeatCooldown = 0;
        internal static int repeatChain = 1;

        //window name for the EgmuCV.
        internal const string windowName = "BecomeJPEG Preview";

        private static readonly Encoding encoder = Encoding.UTF8;

        //list of templates.
        private static List<QualityTemplate> templates = null;

        private static SReadonlyQualityTemplate[] templateBuffer = new SReadonlyQualityTemplate[20];

        //Property that automatically clamps the quality between 0 and 100.
        internal static int CompressionQuality
        {
            get => compressionQuality;
            set
            {
                compressionQuality =
                    (value < 0) ? 0 :
                    (value > 100) ? 100
                    : value;
            }
        }

        /// <summary>
        /// Loads templates from the templates.txt file if possible. Otherwise creates a default list.
        /// </summary>
        internal static void LoadTemplatesFromDrive()
        {
            if (File.Exists(templateFile) == false)
            {
                //default templates.
                templates = new List<QualityTemplate>()
                {
                    new QualityTemplate("Default", 0, 100, 0, 0, 0, 0, 0, 1),
					new QualityTemplate("Medium Quality", 20, 4, 10, 10, 0, 0, 0, 1),
					new QualityTemplate("Awful", 50, 0, 100, 100, 0, 0, 0, 1)
                };
            }
            else
            {
                FileStream stream = File.OpenRead(templateFile);
                byte[] buffer = new byte[stream.Length];
                //read the entire file.
                stream.Read(buffer, 0, (int)stream.Length);
                //immediately dispose the stream.
                stream.Flush();
                stream.Dispose();
                string data = encoder.GetString(buffer);
                string[] entries = data.Split('\n');
                //initialize templates to an appropriate length.
                templates = new List<QualityTemplate>(entries.Length);
                //add all entries as QualityTemplates.
                foreach (var s in entries)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    templates.Add(QualityTemplate.FromString(s));
                }
            }
        }

        //store templates in a file.
        internal static void SaveTemplatesToDrive()
        {
            //always create a new file or replace the contents of the previous one.
            FileStream stream = File.Open(templateFile, FileMode.Create);
            for (int i = 0; i < templates.Count; i++)
            {
                byte[] buffer = encoder.GetBytes(templates[i].ToString() + '\n');
                stream.Write(buffer, 0, buffer.Length);
            }
            stream.Flush();
            stream.Dispose();
            Logger.LogLine("Saved Templates to file.");
        }

        //saves a template with the provided name. if a template with that name already exists, override it.
        internal static void SaveTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            QualityTemplate template = templates.Find(x => x.templateName == name);
            if (template == null)
            {
                Logger.LogLine($"Created new Template \"{name}\".");
                template = new QualityTemplate(name);
                templates.Add(template);
            } 
            else
            {
                Logger.LogLine($"Overriding Template \"{name}\" with new values.");
            }
            template.frameDropChance = frameDropChance;
            template.frameLagRandom = frameLagRandom;
            template.frameLagTime = frameLagTime;
            template.compressionQuality = CompressionQuality;
            template.repeatChance = repeatChance;
            template.repeatFrameCount = repeatFrameCount;
            template.repeatCooldown = repeatCooldown;
            template.repeatChain = repeatChain;
            //write templates to a file.
            SaveTemplatesToDrive();
        }

        //applies a template by name.
        internal static SReadonlyQualityTemplate ApplyTemplate(string name)
        {
            QualityTemplate foundTemplate = templates.Find(x => x.templateName == name);
            if (foundTemplate != null)
            {
                CompressionQuality = foundTemplate.compressionQuality;
                frameLagRandom = foundTemplate.frameLagRandom;
                frameLagTime = foundTemplate.frameLagTime;
                frameDropChance = foundTemplate.frameDropChance;
                repeatChance = foundTemplate.repeatChance;
                repeatFrameCount = foundTemplate.repeatFrameCount;
                repeatCooldown = foundTemplate.repeatCooldown;
                repeatChain = foundTemplate.repeatChain;

                Logger.LogLine($"Applied Template \"{name}\".");
                return new SReadonlyQualityTemplate(foundTemplate);
            }
            else
            {
                Logger.LogLine($"Could not find Template \"{name}\".");
                return default;
            }
        }

        /// <summary>
        /// Apply a template given its index.
        /// </summary>
        /// <param name="index">the index of the template in the collection</param>
        /// <returns>A SReadonlyQualityTemplate that has the values of the applied Template.</returns>
        internal static SReadonlyQualityTemplate ApplyTemplate(int index)
        {
            if (index >= 0 && index < templates.Count)
            {
                QualityTemplate template = templates[index];
                CompressionQuality = template.compressionQuality;
                frameLagRandom = template.frameLagRandom;
                frameLagTime = template.frameLagTime;
                frameDropChance = template.frameDropChance;
                repeatChance = template.repeatChance;
                repeatFrameCount = template.repeatFrameCount;
                repeatCooldown = template.repeatCooldown;
                repeatChain = template.repeatChain;

                Logger.LogLine($"Applied Template \"{template.templateName}\".");
                //return readonly copies of the template values.
                return new SReadonlyQualityTemplate(template);
            }
            else
            {
                if (templates != null)
                    Logger.LogLine($"Invalid ApplyTemplate call. index: {index}.");
                else
                    Logger.LogLine("Templates List null.");
                return default;
            }
        }

        //Removes a named template if it exists.
        //obsolete: should delete via index instead now.
        internal static void DeleteTemplate(string name)
        {
            QualityTemplate foundTemplate = templates.Find(x => x.templateName == name);
            if (foundTemplate != null)
            {
                templates.Remove(foundTemplate);
                //write templates to a file.
                SaveTemplatesToDrive();
                Logger.LogLine($"Deleted template \"{foundTemplate.templateName}\".");
            }
        }

        internal static void DeleteTemplate(int index)
        {
            if (index >= 0 && index < templates.Count)
            {
                QualityTemplate template = templates[index];
                templates.RemoveAt(index);
                SaveTemplatesToDrive();
                Logger.LogLine($"Deleted template \"{template.templateName}\".");
            }
        }

        internal static Span<SReadonlyQualityTemplate> IterateTemplates()
        {
            //resize the buffer if needed.
            if(templateBuffer.Length < templates.Count)
            {
                templateBuffer = new SReadonlyQualityTemplate[templates.Count];
            }
            //fill the buffer
            for(int i = 0; i < templates.Count; i++)
                templateBuffer[i] = new SReadonlyQualityTemplate(templates[i]);
            //return it as span.
            return new Span<SReadonlyQualityTemplate>(templateBuffer, 0, templates.Count);
        }
    }
}
