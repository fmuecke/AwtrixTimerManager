using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimerApp
{
    public partial class Form1 : Form
    {
        // HttpClient for sending HTTP POST requests
        private static readonly HttpClient client = new HttpClient();
        
        // TextBox to hold the IP address entered by the user
        private TextBox ipAddressTextBox;        

        public Form1()
        {
            InitializeComponent();
        }

        int FormatDuration(string duration)
        {
            int result = 0;

            // if duration string ends with "m" or "min", remove it and multiply by 60
            if (duration.EndsWith("m") || duration.EndsWith("min"))
            {
                result = int.Parse(duration.Substring(0, duration.Length - 1)) * 60;
            }
            // if duration string ends with "s" or "sec", remove it
            else if (duration.EndsWith("s") || duration.EndsWith("sec"))
            {
                result = int.Parse(duration.Substring(0, duration.Length - 1));
            }
            // parse the string as integer
            else
            {
                result = int.Parse(duration);
            }
            return result;
        }

        // Method to send a POST request to start the timer
        private static async Task<string> StartTimer(string host, int duration)
        {
            // Create the JSON body
            string body = "{ \"duration\": " + duration + ", \"start_now\" : true }";
            
            // Set the IP address
            string url = "http://" + host + "/api/timer";

            // Send the POST request
            return await SendPostRequest(url, body);
        }

        // Helper method for sending POST requests
        private static async Task<string> SendPostRequest(string url, string jsonBody)
        {
            try
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                return $"Error: {e.Message}";
            }
        }

        // Event handler for each "Send" button
        private async void buttonSend_Click(object sender, EventArgs e)
        {
            // Get the duration from the associated text box
            Button button = sender as Button;
            TextBox textBox = (TextBox)button.Tag;
            

            string ipAddress = ipAddressTextBox.Text;
            if (IsValidIpAddress(ipAddress))
            {
                var duration = FormatDuration(textBox.Text);
                string response = await StartTimer(ipAddress, duration);
                //MessageBox.Show(response, "Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please enter a valid IP address.", "Invalid IP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                
        }

        // Helper method to validate an IP address
        private bool IsValidIpAddress(string ipAddress)
        {
            // A simple IP address validation method
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }

        // Initialize the UI components
        private void InitializeComponent()
        {
            // Initialize components
            this.SuspendLayout();

            // Label for IP address
            Label ipLabel = new Label
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 25),
                Text = "Host:"
            };

            // TextBox for entering IP address
            ipAddressTextBox = new TextBox
            {
                Location = new System.Drawing.Point(120, 20),
                Size = new System.Drawing.Size(150, 25),
                Name = "ipAddressTextBox"
            };

            // Array of TextBoxes for duration input
            TextBox[] textBoxes = new TextBox[4];
            Button[] sendButtons = new Button[4];

            for (int i = 0; i < 4; i++)
            {
                // Create and configure TextBox
                textBoxes[i] = new TextBox
                {
                    Location = new System.Drawing.Point(20, 60 + (i * 40)),
                    Size = new System.Drawing.Size(150, 25),
                    Name = $"textBox{i + 1}"
                };

                // Create and configure Button
                sendButtons[i] = new Button
                {
                    Location = new System.Drawing.Point(180, 60 + (i * 40)),
                    Size = new System.Drawing.Size(75, 25),
                    Text = "Send",
                    Name = $"button{i + 1}",
                    Tag = textBoxes[i] // Associate the button with the corresponding TextBox
                };
                sendButtons[i].Click += new EventHandler(buttonSend_Click);

                // Add TextBox and Button to the form
                this.Controls.Add(textBoxes[i]);
                this.Controls.Add(sendButtons[i]);
            }

            // Add the IP label and textbox to the form
            this.Controls.Add(ipLabel);
            this.Controls.Add(ipAddressTextBox);

            // Configure the form
            this.ClientSize = new System.Drawing.Size(300, 240);
            this.Name = "Form1";
            this.Text = "Timer App";

            // Complete the layout of the form
            this.ResumeLayout(false);
        }       
    }
}
