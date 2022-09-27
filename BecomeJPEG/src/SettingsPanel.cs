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
        public SettingsPanel()
        {
            InitializeComponent();
        }

        //quality needs to be a integer between 0 and 100
        private void QualityInput_TextChanged(object sender, EventArgs e)
        {

        }

        //templatename is the name of the current template, this will not really do anything. the important part is validation.
        private void TemplateName_TextChanged(object sender, EventArgs e)
        {

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
            if(sender is Button button)
            {
                button.Text = "Stop";
            }
        }

        //set up the panel upon loading it.
        private void SettingsPanel_Load(object sender, EventArgs e)
        {
            //double check + cast
            if(sender is SettingsPanel panel)
            {
                //find the template list and set it up with the options from the stored templates.
                var templateList = panel.Controls.Find("TemplateList", true).FirstOrDefault();
                if(templateList != null && templateList is ListBox list)
                {
                    list.Items.Clear();
                    foreach(var template in Settings.templates)
                    {
                        list.Items.Add(template.templateName);
                    }
                }

                //set up logging.
                var logTextBox = panel.Controls.Find("LogText", true).FirstOrDefault();
                if(logTextBox != null && logTextBox is TextBox textBox)
                {
                    Logger.Init(textBox);
                }
            }
        }

        //the index should just be cached in here
        private void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //double clicking a template in the list should cause a change in template.
        private void TemplateList_DoubleClick(object sender, MouseEventArgs e)
        {
            if(sender is ListBox list)
            {
                int index = list.SelectedIndex;
                Settings.ApplyTemplate(index);
            }
        }

        //"Add Template" should overwrite the existing template with the same name, or create a new one.
        private void AddTemplateButton_Click(object sender, EventArgs e)
        {

        }

        //"Template Delete" should delete the selected template (given by its index)
        private void TemplateDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
