﻿using BecomeJPEG.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BecomeJPEG
{
    internal class Settings
    {
        private const string templateFile = "templates.txt";

        //settings and and a Random instance that should just be accessible.
        private static Random rng = new Random();
        internal static float frameDropChance = 0.85f;
        internal static int compressionQuality = 0;
        internal static int frameLagTime = 100;
        internal static int frameLagRandom = 400;
        //readonly windowName.
        internal static readonly string windowName = "BecomeJPEG Preview";

        private static readonly Encoding encoder = Encoding.UTF8;

        //list of templates.
        internal static List<QualityTemplate> templates = null;

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

        internal static void LoadTemplatesFromDrive()
        {
            if (File.Exists(templateFile) == false)
            {
                //default templates.
                templates = new List<QualityTemplate>()
                {
                    new QualityTemplate("high", 0, 100, 0, 0),
                    new QualityTemplate("worst", 0.5f, 0, 100, 100),
                    new QualityTemplate("medium", 0.2f, 4, 10, 20)
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
        }

        //saves a template with the provided name. if a template with that name already exists, override it.
        internal static void SaveTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            QualityTemplate template = templates.Find(x => x.templateName == name);
            if (template == null)
            {
                template = new QualityTemplate(name);
                templates.Add(template);
            }
            template.frameDropChance = frameDropChance;
            template.frameLagRandom = frameLagRandom;
            template.frameLagTime = frameLagTime;
            template.compressionQuality = CompressionQuality;
            //write templates to a file.
            SaveTemplatesToDrive();
        }

        //applies a template by name.
        internal static void ApplyTemplate(string name)
        {
            QualityTemplate foundTemplate = templates.Find(x => x.templateName == name);
            if (foundTemplate != null)
            {
                CompressionQuality = foundTemplate.compressionQuality;
                frameLagRandom = foundTemplate.frameLagRandom;
                frameLagTime = foundTemplate.frameLagTime;
                frameDropChance = foundTemplate.frameDropChance;
            }
        }

        internal static void ApplyTemplate(int index)
        {
            if (index >= 0 && index < templates.Count)
            {
                QualityTemplate template = templates[index];
                CompressionQuality = template.compressionQuality;
                frameLagRandom = template.frameLagRandom;
                frameLagTime = template.frameLagTime;
                frameDropChance = template.frameDropChance;
                Logger.LogLine($"Applied Template \"{template.templateName}\"");
            }
            else
            {
                Logger.LogLine($"Invalid ApplyTemplate call. index: {index}");
            }
        }

        //Removes a named template if it exists.
        internal static void DeleteTemplate(string name)
        {
            QualityTemplate foundTemplate = templates.Find(x => x.templateName == name);
            if (foundTemplate != null)
            {
                templates.Remove(foundTemplate);
                //write templates to a file.
                SaveTemplatesToDrive();
            }
        }
    }
}
