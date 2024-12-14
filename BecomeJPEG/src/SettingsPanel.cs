using DirectShowLib;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;

namespace BecomeJPEG
{
    //--TODO: basically all of this. WinForms. See Design panel.
    public partial class SettingsPanel : Form
    {
        private int selectedTemplateIndex = -1;

        private string templateName = "";

        //Regex that validates integers from 0-999
        private readonly Regex int999_Regex = new Regex("(0|([1-9][0-9]{0,2}))");
        //Regex that validates integers from 0-9999
        private readonly Regex int9999_Regex = new Regex("(0|([1-9][0-9]{0,3}))");

        private JpegCamWindow cameraWindow;
        private Task cameraWindowTask;

        //storing VideoCaptures for re-use is a bit of a mixed bag:
        // + avoids memory issues, because it leaks upon creation/deletion
        // - causes the activity LED to stay on permanently while the settings panel is opened.
        // - Captures only allow you to set the Resolution before the first frame was grabbed.
        private VideoCapture[] captures;
        private int selectedDevice = -1;

        private Resolution[][] resolutionsPerDevice;

        public SettingsPanel()
        {
            InitializeComponent();
        }

        //set up the panel upon loading it.
        private void SettingsPanel_Load(object sender, EventArgs e)
        {
            //Setup the template list.
            RefreshTemplateList();

            //set up logging.
            Logger.Init(this.LogText);

            //try to set the "Default" template
            SetTextsFrom(Settings.ApplyTemplate("Default"));

            //Fetch list of devices
            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            //each capture corresponds to one specific device.
            captures = new VideoCapture[devices.Length];

            //start-stop-button always starts out disabled before a selection is made.
            StartStopButton.Enabled = false;
            //requires at least one device to be present.
            if (devices.Length == 0)
            {
                Logger.LogLine("Could not find any Camera Devices.");
                DeviceSelection.Enabled = false;
            }
            else
            {
                var deviceList = this.DeviceSelection.Items;
                deviceList.Clear();
                resolutionsPerDevice = new Resolution[devices.Length][];

                //put em all in the list, and dispose of them
                for (int i = 0; i < devices.Length; i++)
                {
                    resolutionsPerDevice[i] = Util.GetAllAvailableResolutions(devices[i]).ToArray();
                    deviceList.Add(devices[i].Name);
                    devices[i].Dispose();
                }
            }
        }

        //Upon closing dispose of all unused data properly.
        private void SettingsPanel_Closing(object sender, FormClosingEventArgs e)
        {
            foreach(var cap in captures)
                cap?.Dispose();
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
            if (DroprateInput.Text.Length == 0)
                return;
            int val = ValidateIntTextBox(DroprateInput, int999_Regex, (int)(Settings.frameDropChance));
            if(val != -1)
            {
                Settings.frameDropChance = val;
            }
            DroprateInput.Text = Settings.frameDropChance.ToString();
        }
        private void DroprateInput_StopEdit(object sender, EventArgs e)
        {
            Settings.frameDropChance = ResetIntTextBoxToZeroIfEmpty(DroprateInput, Settings.frameDropChance);
        }

        //lagtime is an integer from 0 to 9999
        private void LagtimeInput_TextChanged(object sender, EventArgs e)
        {
            if (LagtimeInput.Text.Length == 0)
                return;
            int val = ValidateIntTextBox(LagtimeInput, int9999_Regex, Settings.frameLagTime);
            if(val != -1)
            {
                Settings.frameLagTime = val;
            }
            LagtimeInput.Text = Settings.frameLagTime.ToString();
        }
        private void LagtimeInput_StopEdit(object sender, EventArgs e)
        {
            Settings.frameLagTime = ResetIntTextBoxToZeroIfEmpty(LagtimeInput, Settings.frameLagTime);
        }

        //lagrandom is an integer from 0 to 9999
        private void LagrandomInput_TextChanged(object sender, EventArgs e)
        {
            if (LagrandomInput.Text.Length == 0)
                return; 
            int val = ValidateIntTextBox(LagrandomInput, int9999_Regex, Settings.frameLagRandom);
            if( val != -1)
            {
                Settings.frameLagRandom = val;
            }
            LagrandomInput.Text = Settings.frameLagRandom.ToString();
        }
        private void LagrandomInput_StopEdit(object sender, EventArgs e)
        {
            Settings.frameLagRandom = ResetIntTextBoxToZeroIfEmpty(LagrandomInput, Settings.frameLagRandom);
        }

        //0 to 100 really, but its ok
		private void RepeatChanceInput_TextChanged(object sender, EventArgs e)
		{
			if (RepeatChanceInput.Text.Length == 0)
				return; //skip over empty text. That should be seperately handled in StopEdit. (Leave)
			int val = ValidateIntTextBox(RepeatChanceInput, int999_Regex, Settings.repeatChance);
			if (val != -1)
			{
				Settings.repeatChance = val;
			}
			RepeatChanceInput.Text = Settings.repeatChance.ToString();
		}
		private void RepeatChanceInput_StopEdit(object sender, EventArgs e)
		{
			Settings.repeatChance = ResetIntTextBoxToZeroIfEmpty(RepeatChanceInput, Settings.repeatChance);
		}

        //allows 0 to 999 but is recommended to keep down in the low 2 digits
		private void RepeatFrameCountInput_TextChanged(object sender, EventArgs e)
		{
			if (RepeatFrameCountInput.Text.Length == 0)
				return; //skip over empty text. That should be seperately handled in StopEdit. (Leave)
			int val = ValidateIntTextBox(RepeatFrameCountInput, int999_Regex, Settings.repeatFrameCount);
			if (val != -1)
			{
				Settings.repeatFrameCount = val;
			}
			RepeatFrameCountInput.Text = Settings.repeatFrameCount.ToString();
		}
		private void RepeatFrameCountInput_StopEdit(object sender, EventArgs e)
		{
			Settings.repeatFrameCount = ResetIntTextBoxToZeroIfEmpty(RepeatFrameCountInput, Settings.repeatFrameCount);
		}

        //0 to 9999 ms
		private void RepeatCooldownInput_TextChanged(object sender, EventArgs e)
		{
			if (RepeatCooldownInput.Text.Length == 0)
				return; //skip over empty text. That should be seperately handled in StopEdit. (Leave)
			int val = ValidateIntTextBox(RepeatCooldownInput, int9999_Regex, Settings.repeatCooldown);
			if (val != -1)
			{
				Settings.repeatCooldown = val;
			}
			RepeatCooldownInput.Text = Settings.repeatCooldown.ToString();
		}
		private void RepeatCooldownInput_StopEdit(object sender, EventArgs e)
		{
			Settings.repeatCooldown = ResetIntTextBoxToZeroIfEmpty(RepeatCooldownInput, Settings.repeatCooldown);
		}

        //ideally kept between 0 and 10. it should still try to move on and not repeat endlessly.
		private void RepeatChainInput_StopEdit(object sender, EventArgs e)
		{
			if (RepeatChainInput.Text.Length == 0)
				return; //skip over empty text. That should be seperately handled in StopEdit. (Leave)
			int val = ValidateIntTextBox(RepeatChainInput, int999_Regex, Settings.repeatChain);
			if (val != -1)
			{
				Settings.repeatChain = val;
			}
			RepeatChainInput.Text = Settings.repeatChain.ToString();
		}
		private void RepeatChainInput_TextChanged(object sender, EventArgs e)
		{
			Settings.repeatChain = ResetIntTextBoxToZeroIfEmpty(RepeatChainInput, Settings.repeatChain);
		}

		//The startstop button should toggle between "stop" and "start".
		//start => start the camera grabbing stuff, the part that actually does the JPEG funny.
		//stop => shut down the jpeg window and stop camera frame grabbing.
		private void StartStopButton_Click(object sender, EventArgs e)
        {
            if(cameraWindow == null || cameraWindow.IsActive == false)
            {
                VideoCapture capture = captures[DeviceSelection.SelectedIndex];
                //create capture if not yet created.
                if(capture == null)
                {
                    capture = new VideoCapture(DeviceSelection.SelectedIndex);
                    captures[DeviceSelection.SelectedIndex] = capture;
                }
                //fetch desired resolution
                var res = resolutionsPerDevice[DeviceSelection.SelectedIndex][ResolutionSelection.SelectedIndex];
                //create the window.
                cameraWindow = new JpegCamWindow(capture, res);
                cameraWindow.OnBeforeExit += ResetStartButton;
                cameraWindowTask = cameraWindow.Run();
                //dispose once it finished running.
                cameraWindowTask.GetAwaiter().OnCompleted(DisposeTask);
                StartStopButton.Text = "Stop";

                void DisposeTask()
                {
                    capture.Stop();
                    cameraWindowTask.Dispose();
                    cameraWindowTask = null;
                    StartStopButton.Enabled = true;
                    cameraWindow = null;
                }
                
            }
            else
            {
                cameraWindow.IsActive = false;
                ResetStartButton();
            }

            void ResetStartButton()
            {
                StartStopButton.Text = "Start";
                StartStopButton.Enabled = false;
            }
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
        /// Notification that the DeviceSelections selected index has changed.
        /// </summary>
        private void DeviceSelection_IndexChanged(object sender, EventArgs e)
        {
            int previous = selectedDevice;
            selectedDevice = DeviceSelection.SelectedIndex;
            if (selectedDevice >= 0)
            {
                //enable resolution selection and assign items.
                ResolutionSelection.Enabled = true;
                if (selectedDevice != previous)
                {
                    //clear and assign new resolutions
                    var itms = ResolutionSelection.Items; itms.Clear();
                    itms.AddRange(resolutionsPerDevice[selectedDevice]);
                    ResolutionSelection.SelectedIndex = 0;
                    ResolutionSelection.Text = "-";

                    //if the webcam window is not currently open, disable the button until a resolution was selected.
                    if (cameraWindow == null)
                    {
                        StartStopButton.Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// resolutionSelection selected index has changed.
        /// </summary>
        private void ResolutionSelectionChanged(object sender, EventArgs e)
        {
            if(ResolutionSelection.SelectedIndex >= 0)
            {
                StartStopButton.Enabled = true;
            }
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
                this.RepeatChanceInput.Text = qualitySettings.repeatChance.ToString();
                this.RepeatFrameCountInput.Text = qualitySettings.repeatFrameCount.ToString();
                this.RepeatCooldownInput.Text = qualitySettings.repeatCooldown.ToString();
                this.RepeatChainInput.Text = qualitySettings.repeatChain.ToString();
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
        private void SetNoneActiveOnEnterPress(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                this.ActiveControl = null;
        }

	}
}
