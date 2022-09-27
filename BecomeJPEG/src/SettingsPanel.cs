using BecomeJPEG.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BecomeJPEG
{
    //--TODO: basically all of this. WinForms. See Design panel.
    public partial class SettingsPanel : Form
    {
        private int selectedTemplateIndex = -1;

        private string templateName = "";

        private const string number_0_100_regex = "^(100|[0-9]?[0-9])$";

        public SettingsPanel()
        {
            InitializeComponent();
        }

        //set up the panel upon loading it.
        private void SettingsPanel_Load(object sender, EventArgs e)
        {
            //double check + cast
            if (sender is SettingsPanel panel)
            {
                //Setup the template list.
                RefreshTemplateList();

                //set up logging.
                Logger.Init(this.LogText);

            }
        }

        //quality needs to be a integer between 0 and 100
        private void QualityInput_TextChanged(object sender, EventArgs e)
        {
            
        }

        //templatename is the name of the current template, this will not really do anything. the important part is validation.
        private void TemplateName_TextChanged(object sender, EventArgs e)
        {
            templateName = TemplateNameInput.Text;
        }

        //droprate should also just be an integer from 0 to 100, doesnt make much difference for it to be a float.
        private void DroprateInput_TextChanged(object sender, EventArgs e)
        {

        }

        //lagtime is an integer from 0 to 9999
        private void LagtimeInput_TextChanged(object sender, EventArgs e)
        {

        }

        //lagrandom is an integer from 0 to 9999
        private void LagrandomInput_TextChanged(object sender, EventArgs e)
        {

        }

        //The startstop button should toggle between "stop" and "start".
        //start => start the camera grabbing stuff, the part that actually does the JPEG funny.
        //stop => shut down the jpeg window and stop camera frame grabbing.
        private void StartStopButton_Click(object sender, EventArgs e)
        {
            StartStopButton.Text = "Stop";
        }

        //the index should just be cached in here
        private void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTemplateIndex = TemplateList.SelectedIndex;
        }

        //double clicking a template in the list should cause a change in template.
        private void TemplateList_DoubleClick(object sender, MouseEventArgs e)
        {
            int index = TemplateList.SelectedIndex;
            SReadonlyQualityTemplate qualitySettings = Settings.ApplyTemplate(index);
            //check if the qualitySettings struct is valid.
            if(qualitySettings.isValid)
            {
                //update all textboxes.
                this.TemplateNameInput.Text = qualitySettings.name;
                this.templateName           = qualitySettings.name;
                this.LagrandomInput.Text    = qualitySettings.lagRandom.ToString();
                this.LagtimeInput.Text      = qualitySettings.lagTime.ToString();
                this.DroprateInput.Text     = qualitySettings.dropChance.ToString("0.0");
                this.QualityInput.Text      = qualitySettings.quality.ToString();
            }

        }

        //"Add Template" should overwrite the existing template with the same name, or create a new one.
        private void AddTemplateButton_Click(object sender, EventArgs e)
        {

        }

        //"Template Delete" should delete the selected template (given by its index)
        private void TemplateDelete_Click(object sender, EventArgs e)
        {
            Settings.DeleteTemplate(selectedTemplateIndex);
            RefreshTemplateList();
        }

        /// <summary>
        /// Completely refreshes the TemplateList by repopulating it.
        /// </summary>
        //seek for a way to improve this.
        private void RefreshTemplateList()
        {
            if(TemplateList == null)
            {
                Logger.LogLine("TemplateList unavailable.");
                return;
            }
            var items = TemplateList.Items;
            items.Clear();
            foreach (var template in Settings.IterateTemplates())
            {
                items.Add(template.templateName);
            }
        }
    }
}
