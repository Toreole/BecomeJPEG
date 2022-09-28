using BecomeJPEG.src;
using Emgu.CV.Flann;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace BecomeJPEG
{
    //--TODO: basically all of this. WinForms. See Design panel.
    public partial class SettingsPanel : Form
    {
        private int selectedTemplateIndex = -1;

        private string templateName = "";

        private readonly Regex int999_Regex = new Regex("(0|([1-9][0-9]{0,2}))");
        private readonly Regex int9999_Regex = new Regex("(0|([1-9][0-9]{0,3}))");

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

                //try to set the "Default" template
                SetTextsFrom(Settings.ApplyTemplate("Default"));

            }
        }

        //quality needs to be a integer between 0 and 100
        private void QualityInput_TextChanged(object sender, EventArgs e)
        {
            if (QualityInput.Text.Length == 0)
                return; //skip over empty text. That should be seperately handled in StopEdit. (Leave)
            int val = ValidateIntTextBox(QualityInput, int999_Regex, Settings.CompressionQuality);
            if(val != -1)
            {
                Settings.CompressionQuality = val;
            }
            QualityInput.Text = Settings.CompressionQuality.ToString();
            
        }

        private void QualityInput_StopEdit(object sender, EventArgs e)
        {
            //if the textbox is empty (or whitespace), reset it to 0
            Settings.CompressionQuality = ResetIntTextBoxToZeroIfEmpty(QualityInput, Settings.CompressionQuality);
            
        }

        //templatename is the name of the current template, this will not really do anything.
        private void TemplateName_StopEdit(object sender, EventArgs e)
        {
            templateName = TemplateNameInput.Text;
        }
        private void TemplateName_KeyDown(object sender, KeyEventArgs e)
        {
            SetNoneActiveOnEnterPress(e);
        }

        //droprate should also just be an integer from 0 to 100, doesnt make much difference for it to be a float.
        private void DroprateInput_TextChanged(object sender, EventArgs e)
        {
            int val = ValidateIntTextBox(DroprateInput, int999_Regex, (int)(Settings.frameDropChance));
            if(val != -1)
            {
                Settings.frameDropChance = val;
            }
            DroprateInput.Text = Settings.frameDropChance.ToString();
        }

        //lagtime is an integer from 0 to 9999
        private void LagtimeInput_TextChanged(object sender, EventArgs e)
        {
            int val = ValidateIntTextBox(LagtimeInput, int9999_Regex, Settings.frameLagTime);
            if(val != -1)
            {
                Settings.frameLagTime = val;
            }
            LagtimeInput.Text = Settings.frameLagTime.ToString();
        }

        //lagrandom is an integer from 0 to 9999
        private void LagrandomInput_TextChanged(object sender, EventArgs e)
        {
            int val = ValidateIntTextBox(LagrandomInput, int9999_Regex, Settings.frameLagRandom);
            if( val != -1)
            {
                Settings.frameLagRandom = val;
            }
            LagrandomInput.Text = Settings.frameLagRandom.ToString();
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
            SetTextsFrom(Settings.ApplyTemplate(index));
        }

        //"Add Template" should overwrite the existing template with the same name, or create a new one.
        private void AddTemplateButton_Click(object sender, EventArgs e)
        {
            Settings.SaveTemplate(templateName);
            RefreshTemplateList();
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
                items.Add(template.name);
            }
        }

        /// <summary>
        /// Sets the text boxes from the provided readonlyqualitytemplate.
        /// </summary>
        /// <param name="qualitySettings"></param>
        private void SetTextsFrom(SReadonlyQualityTemplate qualitySettings)
        {
            if(qualitySettings.isValid)
            {
                //update all textboxes.
                this.TemplateNameInput.Text = qualitySettings.name;
                this.templateName = qualitySettings.name;
                this.LagrandomInput.Text = qualitySettings.lagRandom.ToString();
                this.LagtimeInput.Text = qualitySettings.lagTime.ToString();
                this.DroprateInput.Text = qualitySettings.dropChance.ToString("0");
                this.QualityInput.Text = qualitySettings.quality.ToString();
            }
        }

        /// <summary>
        /// Validates the text in a given TextBox according to the regex.
        /// Only use this in TextChanged events.
        /// </summary>
        /// <param name="textBox">The TextBox</param>
        /// <param name="regex">regex for verifying the value</param>
        /// <param name="fallbackValue">fallback when input is invalid. is set as the TextBox.Text</param>
        /// <returns></returns>
        private int ValidateIntTextBox(TextBox textBox, Regex regex, int fallbackValue)
        {
            if (textBox.Text.Length == 0)
                return -1; //skip over empty text. That should be seperately handled in StopEdit. (Leave)
           
            var matches = regex.Matches(textBox.Text);
            if (matches.Count == 0)
            {
                QualityInput.Text = fallbackValue.ToString();
                return fallbackValue;
            }
            else
            {
                string text = matches[matches.Count - 1].Value; //use the last match in case of a leading 0 (e.g. 050 => 50)
                //since the regex guarantees the string to be a valid integer, directly use int.Parse.
                return int.Parse(text);
            }
        }

        /// <summary>
        /// Resets a TextBox's Text to "0" after it lost focus, and it was empty or the text was white space. 
        /// Keeps the previous value untouched otherwise.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="regularValue"></param>
        /// <returns>The value that is used in the TextBox</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ResetIntTextBoxToZeroIfEmpty(TextBox textBox, int regularValue)
        {
            string text = textBox.Text;
            if (text.Length == 0 || string.IsNullOrWhiteSpace(text))
            {
                textBox.Text = "0";
                return 0;
            }
            return regularValue;
        }

        /// <summary>
        /// Sets Form.ActiveControl to null if the KeyEvent is an Enter/Return press.
        /// </summary>
        /// <param name="e"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetNoneActiveOnEnterPress(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                this.ActiveControl = null;
        }

    }
}
