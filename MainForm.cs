using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OVRefiner.Services;

namespace OVRefiner
{
    public partial class MainForm : Form
    {
        private readonly DnsService _dnsService;
        private readonly LoggerService _logger;
        private readonly ConfigProcessor _processor;
        private int _errorCount = 0; 

        public MainForm()
        {
            InitializeComponent();
            cmbDoh.SelectedIndex = 0; 
            
            _dnsService = new DnsService();
            _logger = new LoggerService(rtbLog);
            _processor = new ConfigProcessor(_dnsService, _logger);
        }

        private void RbTypeOvpn_CheckedChanged(object sender, EventArgs e)
        {
            bool isOvpn = rbTypeOvpn.Checked;
            chkIncludeOriginal.Enabled = !isOvpn;
            rbInputSub.Enabled = !isOvpn;
            if (isOvpn)
            {
                rbInputFile.Checked = true;
                rbInputFile.Text = "Select Folder"; // Update text to indicate folder selection
                rbInputSub.Checked = false;
            }
            else
            {
                rbInputFile.Text = "Text File";
            }
        }

        private void CmbDoh_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isCustom = cmbDoh.SelectedIndex == 2;
            txtCustomDoh.Enabled = isCustom;
            chkDnsBase64.Enabled = isCustom;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (rbTypeOvpn.Checked)
            {
                // OVPN: Select a Folder
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing .ovpn files";
                    fbd.UseDescriptionForTitle = true;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtInputPath.Text = fbd.SelectedPath;
                    }
                }
            }
            else
            {
                // V2Ray: Select a File
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Text Files|*.txt|All Files|*.*" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK) txtInputPath.Text = ofd.FileName;
                }
            }
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInputPath.Text))
            {
                MessageBox.Show("Please select input.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _errorCount = 0;

            // 1. Configure DNS
            string dohUrl = "";
            bool useBase64 = false;

            if (cmbDoh.SelectedIndex == 0) dohUrl = "https://dns.google/resolve";
            else if (cmbDoh.SelectedIndex == 1) dohUrl = "https://cloudflare-dns.com/dns-query";
            else
            {
                dohUrl = txtCustomDoh.Text;
                useBase64 = chkDnsBase64.Checked;
            }

            if (string.IsNullOrEmpty(dohUrl))
            {
                _logger.LogError("DoH URL is missing.");
                return;
            }
            
            _dnsService.Configure(dohUrl, useBase64);

            btnStart.Enabled = false;
            progressBar.Style = ProgressBarStyle.Marquee;
            
            _processor.ClearBuffers();

            try
            {
                if (rbTypeV2ray.Checked)
                {
                    await ProcessV2Ray();
                }
                else
                {
                    await ProcessOvpn();
                }

                // Final Status Check
                if (_errorCount == 0)
                {
                    _logger.LogSuccess("Operation completed successfully.");
                    //MessageBox.Show("Done!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _logger.LogWarning($"Completed with {_errorCount} errors. Check log.");
                    MessageBox.Show($"Completed with {_errorCount} errors.", "Finished with Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fatal Error: {ex.Message}");
            }
            finally
            {
                btnStart.Enabled = true;
                progressBar.Style = ProgressBarStyle.Blocks;
            }
        }

        private async System.Threading.Tasks.Task ProcessV2Ray()
        {
            string inputFile = txtInputPath.Text;
            if (!File.Exists(inputFile))
            {
                 _logger.LogError("Input file not found.");
                 _errorCount++;
                return;
            }

            string rawContent = await File.ReadAllTextAsync(inputFile);

            // Decode Subscription if needed
            if (rbInputSub.Checked)
            {
                try
                {
                    rawContent = rawContent.Replace("\n", "").Replace("\r", "").Trim();
                    byte[] data = Convert.FromBase64String(rawContent);
                    rawContent = Encoding.UTF8.GetString(data);
                }
                catch
                {
                    _logger.LogError("Failed to decode Base64 subscription.");
                    _errorCount++;
                }
            }

            var lines = rawContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                bool result = await _processor.ProcessV2RayLineAsync(line.Trim(), chkIncludeOriginal.Checked);
                if (!result) _errorCount++;
            }

            // Define Output Path for V2Ray
            // Requirement: originalfilename-{MM:HH:SS}.txt (Using '-' because ':' is invalid in Windows filenames)
            string directory = Path.GetDirectoryName(inputFile);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(inputFile);
            string timestamp = DateTime.Now.ToString("yyyyMMdd-mmhhss"); 
            string outputFileName = $"{fileNameNoExt}-{timestamp}.txt";
            string fullOutputPath = Path.Combine(directory, outputFileName);

            _logger.LogInfo($"Saving results to: {outputFileName}");
            
            await _processor.SaveV2RayResultsAsync(fullOutputPath);
        }

        private async System.Threading.Tasks.Task ProcessOvpn()
        {
            string inputFolder = txtInputPath.Text;

            if (!Directory.Exists(inputFolder))
            {
                _logger.LogError("Input folder not found.");
                _errorCount++;
                return;
            }

            // Define Output Folder for OVPN
            // Requirement: Create folder next to original
            string parentDir = Directory.GetParent(inputFolder)?.FullName ?? inputFolder;
            string inputFolderName = new DirectoryInfo(inputFolder).Name;
            string outputFolderName = $"{inputFolderName}_Processed_{DateTime.Now:MM-HH-ss}"; // Using '-' for time
            string outputFullPath = Path.Combine(parentDir, outputFolderName);

            Directory.CreateDirectory(outputFullPath);
            _logger.LogInfo($"Created output folder: {outputFullPath}");

            string[] files = Directory.GetFiles(inputFolder, "*.ovpn");
            
            if (files.Length == 0)
            {
                _logger.LogWarning("No .ovpn files found in selected folder.");
                return;
            }

            foreach (var file in files)
            {
                bool result = await _processor.ProcessOvpnFileAsync(file, outputFullPath, false);
                if (!result) _errorCount++;
            }
            
            // For OVPN we open the folder because it's a new directory
            System.Diagnostics.Process.Start("explorer.exe", outputFullPath);
        }

        private void LnkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var ps = new System.Diagnostics.ProcessStartInfo("https://github.com/SinorDev/OV-Refiner")
                {
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link: " + ex.Message);
            }
        }
    }
}